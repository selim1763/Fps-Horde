using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayAnimationIdleOther : MonoBehaviour
{

	public float timer = 0f;
	public float timerBettwen = 5f;
	private Animator curAnimator;

	void Start ()
	{

		curAnimator = GetComponent<Animator> ();

	}

	bool AnimationIsIdle ()
	{
		bool IsIdle = false;

		AnimatorStateInfo StateInfo = curAnimator.GetCurrentAnimatorStateInfo (0);

		if (StateInfo.IsName ("BaseLayer.Idle")) {
			IsIdle = true;
		}

		return IsIdle;
	}

	 
	void FixedUpdate ()
	{


		if (AnimationIsIdle ()) {
			timer += Time.deltaTime;

		} else {
			curAnimator.ResetTrigger ("IdleOther");
			timer = 0f;
		}

		if (timer >= timerBettwen) {

			curAnimator.SetTrigger ("IdleOther");

		}

	}
}
