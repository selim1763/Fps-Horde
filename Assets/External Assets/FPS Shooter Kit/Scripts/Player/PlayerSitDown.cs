using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using UnityStandardAssets.Characters.FirstPerson;

public class PlayerSitDown : MonoBehaviour
{

	public Transform cameraPlayer, posSitDown, posStandtUp;
	public GameObject GunColUp, GunColDown;
	public CapsuleCollider StandUp, SitDown;
	private bool inTunnel = false;
	private bool stateSitDown = false;
	private Vector3 playerVector;
	private Health playerHealth;
	private RigidbodyFirstPersonController rbFirstPresonController;
	private bool SetPositionSit = false;

	void Start ()
	{
		
		playerHealth = GetComponent<Health> ();
		rbFirstPresonController = GetComponent<RigidbodyFirstPersonController> ();
	
	}


	public void ChangingInTunnel (bool tunnel)
	{

		inTunnel = tunnel;

	}

	void Update ()
	{

		//If the player's health is greater than zero
		if (playerHealth.health > 0) {

			// Set playerVector current player's position
			playerVector = cameraPlayer.position;

			// if if the player pressed the button and current stateSitDown is false and current position not sit, then set variable SetPosition is true
			if (CrossPlatformInputManager.GetButton ("SitDown") && stateSitDown == false && !SetPositionSit) {
				
				SetPositionSit = true;
				this.transform.position = new Vector3 (this.transform.position.x, this.transform.position.y - 0.28f, this.transform.position.z); 
				
			}
			// If the player released the "Sit" button and the "In the tunnel" variable is false and the "State to sit" variable is true 
			//	and the "Set position to sit" variable is true, then the variable "Set position to sit" is false.
			if (!CrossPlatformInputManager.GetButton ("SitDown") && !inTunnel && stateSitDown && SetPositionSit) {

				SetPositionSit = false;
				this.transform.position = new Vector3 (this.transform.position.x, this.transform.position.y + 0.28f, this.transform.position.z); 
			}
			//If variable SetPosition is true, then move down camera, changing Capsule Collider and set variable SitDown is true and variable SandUp is false.
			if (SetPositionSit) {
				
				if (cameraPlayer.position.y > posSitDown.position.y) {

					playerVector.y -= 4f * Time.deltaTime;
				} else {
					stateSitDown = true;
					rbFirstPresonController.m_Capsule = SitDown;
				}
				cameraPlayer.position = playerVector;

				GunColUp.gameObject.SetActive (false);
				GunColDown.gameObject.SetActive (true);
				SitDown.enabled = true;
				StandUp.enabled = false;

			} 

			//If variable SetPositionSit is false and variable stateSitDown is true, then move up camera and changing Capsule Collider 
			// and set variable SitDown is false and variable StandUp is true.
			if (!SetPositionSit && stateSitDown) {

				if (cameraPlayer.position.y < posStandtUp.position.y) {

					playerVector.y += 4f * Time.deltaTime;
				} else {
					stateSitDown = false;
					rbFirstPresonController.m_Capsule = StandUp;
				}
				cameraPlayer.position = playerVector;

				GunColUp.gameObject.SetActive (true);
				GunColDown.gameObject.SetActive (false);
				SitDown.enabled = false;
				StandUp.enabled = true;

			}


		
		}

	}


}
