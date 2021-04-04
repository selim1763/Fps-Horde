using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrenadeGunCollider : MonoBehaviour
{
	public float explosionForce = 4;
	public float radius = 10f;
	public GameObject explosion;
	public bool collisionDetect = false;
	public float timeWait = 2f;



	void ExecuteExplosion ()
	{

		float multiplier = 1f;
		PlayPartSysExplosion ();

		float r = radius * multiplier;
		var cols = Physics.OverlapSphere (transform.position, r);
		var rigidbodies = new List<Rigidbody> ();
		foreach (var col in cols) {
			if (col.attachedRigidbody != null && !rigidbodies.Contains (col.attachedRigidbody)) {
				rigidbodies.Add (col.attachedRigidbody);
			}

			if (col.gameObject.tag.Equals ("HitedEnemy")) {

				EnemyDamage enemDamage = col.gameObject.GetComponent<EnemyDamage> ();
				enemDamage.SetDamage (100);

			}

		}
		foreach (var rb in rigidbodies) {
			rb.AddExplosionForce (explosionForce * multiplier, transform.position, r, 1 * multiplier, ForceMode.Impulse);
		}

	}

	void PlayPartSysExplosion ()
	{

		var newExplosion = Instantiate (explosion);
		newExplosion.transform.position = this.transform.position;
		UnityStandardAssets.Effects.ParticleSystemMultiplier mpexplosion = newExplosion.GetComponent<UnityStandardAssets.Effects.ParticleSystemMultiplier> ();
		mpexplosion.ExecPartSystem ();

	}

	void OnCollisionEnter (Collision collision)
	{
		
		ExecuteExplosion ();



		Destroy (this.gameObject);
	}

}


