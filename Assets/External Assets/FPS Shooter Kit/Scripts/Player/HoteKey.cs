using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using UnityEngine.SceneManagement;
using UnityStandardAssets.Characters.FirstPerson;

public class HoteKey : MonoBehaviour
{

	private RigidbodyFirstPersonController rbfpc ;

	void Start(){
		rbfpc = GetComponent<RigidbodyFirstPersonController> ();
	}

	void FixedUpdate ()
	{
	
		if (CrossPlatformInputManager.GetButtonDown ("Reboot")) {
					
			SceneManager.LoadScene (0, LoadSceneMode.Single);
		}

		if (CrossPlatformInputManager.GetButton ("Aiming")) {

			rbfpc.mouseLook.XSensitivity = 0.5f;
			rbfpc.mouseLook.YSensitivity = 0.5f;

		}

		if (!CrossPlatformInputManager.GetButton ("Aiming")) {

			rbfpc.mouseLook.XSensitivity = 2f;
			rbfpc.mouseLook.YSensitivity = 2f;

		}


	}
}
