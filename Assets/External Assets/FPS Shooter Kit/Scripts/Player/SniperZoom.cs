using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class SniperZoom : MonoBehaviour
{

	public float zoom1 = 8f;
	public float zoom2 = 5f;
	public float zoom3 = 1.5f;
	public Camera camSniper;

	void FixedUpdate ()
	{

		if (CrossPlatformInputManager.GetButtonDown ("ZoomUp")) {
		
			if (camSniper.fieldOfView == zoom1) {
				camSniper.fieldOfView = zoom2;
			} else if (camSniper.fieldOfView == zoom2) {
				camSniper.fieldOfView = zoom3;
			}

		}
		if (CrossPlatformInputManager.GetButtonDown ("ZoomDown")) {

			if (camSniper.fieldOfView == zoom3) {
				camSniper.fieldOfView = zoom2;
			} else if (camSniper.fieldOfView == zoom2) {
				camSniper.fieldOfView = zoom1;
			}

		}

	}
}
