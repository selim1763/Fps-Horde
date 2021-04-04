using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowParazite : MonoBehaviour
{

	public Transform objParazite;
	public float stepY = 3f;
	public Transform EndPosition;
	public bool show = false;

	void FixedUpdate ()
	{

		if (show) {

			if (objParazite.position.y < EndPosition.position.y) {
			
				float EndY = objParazite.position.y + stepY * Time.deltaTime;
				objParazite.position = new Vector3 (objParazite.position.x, EndY, objParazite.position.z);

			}
				

		}

	}
}
