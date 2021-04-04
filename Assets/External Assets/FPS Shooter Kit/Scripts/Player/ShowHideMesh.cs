using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using BehaviorList;

public abstract class ShowHideMesh:MonoBehaviour
{
	
	public PlayerBehaviorControl plBehaviorContr;
	public GameObject[] objectForHide = new GameObject[1];

	public float percentHide = 0f;
	public float percentShow = 0f;
	private bool Hide = false;

	private Animator animator;
	private WeaponAmmo Ammo;

	[Serializable]
	public struct BehaviorAnimator
	{
		public string AnimationName;
	}

	public static int NumbBehSound = 1;
	public BehaviorAnimator[] BhAnimator = new BehaviorAnimator[NumbBehSound];

	void Awake ()
	{
		animator = plBehaviorContr.CurAnimator;
		Ammo = plBehaviorContr.Ammo;
	}

	void HideObject ()
	{

		for (int i = 0; i < objectForHide.Length; i++) {
			objectForHide [i].SetActive (false);
		}
		Hide = true;

	}

	void ShowObject ()
	{
		
		for (int i = 0; i < objectForHide.Length; i++) {
			objectForHide [i].SetActive (true);
		}
		Hide = false;

	}


	public void ShowHideObject ()
	{
		bool IsAnimation = false;
		foreach (BehaviorAnimator nBhAnimator in BhAnimator) {

			if (animator.GetCurrentAnimatorStateInfo (0).IsName (nBhAnimator.AnimationName) && !Hide) {
				IsAnimation = true;

				int i = (int)animator.GetCurrentAnimatorStateInfo (0).normalizedTime;

				if (animator.GetCurrentAnimatorStateInfo (0).normalizedTime > i + percentHide) {
					HideObject ();
				}

			} else if (animator.GetCurrentAnimatorStateInfo (0).IsName (nBhAnimator.AnimationName) && Hide) {
				IsAnimation = true;
				int i = (int)animator.GetCurrentAnimatorStateInfo (0).normalizedTime;

				if (animator.GetCurrentAnimatorStateInfo (0).normalizedTime > i + percentShow && animator.GetCurrentAnimatorStateInfo (0).normalizedTime < i + percentHide) {
					ShowObject ();
				}

			}

		}

		if (!IsAnimation) {

			if (Ammo.bulletCount > 0) {
				ShowObject ();
			}

		} 


	}


}
