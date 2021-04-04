using System;
using UnityEngine;

namespace UnityStandardAssets.Effects
{
	public class ParticleSystemMultiplier : MonoBehaviour
	{
		// Script to scale the size, speed and lifetime of a particle system

		public float multiplier = 1;

		public AudioSource soundExplosion;

		public void ExecPartSystem ()
		{
			soundExplosion.Play ();
			var systems = this.GetComponentsInChildren<ParticleSystem> ();
			foreach (ParticleSystem system in systems) {
				ParticleSystem.MainModule mainModule = system.main;
				mainModule.startSizeMultiplier *= multiplier;
				mainModule.startSpeedMultiplier *= multiplier;
				mainModule.startLifetimeMultiplier *= Mathf.Lerp (multiplier, 1, 0.5f);
				system.Clear ();
				system.Play ();
			}
		}
	}
}
