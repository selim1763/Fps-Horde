using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleSystemGun : ParticleSystemControlBase
{

	public Transform newPositionBuh;
	public GameObject newBush;
	private Rigidbody rbNewBush;
	public Transform movePositionBuh;
	private GameObject bushClone;
	private bool createdNewBush = false;
	private bool forceIsApplid = false;
	private bool getNewPosition = false;
	public float deltaForce = 0.01f;

	void Start(){
		rbNewBush = newBush.GetComponent<Rigidbody> ();
	}

	public override void ParticleSystemAction ()
	{
						
		bushClone = Instantiate (newBush);
		rbNewBush = bushClone.GetComponent<Rigidbody> ();
		forceIsApplid = false;
		createdNewBush = true;

		if (!forceIsApplid) {
			if (!getNewPosition) {
				getNewPosition = true;
				bushClone.transform.position = newPositionBuh.position;
				bushClone.transform.rotation = newPositionBuh.rotation;
			}
			rbNewBush.AddForce (((movePositionBuh.position - newPositionBuh.position) * deltaForce), ForceMode.Impulse);
			forceIsApplid = true;
			getNewPosition = false;
		}

	}

}
