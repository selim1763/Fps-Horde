using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunControlBehavior: MonoBehaviour
{

	public static int numAnimation = 1;
	private Animator animator;
	public ParticleSystemControlBase partSysContr;
	private WeaponAmmo Ammo;
	private int iteration = 0;
	private int nextIteration = 0;
	private bool action = false;
	private bool secondAction = false;
	public bool GunConrlSysEnable = true;
	public GunControlSystem gunContrlSys;
	private string currentAnimation = "";

	[System.Serializable]
	public struct AnimationList
	{
		public string animationName;
		public float percentStart;
		public float percentStartSecond;
		public AudioSource audioForAnimation;

	}

	public AnimationList[] animList = new AnimationList[numAnimation];

	void ExecuteAction ()
	{
		partSysContr.PlayParticleSystem ();

		if (GunConrlSysEnable) {
			gunContrlSys.Shot = true;
		}

		Ammo.BulletCounter ();
		secondAction = false;
	}

	void ExecuteSecondAction ()
	{
		secondAction = true;
		partSysContr.ParticleSystemAction ();

	}

	void PlaySound ()
	{
		action = false;
		foreach (AnimationList nAnim in animList) {
			
			if (animator.GetCurrentAnimatorStateInfo (0).IsName (nAnim.animationName)) {
				action = true;
				if (currentAnimation != nAnim.animationName) {
					iteration = 0;
					currentAnimation = nAnim.animationName;
				}

				int i = (int)animator.GetCurrentAnimatorStateInfo (0).normalizedTime;

				if (animator.GetCurrentAnimatorStateInfo (0).normalizedTime >= i + nAnim.percentStart) {
					
					if (i >= 0 && animator.GetCurrentAnimatorStateInfo (0).loop) {
							
						if (iteration == i) {
							iteration++;
							nAnim.audioForAnimation.Play ();
							ExecuteAction ();

						}
					}

					if (animator.GetCurrentAnimatorStateInfo (0).normalizedTime > i + nAnim.percentStartSecond && !secondAction) {

						ExecuteSecondAction ();

					}

					if (i == 0 && !animator.GetCurrentAnimatorStateInfo (0).loop) {

						if (iteration > 1 && i == 0) {
							iteration = 0;
						}

						if (iteration == i) {
							iteration++;
							nAnim.audioForAnimation.Play ();
							ExecuteAction ();
						}

						if (animator.GetCurrentAnimatorStateInfo (0).normalizedTime > 0.9f) {

							iteration = 0;
						}

					}

				}	

			}

		}

		if (!action) {
			iteration = 0;
			secondAction = false;
		}


	}

	void Start ()
	{
		animator = GetComponent<Animator> ();
		Ammo = GetComponent<WeaponAmmo> ();
	}

	void Update ()
	{

		PlaySound ();

	}
}
