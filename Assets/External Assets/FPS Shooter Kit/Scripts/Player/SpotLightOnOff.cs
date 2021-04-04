using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class SpotLightOnOff : MonoBehaviour
{

	public Light spotLight;
	public AudioSource soundClick;


	void OnOffLight ()
	{

		if (spotLight.enabled) {
			spotLight.enabled = false;
		} else {

			spotLight.enabled = true;
		}

		soundClick.Play ();

	}

	void Update ()
	{
	
		if (CrossPlatformInputManager.GetButtonDown ("SpotOn")) {

			OnOffLight ();


		} 

	}
}
