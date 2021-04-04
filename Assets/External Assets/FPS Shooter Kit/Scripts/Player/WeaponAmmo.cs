using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponAmmo : MonoBehaviour
{

	public int Magazine = 3;
	public int maximalBullet = 10;
	public int maximalBulletGun = 8;
	public int bulletCount = 10;
	public bool magazineEmpty = false;
	public bool OneBullet = false;
	public bool Unlimited = false;

	public void BulletCounter ()
	{

		if (bulletCount > 0) {

			bulletCount--;

		} else {
			
			if (!OneBullet) {
				RechargeMagazine ();
			} else {
				RechargeOneBullet ();
			}

		}

	}

	public void RechargeGun ()
	{

		if (!magazineEmpty) {
			if (!OneBullet) {
				RechargeMagazine ();
			} else {
				
				RechargeOneBullet ();
			}
		}

	}

	void RechargeMagazine ()
	{
		if (Magazine > 0) {
			Magazine--;
			bulletCount = maximalBullet;
		} else {
			magazineEmpty = true;
		}
	}

	void RechargeOneBullet ()
	{

		if (Magazine > 0) {
			Magazine--;
			bulletCount += maximalBullet;
		} else {
			magazineEmpty = true;
		}

	}



}
