using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectWeapon : MonoBehaviour
{

	private Animator plAnimator;
	public AudioSource sound;
	public string AnimationName;
	private int iterator = 0;

	void Start ()
	{
		plAnimator = GetComponent<Animator> ();
	}

	void FixedUpdate ()
	{

		if (plAnimator.GetCurrentAnimatorStateInfo (0).IsName (AnimationName)) {
			int i = (int)plAnimator.GetCurrentAnimatorStateInfo (0).normalizedTime;

			if (i == iterator) {
			
				iterator++;

				sound.Play ();

			}
		} else {

			iterator = 0;
		}

	}
}
