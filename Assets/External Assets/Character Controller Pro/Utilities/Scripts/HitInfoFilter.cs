using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lightbug.Utilities
{

public struct HitInfoFilter
{
    public LayerMask collisionLayerMask;
    public bool ignoreRigidbodies;
    public bool ignoreTriggers;
    public LayerMask ignoredLayerMask;
    public float minimumDistance;
    public float maximumDistance;

    public HitInfoFilter( LayerMask collisionLayerMask, bool ignoreRigidbodies, bool ignoreTriggers , int ignoredLayerMask = 0 , float minimumDistance = 0f , float maximumDistance = Mathf.Infinity )
    {
        this.collisionLayerMask = collisionLayerMask;
        this.ignoreRigidbodies = ignoreRigidbodies;
        this.ignoreTriggers = ignoreTriggers;
        this.ignoredLayerMask = ignoredLayerMask;
        this.minimumDistance = minimumDistance;
        this.maximumDistance = maximumDistance;
    }

        

}

}
