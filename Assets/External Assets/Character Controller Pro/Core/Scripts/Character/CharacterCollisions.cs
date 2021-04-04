using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lightbug.Utilities;


namespace Lightbug.CharacterControllerPro.Core
{

public struct RayArrayInfo
{
	public float averageDistance;
	public Vector3 averageNormal;
}

// IMPORTANT: This class needs to be serializable in order to be compatible with assembly reloads.
[System.Serializable]
public class CharacterCollisions
{   
    enum PhysicsQueryType
	{
		Raycast ,
		SphereCast ,
		CapsuleCast
	}

    CharacterActor characterActor = null;
    PhysicsComponent physicsComponent = null;

    public void Initialize( CharacterActor characterActor , PhysicsComponent physicsComponent )
    {
        this.characterActor = characterActor;
        this.physicsComponent = physicsComponent;
    }


    

    /// <summary>
	/// Checks vertically for the ground using a SphereCast.
	/// </summary>
	public bool CheckForGround( out CollisionInfo collisionInfo , Vector3 position , float stepOffset , float stepDownDistance , HitInfoFilter hitInfoFilter )
    {        
        collisionInfo = new CollisionInfo();

		
		
		float radius = characterActor.BodySize.x / 2f - CharacterConstants.SkinWidth;
        float skin = stepOffset + CharacterConstants.SkinWidth;
        float extraDistance = Mathf.Max( CharacterConstants.GroundCheckDistance , stepDownDistance );
        Vector3 castDisplacement = - characterActor.Up * ( skin + extraDistance );
        Vector3 origin = characterActor.GetBottomCenter( position , stepOffset );
		HitInfo hitInfo;
		
		physicsComponent.SphereCast(
			out hitInfo ,
			origin ,
			radius ,
			castDisplacement ,
			hitInfoFilter
		);
		
		
		UpdateCollisionInfo( out collisionInfo , position , hitInfo , castDisplacement , skin , true , hitInfoFilter );    

        return collisionInfo.collision;
    }


	/// <summary>
	/// Checks vertically for the ground using a Raycast.
	/// </summary>
	public bool CheckForGroundRay( out CollisionInfo collisionInfo , Vector3 position , float stepOffset , float stepDownDistance , HitInfoFilter hitInfoFilter )
    {        
        collisionInfo = new CollisionInfo();

		
        float skin = stepOffset + characterActor.BodySize.x;
        float extraDistance = Mathf.Max( CharacterConstants.GroundCheckDistance , stepDownDistance );
        Vector3 castDisplacement = - characterActor.Up * ( skin + extraDistance );
        Vector3 origin = characterActor.GetBottomCenter( position , stepOffset );
		
		HitInfo hitInfo;		
		int hits = physicsComponent.Raycast( 
			out hitInfo ,
			origin ,
			castDisplacement ,
			hitInfoFilter
		);
		
		UpdateCollisionInfo( out collisionInfo , position , hitInfo , castDisplacement , skin , false , hitInfoFilter );    

        return collisionInfo.collision;
    }

	
	public void FireRaysArray( out RayArrayInfo rayArrayInfo , ref Vector3 position , float stepUpDistance , float stepDownDistance , HitInfoFilter hitInfoFilter )
    {        
		int rings = 3;
		int rotationSubdivisions = 8;

		rayArrayInfo = new RayArrayInfo();

		int hits = 0;
		
		float radius = characterActor.BodySize.x / 2f - CharacterConstants.SkinWidth;
        float skin = stepUpDistance + CharacterConstants.SkinWidth;
        float extraDistance = Mathf.Max( CharacterConstants.GroundCheckDistance , stepDownDistance );
        Vector3 castDisplacement = - characterActor.Up * ( skin + extraDistance );
		HitInfo hitInfo;

		float deltaRotationAngle = 360f / rotationSubdivisions;  
		float deltaRadius = radius / rings;
		Vector3 arrayOrigin = position + characterActor.Up * skin;
		
		for( int i = 0 ; i < rings ; i++ )
		{
			if( i == 0 )
			{
				Vector3 origin = arrayOrigin;

				physicsComponent.Raycast(
					out hitInfo ,
					origin ,
					castDisplacement ,
					hitInfoFilter
				);

				if( hitInfo.hit )
				{
					hits++;
					rayArrayInfo.averageDistance += hitInfo.distance;
					rayArrayInfo.averageNormal += hitInfo.normal;

					// Debug.DrawLine( origin , hitInfo.point );
				}
			}
			else
			{
				Vector3 ray = characterActor.Forward * ( i * deltaRadius );
				for( int j = 0 ; j < rotationSubdivisions ; j++ )
				{
					ray = Quaternion.AngleAxis( j * deltaRotationAngle , characterActor.Up ) * ray;
					Vector3 origin = arrayOrigin + ray;

					physicsComponent.Raycast(
						out hitInfo ,
						origin ,
						castDisplacement ,
						hitInfoFilter
					);

					if( hitInfo.hit )
					{
						hits++;
						rayArrayInfo.averageDistance += hitInfo.distance;
						rayArrayInfo.averageNormal += hitInfo.normal;

						// Debug.DrawLine( origin , hitInfo.point );
					}
				}
			}

		}

		rayArrayInfo.averageDistance /= hits;
		rayArrayInfo.averageNormal.Normalize();


		position -= characterActor.Up * ( rayArrayInfo.averageDistance - (stepUpDistance + CharacterConstants.SkinWidth) );

    }

	

	
	/// <summary>
	/// Checks for the ground.
	/// </summary>
	public bool CheckForStableGround( out CollisionInfo collisionInfo , Vector3 position , Vector3 direction , float stepOffset , HitInfoFilter hitInfoFilter )
    {
        collisionInfo = new CollisionInfo();

        
		Vector3 origin = characterActor.GetBottomCenter( position );
		float radius = characterActor.BodySize.x / 2f - CharacterConstants.SkinWidth;
        float skin = CharacterConstants.SkinWidth;
        Vector3 castDisplacement = direction * ( 5f * CharacterConstants.GroundCheckDistance + skin + stepOffset );

		HitInfo hitInfo;
        int hits = physicsComponent.SphereCast( 
			out hitInfo ,
			origin ,
			radius ,
			castDisplacement ,
			hitInfoFilter
		);


		UpdateCollisionInfo( out collisionInfo , position , hitInfo , castDisplacement , skin , true , hitInfoFilter );      

        return collisionInfo.collision;
    }

	bool IsASphere => characterActor.BodySize.x == characterActor.BodySize.y;
	
	public bool CastBody( out CollisionInfo collisionInfo , Vector3 position , Vector3 displacement , float bottomOffset , HitInfoFilter hitInfoFilter )
    {
        collisionInfo = new CollisionInfo();
		

		float backstepDistance = 0.1f;
        float skin = CharacterConstants.SkinWidth + backstepDistance;

        Vector3 bottom = characterActor.GetBottomCenter( position , bottomOffset );   
        Vector3 top = characterActor.GetTopCenter( position );      

		bottom -= displacement.normalized * backstepDistance;
		top -= displacement.normalized * backstepDistance;

		float radius = characterActor.BodySize.x / 2f - CharacterConstants.SkinWidth;   

        Vector3 castDisplacement = displacement + displacement.normalized * skin;

		HitInfo hitInfo;
		int hits = 0;

		if( IsASphere )
		{
			hits = physicsComponent.SphereCast(
				out hitInfo ,
				bottom ,
				radius ,
				castDisplacement ,
				hitInfoFilter
			);
		}
		else
		{
			hits = physicsComponent.CapsuleCast(
				out hitInfo ,
				bottom ,
				top ,
				radius ,
				castDisplacement ,
				hitInfoFilter
			);
		}


		UpdateCollisionInfo( out collisionInfo , position , hitInfo , castDisplacement , skin , true , hitInfoFilter );     

        return collisionInfo.collision;
    }


	public bool CastBodyVertically( out CollisionInfo collisionInfo , Vector3 position , float verticalComponent , float backstepDistance , HitInfoFilter hitInfoFilter , bool ignoreOverlaps )
    {		
        collisionInfo = new CollisionInfo();


		float skin = backstepDistance + CharacterConstants.SkinWidth;
		
		Vector3 castDirection = verticalComponent > 0 ? characterActor.Up : - characterActor.Up;

        Vector3 center = verticalComponent > 0 ? 
		characterActor.GetTopCenter( position ) - castDirection * skin :
		characterActor.GetBottomCenter( position ) - castDirection * skin;

		float castMagnitude = Mathf.Max( Mathf.Abs( verticalComponent ) + skin , 0.02f );
        Vector3 castDisplacement = castDirection * castMagnitude;

		float minimumDistance = ignoreOverlaps ? backstepDistance : 0f;

		HitInfo hitInfo;
        int hits = physicsComponent.SphereCast(
			out hitInfo ,
			center ,
			characterActor.BodySize.x / 2f - CharacterConstants.SkinWidth ,
			castDisplacement ,			
			hitInfoFilter	
		);
		
		UpdateCollisionInfo( out collisionInfo , position , hitInfo , castDisplacement , skin , true , hitInfoFilter );
		
        return collisionInfo.collision;
    }

	/// <summary>
	/// Checks if the character is currently overlapping with any obstacle from a given layermask.
	/// </summary>
	public bool CheckOverlapWithLayerMask( Vector3 position , float bottomOffset , HitInfoFilter hitInfoFilter )
	{

		Vector3 bottom = characterActor.GetBottomCenter( position , bottomOffset );   
        Vector3 top = characterActor.GetTopCenter( position );      
		float radius = characterActor.BodySize.x / 2f - CharacterConstants.SkinWidth;  
		
		bool overlap = physicsComponent.OverlapCapsule(
			bottom ,
			top ,
			radius ,
			hitInfoFilter
		);		
		
		return overlap;
	}

	/// <summary>
	/// Checks if the character size fits at a specific location.
	/// </summary>
	public bool CheckBodySize( Vector3 size , Vector3 position , HitInfoFilter hitInfoFilter )
    {
		
        Vector3 bottom = characterActor.GetBottomCenter( position , size );   
        Vector3 top = characterActor.GetTopCenter( position , size ); 
		float radius = size.x / 2f - CharacterConstants.SkinWidth;

		// GetBottomCenterToTopCenter.normalized ---> Up

        Vector3 castDisplacement = characterActor.GetBottomCenterToTopCenter( size ) + characterActor.Up * CharacterConstants.SkinWidth;

		HitInfo hitInfo;
		physicsComponent.SphereCast( 
			out hitInfo ,
			bottom ,
			radius ,
			castDisplacement ,
			hitInfoFilter
		);


		bool overlap = hitInfo.hit;
		
		return !overlap;
    }

	/// <summary>
	/// Checks if the character size fits in place.
	/// </summary>
	public bool CheckBodySize( Vector3 size , HitInfoFilter hitInfoFilter )
    {        
        return CheckBodySize( size , characterActor.Position , hitInfoFilter );
    }

  
    void UpdateCollisionInfo( out CollisionInfo collisionInfo , Vector3 position , HitInfo hitInfo , Vector3 castDisplacement , float skin , bool calculateEdge = true , HitInfoFilter hitInfoFilter = new HitInfoFilter() )
    {
		collisionInfo = new CollisionInfo();
		
        collisionInfo.collision = hitInfo.hit;

        if( collisionInfo.collision )
        {            
            collisionInfo.displacement = castDisplacement.normalized * ( hitInfo.distance - skin );
                    
            collisionInfo.hitInfo = hitInfo;
            collisionInfo.contactSlopeAngle = Vector3.Angle( characterActor.Up , hitInfo.normal );

			if( calculateEdge )
            	UpdateEdgeInfo( ref collisionInfo , position , hitInfoFilter );
        }
        else
        {
            collisionInfo.displacement = castDisplacement.normalized * ( castDisplacement.magnitude - skin );
        }

    }	


	void UpdateEdgeInfo( ref CollisionInfo collisionInfo , Vector3 position , HitInfoFilter hitInfoFilter )
    {
        Vector3 center = characterActor.GetBottomCenter( position , characterActor.StepOffset );

        Vector3 castDirection = ( collisionInfo.hitInfo.point - center ).normalized;
		Vector3 castDisplacement = castDirection * CharacterConstants.EdgeRaysCastDistance;

		Vector3 upperHitPosition = center + characterActor.Up * CharacterConstants.EdgeRaysSeparation;
		Vector3 lowerHitPosition = center - characterActor.Up * CharacterConstants.EdgeRaysSeparation;

		HitInfo upperHitInfo;
		physicsComponent.Raycast(
			out upperHitInfo,
			upperHitPosition ,
			castDisplacement ,
			hitInfoFilter
		);

        
        HitInfo lowerHitInfo;
		physicsComponent.Raycast(
			out lowerHitInfo,
			lowerHitPosition ,
			castDisplacement ,
			hitInfoFilter
		);
        		

		collisionInfo.edgeUpperNormal = upperHitInfo.normal;      
		collisionInfo.edgeLowerNormal = lowerHitInfo.normal;

        collisionInfo.edgeUpperSlopeAngle = Vector3.Angle( collisionInfo.edgeUpperNormal , characterActor.Up );
        collisionInfo.edgeLowerSlopeAngle = Vector3.Angle( collisionInfo.edgeLowerNormal , characterActor.Up );
	
		collisionInfo.edgeAngle = Vector3.Angle( collisionInfo.edgeUpperNormal , collisionInfo.edgeLowerNormal );

        collisionInfo.isAnEdge = CustomUtilities.isBetween( collisionInfo.edgeAngle , CharacterConstants.MinEdgeAngle , CharacterConstants.MaxEdgeAngle , true );
        collisionInfo.isAStep = CustomUtilities.isBetween( collisionInfo.edgeAngle , CharacterConstants.MinStepAngle , CharacterConstants.MaxStepAngle , true );
        
        
    }
}

}