using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This script defines time to destroy game object

public class BushTimeLife : MonoBehaviour {
	//timer
	public float timer = 0f;
	//Game object lifetime in seconds
	public float timeLife = 10f;
	
	// Use this for initialization
	void Start () {
		
		timer = 0f;

	}
	
	// Update is called once per frame
	void Update () {

		timer += Time.fixedDeltaTime;
		if (timer >= timeLife) {

			Destroy (this.gameObject);

		}
		
	}
}
