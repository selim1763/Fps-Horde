using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFireAttack : MonoBehaviour
{

	public int Damage = 10;

	void OnParticleCollision (GameObject other)
	{

	
		if (other.tag.Equals ("Player")) {

			Health playerHealth = other.GetComponent<Health> ();

			playerHealth.SetDamage (Damage);

		}

	}



}
