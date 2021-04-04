using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{

	public int health = 100;
	public Collider[] colliderList = new Collider[1];

	public void SetDamage (int damage)
	{

		health -= damage;

	}

	public void DeactivateCollider ()
	{

		for (int i = 0; i < colliderList.Length; i++) {

			colliderList [i].enabled = false;
			
		}
	

	}


}
