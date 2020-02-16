using UnityEngine;
using System.Collections;

// Hero Animation State/variable Hashes
public class AnimatorHashIds : MonoBehaviour {
	
	// States
	public int IdleState { get; private set; }
	public int LocomotionIdState { get; private set; }
    public int DyingState { get; private set; }
    public int SneakingState { get; private set; }
    public int RollingState { get; private set; }
    public int StranglingState { get; private set; }

    // Variables
    public int DeadBool { get; private set; }
    public int SneakingBool { get; private set; }
    public int PlayerInSightBool { get; private set; }
    public int InPursuitBool { get; private set; }
    public int InPatrolBool { get; private set; }
    public int InYoujinBool { get; private set; }
    public int OpenBool { get; private set; }
    public int CoverBool { get; private set; }
    public int LookingAroundBool { get; private set; }
    public int CrouchCoverBool { get; private set; }
    public int CrawlingBool { get; private set; }
    public int RollingBool { get; private set; }
    public int StranglingBool { get; private set; }
    public int IsStrangledBool { get; private set; }
    public int PlayerShootingBool { get; private set; }
    public int PlayerRaiseWeapon { get; private set; }

    public int ShotFloat { get; private set; }
    public int AimWeightFloat { get; private set; }
    public int AimWeightPlayerFloat { get; private set; }
    public int AngularSpeedFloat { get; private set; }
    public int SpeedFloat { get; private set; }
    public int SpeedFloatEnemy { get; private set; }
    public int Direction { get; private set; }
    public int Angle { get; private set; }


    // Use this for initialization
    void Start () {
		// Hash all animation names for performance
		IdleState = Animator.StringToHash("Base Layer.Idle");
		LocomotionIdState = Animator.StringToHash("Base Layer.Locomotion");
		DyingState = Animator.StringToHash("Base Layer.Dying");
		SneakingState = Animator.StringToHash("Base Layer.Sneak");
		RollingState = Animator.StringToHash ("Base Layer.Roll");
		StranglingState = Animator.StringToHash ("Base Layer.Strangling");

        DeadBool = Animator.StringToHash("Dead");
		SneakingBool = Animator.StringToHash("Sneak");
		PlayerInSightBool = Animator.StringToHash("PlayerInSight");
		InPursuitBool = Animator.StringToHash("InPursuit");
		InPatrolBool = Animator.StringToHash("InPatrol");
		InYoujinBool = Animator.StringToHash("InYoujin");
		OpenBool = Animator.StringToHash("Open");
		CoverBool = Animator.StringToHash("Cover");
		LookingAroundBool = Animator.StringToHash("LookAround"); 
		CrouchCoverBool = Animator.StringToHash("CrouchCover");
		CrawlingBool = Animator.StringToHash ("Crawling");
		RollingBool = Animator.StringToHash ("Rolling");
        StranglingBool = Animator.StringToHash ("Strangling");
		IsStrangledBool = Animator.StringToHash ("BeingStrangled");
		PlayerShootingBool = Animator.StringToHash ("PlayerShooting");
		PlayerRaiseWeapon = Animator.StringToHash ("PlayerRaiseWeapon");

		ShotFloat = Animator.StringToHash("Shot");
		AimWeightFloat = Animator.StringToHash("AimWeight");
		AimWeightPlayerFloat = Animator.StringToHash ("AimWeightPlayer");
		AngularSpeedFloat = Animator.StringToHash("AngularSpeed");
        SpeedFloat = Animator.StringToHash("Speed");
		SpeedFloatEnemy = Animator.StringToHash ("SpeedEnemy");
		Direction = Animator.StringToHash("Direction");
        Angle = Animator.StringToHash("Angle");
    }
}
