using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCollisionDetection: MonoBehaviour
{


	void OnCollisionEnter (Collision collision)
	{

		if (collision.gameObject.tag.Equals ("BuildingBlock")) {

			var cols = Physics.OverlapSphere (transform.position, 1f);
			var rigidbodies = new List<Rigidbody> ();

			foreach (var col in cols) {

				if (col.attachedRigidbody != null && !rigidbodies.Contains (col.attachedRigidbody)) {

					rigidbodies.Add (col.attachedRigidbody);
				}

			}

			foreach (var rb in rigidbodies) {
				rb.AddExplosionForce (0.1f, transform.position, 2.5f, 1f, ForceMode.Impulse);
			}


		}
	}

}
