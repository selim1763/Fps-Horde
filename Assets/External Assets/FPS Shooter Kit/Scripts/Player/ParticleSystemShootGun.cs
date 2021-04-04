using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleSystemShootGun : ParticleSystemControlBase {

	public Transform newPositionBuh;
	public GameObject newBush;
	public Rigidbody rbNewBush;
	public Transform movePositionBuh;
	private GameObject bushClone;
	public bool createdNewBush = false;
	public bool getForce = false;
	public bool getNewPosition = false;
	public float deltaForce = 0.01f;

	public GameObject PatronMesh;

	public bool SecondAction = false;

	public override	void ParticleSystemAction(){
					
			bushClone = Instantiate (newBush);
			rbNewBush = bushClone.GetComponent<Rigidbody> ();
			getForce = false;
			createdNewBush = true;


		if ( !getForce) {
			if (!getNewPosition) {
				getNewPosition = true;
				bushClone.transform.position = newPositionBuh.position;
				bushClone.transform.rotation = newPositionBuh.rotation;
			}
			rbNewBush.AddForce (((movePositionBuh.position - newPositionBuh.position) * deltaForce), ForceMode.Impulse);
			getForce = true;
			getNewPosition = false;
		}

	}

	void ShowPatron(){

		if (SecondAction) {
			PatronMesh.SetActive (true);
		} else {
			PatronMesh.SetActive (false);
		}
	}
	
	 
	void FixedUpdate () {

		Debug.DrawLine(newPositionBuh.position , movePositionBuh.position, Color.green);

	}
}
