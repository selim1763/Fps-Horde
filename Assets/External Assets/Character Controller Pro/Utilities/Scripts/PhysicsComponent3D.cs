using UnityEngine;

namespace Lightbug.Utilities
{

/// <summary>
/// An implementation of a PhysicsComponent for 3D physics.
/// </summary>
public sealed class PhysicsComponent3D : PhysicsComponent
{
    Collider[] colliders = null;
	RaycastHit[] raycastHits = new RaycastHit[10];
	Collider[] overlappedColliders = new Collider[10];

    ContactPoint[] contactsBuffer = new ContactPoint[10];

    new Rigidbody rigidbody = null;

    protected override void Awake()
    {
        base.Awake();
        
        
        colliders = GetComponentsInChildren<Collider>();
    }

    protected override void Start()
    {
        base.Start();
        
        rigidbody = GetComponent<Rigidbody>();
    }
    
    

    void OnTriggerStay( Collider other )
    {        
        if( ignoreCollisionMessages )
            return;
        
        
        bool found = false;

        Trigger trigger = new Trigger();

        for( int i = 0 ; i < Triggers.Count ; i++ )
        {
            if( Triggers[i].gameObject != other.gameObject )
                continue;
            
            found = true;

            

            // Ignore old Triggers
            if( !Triggers[i].firstContact )
                continue;
            
            // Set the firstContact field to false
            trigger = Triggers[i];
            trigger.firstContact = false;
            Triggers[i] = trigger;


            break;
            
        }

        // First contact
        if( !found )
        {            
            trigger.Set( true , other );
            Triggers.Add( trigger );
        }
        
    }

    void OnTriggerExit( Collider other )
    {
        if( ignoreCollisionMessages )
            return;

        for( int i = Triggers.Count - 1 ; i >= 0 ; i-- )
        {            
            
            if( Triggers[i].collider3D == other )
            {
                Triggers.RemoveAt( i );

                break;
            }
        }
    }
    
    void OnCollisionEnter( Collision collision )
    {
        if( ignoreCollisionMessages )
            return;

        int bufferHits = collision.GetContacts( contactsBuffer );
        
        // Add the contacts to the list
        for( int i = 0 ; i < bufferHits ; i++ )
        {
            ContactPoint contact = contactsBuffer[i];

            Contact outputContact = new Contact();

            outputContact.Set( true , contact , collision );

            Contacts.Add( outputContact );
        } 

        
    }

    void OnCollisionStay( Collision collision )
    {
        if( ignoreCollisionMessages )
            return;
            
        int bufferHits = collision.GetContacts( contactsBuffer );
        
        // Add the contacts to the list
        for( int i = 0 ; i < bufferHits ; i++ )
        {
            ContactPoint contact = contactsBuffer[i];

            Contact outputContact = new Contact();

            outputContact.Set( false , contact , collision );

            Contacts.Add( outputContact );
        }
    }


    protected override LayerMask GetCollisionLayerMask()
    {
        int objectLayer = gameObject.layer;
        LayerMask output = 0;

		for( int i = 0 ; i < 32 ; i++ )
		{            
			bool exist = !Physics.GetIgnoreLayerCollision( i , objectLayer );            
            
            if( exist )
                output.value += 1 << i;
		}

        return output;
    }

    public override void IgnoreCollision( HitInfo hitInfo , bool ignore )
    {
        if( hitInfo.collider3D == null )
            return;
        
        for( int i = 0 ; i < colliders.Length ; i++ )
            Physics.IgnoreCollision( colliders[i] , hitInfo.collider3D , ignore );
    }

    public override void IgnoreLayerCollision( int targetLayer , bool ignore )
    {
        Physics.IgnoreLayerCollision( gameObject.layer , targetLayer , ignore );
    }

    public override void IgnoreLayerMaskCollision( LayerMask layerMask , bool ignore )
    {
        int layerMaskValue = layerMask.value;
        int currentLayer = 1;

		for( int i = 0 ; i < 32 ; i++ )
		{
			bool exist = ( layerMaskValue & currentLayer ) > 0;

            if( exist )
                IgnoreLayerCollision( i , ignore );

            currentLayer <<= 1;
		}
        
    }

    

    // Casts ──────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────
    
    public override int Raycast(out HitInfo hitInfo, Vector3 origin, Vector3 castDisplacement, HitInfoFilter hitInfoFilter )
    {
        hits = Physics.RaycastNonAlloc(
			origin ,
			castDisplacement.normalized ,            
            raycastHits ,
            castDisplacement.magnitude ,
			hitInfoFilter.collisionLayerMask ,
            hitInfoFilter.ignoreTriggers ? QueryTriggerInteraction.Ignore : QueryTriggerInteraction.Collide
		);  
        
        GetClosestHit( out hitInfo , castDisplacement , hitInfoFilter );

        return hits;
    }


	public override int CapsuleCast( out HitInfo hitInfo , Vector3 bottom , Vector3 top , float radius  , Vector3 castDisplacement , HitInfoFilter hitInfoFilter )
    {        
        hits = Physics.CapsuleCastNonAlloc(
            bottom ,
            top ,  
            radius ,         
            castDisplacement.normalized ,
            raycastHits ,
            castDisplacement.magnitude ,
            hitInfoFilter.collisionLayerMask ,
            hitInfoFilter.ignoreTriggers ? QueryTriggerInteraction.Ignore : QueryTriggerInteraction.Collide
        );

        GetClosestHit( out hitInfo , castDisplacement , hitInfoFilter );

        return hits;
    }



    public override int SphereCast( out HitInfo hitInfo , Vector3 center , float radius , Vector3 castDisplacement , HitInfoFilter hitInfoFilter )
    {
        hits = Physics.SphereCastNonAlloc(
            center ,
            radius ,
            castDisplacement.normalized ,
            raycastHits ,
            castDisplacement.magnitude ,
            hitInfoFilter.collisionLayerMask ,
            hitInfoFilter.ignoreTriggers ? QueryTriggerInteraction.Ignore : QueryTriggerInteraction.Collide
        );

        GetClosestHit( out hitInfo , castDisplacement , hitInfoFilter );

        return hits;
    }


    // Overlaps ──────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────
    
    public override bool OverlapSphere( Vector3 center , float radius , HitInfoFilter hitInfoFilter )
    {        
        
        int hits = Physics.OverlapSphereNonAlloc(
            center ,
            radius ,
            overlappedColliders ,
            hitInfoFilter.collisionLayerMask ,
            hitInfoFilter.ignoreTriggers ? QueryTriggerInteraction.Ignore : QueryTriggerInteraction.Collide
        );

        IgnoreOverlappedColliders( hitInfoFilter.ignoredLayerMask );
        
        this.hits = hits;

        return hits != 0;
    }

    public override bool OverlapCapsule( Vector3 bottom , Vector3 top , float radius , HitInfoFilter hitInfoFilter )
    {  

        int hits = Physics.OverlapCapsuleNonAlloc(
            bottom ,
            top ,  
            radius ,
            overlappedColliders ,
            hitInfoFilter.collisionLayerMask ,
            hitInfoFilter.ignoreTriggers ? QueryTriggerInteraction.Ignore : QueryTriggerInteraction.Collide
        );

        IgnoreOverlappedColliders( hitInfoFilter.ignoredLayerMask );

        this.hits = hits;  

        return hits != 0;
    }
    

    // ─────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────

    protected override void IgnoreOverlappedColliders( LayerMask ignoredLayerMask )
    {
        for( int i = 0 ; i < hits ; i++ )
        {
            Collider hitCollider = overlappedColliders[i];            
            
            for( int j = 0 ; j < colliders.Length ; j++ )
            {
                Collider thisCollider = colliders[j];
                
                if( hitCollider == thisCollider )
                    continue;
                
                if( CustomUtilities.BelongsToLayerMask( hitCollider.gameObject.layer , ignoredLayerMask ) )
                    Physics.IgnoreCollision( thisCollider , hitCollider , true );
            }            
            
        }
    }

    
    void GetHitInfo( ref HitInfo hitInfo , RaycastHit raycastHit  , Vector3 castDirection)
    {

        if( raycastHit.collider != null )
        {                    
            hitInfo.point = raycastHit.point;
            hitInfo.normal = raycastHit.normal;
            hitInfo.distance = raycastHit.distance;
            hitInfo.direction = castDirection;
            hitInfo.transform = raycastHit.transform;
            hitInfo.collider3D = raycastHit.collider;
            hitInfo.rigidbody3D = raycastHit.rigidbody;     
        }
    }

    protected override void GetClosestHit( out HitInfo hitInfo , Vector3 castDisplacement , HitInfoFilter hitInfoFilter )
    {
        RaycastHit closestRaycastHit = new RaycastHit();
        closestRaycastHit.distance = Mathf.Infinity;

        hitInfo = new HitInfo();
        hitInfo.hit = false;

        for( int i = 0 ; i < hits ; i++ )
        {
            RaycastHit raycastHit = raycastHits[i];             

            if( raycastHit.distance == 0 )
                continue;

                
            bool continueSelf = false;
            for( int j = 0 ; j < colliders.Length ; j++ )
            {
                Collider thisCollider = colliders[j];
                
                if( raycastHit.collider == thisCollider )
                    continueSelf = true;
                
                if( CustomUtilities.BelongsToLayerMask( raycastHit.transform.gameObject.layer , hitInfoFilter.ignoredLayerMask ) )
                    Physics.IgnoreCollision( thisCollider , raycastHit.collider , true );
            }

            if( continueSelf )
                continue;

            if( raycastHit.distance < hitInfoFilter.minimumDistance || raycastHit.distance > hitInfoFilter.maximumDistance )
                continue;
                
            if( hitInfoFilter.ignoreRigidbodies && raycastHit.collider.attachedRigidbody != null )
                continue;
                
            hitInfo.hit = true;

            if( raycastHit.distance < closestRaycastHit.distance )
                closestRaycastHit = raycastHit;
            
        }

        GetHitInfo( ref hitInfo , closestRaycastHit , castDisplacement.normalized );        

    }

    


}

}