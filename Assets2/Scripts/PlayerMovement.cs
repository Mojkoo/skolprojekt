﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

/*
	Datum: 2017-09-24
*/

enum e_PlayerStates {
	IDLE,
	MOVE,
	LEDGEHANG,
	ATTACK
}

public class PlayerMovement : NetworkBehaviour {

	public float acceleration = 0.2f;
	public float deacceleration = 0.2f;

	public GameObject hitParticlePrefab;
	public GameObject impactOnGroundParticlePrefab;
    public GameObject o;

    Player player;

	float movespeed = 3;
	float attackRange = 1;
	float jumpspeed = 6;
	float rotationSpeed = 6;

	float xAxis;
	float zAxis;
	bool hasHitEnemy = false;
	bool isTouchingFloor;
	bool isHanging;
	Quaternion rotationTowardsWall;
	

	int state = (int)e_PlayerStates.MOVE;


	public Animator animator;	
	public Quaternion rot;

	Vector3 prevPosition;
	Rigidbody rb;
	Vector3 positionFromServer;

	void Start () {
		animator = GetComponent<Animator>();
		rb = GetComponent<Rigidbody>();
        player = GetComponent<Player>();
	}

	void jump () {
		Debug.Log("Jump!");
		float y = rb.velocity.y;
		y = 6f;
		rb.velocity = new Vector3 (rb.velocity.x, y, rb.velocity.z);
	}

	void FixedUpdate () {
		if (!isLocalPlayer) return;

		switch (state) {
		case (int)e_PlayerStates.IDLE:
			if (isMoving ()) {
				state = (int)e_PlayerStates.MOVE;
			}
			break;

		case (int)e_PlayerStates.MOVE:
			rb.velocity = new Vector3 (xAxis, 0, zAxis) * movespeed + (Vector3.up * rb.velocity.y);			

			if (isMoving ()) {
				rot = Quaternion.LookRotation (new Vector3 (xAxis, 0, zAxis));
			}
			transform.rotation = Quaternion.Slerp (transform.rotation, rot, rotationSpeed * Time.deltaTime);

			if (isTouchingFloor) {
				isHanging = false;
				if (Input.GetKey (KeyCode.Space)) {
					jump ();
				}
			}
			break;

		case (int)e_PlayerStates.LEDGEHANG:
			freeze();
			if (Input.GetKey(KeyCode.Space) && isHanging) {
				state = (int)e_PlayerStates.MOVE;
				unfreeze();
				jump();
			}
			break;

		case (int)e_PlayerStates.ATTACK:

			AnimatorStateInfo stateInfo0 = animator.GetCurrentAnimatorStateInfo (0);
			if(stateInfo0.IsName("Slash")){
				if (stateInfo0.normalizedTime > 0f) {
					damageInRange (this.attackRange);
				}
				if(stateInfo0.normalizedTime > 1f){
					animator.SetBool("slash", false);
					unfreeze();
					state = (int)e_PlayerStates.IDLE;
				}
			}

			if (animator.GetBool ("slash")) {
				freeze ();
			}
			break;
		}

		if (canLedgeGrab ()) {
			state = (int)e_PlayerStates.LEDGEHANG;
		}	
	}

	public bool canLedgeGrab () {
		Vector3 startPos = new Vector3 (transform.position.x, transform.position.y - 1.4f, transform.position.z);
		Ray ray = new Ray (startPos, transform.forward);
		RaycastHit hit;
		bool hasHit = Physics.Raycast (ray, out hit, 1f);
		Debug.DrawRay(startPos, transform.forward, Color.blue);

		if (hasHit && !isTouchingFloor && !isHanging) {
			rotationTowardsWall = rotateTowards(hit.transform.position);
			isHanging = true;
			return true;
		}
		return false;
	}

	public bool isAboutToCollide () {
		RaycastHit hit;
		if (rb.SweepTest (Vector3.forward, out hit)) {
			return true;
		}
		return false;
	}

	void Update () {
		if (!isLocalPlayer)
			return;
		xAxis = Input.GetAxisRaw ("Horizontal");
		zAxis = Input.GetAxisRaw ("Vertical");

		CmdPositionToServer (transform.position, transform.rotation);

		AnimatorStateInfo stateInfo0 = animator.GetCurrentAnimatorStateInfo (0);

		if (Input.GetKey (KeyCode.LeftControl) && !stateInfo0.IsName ("Slash")) {
			animator.SetBool ("slash", true);
			state = (int)e_PlayerStates.ATTACK;
		}
        Skill skillThatWillBeLaunched = player.getSkillUiManager().isPlayerTryingToActivateSkill();
        if(skillThatWillBeLaunched != null) {
            this.player.getSkillManager().castSkill(skillThatWillBeLaunched);
            animator.SetBool("magicAttack_1", true);
        }
        /*if (Input.GetKeyDown(KeyCode.Z)) {
            castSkill(this.player.skills[0]);
        }*/
	}

	void damageInRange (float attackRange) {
		Instantiate(impactOnGroundParticlePrefab).transform.position = this.transform.position;
		Collider[] colliders = Physics.OverlapSphere (transform.position, attackRange);
		for (int i = 0; i < colliders.Length; i++) {
            if(colliders[i].CompareTag("Enemy")){
                CmdDamageEnemy(colliders[i].gameObject, this.gameObject, 10);
            }
		}
	}

	void freeze (){
		rb.constraints = RigidbodyConstraints.FreezeAll;
	}
	void unfreeze(){
		rb.constraints = RigidbodyConstraints.FreezeRotation;
	}

	void LateUpdate(){
		prevPosition = transform.position;
	}

	bool isMoving () {
		if (xAxis != 0 || zAxis != 0) {
			animator.SetBool("walk",true);
			return true;
		}
		animator.SetBool("walk",false);
		return false;
	}

	void OnCollisionEnter (Collision col) {
		if (col.gameObject.CompareTag ("Ground")) {
			isTouchingFloor = true;
		}
		if (col.gameObject.CompareTag ("Wall")) {
			//.velocity = -transform.forward * 2f;
		}
		//rb.AddForce(-transform.forward * 200f);
	}

	void OnCollisionExit (Collision col) {
		if (col.gameObject.CompareTag ("Ground")) {
			isTouchingFloor = false;
		}		
	}

	Quaternion rotateTowards(Vector3 pos) {
		float x = (pos - this.transform.position).normalized.x;
		float y = 0;
		float z = (pos - this.transform.position).normalized.z;

		Quaternion r = Quaternion.LookRotation(new Vector3(x,y,z));
		Vector3 v = r.eulerAngles;
		v.x = Mathf.Round(v.x / 90) * 90;
		v.y = Mathf.Round(v.y / 90) * 90;
		v.z = Mathf.Round(v.z / 90) * 90;
		r.eulerAngles = v;
		return r;
	}

	[Command]
	void CmdDamageEnemy(GameObject enemy, GameObject damager, int dmg){
        if(enemy != null){
            enemy.GetComponent<MobManager>().damage(dmg, damager);
        }
	}

    /*
	[ClientRpc]
	void RpcDamageEnemy(MobManager mob, int dmg, Player damager) {
		mob.damage (dmg, damager);
	}*/

	[Command]
	void CmdPositionToServer(Vector3 pos, Quaternion rot){
		transform.position = pos;
		transform.rotation = rot;
		RpcPositionToClients(pos, rot);
	}

	[ClientRpc]
	void RpcPositionToClients (Vector3 pos, Quaternion rot) {
		if (!isLocalPlayer) {
			transform.position = pos;
			transform.rotation = rot;
		}
	}
}
