using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lightbug.Utilities;

namespace Lightbug.CharacterControllerPro.Core
{

/// <summary>
/// This component is responsible for smoothing out the graphics-related elements (under the root object) based on the character movement (CharacterActor).
/// It allows you to modify the position and rotation accordingly, producing a great end result.
/// </summary>
[AddComponentMenu("Character Controller Pro/Core/Character Graphics/Root Controller")]
[DefaultExecutionOrder( ExecutionOrder.CharacterGraphicsOrder )]
public class CharacterGraphicsRootController : CharacterGraphics
{      
    [Header("Rotation interpolation")]
    
    [Tooltip("Whether or not to smooth out the rotation change.")]
    [SerializeField]
    bool lerpRotation = true;

    [Condition( "lerpRotation" , ConditionAttribute.ConditionType.IsTrue ) ]
    [SerializeField]
    float rotationLerpSpeed = 25f;

    [Header("Vertical displacement interpolation")]
    
    [Tooltip("Whether or not to smooth out the vertical position change (vertical displacement). A vertical displacement happens everytime the character " + 
    "increase/decrease its vertical position (slopes, step up, step down, etc.). This feature is considered enabled if the character is 'stable' and the ground is not a rigidbody.")]
    [SerializeField]
    bool lerpVerticalDisplacement = true;

    
    [Tooltip("How fast the step up action is going to be.")]
    [Condition( "lerpVerticalDisplacement" , ConditionAttribute.ConditionType.IsTrue ) ]
    [SerializeField]
    float positiveDisplacementSpeed = 20f;

    [Tooltip("How fast the step down action is going to be.")]
    [Condition( "lerpVerticalDisplacement" , ConditionAttribute.ConditionType.IsTrue ) ]
    [SerializeField]
    float negativeDisplacementSpeed = 40f;

    [Tooltip("The duration (in seconds) used to transition from active (e.g. the character was stable) to inactive (e.g. the character becomes unstable). Basically, the lerp speed (t factor) will transition linearly from 0 to 1 using this value.")]
    [Condition( "lerpVerticalDisplacement" , ConditionAttribute.ConditionType.IsTrue ) ]
    [Min(0.01f)]
    [SerializeField]
    float stableToUnstableDuration = 0.25f;

    // ─────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────

    Vector3 previousPosition = default( Vector3 );
    Quaternion previousRotation = default( Quaternion );

    Vector3 initialLocalForward = default( Vector3 );

    void Start()
    {
        initialLocalForward = CharacterActor.transform.InverseTransformDirection( transform.forward );

        previousPosition = transform.position;
        previousRotation = transform.rotation;
    }

    
    void OnEnable()
    {
        CharacterActor.OnTeleport += OnTeleport;
    }

    void OnDisable()
    {
        CharacterActor.OnTeleport -= OnTeleport;
    }

    bool teleportFlag = false;

    void OnTeleport( Vector3 position , Quaternion rotation )
    {
        teleportFlag = true;        
    }
    
    void Update() 
    {
        if( CharacterActor == null )
        {
            this.enabled = false;
            return;            
        }
        
        float dt = Time.deltaTime;

        HandleRotation( dt );
        HandleVerticalDisplacement( dt );

        if( teleportFlag )
            teleportFlag = false;
        

    }
    

    /// <summary>
    /// This property prevents interpolation if the actor is unstable or is on top of a rigidbody (ground).
    /// </summary>
    bool CanLerpVertically => CharacterActor.IsStable && !CharacterActor.IsGroundARigidbody;

    float timer = 0f;

    void HandleVerticalDisplacement( float dt )
    {
        if( !lerpVerticalDisplacement )
            return;

        if( teleportFlag )
        {
            transform.position = CharacterActor.Position;
            previousPosition = transform.position;

            timer = 0f;

            return;
        }
        
        Vector3 planarDisplacement = Vector3.ProjectOnPlane( CharacterActor.transform.position - previousPosition, CharacterActor.Up );
        Vector3 verticalDisplacement = Vector3.Project( CharacterActor.transform.position - previousPosition, CharacterActor.Up );
        float t = 0f;

        if( CanLerpVertically )
        {
            timer = 0f;
            t = ( CharacterActor.transform.InverseTransformVectorUnscaled( verticalDisplacement ).y > 0f ? positiveDisplacementSpeed : negativeDisplacementSpeed ) * dt;
        }
        else
        {
            timer += dt;
            // If the character is unable to lerp vertically, then lerp the "t" factor from 0 (no lerping) to 1 over "StateToUnstableDuration" seconds.
            t = Mathf.Clamp01( timer / Mathf.Max( stableToUnstableDuration , 0.01f ) );
        }
               
        
        
        transform.position = previousPosition + planarDisplacement + Vector3.Lerp( Vector3.zero , verticalDisplacement , t ); 

        previousPosition = transform.position;
    }

    void HandleRotation( float dt )
    {
        if( !lerpRotation )
            return;

        if( teleportFlag )
        {
            transform.localRotation = Quaternion.identity;
            previousRotation = transform.rotation;

            return;
        }
        
        transform.rotation = Quaternion.Slerp( previousRotation , CharacterActor.Rotation , rotationLerpSpeed * dt );

        previousRotation = transform.rotation;
    }
    
    



}

}

