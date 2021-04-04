using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using BehaviorList;
namespace ChangingGunSystem{
	
	public class ChangingGun : MonoBehaviour {
			
		[Serializable]
		public class BlockAnimation{


			public string animationName = "";
			public bool currentAnimator = false;
			public Animator animatorObject = null; 
			public string buttonName = "";
			public GameObject gameObjForAnimator = null;
			public PlayerBehaviorControl scriptAnimators ;
			 

			public void ResetAllValues(){
				currentAnimator = false;
				animationName = "";
				animatorObject = null;
				buttonName = "";
				gameObjForAnimator = null;
				scriptAnimators = null;
			}

			public void CopyValues(BlockAnimation objValues){

				animationName = objValues.animationName;
				currentAnimator = objValues.currentAnimator;
				animatorObject = objValues.animatorObject;
				buttonName = objValues.buttonName;
				gameObjForAnimator = objValues.gameObjForAnimator;
				scriptAnimators = objValues.scriptAnimators;
				
			}

		}

		public	BlockAnimation[] EntAnimList = new BlockAnimation [10];

		public BlockAnimation CurrentAnimator, NextAnimator;
			
		[HideInInspector]	public bool changeAnimation = false;

		public CountAmmo counterAmmo;
		private Health playerHealth;

		void Start(){
			
			SearchCurrentAnimation ();
			playerHealth = GetComponent<Health> ();

		}



		void WeaponСhange(){

			if (CrossPlatformInputManager.GetButton ("1")) {
				GetAnimatorByButtonName ("1");
			}

			if (CrossPlatformInputManager.GetButton ("2")) {
				GetAnimatorByButtonName ("2");
			}

			if (CrossPlatformInputManager.GetButton ("3")) {
				GetAnimatorByButtonName ("3");
			}

			if (CrossPlatformInputManager.GetButton ("4")) {
				GetAnimatorByButtonName ("4");
			}

			if (CrossPlatformInputManager.GetButton ("5")) {
				GetAnimatorByButtonName ("5");
			}

			if (CrossPlatformInputManager.GetButton ("6")) {
				GetAnimatorByButtonName ("6");
			}

			if (CrossPlatformInputManager.GetButton ("7")) {
				GetAnimatorByButtonName ("7");
			}

			if (CrossPlatformInputManager.GetButton ("8")) {
				GetAnimatorByButtonName ("8");
			}

			if (CrossPlatformInputManager.GetButton ("9")) {
				GetAnimatorByButtonName ("9");
			}

			if (CrossPlatformInputManager.GetButton ("0")) {
				GetAnimatorByButtonName ("0");
			}
		}

		void GetAnimatorByButtonName(string butName){

			if (!CurrentAnimator.buttonName.Equals (butName)) {
				int i = 0;
				if (!changeAnimation) {
					while (EntAnimList [i].buttonName.Equals (butName) || i < 10) {

						if (EntAnimList [i].buttonName.Equals (butName) && !CurrentAnimator.buttonName.Equals (butName)) {
							NextAnimator.CopyValues (EntAnimList [i]);
							//NextAnimator.nextAnimator = true;
							counterAmmo.Ammo = NextAnimator.gameObjForAnimator.gameObject.GetComponent<WeaponAmmo> ();
							changeAnimation = true;
							break;
						}
						i++;
					}
				}

			}
		}

		void SearchCurrentAnimation(){
									
				int i = 0;
				while (!EntAnimList [i].currentAnimator || i < 10) {

					if (EntAnimList [i].currentAnimator) {

						CurrentAnimator.CopyValues (EntAnimList [i]);
						CurrentAnimator.gameObjForAnimator.SetActive (true);
						counterAmmo.Ammo = CurrentAnimator.gameObjForAnimator.gameObject.GetComponent<WeaponAmmo>();
						break;
					}

					i++;
				}



		}

		void ListenerAnimations(){

			

			if(changeAnimation){
				
				CurrentAnimator.scriptAnimators.plrBehavior.NextBehavior = PlayerBehavior.Hide;
				AnimatorStateInfo stateInfo = CurrentAnimator.animatorObject.GetCurrentAnimatorStateInfo (0);
					
				if (stateInfo.IsName ("BaseLayer.Hide") && stateInfo.normalizedTime >= 0.8f) {

					CurrentAnimator.gameObjForAnimator.SetActive (false);
					CurrentAnimator.ResetAllValues ();

					CurrentAnimator.CopyValues (NextAnimator);
					CurrentAnimator.gameObjForAnimator.SetActive (true);
					CurrentAnimator.scriptAnimators.plrBehavior.CurrentBehavior = PlayerBehavior.Get;
					CurrentAnimator.scriptAnimators.plrBehavior.NextBehavior = PlayerBehavior.Idle;
					CurrentAnimator.scriptAnimators.plrBehavior.SetAmmo(CurrentAnimator.scriptAnimators.Ammo);

					changeAnimation = false;

				}

							
			}

		}

		void Update () {
			if (playerHealth.health > 0) {
				WeaponСhange ();
				ListenerAnimations ();
			}

		}

	}
}