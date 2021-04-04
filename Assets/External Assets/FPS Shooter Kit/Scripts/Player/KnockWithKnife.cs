using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorList;

public class KnockWithKnife : MonoBehaviour {

	public Collider knifeCollider;
	public int Damage = 1;
	public bool Activate = false;
	public float percent = 0.5f;

	public PlayerBehaviorControl plBehavior;
	public static int NumberBehavior = 2;
	public PlBehavior[] Behavior = new PlBehavior[NumberBehavior];

	void OnTriggerEnter(Collider collision){

		if (collision.gameObject.tag.Equals ("HitedEnemy") && Activate) {

			collision.gameObject.GetComponent<EnemyDamage> ().SetDamage (Damage);
		}

	}

	void ActivateKnockKnife(){

		AnimatorStateInfo StateInfo = plBehavior.CurAnimator.GetCurrentAnimatorStateInfo (0);
		bool isAction = false;
		foreach (PlBehavior nBehavior in Behavior) {

			if (StateInfo.IsName (nBehavior.AnimationName)) {
				int i = (int)StateInfo.normalizedTime;

				if (StateInfo.normalizedTime < (i + percent)) {
					isAction = true;
					Activate = true;
				}

			}

		}

		if (!isAction) {
			Activate = false;
		}

	}

	void FixedUpdate () {
		 
		ActivateKnockKnife ();
	}
}
