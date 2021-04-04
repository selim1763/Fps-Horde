using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using BehaviorList;

public class PlayerBehaviorControl : MonoBehaviour
{
	[HideInInspector]public Animator CurAnimator;
	public PlayerBehaviorListener plrBehavior;
	private string NameAnimationBlocking = "";
	[HideInInspector]public WeaponAmmo Ammo;

	public static int NumberTriggers = 11;

	[Serializable]
	public struct structTrigger
	{
		public string TriggerName;
		public string AnimationName;
		public PlayerBehavior Behavior;
		public bool enableAmmo;
		public bool enableMagazine;
		public bool chargingLimit;

		public structTrigger (string trigName, string animName, PlayerBehavior plBehavior)
		{
			TriggerName = trigName;
			AnimationName = animName;
			Behavior = plBehavior;
			enableAmmo = false;
			enableMagazine = false;
			chargingLimit = false;
		}
	}

	void Start ()
	{
		Ammo = GetComponent<WeaponAmmo> ();
		plrBehavior.SetAmmo (Ammo);
		CurAnimator = GetComponent<Animator> ();
	}


	public structTrigger[] Triggers = new structTrigger[NumberTriggers];

	private bool ChangeBehaviorBlock = false;

	public string[] BlockedAnimation = new string[0];

	structTrigger GetCurrentBehavior ()
	{

		AnimatorStateInfo StateInfo = CurAnimator.GetCurrentAnimatorStateInfo (0);
		structTrigger nexTrigger = new structTrigger ("None", "None", PlayerBehavior.None);

		foreach (structTrigger nTrigger in Triggers) {

			if (StateInfo.IsName (nTrigger.AnimationName)) {

				nexTrigger = nTrigger;

			}

		}

		return nexTrigger;

	}

	structTrigger GetNextBehavior ()
	{


		foreach (structTrigger nTrigger in Triggers) {

			if (plrBehavior.NextBehavior == nTrigger.Behavior) {
				
				return nTrigger;
			}

		}

		return new structTrigger ("None", "None", PlayerBehavior.None);

	}

	void SetNewBehavior ()
	{
		
		structTrigger curTrigger = GetCurrentBehavior ();
		structTrigger nexTrigger = GetNextBehavior ();

		bool animationBlocked = false;
		if (BlockedAnimation.Length > 0) {
			
			AnimatorStateInfo StateInfo = CurAnimator.GetCurrentAnimatorStateInfo (0);
		
			foreach (string nBlockAnim in BlockedAnimation) {

				if (StateInfo.IsName (nBlockAnim)) {
					animationBlocked = true;
					int i = (int)StateInfo.normalizedTime;

					if (StateInfo.normalizedTime < i + 0.9f) {

						ChangeBehaviorBlock = true;
						NameAnimationBlocking = nBlockAnim;
						break;
					} else {

						ChangeBehaviorBlock = false;
					}

				}
							

			}

			if (ChangeBehaviorBlock) {
			
				if (StateInfo.IsName (NameAnimationBlocking)) {

					int numI = (int)StateInfo.normalizedTime;

					if (StateInfo.normalizedTime > numI + 0.8) {
						ChangeBehaviorBlock = false;
					}

				}

			}

			if (!ChangeBehaviorBlock) {

				if (plrBehavior.CurrentBehavior != PlayerBehavior.Hide) {
					if (nexTrigger.Behavior != curTrigger.Behavior && nexTrigger.Behavior != PlayerBehavior.None) {

						if (curTrigger.TriggerName != "None") {
							CurAnimator.ResetTrigger (curTrigger.TriggerName);
						}


						plrBehavior.CurrentBehavior = GetNextBehavior ().Behavior;

						if (nexTrigger.enableAmmo && Ammo.bulletCount > 0) {

							CurAnimator.SetTrigger (nexTrigger.TriggerName);
						} else if (!nexTrigger.enableAmmo) {
							
							CurAnimator.SetTrigger (nexTrigger.TriggerName);
						}

						if (nexTrigger.enableAmmo && Ammo.bulletCount == 0) {

							CurAnimator.SetTrigger ("Idle");

						}

						
					}

				}
			}


		} else {

			if (plrBehavior.CurrentBehavior != PlayerBehavior.Hide) {
				if (nexTrigger.Behavior != curTrigger.Behavior && nexTrigger.Behavior != PlayerBehavior.None) {

					CurAnimator.ResetTrigger (curTrigger.TriggerName);

					plrBehavior.CurrentBehavior = GetNextBehavior ().Behavior;
				 

					if (nexTrigger.enableAmmo && Ammo.bulletCount > 0) {

						CurAnimator.SetTrigger (nexTrigger.TriggerName);
					} else if (!nexTrigger.enableAmmo) {

						CurAnimator.SetTrigger (nexTrigger.TriggerName);
					}

					if (nexTrigger.enableAmmo && Ammo.bulletCount == 0) {

						CurAnimator.SetTrigger ("Idle");

					}

				}

			}
		}

		if (!animationBlocked) {
			ChangeBehaviorBlock = false;
		}
	}

	void StopCurAnimation ()
	{

		AnimatorStateInfo StateInfo = CurAnimator.GetCurrentAnimatorStateInfo (0);
		structTrigger curTrigger = GetCurrentBehavior ();

		if (StateInfo.IsName (curTrigger.AnimationName)) {

			if (curTrigger.enableAmmo && Ammo.bulletCount == 0) {
				CurAnimator.ResetTrigger ("Shot");
				CurAnimator.ResetTrigger ("AimingShot");
				plrBehavior.CurrentBehavior = PlayerBehavior.Idle;
			}
		
		}

		if (curTrigger.chargingLimit && Ammo.bulletCount == Ammo.maximalBulletGun) {
			
			CurAnimator.ResetTrigger ("Recharge");
			plrBehavior.CurrentBehavior = PlayerBehavior.Idle;
		}


	}



	void Update ()
	{
		
		SetNewBehavior ();
		StopCurAnimation ();

	}
}
