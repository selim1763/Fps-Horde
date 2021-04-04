using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EnemyBehavior;

public class EnemyBehaviorControl : MonoBehaviour
{

	[HideInInspector]public EnemyBehaviorList CurrentBehavior;
	[HideInInspector]public Animator animator;

	[System.Serializable]
	public struct Behaviors
	{
		public string animationName;
		public EnemyBehaviorList Behavior;
		public string triggerName;
	}

	public static int numBehavior = 1;

	public Behaviors[] behList = new Behaviors[numBehavior];

	void Start ()
	{
		animator = GetComponent<Animator> ();
		CurrentBehavior = EnemyBehaviorList.Idle;
		
	}

	void GetCurrentState ()
	{

		AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo (0);

		foreach (Behaviors nBehavior in behList) {

			if (stateInfo.IsName (nBehavior.animationName)) {

				int i = (int)stateInfo.normalizedTime;

				if (CurrentBehavior != nBehavior.Behavior) {

					if (stateInfo.normalizedTime >= i + 0f && stateInfo.normalizedTime <= i + 0.1f) {
						CurrentBehavior = nBehavior.Behavior;
						break;
					}

				}
				if (CurrentBehavior == nBehavior.Behavior) {

					break;
				}
			}
		}

	}


	void FixedUpdate ()
	{
		GetCurrentState ();
	}
}
