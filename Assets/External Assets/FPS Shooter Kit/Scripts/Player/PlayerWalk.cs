using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;
using UnityStandardAssets.CrossPlatformInput;

public class PlayerWalk : MonoBehaviour
{

	 
	public float timer = 0f;
	public float timerJump = 0f;
	public float waitTimeWalk = 0.5f;
	public float waitTimeRun = 0.28f;
	public float waitJump = 0.2f;

	public AudioSource audioWalk;
	public AudioSource audioRun;
	public AudioSource audioJump;
	public AudioSource audioLanding;
	private RigidbodyFirstPersonController player;
	private PlayerBehaviorListener PlBehaviorListr;

	private bool run = false;
	private bool jump = false;
	private bool jumpPlay = false;
	private Health playerHealth;

	void Start ()
	{
		player = GetComponent<RigidbodyFirstPersonController> ();
		PlBehaviorListr = GetComponent<PlayerBehaviorListener> ();
		playerHealth = GetComponent<Health> ();
	}

	void FixedUpdate ()
	{
		if (playerHealth.health > 0) {
		
			timer += Time.deltaTime;
			timerJump += Time.deltaTime;

			if (PlBehaviorListr.MovementState == BehaviorList.PlayerBehavior.Run) {
				run = true;
			} else {
				run = false;
			}

			if (!run) {
				if (timer >= waitTimeWalk && player.speedCur > 0f && !player.Jumping) {
					AudioPlaySoundWalk ();
				}
			}

			if (run && player.Grounded) {
				if (timer >= waitTimeRun && player.speedCur > 0f) {

					AudioPlaySoundWalk ();
				}
			}

			if (player.Jumping && !jump) {

				jump = true;
			}

			if (jump && !jumpPlay) {
			
				AudioPlayJump ();
			}

			if (timerJump >= waitJump && jumpPlay) {
				if (player.Grounded) {
					AudioPlayLanding ();
				} 
			}

			if (CrossPlatformInputManager.GetButton ("Aiming")) {

				waitTimeWalk = 0.8f;
			 
			}

			if (!CrossPlatformInputManager.GetButton ("Aiming")) {

				waitTimeWalk = 0.5f;

			}
		}
	}

	void AudioPlayJump ()
	{
		timerJump = 0f;
		audioJump.Play ();
		jumpPlay = true;
	}

	void AudioPlayLanding ()
	{
		audioLanding.Play ();
		jumpPlay = false;
		jump = false;
	}

	void AudioPlaySoundWalk ()
	{

		timer = 0f;

		if (!run) {
			audioWalk.Play ();
		}
		if (run) {
			audioRun.Play ();
		}
	}



}
