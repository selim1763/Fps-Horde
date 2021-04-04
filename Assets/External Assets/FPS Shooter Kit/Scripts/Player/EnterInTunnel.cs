using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnterInTunnel : MonoBehaviour
{
	
	public PlayerSitDown playerSitDown;

 
	//If the player exit in trigger, then variable inTunnel from class PlayerSitDown set false
	void OnTriggerExit (Collider other)
	{
		if (other.gameObject.tag.Equals ("Player")) {
			playerSitDown.ChangingInTunnel (false);
		}
	}

	//If the player enter in trigger, then variable inTunnel from class PlayerSitDown set true
	void OnTriggerEnter (Collider other)
	{

		if (other.gameObject.tag.Equals ("Player")) {
			playerSitDown.ChangingInTunnel (true);
		}
	}
}
