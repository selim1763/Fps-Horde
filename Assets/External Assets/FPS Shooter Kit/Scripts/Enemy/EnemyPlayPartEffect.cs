using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPlayPartEffect : MonoBehaviour
{
	 
	public static int numAnimation = 1;
	public Animator animator;
	public int nI = 0;
	public EnemyHealth EnmHealth;

	private bool EnemyDeath = false;


	[System.Serializable]
	public struct AnimationList
	{
		public string animationName;
		public float percentStart;
		public float percentStop;
		public ParticleSystem partSys;

	}

	public AnimationList[] animList = new AnimationList[numAnimation];

	void PlayParticleEffect ()
	{
	

		foreach (AnimationList nAnim in animList) {

			if (animator.GetCurrentAnimatorStateInfo (0).IsName (nAnim.animationName)) {
				


				int i = (int)animator.GetCurrentAnimatorStateInfo (0).normalizedTime;
				
				if (animator.GetCurrentAnimatorStateInfo (0).normalizedTime >= i + nAnim.percentStart) {
				
					if (i >= 0 && animator.GetCurrentAnimatorStateInfo (0).loop) {
					
						
						if (nI != i || i == 0 && nAnim.partSys.isStopped) {
							nI = i;
							nAnim.partSys.Play ();				
						}
						if (animator.GetCurrentAnimatorStateInfo (0).normalizedTime >= i + nAnim.percentStop) {
							nAnim.partSys.Stop ();
						}

					}

					if (i == 0 && !animator.GetCurrentAnimatorStateInfo (0).loop) {

						if (animator.GetCurrentAnimatorStateInfo (0).normalizedTime >= nAnim.percentStart
						    && animator.GetCurrentAnimatorStateInfo (0).normalizedTime < nAnim.percentStop) {
							nAnim.partSys.Play ();
						} else {
						
							nAnim.partSys.Stop ();
						}

					}

				}

			}	

		}


	}


	void StopParticleEffect ()
	{

		foreach (AnimationList nAnim in animList) {
			
			nAnim.partSys.Stop ();

		}

	}


	void FixedUpdate ()
	{

		if (!EnemyDeath) {
		
			PlayParticleEffect ();
		}

		if (EnmHealth.health <= 0 && !EnemyDeath) {

			EnemyDeath = true;

			StopParticleEffect ();

		}
	}
}
