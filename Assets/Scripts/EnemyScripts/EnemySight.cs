using System;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(SphereCollider))]
[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Animator))]
public class EnemySight : MonoBehaviour
{
    [SerializeField]
    private float _fieldOfViewAngle = 90f;				// Number of degrees, centred on forward, for the enemy see.
    [SerializeField]
    private float _shootingDistance = 5;

    private NavMeshAgent _nav;                              // Reference to the NavMeshAgent component.
    private SphereCollider _playerDetectableSphereCollider; // Reference to the enemy sight sphere collider trigger component.
    private Animator _anim;								    // Reference to the Animator.
    private GameObject _player;                             // Reference to the player.
    private Animator _playerAnim;                           // Reference to the player's animator component.
    private PlayerLifeHandler _playerLifeHandler;           // Reference to the player's health script.
    private AnimatorHashIds _animatorHashIds;               // Reference to the HashIDs.
    private ParticleSystem _interrogativePointParticleSystem;
    private GameObject _interrogativePointObject;
    private bool _resetFOVColor;
    private CharacterControllerLogic _characterControllerLogic;
    private CapsuleCollider _playerCapsuleCollider;
    private SoundManager _soundManager;

    public bool InPatrol { get; set; }
    public bool InPursuit { get; set; }
    public bool PlayerInSight { get; set; }             // Whether or not the player is currently sighted.
    public Vector3 PersonalLastSighting { get; set; }               // Last place this enemy spotted the player.

    public float ShootingDistance
    {
        get { return _shootingDistance; }
    }
    public bool IsDead { get; set; }

    public GameObject FOV;
    public Texture FOV1;
	public Texture FOV2;
	public Texture FOV3;

    void Awake ()
	{
		// Setting up the references.
		_nav = GetComponent<NavMeshAgent>();
		_playerDetectableSphereCollider = GetComponent<SphereCollider>();
		_anim = GetComponent<Animator>();
		_player = GameObject.FindGameObjectWithTag(Tags.player);
		_playerAnim = _player.GetComponent<Animator>();
		_playerLifeHandler = _player.GetComponent<PlayerLifeHandler>();

		_animatorHashIds = GameObject.FindGameObjectWithTag(Tags.animatorHashController).GetComponent<AnimatorHashIds>();
		_characterControllerLogic = GameObject.FindGameObjectWithTag (Tags.player).GetComponent<CharacterControllerLogic> ();

        _interrogativePointObject = transform.Find("InterrogativePoint").gameObject;
        _interrogativePointParticleSystem = _interrogativePointObject.GetComponent<ParticleSystem>();
        _interrogativePointObject.SetActive(false);
        _playerCapsuleCollider = _player.GetComponent<CapsuleCollider>();

        _resetFOVColor = true;

        _soundManager = GameObject.FindGameObjectWithTag(Tags.soundmanager).GetComponent<SoundManager>();

        InPatrol = true;
        InPursuit = false;
        PlayerInSight = false;
        PersonalLastSighting = new Vector3(1000, 1000, 1000);
        IsDead = false;
    }

	soundName GetRandomYoujinSoundName()
	{
        int randomVal = Random.Range(1, Enum.GetNames(typeof(soundName)).Length);

        switch (randomVal)
		{
			case 1 :
			return soundName.SE_EnemyYoujin1;
			case 2 :
			return soundName.SE_EnemyYoujin2;
			case 3 :
			return soundName.SE_EnemyYoujin3;
			case 4 :
			return soundName.SE_EnemyYoujin4;
			case 5 :
			return soundName.SE_EnemyYoujin5;
			default:
			return soundName.SE_EnemyYoujin1;
		}
	}
	
	void Update ()
	{
	
		// If the player is alive...
		if(_playerLifeHandler._health > 0f && !IsDead)
		{
			// ... set the animator parameter to whether the player is in sight or not.
			_anim.SetBool(_animatorHashIds.PlayerInSightBool, PlayerInSight);
			_anim.SetBool (_animatorHashIds.InPursuitBool, InPursuit);
			_anim.SetBool (_animatorHashIds.InPatrolBool, InPatrol);

			int youJinLayerTransition = _anim.GetAnimatorTransitionInfo(3).nameHash;
			int shootingLayerTransition = _anim.GetAnimatorTransitionInfo(1).nameHash;

			//In YOUJIN mode
			if(youJinLayerTransition == _animatorHashIds.Empty_YoujinModeTrans && _interrogativePointObject.activeSelf == false)
			{
				FOV.GetComponent<Renderer>().material.SetTexture("_MainTex", FOV3);
				_interrogativePointObject.SetActive(true);
				_interrogativePointParticleSystem.Play();
				_anim.SetBool(_animatorHashIds.InYoujinBool, true);
				_soundManager.PlaySound(GetRandomYoujinSoundName());
			}
			//In PATROL mode
			else if((youJinLayerTransition == _animatorHashIds.WeaponRaise_WeaponLower || 
			         shootingLayerTransition == _animatorHashIds.Empty_WeaponRaiseTransition ||
			         shootingLayerTransition == _animatorHashIds.Empty_WeaponShootTransition) && 
			        _interrogativePointObject.activeSelf == true)
			{
				FOV.GetComponent<Renderer>().material.SetTexture("_MainTex", FOV1);

				_interrogativePointParticleSystem.Stop();
				_interrogativePointObject.SetActive(false);
				_anim.SetBool(_animatorHashIds.InYoujinBool, false);
			}

			//PURSUIT mode
			if((shootingLayerTransition == _animatorHashIds.Empty_WeaponRaiseTransition ||
			   shootingLayerTransition == _animatorHashIds.Empty_WeaponShootTransition) && _resetFOVColor)
			{
				FOV.GetComponent<Renderer>().material.SetTexture("_MainTex", FOV2);
				_resetFOVColor = false;
				_characterControllerLogic.NumberOfEnemyInPursuitMode ++;
			}

			else if(_anim.GetBool(_animatorHashIds.InPatrolBool) == true && !_resetFOVColor)
			{
				FOV.GetComponent<Renderer>().material.SetTexture("_MainTex",FOV1);
				_resetFOVColor = true;
			}

		}

		//Player is Dead
		else
		{
			// ... set the animator parameter to false.
			_anim.SetBool(_animatorHashIds.PlayerInSightBool, false);
			_anim.SetBool (_animatorHashIds.InPursuitBool, false);
			_anim.SetBool (_animatorHashIds.InPatrolBool, true);
		}

	}

	void LateUpdate()
	{
		Vector3 euler = FOV.transform.localEulerAngles;
		euler.x = 0;
		euler.z = 0;
		FOV.transform.localEulerAngles = euler;
	}
	

	void OnTriggerStay (Collider other)
    {
		// If the player has entered the trigger sphere...
        if(other.gameObject == _player)
        {
			// By default the player is not in sight.
			PlayerInSight = false;

			_anim.SetLayerWeight(3,1);

			// Create a vector from the enemy to the player and store the angle between it and forward.
            Vector3 direction = other.transform.position - transform.position;

			float angle = Vector3.Angle(direction, transform.forward);

			// Store the name hashes of the current states.
			int playerLayerZeroStateHash = _playerAnim.GetCurrentAnimatorStateInfo(0).nameHash;

			RaycastHit hit;
//				Debug.DrawLine(transform.position + caps.center.y*transform.up, transform.position + caps.center.y*transform.up + direction.normalized, Color.cyan);
			// ... and if a raycast towards the player hits something...
			if(Physics.Raycast(transform.position + _playerCapsuleCollider.center.y*transform.up, direction.normalized, out hit, _playerDetectableSphereCollider.radius))
			{

				// ... and if the raycast hits the player (there is no obstacles between the current enemy and the player)...
				if(hit.collider.gameObject == _player)
				{
					// If the angle between forward and where the player is, is less than half the angle of view...
					if(angle < _fieldOfViewAngle * 0.5f)
					{
						// ... the player is in sight.
						PersonalLastSighting = _player.transform.position;
						PlayerInSight = true;
						InPursuit = true;
						InPatrol = false;
						_anim.SetLayerWeight(3,0);
					}

					// If the player is walking, running we enter in Youjin mode
					else if(playerLayerZeroStateHash == _animatorHashIds.LocomotionIdState)
					{
						// ... set the last personal sighting of the player to the player's current position.
						PersonalLastSighting = _player.transform.position;
						InPatrol = false;
						
					}
					//If the player has not been seen and is close enough he will be able to strangle the enemy
					if(!PlayerInSight && !InPursuit && direction.magnitude < 1.5f)
					{
						_characterControllerLogic.IsCloseToEnemy = true;
						_characterControllerLogic.CloseEnemyAnimator = _anim;
						_characterControllerLogic.CloseEnemy = this.gameObject;
					}

					else if (_characterControllerLogic.IsCloseToEnemy)
					{
						_characterControllerLogic.IsCloseToEnemy = false;
					}
				}
			}

        }
    }
	
	
	void OnTriggerExit (Collider other)
	{
		// If the player leaves the trigger zone...
		if(other.gameObject == _player)
			// ... the player is not in sight.
			PlayerInSight = false;
	}
	
	
	float CalculatePathLength (Vector3 targetPosition)
	{
		// Create a path and set it based on a target position.
		NavMeshPath path = new NavMeshPath();
		if(_nav.enabled)
			_nav.CalculatePath(targetPosition, path);
		
		// Create an array of points which is the length of the number of corners in the path + 2.
		Vector3 [] allWayPoints = new Vector3[path.corners.Length + 2];
		
		// The first point is the enemy's position.
		allWayPoints[0] = transform.position;
		
		// The last point is the target position.
		allWayPoints[allWayPoints.Length - 1] = targetPosition;
		
		// The points inbetween are the corners of the path.
		for(int i = 0; i < path.corners.Length; i++)
		{
			allWayPoints[i + 1] = path.corners[i];
		}
		
		// Create a float to store the path length that is by default 0.
		float pathLength = 0;
		
		// Increment the path length by an amount equal to the distance between each waypoint and the next.
		for(int i = 0; i < allWayPoints.Length - 1; i++)
		{
			pathLength += Vector3.Distance(allWayPoints[i], allWayPoints[i + 1]);
		}
		
		return pathLength;
	}
}
