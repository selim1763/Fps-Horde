using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lightbug.Utilities;

namespace Lightbug.CharacterControllerPro.Core
{

public enum CharacterActorState
{
	NotGrounded ,
	StableGrounded ,
	UnstableGrounded
}


/// <summary>
/// This class represents a character actor. It contains all the character information, collision flags, collision events, and so on. It also responsible for the execution order 
/// of everything related to the character, such as movement, rotation, teleportation, rigidbodies interactions, body size, etc. Since the character can be 2D or 3D, this abstract class must be implemented in the 
/// two formats, one for 2D and one for 3D.
/// </summary>
[AddComponentMenu("Character Controller Pro/Core/Character Actor")]
[RequireComponent(typeof( CharacterBody ))]
[DefaultExecutionOrder( ExecutionOrder.CharacterActorOrder )]
public class CharacterActor : PhysicsActor
{
	[Header( "Collision" )]	
	
	[Tooltip("One way platforms are objects that can be contacted by the character feet (bottom sphere) while descending.")]
	public LayerMask oneWayPlatformsLayerMask = 0;
	

	[Header("Grounding")]	

	[Tooltip( "Prevents the character from enter grounded state (IsGrounded will be false)" ) ]
	public bool alwaysNotGrounded = false;

	[Condition( "alwaysNotGrounded" , ConditionAttribute.ConditionType.IsFalse )]
	[Tooltip("If enabled the character will do an initial ground check (at \"Start\"). If the test fails the starting state will be \"Not grounded\".")]
	public bool forceGroundedAtStart = true;

	[Space(10f)]

	[Tooltip("The slope limit set the maximum angle considered as stable.")]	
	[Range(1f, 85f)]
	public float slopeLimit = 55f;

	[Tooltip("The offset distance applied to the bottom of the character. A higher offset means more walkable surfaces")]
	[Min( 0f )]
	public float stepUpDistance = 0.5f;

	[Tooltip("The distance the character is capable of detecting ground. Use this value to clamp (or not) the character to the ground.")]
	[Min( 0f )]
	public float stepDownDistance = 0.5f;


	[Space(10f)]
	

	[Tooltip("With this enabled the character bottom sphere (capsule) will be simulated as a cylinder. This works only when the character is standing on an edge.")]
	public bool edgeCompensation = false;

	[Space(10f)]

	[Tooltip("This will prevent the character from stepping over an unstable surface (a \"bad\" step). This requires a bit more processing, so if your character does not need this level of precision " + 
	"you can disable it.")]
	public bool preventBadSteps = true;
		

	[Space(10f)]


	[Header("Dynamic ground")]	
	
	[Tooltip("Should the character be affected by the movement of the ground?")]
	public bool supportDynamicGround = true;	
	
	[Condition( "supportDynamicGround" , ConditionAttribute.ConditionType.IsTrue , ConditionAttribute.VisibilityType.NotEditable )]
	[Tooltip("The forward direction of the character will be affected by the rotation of the ground (only yaw motion allowed).")]
	public bool rotateForwardDirection = true;

	[Condition( "supportDynamicGround" , ConditionAttribute.ConditionType.IsTrue , ConditionAttribute.VisibilityType.NotEditable )]
	[Tooltip("This is the maximum ground velocity delta (from the previous frame to the current one) tolerated by the character." + 
	"\n\nIf the ground accelerates too much, then the character will stop moving with it." )]
	public float maxGroundVelocityChange = 30f;

		
	[Condition( "supportDynamicGround" , ConditionAttribute.ConditionType.IsTrue , ConditionAttribute.VisibilityType.NotEditable )]
	[Tooltip("This is the maximum ground velocity that the character can ignore when calling \"ForceNotGrounded\" (e.g. when doing a jump while being on a moving platform). " + 
	"\n\nIf the ground velocity is greater than this value, then the character will add that velocity as its own velocity.")]
	public float maxForceNotGroundedGroundVelocity = 10f;


	[Header( "Velocity" )]

	[Tooltip( "Whether or not to project the initial velocity (stable) onto walls." )]
	[SerializeField]
	public bool slideOnWalls = true;	

	[Tooltip( "Should the actor re-assign the rigidbody velocity after the simulation is done?\n\n" + 
	"PreSimulationVelocity: the character uses the velocity prior to the simulation (modified by this component).\nPostSimulationVelocity: the character uses the velocity received from the simulation (no re-assignment).\nInputVelocity: the character \"gets back\" its initial velocity (before being modified by this component)." )]
	public CharacterVelocityMode stablePostSimulationVelocity = CharacterVelocityMode.UsePostSimulationVelocity;

	[Tooltip( "Should the actor re-assign the rigidbody velocity after the simulation is done?\n\n" + 
	"PreSimulationVelocity: the character uses the velocity prior to the simulation (modified by this component).\nPostSimulationVelocity: the character uses the velocity received from the simulation (no re-assignment).\nInputVelocity: the character \"gets back\" its initial velocity (before being modified by this component)." )]
	public CharacterVelocityMode unstablePostSimulationVelocity = CharacterVelocityMode.UsePostSimulationVelocity;
	

	[Header("Size")]

	[Min(0f)]
	public float sizeLerpSpeed = 8f;

	[Header("Rotation")]

	[Tooltip("Should this component define the character \"Up\" direction?")]
	public bool constraintRotation = true;

	[Condition( "constraintRotation" , ConditionAttribute.ConditionType.IsTrue , ConditionAttribute.VisibilityType.NotEditable )]
	[Tooltip("The desired up direction.")]
	public Vector3 constraintUpDirection = Vector3.up;	

	[Condition( "constraintRotation" , ConditionAttribute.ConditionType.IsTrue , ConditionAttribute.VisibilityType.NotEditable )]
	public Transform upDirectionReference = null;

	[Condition( "upDirectionReference" , ConditionAttribute.ConditionType.HasReference , ConditionAttribute.VisibilityType.Hidden )]
	public VerticalAlignmentSettings.VerticalReferenceMode upDirectionReferenceMode = VerticalAlignmentSettings.VerticalReferenceMode.Away;

		
	[Header("Physics")]

	public bool canPushDynamicRigidbodies = true;

	[Condition( "canPushDynamicRigidbodies" , ConditionAttribute.ConditionType.IsTrue , ConditionAttribute.VisibilityType.NotEditable )]
	public LayerMask pushableRigidbodyLayerMask = -1;

	public bool applyWeightToGround = true;

	[Condition( "applyWeightToGround" , ConditionAttribute.ConditionType.IsTrue , ConditionAttribute.VisibilityType.NotEditable  )]
	public float weightGravity = CharacterConstants.DefaultGravity;

	


	// ─────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────
	// ─────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────
	// ─────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────
	
	
	public float StepOffset
	{
		get
		{						
			return stepUpDistance - BodySize.x / 2f;
		}
	}


	public void OnValidate()
	{
		if( CharacterBody == null )
			CharacterBody = GetComponent<CharacterBody>();
				
		stepUpDistance = Mathf.Clamp( 
			stepUpDistance , 
			CharacterConstants.ColliderMinBottomOffset + CharacterBody.BodySize.x / 2f ,
			CharacterBody.BodySize.y - CharacterBody.BodySize.x / 2f 
		);

	}
	

	/// <summary>
	/// Gets the CharacterBody component associated with this character actor.
	/// </summary>
	public bool Is2D => RigidbodyComponent.Is2D;

	/// <summary>
    /// Gets the RigidbodyComponent component associated with the character.
    /// </summary>
	public override RigidbodyComponent RigidbodyComponent => CharacterBody.RigidbodyComponent;

	/// <summary>
    /// Gets the ColliderComponent component associated with the character.
    /// </summary>
	public ColliderComponent ColliderComponent => CharacterBody.ColliderComponent;
	
	/// <summary>
	/// Gets the physics component from the character.
	/// </summary>
	public PhysicsComponent PhysicsComponent { get; private set; }

	/// <summary>
	/// Gets the CharacterBody component associated with this character actor.
	/// </summary>
	public CharacterBody CharacterBody { get; private set; }

	/// <summary>
	/// Returns the current character actor state. This enum variable contains the information about the grounded and stable state, all in one.
	/// </summary>
	public CharacterActorState CurrentState
	{
		get
		{
			if( IsGrounded )
				return IsStable ? CharacterActorState.StableGrounded : CharacterActorState.UnstableGrounded;
			else			
				return CharacterActorState.NotGrounded;
		}
	}

	/// <summary>
	/// Returns the character actor state from the previous frame.
	/// </summary>
	public CharacterActorState PreviousState
	{
		get
		{
			if( WasGrounded )
				return WasStable ? CharacterActorState.StableGrounded : CharacterActorState.UnstableGrounded;
			else			
				return CharacterActorState.NotGrounded;
		}
	}

	#region Collision Properties

	public LayerMask ObstaclesLayerMask => PhysicsComponent.CollisionLayerMask | oneWayPlatformsLayerMask;
	public LayerMask ObstaclesWithoutOWPLayerMask => PhysicsComponent.CollisionLayerMask & ~( oneWayPlatformsLayerMask );
	
	/// <summary>
	/// Returns true if the character is standing on an edge.
	/// </summary>
	public bool IsOnEdge =>characterCollisionInfo.isOnEdge;

	/// <summary>
	/// Returns the angle between the both sides of the edge.
	/// </summary>
	public float EdgeAngle => characterCollisionInfo.edgeAngle;

	/// <summary>
	/// Gets the grounded state, true if the ground object is not null, false otherwise.
	/// </summary>
	public bool IsGrounded => characterCollisionInfo.groundObject != null;
	
	/// <summary>
	/// Gets the angle between the up vector and the stable normal.
	/// </summary>
	public float GroundSlopeAngle => characterCollisionInfo.groundSlopeAngle;

	/// <summary>
	/// Gets the contact point obtained directly from the ground test (sphere cast).
	/// </summary>
	public Vector3 GroundContactPoint => characterCollisionInfo.groundContactPoint;

	/// <summary>
	/// Gets the normal vector obtained directly from the ground test (sphere cast).
	/// </summary>
	public Vector3 GroundContactNormal => characterCollisionInfo.groundContactNormal;

	/// <summary>
	/// Gets the normal vector used to determine stability. This may or may not be the normal obtained from the ground test.
	/// </summary>
	public Vector3 GroundStableNormal => IsStable ? characterCollisionInfo.groundStableNormal : Up; 


	/// <summary>
	/// Gets the GameObject component of the current ground.
	/// </summary>
	public GameObject GroundObject => characterCollisionInfo.groundObject;

	/// <summary>
	/// Gets the Transform component of the current ground.
	/// </summary>
	public Transform GroundTransform => GroundObject != null ? GroundObject.transform : null;
	
	/// <summary>
	/// Gets the Collider2D component of the current ground.
	/// </summary>
	public Collider2D GroundCollider2D => characterCollisionInfo.groundCollider2D;
	/// <summary>
	/// Gets the Collider3D component of the current ground.
	/// </summary>
	public Collider GroundCollider3D => characterCollisionInfo.groundCollider3D;
	
	// Wall ──────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────	

	/// <summary>
	/// Gets the wall collision flag, true if the character hit a wall, false otherwise.
	/// </summary>
	public bool WallCollision => characterCollisionInfo.wallCollision; 


	/// <summary>
	/// Gets the angle between the contact normal (wall collision) and the Up direction.
	/// </summary>	
	public float WallAngle => characterCollisionInfo.wallAngle; 


	/// <summary>
	/// Gets the current contact (wall collision).
	/// </summary>
	public Contact WallContact => characterCollisionInfo.wallContact; 


	// Head ──────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────	

	/// <summary>
	/// Gets the head collision flag, true if the character hits something with its head, false otherwise.
	/// </summary>
	public bool HeadCollision => characterCollisionInfo.headCollision; 


	/// <summary>
	/// Gets the angle between the contact normal (head collision) and the Up direction.
	/// </summary>
	public float HeadAngle => characterCollisionInfo.headAngle; 


	/// <summary>
	/// Gets the current contact (head collision).
	/// </summary>
	public Contact HeadContact => characterCollisionInfo.headContact; 

	
	// ─────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────
	
	/// <summary>
	/// Gets the current stability state of the character. Stability is equal to "grounded + slope angle <= slope limit".
	/// </summary>
	public bool IsStable => IsGrounded && characterCollisionInfo.groundSlopeAngle <= slopeLimit;

	/// <summary>
	/// Returns true if the character is grounded onto an unstable ground, false otherwise.
	/// </summary>
	public bool IsOnUnstableGround => IsGrounded && characterCollisionInfo.groundSlopeAngle > slopeLimit;
	
	/// <summary>
	/// Gets the previous grounded state.
	/// </summary>
    public bool WasGrounded { get; private set; }	
	
	/// <summary>
	/// Gets the previous stability state.
	/// </summary>
    public bool WasStable { get; private set; }

		

	/// <summary>
	/// Gets the RigidbodyComponent component from the ground.
	/// </summary>
	public RigidbodyComponent GroundRigidbodyComponent
	{
		get
		{
			if( !IsStable )
				groundRigidbodyComponent = null;

			return groundRigidbodyComponent;
		}		
	}

	RigidbodyComponent groundRigidbodyComponent = null;

	/// <summary>
	/// Gets the ground rigidbody position.
	/// </summary>
	public Vector3 GroundPosition
	{ 
		get
		{
			return Is2D ? 
			new Vector3 (
				GroundCollider2D.attachedRigidbody.position.x ,
				GroundCollider2D.attachedRigidbody.position.y ,
				GroundTransform.position.z
			 ) : GroundCollider3D.attachedRigidbody.position;
		} 
	}

	/// <summary>
	/// Gets the ground rigidbody rotation.
	/// </summary>
	public Quaternion GroundRotation
	{ 
		get
		{ 
			return Is2D ? Quaternion.Euler( 0f , 0f , GroundCollider2D.attachedRigidbody.rotation ) : GroundCollider3D.attachedRigidbody.rotation;
		} 
	}

	/// <summary>
	/// Returns true if the current ground is a Rigidbody (2D or 3D), false otherwise.
	/// </summary>
	public bool IsGroundARigidbody
	{
		get
		{
			return Is2D ? characterCollisionInfo.groundRigidbody2D != null : characterCollisionInfo.groundRigidbody3D != null;
		}
	}

	/// <summary>
	/// Returns true if the current ground is a kinematic Rigidbody (2D or 3D), false otherwise.
	/// </summary>
	public bool IsGroundAKinematicRigidbody
	{
		get
		{
			return Is2D ? characterCollisionInfo.groundRigidbody2D.isKinematic : characterCollisionInfo.groundRigidbody3D.isKinematic;
		}
	}
	
	/// <summary>
	/// Returns the point velocity (Rigidbody API) of the ground using the character position.
	/// </summary>
	public Vector3 DynamicGroundPointVelocity => Is2D ? 
		(Vector3)characterCollisionInfo.groundRigidbody2D.GetPointVelocity( Position ) : 
		characterCollisionInfo.groundRigidbody3D.GetPointVelocity( Position );
	
	
	
	/// <summary>
	/// Returns a concatenated string containing all the current collision information.
	/// </summary>
	public override string ToString()
	{
		const string nullString = " ---- ";
		

		string triggerString = "";

		for( int i = 0 ; i < Triggers.Count ; i++ )
		{
			triggerString += " - " + Triggers[i].gameObject.name + "\n";
		}

		return string.Concat( 
			"Ground : \n" ,
			"──────────────────\n" ,
			"Is Grounded : " , IsGrounded , '\n' ,
			"Is Stable : " , IsStable , '\n' ,
			"Slope Angle : " , characterCollisionInfo.groundSlopeAngle , '\n' ,
			"Is On Edge : " , characterCollisionInfo.isOnEdge , '\n' ,
			"Edge Angle : " , characterCollisionInfo.edgeAngle , '\n' ,
			"Object Name : " , characterCollisionInfo.groundObject != null ? characterCollisionInfo.groundObject.name : nullString , '\n' ,
			"Layer : " , LayerMask.LayerToName( characterCollisionInfo.groundLayer ) , '\n' , 	
			"Rigidbody Type: " , GroundRigidbodyComponent != null ? GroundRigidbodyComponent.IsKinematic ? "Kinematic" : "Dynamic" : nullString , '\n' ,
			"Dynamic Ground : " , GroundRigidbodyComponent != null ? "Yes" : "No" , "\n\n" ,			
			"Wall : \n" ,
			"──────────────────\n" ,			
			"Wall Collision : " , characterCollisionInfo.wallCollision , '\n' ,	
			"Wall Angle : " , characterCollisionInfo.wallAngle , "\n\n" ,
			"Head : \n" ,
			"──────────────────\n" ,			
			"Head Collision : " , characterCollisionInfo.headCollision , '\n' , 
			"Head Angle : " , characterCollisionInfo.headAngle , "\n\n" ,
			"Triggers : \n" ,
			"──────────────────\n" ,
			"Current : " , CurrentTrigger.gameObject != null ? CurrentTrigger.gameObject.name : nullString , '\n' ,		
			triggerString	
		);
	}

#endregion

	protected CharacterCollisionInfo characterCollisionInfo = new CharacterCollisionInfo();

	/// <summary>
	/// Gets a structure with all the information regarding character collisions. Most of the character properties (e.g. IsGrounded, IsStable, GroundObject, and so on)
	/// can be obtained from this structure.
	/// </summary>
	public CharacterCollisionInfo CharacterCollisionInfo => characterCollisionInfo;

	
#if UNITY_TERRAIN_MODULE
	Dictionary< Transform , Terrain > terrains = new Dictionary< Transform , Terrain >();
#endif
	Dictionary< Transform , RigidbodyComponent > groundRigidbodyComponents = new Dictionary< Transform , RigidbodyComponent >();

	

	public float GroundedTime{ get; private set; }    
	public float NotGroundedTime{ get; private set; }
    
	public float StableElapsedTime{ get; private set; }
	public float UnstableElapsedTime{ get; private set; }
	
	
	/// <summary>
	/// Gets the current body size (width and height).
	/// </summary>
    public Vector2 BodySize { get; private set; }

	/// <summary>
	/// Gets the current body size (width and height).
	/// </summary>
    public Vector2 DefaultBodySize => CharacterBody.BodySize;


	/// <summary>
	/// Gets/Sets the rigidbody velocity.
	/// </summary>
	public Vector3 Velocity
	{
		get
		{
			return RigidbodyComponent.Velocity;
		}
		set
		{
			RigidbodyComponent.Velocity = value;
		}
	}
	
	

	/// <summary>
	/// Gets/Sets the rigidbody velocity projected onto a plane formed by its up direction.
	/// </summary>
	public Vector3 PlanarVelocity
	{
		get
		{
			return Vector3.ProjectOnPlane( Velocity , Up );
		}
		set
		{
			Velocity = Vector3.ProjectOnPlane( value , Up ) + VerticalVelocity;
		}
	}

	

	/// <summary>
	/// Gets/Sets the rigidbody velocity projected onto its up direction.
	/// </summary>
	public Vector3 VerticalVelocity
	{
		get
		{
			return Vector3.Project( Velocity , Up );
		}
		set
		{
			Velocity = PlanarVelocity + Vector3.Project( value , Up );
		}
	}

	



	/// <summary>
	/// Gets/Sets the rigidbody velocity projected onto a plane formed by its up direction.
	/// </summary>
	public Vector3 StableVelocity
	{
		get
		{
			return  CustomUtilities.ProjectVectorOnPlane( Velocity , GroundStableNormal, Up );
		}
		set
		{			 
			Velocity = CustomUtilities.ProjectVectorOnPlane( value , GroundStableNormal, Up );
		}
	}

	
	public Vector3 LastGroundedVelocity{ get; private set; }

	
	
	/// <summary>
	/// Gets/Sets the rigidbody local velocity.
	/// </summary>
	public Vector3 LocalVelocity
	{
		get
		{
			return transform.InverseTransformVectorUnscaled( Velocity );
		}
		set
		{
			Velocity = transform.TransformVectorUnscaled( value );
		}
	}

	/// <summary>
	/// Gets/Sets the rigidbody local planar velocity.
	/// </summary>
	public Vector3 LocalPlanarVelocity
	{
		get
		{
			return transform.InverseTransformVectorUnscaled( PlanarVelocity );
		}
		set
		{
			PlanarVelocity = transform.TransformVectorUnscaled( value );
		}
	}

	


	/// <summary>
	/// Returns true if the character local vertical velocity is less than zero. 
	/// </summary>
	public bool IsFalling
	{
		get
		{
			return LocalVelocity.y < 0f;
		}
	}

	/// <summary>
	/// Returns true if the character local vertical velocity is greater than zero.
	/// </summary>
	public bool IsAscending
	{
		get
		{
			return LocalVelocity.y > 0f;
		}
	}
	


	#region public Body properties

	/// <summary>
	/// Gets the center of the collision shape.
	/// </summary>
	public Vector3 Center
	{
		get
		{
			return GetCenter( Position );
		}
	}

	/// <summary>
	/// Gets the center of the collision shape.
	/// </summary>
	public Vector3 Top
	{
		get
		{
			return GetTop( Position );
		}
	}

	/// <summary>
	/// Gets the center of the collision shape.
	/// </summary>
	public Vector3 Bottom
	{
		get
		{
			return GetBottom( Position );
		}
	}

	/// <summary>
	/// Gets the center of the collision shape.
	/// </summary>
	public Vector3 TopCenter
	{
		get
		{
			return GetTopCenter( Position );
		}
	}

	/// <summary>
	/// Gets the center of the collision shape.
	/// </summary>
	public Vector3 BottomCenter
	{
		get
		{
			return GetBottomCenter( Position , 0f );
		}
	}

	/// <summary>
	/// Gets the center of the collision shape.
	/// </summary>
	public Vector3 OffsettedBottomCenter
	{
		get
		{
			return GetBottomCenter( Position , StepOffset );
		}
	}

#endregion

#region Body functions

	/// <summary>
	/// Gets the center of the collision shape.
	/// </summary>
	public Vector3 GetCenter( Vector3 position )
	{
		return position + Up * BodySize.y / 2f;
	}	

	/// <summary>
	/// Gets the top most point of the collision shape.
	/// </summary>
	public Vector3 GetTop( Vector3 position )
	{
		return position + Up * ( BodySize.y - CharacterConstants.SkinWidth );
	}

	/// <summary>
	/// Gets the bottom most point of the collision shape.
	/// </summary>
	public Vector3 GetBottom( Vector3 position )
	{
		return position + Up * CharacterConstants.SkinWidth;
	}

	/// <summary>
	/// Gets the center of the top sphere of the collision shape.
	/// </summary>
	public Vector3 GetTopCenter( Vector3 position )
	{
		return position + Up * ( BodySize.y - BodySize.x / 2f );
	}

	/// <summary>
	/// Gets the center of the top sphere of the collision shape (considering an arbitrary body size).
	/// </summary>
	public Vector3 GetTopCenter( Vector3 position , Vector2 bodySize )
	{
		return position + Up * ( bodySize.y - bodySize.x / 2f );
	}

	/// <summary>
	/// Gets the center of the bottom sphere of the collision shape.
	/// </summary>
	public Vector3 GetBottomCenter( Vector3 position , float bottomOffset = 0f )
	{
		return position + Up * ( BodySize.x / 2f + bottomOffset );
	}
	

	/// <summary>
	/// Gets the center of the bottom sphere of the collision shape (considering an arbitrary body size).
	/// </summary>
	public Vector3 GetBottomCenter( Vector3 position , Vector2 bodySize )
	{
		return position + Up * bodySize.x / 2f;
	}

	/// <summary>
	/// Gets the a vector that goes from the bottom center to the top center (topCenter - bottomCenter).
	/// </summary>
	public Vector3 GetBottomCenterToTopCenter()
	{
		return Up * ( BodySize.y - BodySize.x );
	}

	/// <summary>
	/// Gets the a vector that goes from the bottom center to the top center (topCenter - bottomCenter).
	/// </summary>
	public Vector3 GetBottomCenterToTopCenter( Vector2 bodySize )
	{
		return Up * ( bodySize.y - bodySize.x );
	}

	
#endregion

	
	CharacterCollisions characterCollisions = new CharacterCollisions();
	
	public CharacterCollisions CharacterCollisions => characterCollisions;
	
	protected override void Awake()
	{			

		base.Awake();

		CharacterBody = GetComponent<CharacterBody>();
		targetBodySize = CharacterBody.BodySize;
		BodySize = targetBodySize;
				
		if( Is2D )
			PhysicsComponent = gameObject.AddComponent<PhysicsComponent2D>();
		else
			PhysicsComponent = gameObject.AddComponent<PhysicsComponent3D>();
		
		SetColliderSize();		
		
		RigidbodyComponent.IsKinematic = false;
		RigidbodyComponent.UseGravity  = false;
		RigidbodyComponent.Mass        = CharacterBody.Mass;
		RigidbodyComponent.LinearDrag  = 0f;
		RigidbodyComponent.AngularDrag = 0f;		
		RigidbodyComponent.Constraints = RigidbodyConstraints.FreezeRotation;


		characterCollisions.Initialize( this , PhysicsComponent );

	
	}

	
	protected override void Start()
	{		
		base.Start();

		// Initial OWP check
		characterCollisions.CheckOverlapWithLayerMask( 
			Position ,
			0f ,
			new HitInfoFilter( 
				ObstaclesLayerMask ,
				false ,
				true ,
				oneWayPlatformsLayerMask
			)
		);

		// Initial "Force Grounded"
		if( forceGroundedAtStart && !alwaysNotGrounded )
			ForceGrounded();
		
		
		forward2D = transform.right;

		

	}

	
	protected override void OnEnable()
	{
		base.OnEnable();

		OnTeleport += OnTeleportMethod;		
	}

	protected override void OnDisable()
	{		
		base.OnDisable();

		OnTeleport -= OnTeleportMethod;
	}

	
	void OnTeleportMethod( Vector3 position , Quaternion rotation )
	{
		Velocity = Vector3.zero;
		stableProbeGroundVelocity = Vector3.zero;
	}
	
	
	/// <summary>
	/// Applies a force at the ground contact point, in the direction of the weight (mass times gravity).
	/// </summary>
	protected virtual void ApplyWeight( Vector3 contactPoint )
    {
		if( !applyWeightToGround )
			return;
		
		
		if( Is2D )
		{
			if( GroundCollider2D == null )
            	return;

			if( GroundCollider2D.attachedRigidbody == null )
				return;
			
        	GroundCollider2D.attachedRigidbody.AddForceAtPosition( - transform.up * CharacterBody.Mass * weightGravity , contactPoint );
		}
		else
		{
			if( GroundCollider3D == null )
            	return;

			if( GroundCollider3D.attachedRigidbody == null )
				return;

        
        	GroundCollider3D.attachedRigidbody.AddForceAtPosition( - transform.up * CharacterBody.Mass * weightGravity , contactPoint );
		}

        
    }

	

	// -------------------------------------------------------------------------------------------------------
	

	void SetColliderSize()
    {		

        float verticalOffset = IsStable ? Mathf.Max( StepOffset , CharacterConstants.ColliderMinBottomOffset ) : 0f;

        float radius = BodySize.x / 2f;
		float height = BodySize.y - verticalOffset;

        ColliderComponent.Size = new Vector2( 2f * radius , height );
		ColliderComponent.Offset = Vector2.up * ( verticalOffset + height / 2f );
    }



	/// <summary>
    /// Gets/Sets the current rigidbody position. This action will produce an "interpolation reset", meaning that (visually) the object will move instantly to the target.
    /// </summary>
	public Vector3 Position
	{
		get
		{
			return RigidbodyComponent.Position;
		}
		set
		{
			RigidbodyComponent.Position = value;


			ResetInterpolationPosition();
		}
	}

	/// <summary>
    /// Gets/Sets the current rigidbody rotation. This action will produce an "interpolation reset", meaning that (visually) the object will rotate instantly to the target.
    /// </summary>
	public Quaternion Rotation
	{
		get
		{
			return RigidbodyComponent.Rotation;
		}
		set
		{	
			RigidbodyComponent.Rotation = value;
			ResetInterpolationRotation();
		}
	}
	
    /// <summary>
    /// Gets/Sets the current up direction based on the rigidbody rotation (not necessarily transform.up).
    /// </summary>
	public Vector3 Up
	{
		get
		{
			return RigidbodyComponent.Up;
		}
		set
		{			
			if( value == Vector3.zero )
				return;

			Quaternion deltaRotation = Quaternion.FromToRotation( Up , value.normalized );

			RotateInternal( deltaRotation  );
		}
	}

	Vector3 forward2D = Vector3.right;
	
	/// <summary>
    /// Gets/Sets the current forward direction based on the rigidbody rotation (not necessarily transform.forward).
    /// </summary>
	public Vector3 Forward
	{
		get
		{
			return Is2D ? forward2D : RigidbodyComponent.Rotation * Vector3.forward;
		}
		set
		{			
			
			if( value == Vector3.zero )
				return;

			if( Is2D )
			{
				forward2D = Vector3.Project( value , Right ).normalized;				
			}
			else
			{
				
				
				// If the up direction is fixed, then make sure the rotation is 100% yaw (up axis).
				if( constraintRotation )
				{
					float signedAngle = Vector3.SignedAngle( Forward , value , Up );
					Quaternion deltaRotation = Quaternion.AngleAxis( signedAngle , Up );
					RotateInternal( deltaRotation );
				}
				else
				{
					Quaternion deltaRotation = Quaternion.FromToRotation( Forward , value.normalized );
					RotateInternal( deltaRotation );
				}

				

			}	

			
		}
	}

	/// <summary>
    /// Gets the current up direction based on the rigidbody rotation (not necessarily transform.right).
    /// </summary>
	public Vector3 Right
	{
		get
		{
			return RigidbodyComponent.Rotation * Vector3.right;			
		}
	}
	
	/// <summary>
	/// Rotates the character doing yaw rotation (around its vertical axis).
	/// </summary>
	/// <param name="angle">The angle in degrees.</param>
	public void SetYaw( float angle )
	{
		Forward = Quaternion.AngleAxis( angle , Up ) * Forward;			
	}


	// Contacts ──────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────

	/// <summary>
    /// Gets a list with all the current contacts.
    /// </summary>
	public List<Contact> Contacts
	{
		get
		{
			if( PhysicsComponent == null)
				return null;
			
			return PhysicsComponent.Contacts;
		}
	}



	// Triggers ──────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────

	/// <summary>
	/// Gets the most recent trigger.
	/// </summary>
	public Trigger CurrentTrigger
	{ 
		get
		{ 
			if( PhysicsComponent.Triggers.Count == 0 )
				return new Trigger();	// "Null trigger"
			
			return PhysicsComponent.Triggers[ PhysicsComponent.Triggers.Count - 1 ]; 
		} 
	}

	/// <summary>
	/// Gets a list with all the triggers.
	/// </summary>
	public List<Trigger> Triggers
	{ 
		get
		{			
			return PhysicsComponent.Triggers; 
		} 
	}
		

	public bool IsKinematic
	{
		get
		{
			return RigidbodyComponent.IsKinematic;
		}
		set
		{		
			RigidbodyComponent.IsKinematic = value;			
		}
	}

	public enum CharacterVelocityMode
	{		
		UseInputVelocity ,
		UsePreSimulationVelocity ,
		UsePostSimulationVelocity
	}	
			
	
	/// <summary>
	/// Gets the character velocity vector (Velocity) assigned prior to the FixedUpdate call. This is also known as the "input" velocity, 
	/// since it is the value the user has specified.
	/// </summary>
	public Vector3 InputVelocity { get; private set; }

	/// <summary>
	/// Gets a velocity vector which is the input velocity modified, based on the character actor internal rules (step up, slope limit, etc). 
	/// This velocity corresponds to the one used by the physics simulation.
	/// </summary>
	public Vector3 PreSimulationVelocity { get; private set; }

	/// <summary>
	/// Gets the character velocity as the result of the Physics simulation.
	/// </summary>
	public Vector3 PostSimulationVelocity{ get; private set; }

	/// <summary>
	/// Gets the difference between the post-simulation velocity (after the physics simulation) and the pre-simulation velocity (just before the physics simulation). 
	/// This value is useful to detect any external response due to the physics simulation, such as hits coming from other rigidbodies.
	/// </summary>
	public Vector3 ExternalVelocity { get; private set; }
	
	
	void HandleRotation( float dt )
	{     
		
		if( !constraintRotation ) 
			return;
		
		if( upDirectionReference != null )
		{               
			Vector3 targetPosition = Position + Velocity * dt;
			float sign = upDirectionReferenceMode == VerticalAlignmentSettings.VerticalReferenceMode.Towards ? 1f : -1f;

			constraintUpDirection =  sign * ( upDirectionReference.position - targetPosition ).normalized; 
			
		}
		
		Up = constraintUpDirection;		

    }

	
	Vector2 targetBodySize;

	void HandleSize( float dt )
	{
		BodySize = Vector2.Lerp( BodySize , targetBodySize , sizeLerpSpeed * dt );
		SetColliderSize();
    }

	List<Contact> wallContacts = new List<Contact>();

	/// <summary>
	/// Returns a lits of all the contacts involved with wall collision events.
	/// </summary>
	public List<Contact> WallContacts => wallContacts;


	List<Contact> headContacts = new List<Contact>();

	/// <summary>
	/// Returns a lits of all the contacts involved with head collision events.
	/// </summary>
	public List<Contact> HeadContacts => headContacts;

	void GetContactsInformation()
	{
		bool wasCollidingWithWall = characterCollisionInfo.wallCollision;
		bool wasCollidingWithHead = characterCollisionInfo.headCollision;
	  	
		wallContacts.Clear();
		headContacts.Clear();

		for( int i = 0 ; i < Contacts.Count ; i++ )
		{
			Contact contact = Contacts[i];
			
			float verticalAngle = Vector3.Angle( Up , contact.normal );
			
			// Get the wall information -------------------------------------------------------------
			if( CustomUtilities.isCloseTo( verticalAngle , 90f , CharacterConstants.WallContactAngleTolerance ) )
				wallContacts.Add( contact );
			

			// Get the head information -----------------------------------------------------------------
			if( verticalAngle >= CharacterConstants.HeadContactMinAngle )
				headContacts.Add( contact );
			
			
		}


		if( wallContacts.Count == 0 )
		{
			characterCollisionInfo.ResetWallInfo();	
		}
		else
		{
			Contact wallContact = wallContacts[0];

			characterCollisionInfo.SetWallInfo( wallContact , this );

			if( !wasCollidingWithWall )
			{
				if( OnWallHit != null )
					OnWallHit( wallContact );
			}

		}
	

		if( headContacts.Count == 0 )
		{
			characterCollisionInfo.ResetHeadInfo();	
		}
		else
		{
			Contact headContact = headContacts[0];

			characterCollisionInfo.SetHeadInfo( headContact , this );

			if( !wasCollidingWithHead )
			{
				if( OnHeadHit != null )
					OnHeadHit( headContact );
			}
		}


	}


	

	
	protected override void PreSimulationUpdate( float dt )
	{
		WasGrounded = IsGrounded;
        WasStable = IsStable;		
		
		InputVelocity = Velocity;
		
		HandleSize( dt );
		HandlePosition( dt );
		
		PreSimulationVelocity = Velocity;		
		
		// ------------------------------------------------------------
		
		if( IsStable )
		{
			StableElapsedTime += dt;
			UnstableElapsedTime = 0f;
		}
		else
		{
			StableElapsedTime = 0f;
			UnstableElapsedTime += dt;
		}
		
		if( IsGrounded )
		{
			NotGroundedTime = 0f;
			GroundedTime += dt;

			LastGroundedVelocity = Velocity;

			if( !WasGrounded )
				if( OnGroundedStateEnter != null )
					OnGroundedStateEnter( LocalVelocity );
			
		}
		else
		{
			NotGroundedTime += dt;
			GroundedTime = 0f;		
			
			if( WasGrounded )
				if( OnGroundedStateExit != null )
					OnGroundedStateExit();	
				
		}
		
		if( forceNotGroundedFrames != 0 )
			forceNotGroundedFrames--;

		// After the simulation the contacts list is updated. A "clear" needs to be perform before the simulation.
		PhysicsComponent.ClearContacts();
	}

	

	

	
	protected override void PostSimulationUpdate( float dt )
	{
		HandleRotation( dt );

		GetContactsInformation();				
		
		Velocity -= StableProbeGroundVelocity;		
		PostSimulationVelocity = Velocity;	
		ExternalVelocity = PostSimulationVelocity - PreSimulationVelocity;


		if( IsStable )
		{
			if( supportDynamicGround )
			{
				if( IsGroundARigidbody )
				{
					// The ground might hit the character really hard (e.g. fast ascending platform), causing this to get some extra velocity (unwanted behaviour). 
					// So, ignore any incoming velocity from the ground by replacing it with the input velocity.
					IgnoreGroundCollision();


					Vector3 targetPosition = Position;
					Quaternion targetRotation = Rotation;

					ProcessDynamicGround( ref targetPosition , ref targetRotation , dt );
					
					
					if( GroundDeltaVelocity.magnitude > maxGroundVelocityChange )
					{
						float upToDynamicGroundVelocityAngle = Vector3.Angle( GroundVelocity.normalized , Up );

						// WIP!
						if( upToDynamicGroundVelocityAngle < 45f )
							ForceNotGrounded( 0.1f );
						
						RigidbodyComponent.Velocity = PreviousGroundVelocity;
						RigidbodyComponent.Position += PreviousGroundVelocity * dt;
						RigidbodyComponent.Rotation = targetRotation;

					}
					else
					{
						CollisionInfo collisionInfo;
						bool hit = characterCollisions.CastBody( 
							out collisionInfo ,
							Position ,
							targetPosition - Position ,
							0f ,
							new HitInfoFilter( 
								ObstaclesLayerMask ,
								false ,
								true 
							)
						);

						bool validHit = hit && ( collisionInfo.hitInfo.transform != GroundTransform );

						// Important! Do not write "Position" and "Rotation" properties (they reset interpolation).
						if( validHit )
						{						
							RigidbodyComponent.Position += collisionInfo.displacement;						
						}
						else
						{
							RigidbodyComponent.Position = targetPosition;
							RigidbodyComponent.Rotation = targetRotation;
						}

					
						
						
					}
					
															
				}
			}
			

		}		

		// Velocity assignment ------------------------------------------------------
		SetPostSimulationVelocity();
		

	}

	void SetPostSimulationVelocity()
	{
		if( IsStable )
		{

			switch( stablePostSimulationVelocity )
			{
				case CharacterVelocityMode.UseInputVelocity:

					Velocity = InputVelocity;

					break;
				case CharacterVelocityMode.UsePreSimulationVelocity:

					Velocity = PreSimulationVelocity;		
					
					// Take the rigidbody velocity and convert that into planar velocity
					if( WasStable )					
						PlanarVelocity = Velocity.magnitude * PlanarVelocity.normalized;	
					
					break;
				case CharacterVelocityMode.UsePostSimulationVelocity:

					// Take the rigidbody velocity and convert that into planar velocity
					if( WasStable )					
						PlanarVelocity = Velocity.magnitude * PlanarVelocity.normalized;

					break;
			}

			
		}
		else
		{
			switch( unstablePostSimulationVelocity )
			{
				case CharacterVelocityMode.UseInputVelocity:

					Velocity = InputVelocity;

					break;
				case CharacterVelocityMode.UsePreSimulationVelocity:

					Velocity = PreSimulationVelocity;			
					
					break;
				case CharacterVelocityMode.UsePostSimulationVelocity:

					break;
			}
		}
	}

	
	void IgnoreGroundCollision()
	{
		for( int i = 0 ; i < Contacts.Count ; i++ )
		{
			if( !Contacts[i].isRigidbody )
				continue;

			if( !Contacts[i].isKinematicRigidbody )
				continue;

			if( Contacts[i].gameObject.transform == GroundTransform )
			{
				Velocity = InputVelocity;
				break;
			}
		}
	}
	
	
	#region Events

	

	/// <summary>
	/// This event is called when the character hits its head (not grounded).
	/// 
	/// The related collision information struct is passed as an argument.
	/// </summary>
	public event System.Action<Contact> OnHeadHit;

	/// <summary>
	/// This event is called everytime the character is blocked by an unallowed geometry, this could be
	/// a wall or a steep slope (depending on the "slopeLimit" value).
	/// 
	/// The related collision information struct is passed as an argument.
	/// </summary>
	public event System.Action<Contact> OnWallHit;

	/// <summary>
	/// This event is called everytime the character teleports.
	/// 
	/// The teleported position and rotation are passed as arguments.
	/// </summary>
	public event System.Action<Vector3,Quaternion> OnTeleport;	

	/// <summary>
	/// This event is called when the character enters the grounded state.
	/// 
	/// The local linear velocity is passed as an argument.
	/// </summary>
	public event System.Action<Vector3> OnGroundedStateEnter;

	/// <summary>
	/// This event is called when the character exits the grounded state.
	/// </summary>
	public event System.Action OnGroundedStateExit;
	

	#endregion
	

	/// <summary>
	/// Sets the teleportation position and rotation using an external Transform reference. 
	/// The character will move/rotate internally using its own internal logic.
	/// </summary>
	public void Teleport( Transform reference )
	{
		Teleport( reference.position , reference.rotation );

	}	

	/// <summary>
	/// Sets the teleportation position and rotation. 
	/// The character will move/rotate internally using its own internal logic.
	/// </summary>
	public void Teleport( Vector3 position , Quaternion rotation )
	{
		Position = position;
		Rotation = rotation;

		if( OnTeleport != null )
			OnTeleport( Position , Rotation );
	}	

	/// <summary>
	/// Sets the teleportation position. 
	/// The character will move/rotate internally using its own internal logic.
	/// </summary>
	public void Teleport( Vector3 position )
	{
		Position = position;

		if( OnTeleport != null )
			OnTeleport( Position , Rotation );

	}
			

	/// <summary>
	/// Gets the velocity of the ground (rigidbody).
	/// </summary>
	public Vector3 GroundVelocity { get; private set; }

	/// <summary>
	/// Gets the previous velocity of the ground (rigidbody).
	/// </summary>
	public Vector3 PreviousGroundVelocity { get; private set; }

	/// <summary>
	/// The ground change in velocity (current velocity - previous velocity).
	/// </summary>
	public Vector3 GroundDeltaVelocity => GroundVelocity - PreviousGroundVelocity;

	/// <summary>
	/// The ground acceleration (GroundDeltaVelocity / dt).
	/// </summary>
	public Vector3 GroundAcceleration => ( GroundVelocity - PreviousGroundVelocity ) / Time.fixedDeltaTime;

	
	Vector3 stableProbeGroundVelocity = default( Vector3 );
	
	/// <summary>
	/// The character velocity calculated when performing the ground check algorithm.
	/// </summary>
	public Vector3 StableProbeGroundVelocity
	{
		get
		{
			return IsStable ? stableProbeGroundVelocity : Vector3.zero;
		}
	}
	
	/// <summary>
	/// Returns true if the ground vertical displacement (moving ground) is positive.
	/// </summary>
	public bool IsGroundAscending => transform.InverseTransformVectorUnscaled( Vector3.Project( GroundVelocity * Time.deltaTime , Up ) ).y > 0;

#if UNITY_TERRAIN_MODULE

	/// <summary>
	/// Gets the current terrain the character is standing on.
	/// </summary>
	public Terrain CurrentTerrain { get; private set; }

	/// <summary>
	/// Returns true if the character is standing on a terrain.
	/// </summary>
	public bool IsOnTerrain => CurrentTerrain != null;

#endif


	bool IsAllowedToFollowRigidbodyReference => IsStable && supportDynamicGround && IsGroundARigidbody;

	
	void ProcessDynamicGroundData( Vector3 position )
	{				
		if( IsAllowedToFollowRigidbodyReference )
		{
			preSimulationGroundRotation = GroundRotation;
			preSimulationGroundPosition = GroundPosition;
			groundToCharacter = position - GroundPosition;
		}
		else
		{
			GroundVelocity = Vector3.zero;
			PreviousGroundVelocity = Vector3.zero;
		}
		
	}
	

	void HandlePosition( float dt )
	{

		Vector3 position = Position;
		
		if( alwaysNotGrounded )
			ForceNotGrounded();

		if( IsKinematic )
		{
			// if( !IsGrounded )
			// {				
			// 	Move( position );
			// }
			
		}
		else
		{			
			
			if( IsStable )
			{		

				VerticalVelocity = Vector3.zero;

				ApplyWeight( GroundContactPoint );

				
				Vector3 displacement = CustomUtilities.ProjectVectorOnPlane( 
					Velocity * dt ,
					GroundStableNormal ,
					Up
				);

				CollideAndSlide( ref position , displacement , false );		

				
				ProbeGround( ref position , dt );
				
				ProcessDynamicGroundData( position );
				
				if( !IsStable )
				{

#if UNITY_TERRAIN_MODULE
					CurrentTerrain = null;
#endif
					groundRigidbodyComponent = null;
				}
						
			}
			else
			{							
				
				CollideAndSlideUnstable( ref position , Velocity * dt );
				
			}
			
			Move( position );
			

		}
		
	}


	Vector3 preSimulationGroundPosition;
	Quaternion preSimulationGroundRotation;

	

	/// <summary>
	/// Sets the rigidbody velocity based on a target position. The same can be achieved by setting the velocity value manually.
	/// </summary>
	public void Move( Vector3 position )
	{		
		RigidbodyComponent.Move( position );
	}


	void RotateInternal( Quaternion deltaRotation )
	{
		Vector3 preRotationCenter = IsGrounded ? GetBottomCenter( Position ) : GetCenter( Position );

		RigidbodyComponent.Rotation = deltaRotation * RigidbodyComponent.Rotation;

		Vector3 postRotationCenter = IsGrounded ? GetBottomCenter( Position ) : GetCenter( Position );

		RigidbodyComponent.Position += preRotationCenter - postRotationCenter; 
	}
	

	Vector3 groundToCharacter;

	void ProcessDynamicGround( ref Vector3 position , ref Quaternion rotation , float dt )
	{
		if( WasStable )
		{
			Quaternion deltaRotation = GroundRotation * Quaternion.Inverse( preSimulationGroundRotation );

			Vector3 localGroundToCharacter = GroundTransform.InverseTransformVectorUnscaled( groundToCharacter );		
			Vector3 rotatedGroundToCharacter = GroundTransform.rotation * localGroundToCharacter;			
			
			position = GroundPosition + ( deltaRotation * groundToCharacter );

			
			if( !Is2D && rotateForwardDirection ) 
			{	
				// Quaternion deltaRotation = referenceRigidbodyRotation * Quaternion.Inverse( GroundTransform.rotation );
				Vector3 forward = deltaRotation * Forward;
				forward = Vector3.ProjectOnPlane( forward , Up ).normalized;
				rotation = Quaternion.LookRotation( forward , Up );
			}
			

			PreviousGroundVelocity = GroundVelocity;	
			GroundVelocity = ( position - Position ) / dt;
	
		}
		else
		{
			GroundVelocity = Vector3.zero;
			PreviousGroundVelocity = Vector3.zero;
			
		}
		
			
	}
	
	
	/// <summary>
	/// Checks if the new character size fits in place. If this check is valid then the real size of the character is changed.
	/// </summary>
	public bool SetBodySize( Vector2 size )
	{ 

		HitInfoFilter filter = new HitInfoFilter( 
			ObstaclesWithoutOWPLayerMask , 
			true ,
			true 
		);

        if( !characterCollisions.CheckBodySize( size , Position , filter ) )
			return false;

		targetBodySize = size;

		return true;
	}

	/// <summary>
	/// Forces the character to be grounded (isGrounded = true) if possible. The detection distance includes the step down distance.
	/// </summary>
	public void ForceGrounded()
	{		
		HitInfoFilter filter = new HitInfoFilter( 
			ObstaclesLayerMask , 
			false , 
			true ,
			oneWayPlatformsLayerMask
		);

		CollisionInfo collisionInfo;
		bool hit = characterCollisions.CheckForGround( 
			out collisionInfo ,
			Position , 
			BodySize.y * 0.8f , // 80% of the height
			stepDownDistance ,
			filter
		);
		

		if( hit )
		{

			ProcessNewGround( collisionInfo.hitInfo.transform , collisionInfo );

			float slopeAngle = Vector3.Angle( Up , GetGroundSlopeNormal( collisionInfo ) );			

			if( slopeAngle <= slopeLimit )
			{
				// Save the ground collision info
				characterCollisionInfo.SetGroundInfo( collisionInfo , this );
				Position += collisionInfo.displacement;				
				
			}
			
		}
	}
	
	
	public Vector3 GetGroundSlopeNormal( CollisionInfo collisionInfo )
	{		

#if UNITY_TERRAIN_MODULE
		if( IsOnTerrain ) 
			return collisionInfo.hitInfo.normal;
#endif
		
		float contactSlopeAngle = Vector3.Angle( Up , collisionInfo.hitInfo.normal );
		if( collisionInfo.isAnEdge )
		{			
			if( contactSlopeAngle < slopeLimit && collisionInfo.edgeUpperSlopeAngle <= slopeLimit && collisionInfo.edgeLowerSlopeAngle <= slopeLimit )
			{
				return Up;
			}
			else if( collisionInfo.edgeUpperSlopeAngle <= slopeLimit )
			{
				return collisionInfo.edgeUpperNormal;
			}
			else if( collisionInfo.edgeLowerSlopeAngle <= slopeLimit )
			{
				return collisionInfo.edgeLowerNormal;
			}
			else
			{
				return collisionInfo.hitInfo.normal;
			}
		}
		else
		{
			return collisionInfo.hitInfo.normal;
		}

		

	}

	void ProbeGround( ref Vector3 position , float dt )
	{
		Vector3 preProbeGroundPosition = position;

		float groundCheckDistance = edgeCompensation ? 
		BodySize.x / 2f + CharacterConstants.GroundCheckDistance :
		CharacterConstants.GroundCheckDistance;

		Vector3 displacement = - Up * Mathf.Max( groundCheckDistance , stepDownDistance );		
		
		HitInfoFilter filter = new HitInfoFilter( 
			ObstaclesLayerMask , 
			false , 
			true 
		);
		
				

		CollisionInfo collisionInfo;
		bool hit = characterCollisions.CheckForGround( 
			out collisionInfo ,
			position ,
			StepOffset ,
			stepDownDistance ,
			filter
		);
		
		if( hit )
		{
			
			float slopeAngle = Vector3.Angle( Up , GetGroundSlopeNormal( collisionInfo ) );
			
			if( slopeAngle <= slopeLimit )
			{						
				// Stable hit ---------------------------------------------------				
				ProcessNewGround( collisionInfo.hitInfo.transform , collisionInfo );

				// Save the ground collision info
				characterCollisionInfo.SetGroundInfo( collisionInfo , this );

				

				// Calculate the final position 
				position += collisionInfo.displacement;			
				

				if( edgeCompensation && IsAStableEdge( collisionInfo ) )
				{
					// calculate the edge compensation and apply that to the final position
					Vector3 compensation = Vector3.Project( ( collisionInfo.hitInfo.point - position ) , Up );
					position += compensation;					
				}
				
				stableProbeGroundVelocity = ( position - preProbeGroundPosition ) / dt;
			}
			else
			{
				// Unstable Hit
				
				// If the unstable ground is far enough then force not grounded
				float castSkinDistance = StepOffset + 2f * CharacterConstants.SkinWidth;
				if( collisionInfo.hitInfo.distance > castSkinDistance )
				{
					
					ForceNotGrounded();
					return;
				}


				if( preventBadSteps )
				{
					// If the unstable ground is close enough then do a new collide and slide
					if( WasGrounded )
					{				
						position = Position;	

						Vector3 unstableDisplacement = CustomUtilities.ProjectVectorOnPlane( 
							Velocity * dt ,
							GroundStableNormal ,
							Up
						);
						

						CollideAndSlide( ref position , unstableDisplacement , true );
					}
				}

				
				characterCollisions.CheckForGroundRay( 
					out collisionInfo , 
					position , 
					StepOffset , 
					stepDownDistance , 
					filter
				);		

				ProcessNewGround( collisionInfo.hitInfo.transform , collisionInfo );
				
				characterCollisionInfo.SetGroundInfo( collisionInfo , this );

				stableProbeGroundVelocity = ( position - preProbeGroundPosition ) / dt;

			}
			
			
			
		}
		else
		{			
			ForceNotGrounded();
			
			
		}	
			
	}


	int forceNotGroundedFrames = 0;

	/// <summary>
	/// Forces the character to abandon the grounded state (isGrounded = false). 
	/// 
	/// TIP: This is useful when making the character jump.
	/// </summary>
	/// /// <param name="groundVelocityInfluenceMultiplier">How much of the velocity coming from the ground should be applied to the character (multiplier).</param>
	/// <param name="ignoreGroundContactFrames">The number of FixedUpdate frames to consume in order to prevent the character to 
	/// re-enter grounded state right after a ForceNotGrounded call. For example, if this number is three (default) then the character will be able to enter grounded state 
	/// after four frames since the original call.</param>
	public void ForceNotGrounded( float groundVelocityInfluenceMultiplier = 1f , int ignoreGroundContactFrames = 3 )
	{		
		forceNotGroundedFrames = ignoreGroundContactFrames;

		bool wasGrounded = IsGrounded;

		if( GroundVelocity.magnitude >= maxForceNotGroundedGroundVelocity )
			Velocity += groundVelocityInfluenceMultiplier * GroundVelocity;

		GroundVelocity = Vector3.zero;
		PreviousGroundVelocity = Vector3.zero;


		characterCollisionInfo.ResetGroundInfo();
	}

	
	bool IsAStableEdge( CollisionInfo collisionInfo )
	{
		return collisionInfo.isAnEdge && collisionInfo.edgeUpperSlopeAngle <= slopeLimit;
	}

	bool IsAnUnstableEdge( CollisionInfo collisionInfo )
	{
		return collisionInfo.isAnEdge && collisionInfo.edgeUpperSlopeAngle > slopeLimit;
	}	

	
	
	protected void CollideAndSlide( ref Vector3 position , Vector3 displacement , bool useFullBody )
	{
		
		Vector3 groundPlaneNormal = GroundStableNormal;
		Vector3 slidingPlaneNormal = Vector3.zero;  

		HitInfoFilter filter = new HitInfoFilter( 
			ObstaclesLayerMask ,
			false ,
			true ,
			oneWayPlatformsLayerMask
		);

		int iteration = 0;
		
		
		while( iteration < CharacterConstants.MaxSlideIterations )
        {
			iteration++;	

			CollisionInfo collisionInfo;
			bool hit = characterCollisions.CastBody(
				out collisionInfo ,
				position ,
				displacement ,
				useFullBody ? 0f : StepOffset ,
				filter
			);
			
			
			if( hit )
			{
				// Even while being stable, if a one way platform is hit then ignore it (physics).				

				if( CheckOneWayPlatform( collisionInfo ) )
				{
					position += displacement;
					break;
				}

				if( canPushDynamicRigidbodies )
				{
					if( collisionInfo.hitInfo.IsRigidbody )
					{
						if( collisionInfo.hitInfo.IsDynamicRigidbody )
						{
							bool canPushThisObject = CustomUtilities.BelongsToLayerMask( collisionInfo.hitInfo.transform.gameObject.layer , pushableRigidbodyLayerMask );
							if( canPushThisObject )
							{
								// Use the entire displacement and stop the collide and slide
								position += displacement;			
								break;					
							}
						}

					}
				}
				

				if( slideOnWalls && !Is2D )
				{
					position += collisionInfo.displacement;
					displacement -= collisionInfo.displacement;

					// Get the new slide plane
					bool blocked = UpdateSlidingPlanes( 
						collisionInfo , 
						ref slidingPlaneNormal , 
						ref groundPlaneNormal , 
						ref displacement
					);
				}
				else
				{
					if( !WallCollision )
						position += collisionInfo.displacement;
					
					break;
				}		
				
				
				
			}
			else
			{
				position += displacement;			
				break;
			}
			
		}

	}

	/// <summary>
	/// Returns true if the current ground layer is considered as a one way platform.
	/// </summary>
	public bool IsGroundAOneWayPlatform => CustomUtilities.BelongsToLayerMask( GroundObject.layer , oneWayPlatformsLayerMask );
	

	protected bool CheckOneWayPlatform( CollisionInfo collisionInfo )
	{
		int collisionLayer = collisionInfo.hitInfo.transform.gameObject.layer;
		return CustomUtilities.BelongsToLayerMask( collisionLayer , oneWayPlatformsLayerMask );		
	}	
		
	protected void CollideAndSlideUnstable( ref Vector3 position , Vector3 displacement )
	{         

		HitInfoFilter filter = new HitInfoFilter( 
			ObstaclesLayerMask ,
			false ,
			true ,
			oneWayPlatformsLayerMask
		);

		
		int iteration = 0;		

		// Used to determine if the character should collide or not with the OWP.
		bool isValidOWP = false;

		while( iteration < CharacterConstants.MaxSlideIterations || displacement == Vector3.zero )
        {
			iteration++;			

			CollisionInfo collisionInfo;
			bool hit = characterCollisions.CastBody(
				out collisionInfo ,
				position ,
				displacement ,
				0f ,
				filter
			);

			if( hit )
			{
				
				if( CheckOneWayPlatform( collisionInfo ) )
				{	

					Vector3 currentPosition = position;
					Vector3 nextPosition = currentPosition + collisionInfo.displacement;	
					
					Vector3 bottomToHitPoint = collisionInfo.hitInfo.point - GetBottomCenter( nextPosition );
					isValidOWP = transform.InverseTransformVectorUnscaled( Vector3.Project( bottomToHitPoint , Up ) ).y < 0;
					
					// // Check one way platforms
					if( !isValidOWP )
					{	
						position += displacement;
						break;
					}
				}
				
				if( canPushDynamicRigidbodies )
				{
					if( collisionInfo.hitInfo.IsRigidbody )
					{
						if( collisionInfo.hitInfo.IsDynamicRigidbody )
						{							
							bool canPushThisObject = CustomUtilities.BelongsToLayerMask( collisionInfo.hitInfo.transform.gameObject.layer , pushableRigidbodyLayerMask );
							if( canPushThisObject )
							{
								position += displacement;			
								break;					
							}
						}
						
					}
				}

				// Fall back to this
				position += collisionInfo.displacement;	
				displacement -= collisionInfo.displacement;	
				
				displacement = Vector3.ProjectOnPlane( displacement , collisionInfo.hitInfo.normal );
			}
			else
			{				
				position += displacement;
				break;
				
			}
			
		}
			
				
		
		if( !alwaysNotGrounded && forceNotGroundedFrames == 0)
		{
			HitInfoFilter groundCheckFilter = new HitInfoFilter( 
				isValidOWP? ObstaclesLayerMask : ObstaclesWithoutOWPLayerMask ,
				false ,
				true
			);

			CollisionInfo groundCheckCollisionInfo;
			characterCollisions.CheckForGround( 
				out groundCheckCollisionInfo , 
				position , 
				StepOffset ,
				CharacterConstants.GroundPredictionDistance , 
				groundCheckFilter
			);

			if( groundCheckCollisionInfo.collision )
			{			
				PredictedGround = groundCheckCollisionInfo.hitInfo.transform.gameObject;
				PredictedGroundDistance = groundCheckCollisionInfo.displacement.magnitude;
				
				bool validGround = PredictedGroundDistance <= CharacterConstants.GroundCheckDistance;

				if( validGround )
				{		
					ProcessNewGround( groundCheckCollisionInfo.hitInfo.transform , groundCheckCollisionInfo );
					characterCollisionInfo.SetGroundInfo( groundCheckCollisionInfo , this );
				}
				else
				{
					characterCollisionInfo.ResetGroundInfo();
				}

			}
			else
			{	
				PredictedGround = null;
				PredictedGroundDistance = 0f;

				characterCollisionInfo.ResetGroundInfo();
				
			}
			
			
			
			
		}
		

	}

	/// <summary>
	/// Gets the object below the character (only valid if the character is falling). The maximum prediction distance is defined by the constant "GroundPredictionDistance".
	/// </summary>
	public GameObject PredictedGround { get; private set; }

	/// <summary>
	/// Gets the distance to the "PredictedGround".
	/// </summary>
	public float PredictedGroundDistance { get; private set; }

	
	
	void ProcessNewGround( Transform newGroundTransform , CollisionInfo collisionInfo )
	{
		bool isThisANewGround = collisionInfo.hitInfo.transform != GroundTransform;
		if( isThisANewGround )
		{
#if UNITY_TERRAIN_MODULE
			CurrentTerrain = terrains.GetOrRegisterValue<Transform , Terrain>( newGroundTransform );  
#endif
			groundRigidbodyComponent = groundRigidbodyComponents.GetOrRegisterValue<Transform , RigidbodyComponent>( newGroundTransform );
		}
	}
			
	bool UpdateSlidingPlanes( CollisionInfo collisionInfo , ref Vector3 slidingPlaneNormal , ref Vector3 groundPlaneNormal , ref Vector3 displacement )
	{
		Vector3 normal = collisionInfo.hitInfo.normal;	

		if( collisionInfo.contactSlopeAngle > slopeLimit )
		{    
			
			if( slidingPlaneNormal != Vector3.zero )
			{
				float correlation = Vector3.Dot( normal , slidingPlaneNormal );
				
				if( correlation > 0 )
					displacement = CustomUtilities.DeflectVector( displacement , groundPlaneNormal , normal );
				else
					displacement = Vector3.zero;                            
				
			}
			else
			{
				displacement = CustomUtilities.DeflectVector( displacement , groundPlaneNormal , normal );
			}

			slidingPlaneNormal = normal;                     
		}
		else
		{
			displacement = CustomUtilities.ProjectVectorOnPlane( 
				displacement , 
				normal ,
                Up 
			);

			groundPlaneNormal = normal;
			slidingPlaneNormal = Vector3.zero;

		}

		return displacement == Vector3.zero;
	}
	
	
	void OnDrawGizmos()
	{				
		if( CharacterBody == null )
			CharacterBody = GetComponent<CharacterBody>();

		Gizmos.color = new Color( 1f , 1f , 1f , 0.2f );
		
		Gizmos.matrix = transform.localToWorldMatrix;
		Vector3 origin = Vector3.up * stepUpDistance;
		Gizmos.DrawWireCube( 
			origin , 
			new Vector3( 1.1f * CharacterBody.BodySize.x , 0.02f , 1.1f * CharacterBody.BodySize.x )
		);

		Gizmos.matrix = Matrix4x4.identity;

	}


}


}