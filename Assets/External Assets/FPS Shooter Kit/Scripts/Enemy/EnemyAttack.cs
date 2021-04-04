using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttack : MonoBehaviour
{

	public int Damage = 80;
	public EnemyHealth Enemyhealth;

	void OnTriggerEnter (Collider PlayerCollider)
	{


		if (PlayerCollider.tag.Equals ("Player") && Enemyhealth.health > 0) {
		
			Health	playerHealth = PlayerCollider.gameObject.GetComponent<Health> ();

			playerHealth.SetDamage (Damage);

		}

	}



}
