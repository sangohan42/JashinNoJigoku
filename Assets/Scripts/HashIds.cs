using UnityEngine;
using System.Collections;

public class HashIds : MonoBehaviour {
	
	// Hero Hashes
	
	public int m_IdleState = 0;
	public int m_LocomotionIdState = 0;
	public int m_dyingState = 0;
	public int m_sneakingState = 0;
	public int m_rollingState = 0;

	public int Empty_YoujinModeTrans;
	public int WeaponRaise_WeaponLower;
	public int Empty_WeaponRaiseTrans;
	public int Empty_WeaponShootTrans;
	public int Locomotion_CoverTrans;
	public int Cover_LocomotionTrans;

	public int deadBool;
	public int sneakingBool;
	public int playerInSightBool;
	public int inPursuitBool;
	public int inPatrolBool;
	public int inYoujinBool;
	public int openBool;
	public int coverBool;
	public int lookingAroundBool;
	public int crouchCoverBool;
	public int crawlingBool;
	public int rollingBool;
	
	public int shotFloat;
	public int aimWeightFloat;
	public int angularSpeedFloat;
	public int speedFloat;
	public int speedFloatEnemy;
	public int direction;
	public int angle;

	
	// Use this for initialization
	void Start () {
		// Hash all animation names for performance
		m_IdleState = Animator.StringToHash("Base Layer.Idle");
		m_LocomotionIdState = Animator.StringToHash("Base Layer.Locomotion");
		m_dyingState = Animator.StringToHash("Base Layer.Dying");
		m_sneakingState = Animator.StringToHash("Base Layer.Sneak");
		m_rollingState = Animator.StringToHash ("Base Layer.Roll");

		Empty_YoujinModeTrans = Animator.StringToHash("Youjin.Empty -> Youjin.WeaponRaise");
		WeaponRaise_WeaponLower = Animator.StringToHash("Youjin.WeaponRaise -> Youjin.WeaponLower");
		Empty_WeaponRaiseTrans = Animator.StringToHash("Shooting.Empty -> Shooting.WeaponRaise");
		Empty_WeaponShootTrans = Animator.StringToHash("Shooting.Empty -> Shooting.WeaponShoot");
		Locomotion_CoverTrans =Animator.StringToHash("Base Layer.Locomotion -> Base Layer.Cover");
		Cover_LocomotionTrans =Animator.StringToHash("Base Layer.Cover -> Base Layer.Locomotion");

		deadBool = Animator.StringToHash("Dead");
		sneakingBool = Animator.StringToHash("Sneak");
		playerInSightBool = Animator.StringToHash("PlayerInSight");
		inPursuitBool = Animator.StringToHash("InPursuit");
		inPatrolBool = Animator.StringToHash("InPatrol");
		inYoujinBool = Animator.StringToHash("InYoujin");
		openBool = Animator.StringToHash("Open");
		coverBool = Animator.StringToHash("Cover");
		lookingAroundBool = Animator.StringToHash("LookAround"); 
		crouchCoverBool = Animator.StringToHash("CrouchCover");
		crawlingBool = Animator.StringToHash ("Crawling");
		rollingBool = Animator.StringToHash ("Rolling");

		shotFloat = Animator.StringToHash("Shot");
		aimWeightFloat = Animator.StringToHash("AimWeight");
		angularSpeedFloat = Animator.StringToHash("AngularSpeed");
		speedFloat = Animator.StringToHash("Speed");
		speedFloatEnemy = Animator.StringToHash ("SpeedEnemy");
		direction = Animator.StringToHash("Direction");
		angle = Animator.StringToHash("Angle");


	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
