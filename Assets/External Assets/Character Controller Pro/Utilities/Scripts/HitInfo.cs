using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lightbug.Utilities
{

public struct HitInfo
{
    public bool hit;
    public Vector3 normal;
    public Vector3 point;
    public float distance;
    public Vector3 direction;
    public Transform transform;
    public Collider2D collider2D;
    public Collider collider3D;
    public Rigidbody2D rigidbody2D;
    public Rigidbody rigidbody3D;

    public bool Is2D
    {
        get
        {
            return collider2D != null;            
        }
    }

    public bool IsRigidbody
    {
        get
        {
            return rigidbody2D != null || rigidbody3D != null;                        
        }
    }

    
    public bool IsKinematicRigidbody
    {
        get
        {
            if( rigidbody2D != null )
                return rigidbody2D.isKinematic;
            else if( rigidbody3D != null )
                return rigidbody3D.isKinematic;    

            return false;                  
        }
    }

    public bool IsDynamicRigidbody
    {
        get
        {
            if( rigidbody2D != null )
                return rigidbody2D.bodyType == RigidbodyType2D.Dynamic;
            else if( rigidbody3D != null )
                return !rigidbody3D.isKinematic;    

            return false;                  
        }
    }

}

}
