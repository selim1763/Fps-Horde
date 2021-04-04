using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lightbug.Utilities;

namespace Lightbug.CharacterControllerPro.Core
{

/// <summary>
/// This component is responsible for initializing the interpolation data associated with the physics actor.
/// </summary>
[DefaultExecutionOrder( int.MinValue )]
public class PhysicsActorSync : MonoBehaviour
{
    PhysicsActor physicsActor = null;

    
    void Awake()
    {
        physicsActor = GetComponent<PhysicsActor>();
    }

    void FixedUpdate()
    {
        // This instruction that runs before anything else. This makes sure the rigidbody data is always in "sync" with the interpolation data (physics actor).
        physicsActor.SyncBody();
    }
}

}
