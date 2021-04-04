using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaiseSpears: MonoBehaviour
{

	public bool raiseSpears = false;
	public float positionY = 0f;
	public float step = 0.1f;
	public ShowParazite parazite;
	public Transform objSpears;

	void OnTriggerEnter (Collider other)
	{

		if (other.tag.Equals ("Player")) {
			raiseSpears = true;
			parazite.show = true;
			parazite.gameObject.SetActive (true);
		}
	}

			 
	void FixedUpdate ()
	{

		if (raiseSpears) {
			
			if (objSpears.transform.position.y < positionY) {
				float newY = objSpears.position.y + (step * Time.deltaTime);
				objSpears.position = new Vector3 (objSpears.position.x, newY, objSpears.position.z);

			}

		}
		
	}
}
