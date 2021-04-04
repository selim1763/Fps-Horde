using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenDoor : MonoBehaviour
{

	public Animator animDoor;

	void OnTriggerEnter (Collider other)
	{

		if (other.gameObject.tag.Equals ("Player")) {

			animDoor.SetTrigger ("Open");
		}
	}



	void OnTriggerExit (Collider other)
	{

		if (other.gameObject.tag.Equals ("Player")) {
		
			animDoor.SetTrigger ("Close");

		}
	
	}



}
