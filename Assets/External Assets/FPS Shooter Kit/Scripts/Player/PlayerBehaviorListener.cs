using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using UnityStandardAssets.Characters.FirstPerson;
using BehaviorList;
using System;

public class PlayerBehaviorListener : MonoBehaviour
{

	public PlayerBehavior PlayerAction;
	public PlayerBehavior MovementState;
	public PlayerBehavior PlayerPosition;
	public PlayerBehavior CurrentBehavior;
	public PlayerBehavior NextBehavior;
	private RigidbodyFirstPersonController rgdbFPC;
	public float WaitingTimer = 1f;
	private float Timer = 0f;

	private WeaponAmmo Ammo;
	private Health playerHealth;

	//Set ammo from select weapon
	public void SetAmmo (WeaponAmmo newAmmo)
	{
		Ammo = newAmmo;
	}

	//Reset Behavior Get or Hide
	void ResetGetHideState ()
	{
		Timer = 0f;
		CurrentBehavior = PlayerBehavior.Idle;

	}


	//Get player action
	void ActionListener ()
	{
		
		if (CrossPlatformInputManager.GetButton ("Fire1") && MovementState != PlayerBehavior.Aiming && MovementState != PlayerBehavior.AimingWalk && (Ammo.Unlimited || Ammo.bulletCount > 0)) {
			PlayerAction = PlayerBehavior.Shot;
			goto END;
		}

		if (CrossPlatformInputManager.GetButton ("Fire1") && (!Ammo.Unlimited && Ammo.bulletCount == 0) && Ammo.Magazine > 0) {
			PlayerAction = PlayerBehavior.Recharge;
			goto END;
		}
		
		if (CrossPlatformInputManager.GetButton ("Fire1") && (MovementState == PlayerBehavior.Aiming || MovementState == PlayerBehavior.AimingWalk) && (Ammo.Unlimited || Ammo.bulletCount > 0)) {
			PlayerAction = PlayerBehavior.AimingShot;
			goto END;
		}

		if (CrossPlatformInputManager.GetButton ("Fire1") && (MovementState == PlayerBehavior.Aiming || MovementState == PlayerBehavior.AimingWalk) && (!Ammo.Unlimited && Ammo.bulletCount <= 0) && Ammo.Magazine > 0) {
			PlayerAction = PlayerBehavior.Recharge;
			goto END;
		}

		if (CrossPlatformInputManager.GetButton ("Recharge") && !CrossPlatformInputManager.GetButton ("Fire1") && Ammo.Magazine > 0 && Ammo.bulletCount < Ammo.maximalBulletGun) {
			PlayerAction = PlayerBehavior.Recharge;
			goto END;
		}
		
		if (!CrossPlatformInputManager.GetButton ("Fire1") && !CrossPlatformInputManager.GetButton ("Recharge")) {
			PlayerAction = PlayerBehavior.None;
			goto END;
		}

		if (!Ammo.Unlimited && Ammo.bulletCount == 0 && (PlayerAction == PlayerBehavior.Shot || PlayerAction == PlayerBehavior.AimingShot)) {
			if (Ammo.Magazine > 0) {
				PlayerAction = PlayerBehavior.Recharge;
				goto END;
			} else {
				PlayerAction = PlayerBehavior.None;
				goto END;
			}
		}

		PlayerAction = PlayerBehavior.None;

		END:

		if (CrossPlatformInputManager.GetButton ("SitDown")) {
			PlayerPosition = PlayerBehavior.Sit;
		} else {
			PlayerPosition = PlayerBehavior.Stand;
		}

	}

	//Get movement State
	void MovementStateListener ()
	{

		if (CrossPlatformInputManager.GetButton ("Run") && rgdbFPC.speedCur > 1f) {
			MovementState = PlayerBehavior.Run;
		} else if (rgdbFPC.speedCur == 0 && CrossPlatformInputManager.GetButton ("Aiming")) {
			MovementState = PlayerBehavior.Aiming;
		} else if (rgdbFPC.speedCur > 0 && CrossPlatformInputManager.GetButton ("Aiming")) {
			MovementState = PlayerBehavior.AimingWalk;
		} else if (rgdbFPC.speedCur > 0) {
			MovementState = PlayerBehavior.Walk;
		} else if (rgdbFPC.speedCur == 0) {
			MovementState = PlayerBehavior.Idle;
		}

	}

	// Set current Behavior automatically
	void SetCurrentBehavior ()
	{

		if (CurrentBehavior == PlayerBehavior.Hide || NextBehavior == PlayerBehavior.Hide) {
			
		} else if (CurrentBehavior == PlayerBehavior.Get) {
			
		} else if (MovementState == PlayerBehavior.Run) {
			NextBehavior = MovementState;
		} else if (PlayerAction != PlayerBehavior.None) {
			NextBehavior = PlayerAction;
		} else if (PlayerAction == PlayerBehavior.None) {
			NextBehavior = MovementState;
		}
	}



	void Start ()
	{
		rgdbFPC = GetComponent<RigidbodyFirstPersonController> ();	
		playerHealth = GetComponent<Health> ();
		MovementStateListener ();
	}


	void FixedUpdate ()
	{

		if (playerHealth.health > 0) {
			MovementStateListener ();
			ActionListener ();
			SetCurrentBehavior ();

			if (CurrentBehavior == PlayerBehavior.Get || CurrentBehavior == PlayerBehavior.Hide) {
				Timer += Time.deltaTime;

				if (Timer >= WaitingTimer) {
					ResetGetHideState ();
				}

			}
		}
	}
}
