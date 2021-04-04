using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttackDetection : MonoBehaviour
{

	[HideInInspector]public bool attack = false;

	void OnTriggerEnter (Collider other)
	{

		if (other.gameObject.tag.Equals ("Player")) {
			attack = true;
		}

	}

	void OnTriggerExit (Collider other)
	{

		if (other.gameObject.tag.Equals ("Player")) {
			attack = false;
		}

	}


}
