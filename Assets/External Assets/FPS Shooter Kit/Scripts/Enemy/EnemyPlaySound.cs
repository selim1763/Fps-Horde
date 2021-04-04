using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPlaySound : MonoBehaviour
{

	public static int numAnimation = 1;
	private Animator animator;

	[System.Serializable]
	public struct AnimationList
	{
		public string animationName;
		public float percentStart;
		public AudioSource audioForAnimation;

	}

	public AnimationList[] animList = new AnimationList[numAnimation];

	void Start(){
		animator = GetComponent<Animator> ();
	}

	void PlaySound ()
	{

		foreach (AnimationList nAnim in animList) {

			if (animator.GetCurrentAnimatorStateInfo (0).IsName (nAnim.animationName)) {

				int i = (int)animator.GetCurrentAnimatorStateInfo (0).normalizedTime;

				if (animator.GetCurrentAnimatorStateInfo (0).normalizedTime >= i + nAnim.percentStart && animator.GetCurrentAnimatorStateInfo (0).normalizedTime <= i + nAnim.percentStart + 0.1) {
					if (i >= 0 && animator.GetCurrentAnimatorStateInfo (0).loop) {
						nAnim.audioForAnimation.Play ();
					}

					if (i == 0 && !animator.GetCurrentAnimatorStateInfo (0).loop) {
						nAnim.audioForAnimation.Play ();
					}

				}

			}

			if (animator.GetNextAnimatorStateInfo (0).IsName (nAnim.animationName)) {

				int j = (int)animator.GetNextAnimatorStateInfo (0).normalizedTime;

				if (animator.GetNextAnimatorStateInfo (0).normalizedTime >= j + nAnim.percentStart && animator.GetNextAnimatorStateInfo (0).normalizedTime <= j + nAnim.percentStart + 0.1) {

					nAnim.audioForAnimation.Play ();

				}

			}


		}

	}

	

	void FixedUpdate ()
	{
		PlaySound ();

	}
}
