using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDamage : MonoBehaviour
{

	public int damage = 1;
	public EnemyHealth Health;
	public EnemyAI enemyAI;

	public void  SetDamage ()
	{
		enemyAI.Hited = true;
		Health.SetDamage (damage);

	}

	public void  SetDamage (int bulletdamage)
	{
		enemyAI.Hited = true;
		Health.SetDamage (damage + bulletdamage);

	}

}
