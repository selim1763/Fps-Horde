using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lightbug.CharacterControllerPro.Core
{

/// <summary>
/// This component can be used to make a Transform change its scale, based on the CharacterActor height.
/// </summary>
[AddComponentMenu("Character Controller Pro/Core/Character Graphics/Scaler")]
[DefaultExecutionOrder( ExecutionOrder.CharacterGraphicsOrder + 1 )]
public class CharacterGraphicsScaler : CharacterGraphics
{

    [SerializeField]
    VectorComponent scaleHeightComponent = VectorComponent.Y;

    // ─────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────

    enum VectorComponent
    {
        X ,
        Y , 
        Z
    }

    Vector3 initialOffset = Vector3.zero;
    Vector3 initialLocalScale = Vector3.one;

        
    void Start()
    {
        initialOffset = transform.position - CharacterActor.transform.position;
        initialLocalScale = transform.localScale;
    }
    
    void Update()
    {        
        if( !CharacterActor.enabled )
            return;
        
        Vector3 scale = Vector3.one;
        Vector3 offset = Vector3.zero;
        
        switch( scaleHeightComponent )
        {
            case VectorComponent.X:

                scale = new Vector3(
                    initialLocalScale.x * ( CharacterActor.BodySize.y / CharacterActor.DefaultBodySize.y ) , 
                    initialLocalScale.y * ( CharacterActor.BodySize.x / CharacterActor.DefaultBodySize.x ) ,
                    initialLocalScale.z * ( CharacterActor.BodySize.x / CharacterActor.DefaultBodySize.x )
                );
                
                break;
            case VectorComponent.Y:

                scale = new Vector3(
                    initialLocalScale.x * ( CharacterActor.BodySize.x / CharacterActor.DefaultBodySize.x ) , 
                    initialLocalScale.y * ( CharacterActor.BodySize.y / CharacterActor.DefaultBodySize.y ) ,
                    initialLocalScale.z * ( CharacterActor.BodySize.x / CharacterActor.DefaultBodySize.x )
                );
                
                break;
            case VectorComponent.Z:

                scale = new Vector3(
                    initialLocalScale.x * ( CharacterActor.BodySize.x / CharacterActor.DefaultBodySize.x ) , 
                    initialLocalScale.y * ( CharacterActor.BodySize.x / CharacterActor.DefaultBodySize.x ) ,
                    initialLocalScale.z * ( CharacterActor.BodySize.y / CharacterActor.DefaultBodySize.y )
                );
                
                break;
        }

        offset = new Vector3( 
            initialOffset.x * scale.x ,
            initialOffset.y * scale.y ,
            initialOffset.z * scale.z
        );

        // If the root controller is present then use that as the reference position. If not, then fallback to the character actor position.
        Vector3 referencePosition = RootController != null ? RootController.transform.position : CharacterActor.transform.position;
        
        transform.position = referencePosition + transform.TransformDirection( offset );        
        transform.localScale = scale;
        

    }    
     
    

}

}

