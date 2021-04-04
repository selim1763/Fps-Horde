using System.Collections;
using System.Collections.Generic;
using Lightbug.Utilities;
using UnityEngine;

namespace Lightbug.CharacterControllerPro.Core
{

/// <summary>
/// The root abstract class for all the graphics-based components.
/// </summary>
public abstract class CharacterGraphics : MonoBehaviour
{
    protected CharacterActor CharacterActor { get; private set; }
    protected CharacterGraphicsRootController RootController { get; private set; }

    
    protected virtual void OnValidate()
    {
        CharacterActor = this.GetComponentInBranch<CharacterActor>();        

        if( CharacterActor == null )
        {
            Debug.Log("Warning: No CharacterActor component detected in this hierarchy.");
        }
        else
        {
            RootController = CharacterActor.GetComponentInChildren<CharacterGraphicsRootController>();
        }
    }

    protected virtual void Awake()
    {
        CharacterActor = this.GetComponentInBranch<CharacterActor>();

        if( CharacterActor != null )
            RootController = CharacterActor.GetComponentInChildren<CharacterGraphicsRootController>();
            
    }

    
}

}
