using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorList;
using System;

public abstract class ParticleSystemControlBase :MonoBehaviour
{

	public bool EnableParticleSystem = false;

	public ParticleSystem PartSystem;

	public static int NumberBehavior = 1;

	public PlBehavior[] Behavior = new PlBehavior[NumberBehavior];

	abstract public void ParticleSystemAction ();


	public void PlayParticleSystem ()
	{

		if (EnableParticleSystem) {
			
			PartSystem.Play ();
		}

	}


}
