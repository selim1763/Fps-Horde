using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CountAmmo : MonoBehaviour
{

	Text counterAmmo;
	public WeaponAmmo Ammo;

	void Awake ()
	{
		
		counterAmmo = GetComponent<Text> ();
		
	}

	void FixedUpdate ()
	{
		
		counterAmmo.text = "Ammo: " + Ammo.bulletCount + " / " + Ammo.Magazine;
		
	}
}
