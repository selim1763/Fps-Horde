using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lightbug.Utilities;

namespace Lightbug.CharacterControllerPro.Core
{

/// <summary>
/// A physics actor represents a custom 2D/3D interpolated rigidbody.
/// </summary>
public abstract class PhysicsActor : MonoBehaviour
{
	[Header( "Rigidbody" )]

	[Tooltip("Interpolates the Transform component associated with this actor during Update calls. This is a custom implementation, the actor " + 
	"does not use Unity's default interpolation.")]
	public bool interpolateActor = true;

	[Tooltip("Whether or not to use continuous collision detection (rigidbody property). " + 
	"This won't affect character vs static obstacles interactions, but it will affect character vs dynamic rigidbodies.")]	
	public bool useContinuousCollisionDetection = true;

    [Header("Root motion")]

	[Tooltip( "This will activate root motion for the character. With root motion enabled, position and rotation will be handled by the current animation " + 
	"clip." )]
	public bool UseRootMotion = false;

	[Tooltip( "Whether or not to transfer position data from the root motion animation to the character." )]
	public bool UpdateRootPosition = true;

	[Tooltip( "Whether or not to transfer rotation data from the root motion animation to the character." )]
	public bool UpdateRootRotation = true;



    // ─────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────
    
	/// <summary>
	/// This event is called prior to the physics simulation.
	/// </summary>
	public event System.Action<float> OnPreSimulation;
	
	bool interpolationPositionDirtyFlag = false;
	bool interpolationRotationDirtyFlag = false;

    Vector3 startingPosition;
    Vector3 targetPosition;    

    Quaternion startingRotation;
    Quaternion targetRotation; 


    /// <summary>
    /// Gets the RigidbodyComponent component associated with the character.
    /// </summary>
	public abstract RigidbodyComponent RigidbodyComponent { get; }

    Coroutine postSimulationUpdateCoroutine;

    /// <summary>
	/// Gets the Animator component associated with the state controller.
	/// </summary>
	public Animator Animator { get; private set; }

	AnimatorLink animatorLink = null;


    protected virtual void Awake()
    {
		gameObject.GetOrAddComponent<PhysicsActorSync>();

        InitializeAnimation();        
    }
    
	public void SyncBody()
	{
		if( !interpolateActor )
			return;

        startingPosition = targetPosition; 
        startingRotation = targetRotation; 

        RigidbodyComponent.Position = startingPosition;
        RigidbodyComponent.Rotation = startingRotation;	
	}

	void InitializeAnimation()
	{		
		Animator = this.GetComponentInBranch<CharacterActor , Animator>();
		
		if( Animator != null )
		{
			animatorLink = Animator.GetComponent<AnimatorLink>();

			if( animatorLink == null )
				animatorLink = Animator.gameObject.AddComponent<AnimatorLink>();

		}

	}

    public void ResetIKWeights()
	{
		if( animatorLink != null )
			animatorLink.ResetIKWeights();
	}

    protected virtual void OnEnable()
	{
		if( postSimulationUpdateCoroutine == null )
			postSimulationUpdateCoroutine = StartCoroutine( PostSimulationUpdate() );        


		if( animatorLink != null )
		{
			animatorLink.OnAnimatorMoveEvent += OnAnimatorMoveLinkMethod;
			animatorLink.OnAnimatorIKEvent += OnAnimatorIKLinkMethod;
		}
		
	}

	protected virtual void OnDisable()
	{	
		if( postSimulationUpdateCoroutine != null )
		{
			StopCoroutine( postSimulationUpdateCoroutine );
			postSimulationUpdateCoroutine = null;
		}

		if( animatorLink != null )
		{
			animatorLink.OnAnimatorMoveEvent -= OnAnimatorMoveLinkMethod;
			animatorLink.OnAnimatorIKEvent -= OnAnimatorIKLinkMethod;
		}
	}

    protected virtual void Start()
    {    
		RigidbodyComponent.ContinuousCollisionDetection = useContinuousCollisionDetection; 
		RigidbodyComponent.UseInterpolation = false;

        // Interpolation
		targetPosition = startingPosition = transform.position;		
		targetRotation = startingRotation = transform.rotation;
        
    }


    protected virtual void PreSimulationUpdate( float dt ) {}
    protected virtual void PostSimulationUpdate( float dt ) {}

    public event System.Action< Vector3 , Quaternion > OnAnimatorMoveEvent;
    public event System.Action< int > OnAnimatorIKEvent;	
	

	void OnAnimatorMoveLinkMethod( Vector3 deltaPosition , Quaternion deltaRotation )
	{	
		
		if( !UseRootMotion )
			return;
		
		if( OnAnimatorMoveEvent != null )
			OnAnimatorMoveEvent( deltaPosition , deltaRotation );
		
        // InitializeInterpolationData();

		if( RigidbodyComponent.IsKinematic )
		{
			// "Move" cannot be used here, Unity's interpolation is ignored.
			if( UpdateRootPosition )
				RigidbodyComponent.Position += deltaPosition;
						
			if( UpdateRootRotation )
				RigidbodyComponent.Rotation *= deltaRotation;
		}
		else
		{
			if( UpdateRootPosition )
				RigidbodyComponent.Move( RigidbodyComponent.Position + deltaPosition ); 
			
			if( UpdateRootRotation )
				RigidbodyComponent.Rotation *= deltaRotation;

			PreSimulationUpdate( Time.deltaTime );
		}

		if( OnPreSimulation != null )
			OnPreSimulation( Time.deltaTime );

		// Manual sync (in case the Transform component is "dirty").
		transform.position = RigidbodyComponent.Position;		
		transform.rotation = RigidbodyComponent.Rotation;


		
	}

	void OnAnimatorIKLinkMethod( int layerIndex )
	{
		if( OnAnimatorIKEvent != null )
			OnAnimatorIKEvent( layerIndex );
	}


    void Update()   
    {
        ProcessInterpolation();
    }


    void FixedUpdate()
    {
        // 2D -> Transform and Rigidbody are NOT synced.
        // 3D -> Transform and Rigidbody are synced. 

        if( UseRootMotion )
			return;

        PreSimulationUpdate( Time.deltaTime );

		if( OnPreSimulation != null )
			OnPreSimulation( Time.deltaTime );
        
        // Manual sync (in case the Transform component is "dirty").
		transform.position = RigidbodyComponent.Position;		
		transform.rotation = RigidbodyComponent.Rotation;
        
    }

    IEnumerator PostSimulationUpdate()
	{
		YieldInstruction waitForFixedUpdate = new WaitForFixedUpdate();
		while( true )
		{
			yield return waitForFixedUpdate;

			if( this.enabled )
				PostSimulationUpdate( Time.deltaTime );
			

			UpdateInterpolationData();
		}
	}

	

	

    void ProcessInterpolation()
	{		
		if( !interpolateActor )
			return;
		
		float interpolationFactor = ( Time.time - Time.fixedTime ) / ( Time.fixedDeltaTime );
        

        transform.position = Vector3.Lerp( startingPosition , targetPosition , interpolationFactor );
        transform.rotation = Quaternion.Slerp( startingRotation , targetRotation , interpolationFactor );
	}

    void UpdateInterpolationData()
    {
		
		if( !interpolateActor )
			return;


        if( interpolationPositionDirtyFlag )
            interpolationPositionDirtyFlag = false; 
        else
            targetPosition = RigidbodyComponent.Position;
        
        if( interpolationRotationDirtyFlag )
            interpolationRotationDirtyFlag = false;
        else
            targetRotation = RigidbodyComponent.Rotation; 
    }

    
    public void ResetInterpolationPosition()
    {        
        interpolationPositionDirtyFlag = true;

        if( RigidbodyComponent != null )
		{
            targetPosition = startingPosition = RigidbodyComponent.Position;
			transform.position = targetPosition;
		}
    }

    public void ResetInterpolationRotation()
    {            
        interpolationRotationDirtyFlag = true;

        if( RigidbodyComponent != null )
		{
            targetRotation = startingRotation = RigidbodyComponent.Rotation;
			transform.rotation = targetRotation;
		}
    }
    

    
}

}
