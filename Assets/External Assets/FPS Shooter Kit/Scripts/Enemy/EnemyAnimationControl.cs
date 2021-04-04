using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EnemyBehavior;

public class EnemyAnimationControl : MonoBehaviour
{

	private EnemyBehaviorControl enemyBehContr;
	public EnemyBehaviorList currentBehavior;
	public EnemyBehaviorList nextBehavior;


	void GetCurrentAnimationBehavior ()
	{

		AnimatorStateInfo StateInfo = enemyBehContr.animator.GetCurrentAnimatorStateInfo (0);

		var Behaviors = enemyBehContr.behList;

		foreach (var nBehavior in Behaviors) {

			if (StateInfo.IsName (nBehavior.animationName)) {

				currentBehavior = nBehavior.Behavior;
				break;
			}
		}

	}

	void GetNexAnimationBehavior ()
	{

		AnimatorStateInfo StateInfo = enemyBehContr.animator.GetNextAnimatorStateInfo (0);

		var Behaviors = enemyBehContr.behList;

		foreach (var nBehavior in Behaviors) {

			if (StateInfo.IsName (nBehavior.animationName)) {
				
				nextBehavior = nBehavior.Behavior;
				break;
			}
		}

	}

	string FindTriggerAnimation (EnemyBehaviorList behavior)
	{

		var Behaviors = enemyBehContr.behList;

		foreach (var nBehavior in Behaviors) {

			if (nBehavior.Behavior == behavior) {

				return nBehavior.triggerName;
			}

		}

		return "";
	}

	void GetNextBehavior ()
	{

		nextBehavior = enemyBehContr.CurrentBehavior;

	}

	void ChangingAnimation ()
	{

		if (currentBehavior != nextBehavior) {

			enemyBehContr.animator.ResetTrigger (FindTriggerAnimation (currentBehavior));
			currentBehavior = nextBehavior;
			enemyBehContr.animator.SetTrigger (FindTriggerAnimation (nextBehavior));

		}

	}


	void Start ()
	{
		enemyBehContr = GetComponent<EnemyBehaviorControl> ();
	}


	void FixedUpdate ()
	{
		GetNextBehavior ();
		ChangingAnimation ();

	}
}
