using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using UnityEngine.SceneManagement;

public class ChangeOfDayAndNight: MonoBehaviour
{

	public Light dirLight;

	void ChangeDayOrNight (bool day)
	{
	

		if (day) {
			dirLight.enabled = true;
			this.transform.localRotation = Quaternion.Euler (new Vector3 (154f, 2.33f, 0f));
			RenderSettings.ambientIntensity = 1f;
			RenderSettings.reflectionIntensity = 1f;
		} else {
			dirLight.enabled = false;
			this.transform.localRotation = Quaternion.Euler (new Vector3 (270f, 0f, 0f));
			RenderSettings.ambientIntensity = 0f;
			RenderSettings.reflectionIntensity = 0f;
		}


	}

	// Update is called once per frame
	void FixedUpdate ()
	{

		if (CrossPlatformInputManager.GetButton ("Day")) {
		
			ChangeDayOrNight (true);

		} else if (CrossPlatformInputManager.GetButton ("Night")) {
			ChangeDayOrNight (false);
		}


	}
}
