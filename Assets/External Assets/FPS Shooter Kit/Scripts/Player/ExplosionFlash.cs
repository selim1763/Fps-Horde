using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionFlash : MonoBehaviour
{
	public float timer = 0f;
	public float timeLife = 1f;
	Light ligth;

	void Start ()
	{
		timer = 0f;
		ligth = GetComponent<Light> ();
	}

	 
	void FixedUpdate ()
	{

		timer += Time.fixedDeltaTime;
		if (timer >= timeLife) {

			ligth.enabled = false;

		}

	}
}
