using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorList;
using System;

public class PlayerPlaySound : MonoBehaviour
{
	
	private PlayerBehaviorControl plBehaviorContr;
	public ParticleSystemControlBase plSystemContrl;

	public bool PartSysContr = true;
	[HideInInspector]public bool soundPlaed = false;
	public float percentAnimation = 0.9f;
	[HideInInspector]public float timer = 0f;
	public float timeWait = 0.1f;
	private bool timerStart = false;

	public GameObject PatronMesh;
	public bool showMesh = false;
	private bool setVisual = false;




	[Serializable]	
	public struct BehaviorSound
	{
		public static int NumbBehavior = 1;
		public AudioSource SoundFromBehavior;
		public PlBehavior Behavior;
	}

	public static int NumbBehSound = 1;
	public BehaviorSound[] BhSound = new BehaviorSound[NumbBehSound];

	void Start ()
	{
		plBehaviorContr = GetComponent<PlayerBehaviorControl> ();
	}

	void PlaySound ()
	{
		
		foreach (BehaviorSound nBhSound in BhSound) {

			if (plBehaviorContr.CurAnimator.GetCurrentAnimatorStateInfo (0).IsName (nBhSound.Behavior.AnimationName) && !soundPlaed) {
				timer = 0f;
				nBhSound.SoundFromBehavior.Play ();
				soundPlaed = true;
				if (PartSysContr) {

					plBehaviorContr.Ammo.RechargeGun ();
				
				}
				break;
			}


		}

		int i = (int)plBehaviorContr.CurAnimator.GetCurrentAnimatorStateInfo (0).normalizedTime; 
	
		for (int j = 0; j < BhSound.Length; j++) {
			if (plBehaviorContr.CurAnimator.GetCurrentAnimatorStateInfo (0).IsName (BhSound [j].Behavior.AnimationName) &&
			    plBehaviorContr.CurAnimator.GetCurrentAnimatorStateInfo (0).normalizedTime >= (i + percentAnimation)) {
				timer = 0f;
				timerStart = true;
				break;
			}
		}

		if (soundPlaed && timerStart) {
			timer += Time.deltaTime;
			if (timer >= timeWait) {
				soundPlaed = false;

				timerStart = false;
			}
		}

		if (showMesh) {
			ShowPatron ();
		}

	}


	void ShowPatron ()
	{

		if (plBehaviorContr.CurAnimator.GetCurrentAnimatorStateInfo (0).IsName (BhSound [0].Behavior.AnimationName)) {

			int i = (int)plBehaviorContr.CurAnimator.GetCurrentAnimatorStateInfo (0).normalizedTime;
			if ((plBehaviorContr.CurAnimator.GetCurrentAnimatorStateInfo (0).normalizedTime >= (i + 0.3f)) &&
			    (plBehaviorContr.CurAnimator.GetCurrentAnimatorStateInfo (0).normalizedTime <= (i + 0.9f))
			    && !setVisual) {
				setVisual = true;
				PatronMesh.SetActive (true);
			} else if (plBehaviorContr.CurAnimator.GetCurrentAnimatorStateInfo (0).normalizedTime >= (i + 0.9f) && setVisual) {
				PatronMesh.SetActive (false);
				setVisual = false;
			}
		}
	}

	void FixedUpdate ()
	{
		
		PlaySound ();
	
	}
}
