using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lightbug.Utilities
{

/// <summary>
/// This component is an encapsulation of the Physics and Physics2D classes, that serves as a physcics utility class.
/// </summary>
public abstract class PhysicsComponent : MonoBehaviour
{

	protected int hits = 0;
	
	/// <summary>
    /// Gets a list with all the current contacts.
    /// </summary>
	public List<Contact> Contacts { get; protected set; } = new List<Contact>();

	/// <summary>
    /// Gets a list with all the current triggers.
    /// </summary>
	public List<Trigger> Triggers { get; protected set; } = new List<Trigger>();	
	
	protected abstract LayerMask GetCollisionLayerMask();

	/// <summary>
	/// Ignores the collision between this object and some other collider.
	/// </summary>
	public abstract void IgnoreCollision( HitInfo hitInfo , bool ignore );

	/// <summary>
	/// Ignores the collision between this object and a layer.
	/// </summary>
	public abstract void IgnoreLayerCollision( int targetLayer , bool ignore );

	/// <summary>
	/// Ignores the collision between this object and a layer mask.
	/// </summary>
	public abstract void IgnoreLayerMaskCollision( LayerMask layerMask , bool ignore );

	protected abstract void IgnoreOverlappedColliders( LayerMask ignoredLayerMask );

	// Contacts ──────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────
	public void ClearContacts()
	{	
		Contacts.Clear();
	}

	protected abstract void GetClosestHit( out HitInfo hitInfo , Vector3 castDisplacement , HitInfoFilter filter );

	// Casts ──────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────
	
	/// <summary>
	/// Raycast wrapper for 2D/3D physics.
	/// </summary>
	public abstract int Raycast( out HitInfo hitInfo , Vector3 origin , Vector3 castDisplacement , HitInfoFilter filter );

	/// <summary>
	/// SphereCast wrapper for 2D/3D physics.
	/// </summary>
	public abstract int SphereCast( out HitInfo hitInfo , Vector3 center , float radius , Vector3 castDisplacement , HitInfoFilter filter );

	/// <summary>
	/// CapsuleCast wrapper for 2D/3D physics.
	/// </summary>
	public abstract int CapsuleCast( out HitInfo hitInfo , Vector3 bottom , Vector3 top , float radius  , Vector3 castDisplacement , HitInfoFilter filter );
    

    // Overlaps ──────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────

	/// <summary>
	/// OverlapSphere wrapper for 2D/3D physics.
	/// </summary>
    public abstract bool OverlapSphere( Vector3 center , float radius , HitInfoFilter filter );
	
	/// <summary>
	/// OverlapCapsule wrapper for 2D/3D physics.
	/// </summary>
    public abstract bool OverlapCapsule( Vector3 bottom , Vector3 top , float radius , HitInfoFilter filter );

	/// <summary>
	/// Returns a layer mask with all the valid collisions associated with the object, based on the collision matrix (physics settings).
	/// </summary>
	public LayerMask CollisionLayerMask { get; private set; } = 0;

	protected virtual void Awake()
	{
		this.hideFlags = HideFlags.None;
		
		CollisionLayerMask = GetCollisionLayerMask();
	}

	RigidbodyComponent rigidbodyComponent = null;

	protected virtual void Start()
	{
		rigidbodyComponent = GetComponent<RigidbodyComponent>();
	}

	protected bool ignoreCollisionMessages = false;


	Coroutine postSimulationCoroutine = null;

	void OnEnable()
	{
		rigidbodyComponent = GetComponent<RigidbodyComponent>();
		
		if( rigidbodyComponent != null )
			rigidbodyComponent.OnBodyTypeChange += OnBodyTypeChange;
				
		if( postSimulationCoroutine == null )
			postSimulationCoroutine = StartCoroutine( PostSimulationUpdate() );
	}

	void OnDisable()
	{
		if( rigidbodyComponent != null )
			rigidbodyComponent.OnBodyTypeChange -= OnBodyTypeChange;
	}

	void OnBodyTypeChange()
	{
		ignoreCollisionMessages = true;
	}

	void FixedUpdate()
	{
		// Update the collision layer mask (collision matrix) of this object.
		CollisionLayerMask = GetCollisionLayerMask();

		// If there are null triggers then delete them from the list
		for( int i = Triggers.Count - 1 ; i >= 0 ; i-- )
		{					
			if( Triggers[i].gameObject == null )
				Triggers.RemoveAt( i );
		}
		
	}

	IEnumerator PostSimulationUpdate()
	{
		YieldInstruction waitForFixedUpdate = new WaitForFixedUpdate();
		while( true )
		{
			yield return waitForFixedUpdate;
			
			ignoreCollisionMessages = false;
		}
	}

	

	protected bool wasKinematic = false;
}

}

