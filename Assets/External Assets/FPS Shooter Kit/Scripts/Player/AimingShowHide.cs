using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorList;



public class AimingShowHide : MonoBehaviour
{
	//Player behavior listener
	private PlayerBehaviorListener plBehaviorListnr;
	//Game object
	public GameObject Aiming;
	//A flag show or hide
	bool Actived = true;

	void Start ()
	{

		plBehaviorListnr = GetComponent<PlayerBehaviorListener> ();

	}

	void FixedUpdate ()
	{
		//Hide weapon sight
		if (Actived) {
			if (plBehaviorListnr.CurrentBehavior == BehaviorList.PlayerBehavior.Aiming ||
			    plBehaviorListnr.CurrentBehavior == BehaviorList.PlayerBehavior.AimingShot ||
			    plBehaviorListnr.CurrentBehavior == BehaviorList.PlayerBehavior.AimingWalk) {

				Aiming.SetActive (false);
				Actived = false;

			} 
			//Show	weapon sight
		} else if (plBehaviorListnr.CurrentBehavior != BehaviorList.PlayerBehavior.Aiming &&
		         plBehaviorListnr.CurrentBehavior != BehaviorList.PlayerBehavior.AimingShot &&
		         plBehaviorListnr.CurrentBehavior != BehaviorList.PlayerBehavior.AimingWalk) {
			Aiming.SetActive (true);
			Actived = true;
		}

	}
}
