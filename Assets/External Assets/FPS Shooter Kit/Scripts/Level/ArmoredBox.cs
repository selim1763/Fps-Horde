using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ChangingGunSystem;

public class ArmoredBox : MonoBehaviour
{

	public float posY = 1f;
	public float startY = 0f;

	private bool Up = true;
	private bool Down = false;

	void Start ()
	{

		startY = this.transform.position.y;

	}

	void OnTriggerEnter (Collider PlayerCollision)
	{

		if (PlayerCollision.gameObject.tag.Equals ("Player")) {

			ChangingGun playerGun = PlayerCollision.gameObject.GetComponent<ChangingGun> ();


			foreach (ChangingGun.BlockAnimation nGun in playerGun.EntAnimList) {

				if (nGun.animationName.Equals ("HandsGun")) {
				
					WeaponAmmo AmmoGun = nGun.gameObjForAnimator.GetComponent<WeaponAmmo> ();
					AmmoGun.Magazine += 5;

				}

				if (nGun.animationName.Equals ("HandsShootgun")) {

					WeaponAmmo AmmoShootgun = nGun.gameObjForAnimator.GetComponent<WeaponAmmo> ();
					AmmoShootgun.Magazine += 36;

				}

				if (nGun.animationName.Equals ("HandsTommyGun")) {

					WeaponAmmo AmmoTommyGun = nGun.gameObjForAnimator.GetComponent<WeaponAmmo> ();
					AmmoTommyGun.Magazine += 5;

				}

				if (nGun.animationName.Equals ("HandsFNScar")) {

					WeaponAmmo AmmoFNScar = nGun.gameObjForAnimator.GetComponent<WeaponAmmo> ();
					AmmoFNScar.Magazine += 5;

				}

				if (nGun.animationName.Equals ("HandsGrenadeGun")) {

					WeaponAmmo AmmoGrenadeGun = nGun.gameObjForAnimator.GetComponent<WeaponAmmo> ();
					AmmoGrenadeGun.Magazine += 3;

				}

				if (nGun.animationName.Equals ("HandsGrenade")) {
				
					WeaponAmmo AmmoGrenade = nGun.gameObjForAnimator.GetComponent<WeaponAmmo> ();
					AmmoGrenade.bulletCount += 5;

				}

				if (nGun.animationName.Equals ("HandsSniperRifle")) {

					WeaponAmmo AmmoSniperRifle = nGun.gameObjForAnimator.GetComponent<WeaponAmmo> ();
					AmmoSniperRifle.Magazine += 5;
				}

			}

			Destroy (this.gameObject);

		}


	}

	void FixedUpdate ()
	{

		this.transform.Rotate (0f, 30f * Time.deltaTime, 0f);

		if (Up) {

			this.transform.position = new Vector3 (this.transform.position.x, this.transform.position.y + posY * Time.deltaTime, this.transform.position.z); 

			if (this.transform.position.y >= (startY + posY)) {

				Up = false;
				Down = true;
			}

		}

		if (Down) {

			this.transform.position = new Vector3 (this.transform.position.x, this.transform.position.y - posY * Time.deltaTime, this.transform.position.z); 

			if (this.transform.position.y <= startY) {
				Up = true;
				Down = false;

			}

		}

	}

}
