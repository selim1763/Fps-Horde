using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class ParticleSystemGrenadeHand : ParticleSystemControlBase
{
	public Transform newPositionEndGun;
	public GameObject Grenade;
	public Rigidbody rbNewGrenade;
	private GameObject newGrenade;
	public bool createdGrenade = false;
	public bool forceIsApplied = false;
	public float deltaForce = 0.01f;
	public WeaponAmmo Ammo;

	public float timer = 0f;
	public float timeBetween = 0.1f;

	public override void ParticleSystemAction ()
	{



		if (!createdGrenade) {

			newGrenade = Instantiate (Grenade);
			rbNewGrenade = newGrenade.GetComponent<Rigidbody> ();
			createdGrenade = true;

			newGrenade.transform.position = newPositionEndGun.position;
			newGrenade.transform.rotation = newPositionEndGun.rotation;

			rbNewGrenade.AddForce (newPositionEndGun.transform.forward * deltaForce, ForceMode.Impulse);
			createdGrenade = false;

		}
		 
	}

}
