using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropCamera : MonoBehaviour
{

	public Rigidbody rbCamera;
	public Health playerHealth;
	public Collider colliderCamera;
	public ChangingGunSystem.ChangingGun playerCurrentGun;

	void Update ()
	{

		if (playerHealth.health <= 0) {
			playerCurrentGun.CurrentAnimator.gameObjForAnimator.SetActive (false);
			colliderCamera.isTrigger = false;
			rbCamera.isKinematic = false;
			rbCamera.useGravity = true;

		}

	}
}
