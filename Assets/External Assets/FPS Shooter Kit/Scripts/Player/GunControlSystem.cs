using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunControlSystem : MonoBehaviour
{

	public float range = 2f;
	public float rangeshot = 2f;
	public static int numBullet = 16;
	public Transform transfObj;
	public float minRadius = 0.005f;
	public float radius = 1f;
	public float timer = 0f;
	public float timeBettwen = 1f;
	public bool random = false;
	public bool debug = false;
	public int gunDamage = 1;

	[HideInInspector]public bool Shot = false;
	public static int numberOfBullets = 1;

	private float angelx = 0f;
	private float angely = 0f;
	private float angelz = 0f;

	private Vector3 startPosition;
	private Vector3 endPosition;
	private Vector3 startCenter;

	public float timerLight = 0f;
	public float timeFlash = 0.1f;
	public Light ligth;
	public GameObject ParticleHited;

	[System.Serializable]
	public struct Bullet
	{
		public Vector3 vBullet;
		public Vector3 Center;
	}

	public Bullet[] seletedRay = new Bullet[numberOfBullets];

	public Bullet[] listBullet = new Bullet[numBullet];

	void DrawLineBullet ()
	{

		foreach (Bullet nBullet in listBullet) {

			Debug.DrawRay (nBullet.Center, SumTwoVectors (nBullet.vBullet, transfObj.forward * range), Color.green);
		}
	}

	void CalculateDispersionBullet ()
	{

		timer = 0f;

		Vector3 startCenter = new Vector3 (startPosition.x, startPosition.y, startPosition.z);
		angelx = transfObj.rotation.eulerAngles.x;
		angely = transfObj.rotation.eulerAngles.y;
		angelz = transfObj.rotation.eulerAngles.z;


		float delta = (2 * Mathf.PI) / listBullet.Length;

		float theta = 0;

		for (int i = 0; i < listBullet.Length; i++) {
			float nRadius = 0f;
			if (random) {
				nRadius = Random.Range (minRadius, radius);
			} else {
				nRadius = radius;
			}

			float x = nRadius * Mathf.Cos (theta);
			float y = nRadius * Mathf.Sin (theta);
			float z = 0f;
		
			listBullet [i].Center = startCenter;

			listBullet [i].vBullet = Quaternion.Euler (angelx, angely, angelz) * new Vector3 (x, y, z);
		
			theta += delta; 

		}

	}

	void DrawSelectRay ()
	{
		foreach (Bullet nRay in seletedRay) {
		
			Debug.DrawRay (nRay.Center, SumTwoVectors (nRay.vBullet, transfObj.forward * range), Color.red);
		
		}
	}

	void CalculationVectorToWorldSpace ()
	{

		for (int i = 0; i < listBullet.Length; i++) {

			listBullet [i].vBullet = SumTwoVectors (listBullet [i].vBullet, startPosition);
			listBullet [i].Center = SumTwoVectors (listBullet [i].vBullet, endPosition);
		}

	}

	void CalculationStartEndVector ()
	{
		startPosition = transform.TransformPoint (new Vector3 (0, 0, 0));
		endPosition = transfObj.forward * range;
		startCenter = new Vector3 (transfObj.forward.x, transfObj.forward.y, transfObj.forward.z) * range;
	}

	void Start ()
	{
		
		CalculationStartEndVector ();
		CalculationVectorToWorldSpace ();
	}

	void SelectRangomRay ()
	{

		for (int i = 0; i < seletedRay.Length; i++) {

			int selectRay = Random.Range (0, listBullet.Length - 1);
			seletedRay [i] = listBullet [selectRay];

		}
	

	}

	Vector3 SumTwoVectors (Vector3 a, Vector3 b)
	{

		Vector3 result = new Vector3 (a.x + b.x, a.y + b.y, a.z + b.z);

		return result;

	}

	void GunShot ()
	{
		
		foreach (Bullet nRay in seletedRay) {
		
			RaycastHit hit = new RaycastHit ();
			if (Physics.Raycast (nRay.Center, SumTwoVectors (nRay.vBullet, transfObj.forward * rangeshot), out hit, rangeshot)) {
				
				if (hit.collider.gameObject.tag.Equals ("HitedEnemy")) {

					GameObject HitBlood = Instantiate (ParticleHited);
					HitBlood.transform.position = hit.point;
					HitBlood.transform.rotation = hit.transform.rotation;
					EnemyDamage enemDamage = hit.collider.gameObject.GetComponent<EnemyDamage> ();
					enemDamage.SetDamage (gunDamage);

				}
					
			}	
			
		}

		Shot = false;

	}

	void FlashLight ()
	{

		if (timerLight >= timeFlash) {
			ligth.enabled = false;
		}

	}

	void Update ()
	{
		
		CalculationStartEndVector ();

		timer += Time.deltaTime;
		if (timer >= timeBettwen) { 

			CalculateDispersionBullet ();	
			SelectRangomRay ();

		}
		if (debug) {
			
			DrawLineBullet ();
			DrawSelectRay ();
		
		}
		if (Shot) {
			GunShot ();
			timerLight = 0;
			ligth.enabled = true;
		}

		timerLight += Time.deltaTime;

		FlashLight ();


	}

}
