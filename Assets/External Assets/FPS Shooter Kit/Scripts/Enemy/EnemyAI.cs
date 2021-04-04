using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EnemyBehavior;

public class EnemyAI : MonoBehaviour
{

	public bool navMeshEnable = true;
	public UnityEngine.AI.NavMeshAgent navAgent;
	public GameObject positionPlayer;
	public EnemyBehaviorControl enemyBehContrl;
	public EnemyAttackDetection attackDetection;
	public EnemyHealth enmHealth;
	public float curSpeed = 0f;
	public float maxSpeed = 1;
	public float speedRun = 3;
	public float minDistance = 5f;
	public int attackCount = 0;
	public string AnimationIdle, AnimationIdleOther;

	[HideInInspector]public bool Hited = false;

	private bool PlayerVisible = false;

	public EnemyBehaviorList[] attackList = new EnemyBehaviorList[1];
	public string[] AnimationBlock = new string[0];

	void Start ()
	{
		
		if (navMeshEnable) {
		
			navAgent.enabled = true;
			curSpeed = navAgent.velocity.magnitude;
		}

	
		if (navMeshEnable) {
			navAgent.isStopped = true;
		}

	}

	void OnTriggerEnter (Collider other)
	{

		if (enmHealth.health > 0) {
			if (navMeshEnable && other.gameObject.tag.Equals ("Player")) {
				navAgent.isStopped = false;
				PlayerVisible = true;

			}
		}

	}

 
	void SetMaxSpeed ()
	{
		if (navMeshEnable && PlayerVisible) {
			if (navAgent.speed != maxSpeed) {
				navAgent.speed = maxSpeed;
			
			}
		}

	}

	void SetBehavior ()
	{

		if (enmHealth.health > 0) {

			if (!Hited) {

				if (PlayerVisible) {

					if (!navAgent.isStopped) {

						navAgent.SetDestination (positionPlayer.transform.position);
				
						if (navAgent.remainingDistance <= minDistance) {
							maxSpeed = 1f;
						}

						if (navAgent.remainingDistance > minDistance + 0.5f) {

							maxSpeed = 4f;
						}

						if (curSpeed > 0f && curSpeed <= (speedRun)) {

							enemyBehContrl.CurrentBehavior = EnemyBehaviorList.Walk;

						} else if (curSpeed > (speedRun)) {
							enemyBehContrl.CurrentBehavior = EnemyBehaviorList.Run;
						} else if (curSpeed == 0) {

							enemyBehContrl.CurrentBehavior = EnemyBehaviorList.Idle;
						}
						curSpeed = navAgent.velocity.magnitude;
					} else {
						enemyBehContrl.CurrentBehavior = EnemyBehaviorList.Idle;
					}
				} else {
					enemyBehContrl.CurrentBehavior = EnemyBehaviorList.Idle;
				}


				if (attackDetection.attack && attackCount <= 3) {
			
					attackCount++;
					int nAttack = Random.Range (0, (attackList.Length));
					if (nAttack == attackList.Length) {
						nAttack = attackList.Length - 1;
					}
					enemyBehContrl.CurrentBehavior = attackList [nAttack];
	

				} else if (attackCount > 3) {
					attackCount = 0;
					enemyBehContrl.CurrentBehavior = EnemyBehaviorList.Shout;

				} 
			} else {
			
				enemyBehContrl.CurrentBehavior = EnemyBehaviorList.GetHit;
				Hited = false;
			}
		}
		if (enmHealth.health <= 0) {
			navAgent.enabled = false;
			enemyBehContrl.CurrentBehavior = EnemyBehaviorList.Dead;
			enmHealth.DeactivateCollider ();
		}


	}



	void BlockingMoveEnemyOnAnimation ()
	{

		bool AnimIsBlocked = false;

		if (enmHealth.health > 0) {
			if (AnimationBlock.Length > 0 && PlayerVisible) {


				AnimatorStateInfo curAnimation = enemyBehContrl.animator.GetCurrentAnimatorStateInfo (0); 
				AnimatorStateInfo nexAnimation = enemyBehContrl.animator.GetNextAnimatorStateInfo (0);

				foreach (string nAnimation in AnimationBlock) {

					if (curAnimation.IsName (nAnimation)) {
						AnimIsBlocked = true;
						int i = (int)curAnimation.normalizedTime;

						if (curAnimation.normalizedTime <= i + 0.9f) {
						
							navAgent.isStopped = true;

						} else {

							navAgent.isStopped = false;
						}

					}

					if (nexAnimation.IsName (nAnimation)) {

						if (nexAnimation.normalizedTime > 0f) {

							navAgent.isStopped = true;
						}

					}

				}

			}

			if (PlayerVisible && !AnimIsBlocked) {

				navAgent.isStopped = false;

			}

		}

	}

	void SetStateIdleOther ()
	{

		AnimatorStateInfo StateInfo = enemyBehContrl.animator.GetCurrentAnimatorStateInfo (0);

		if (StateInfo.IsName (AnimationIdle)) {

			int i = (int)StateInfo.normalizedTime;

			if (i > 3.9f) {

				enemyBehContrl.CurrentBehavior = EnemyBehaviorList.IdleOther;
			}

		}

		if (StateInfo.IsName (AnimationIdleOther)) {

			if (StateInfo.normalizedTime > 0.9f) {

				enemyBehContrl.CurrentBehavior = EnemyBehaviorList.Idle;

			}

		}

	}

	void FixedUpdate ()
	{

		BlockingMoveEnemyOnAnimation ();
		if (navMeshEnable) {
			SetMaxSpeed ();
		}
		SetBehavior ();
		
		SetStateIdleOther ();

	}
}
