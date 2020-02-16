using UnityEngine;

[RequireComponent(typeof(EnemySight))]
[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Animator))]
public class EnemyAnimation : MonoBehaviour
{
    [SerializeField]
    private float _deadZoneAngleInDegree = 5f;      // The number of degrees for which the rotation isn't controlled by Mecanim.

    private float _deadZoneAngleInRadian;
    private Transform _player;					    // Reference to the player's transform.
	private EnemySight _enemySight;			        // Reference to the EnemySight script.
	private NavMeshAgent _nav;					    // Reference to the nav mesh agent.
	private Animator _animator;					    // Reference to the Enemy Animator.
	private AnimatorHashIds _animatorHashIds;	    // Reference to the HashIDs script.
	private EnemyAnimatorSetup _enemyAnimatorSetup;	// An instance of the AnimatorSetup helper class.

	void Awake ()
	{
		// Setting up the references.
		_player = GameObject.FindGameObjectWithTag(Tags.player).transform;
		_enemySight = GetComponent<EnemySight>();
		_nav = GetComponent<NavMeshAgent>();
		_animator = GetComponent<Animator>();
		_animatorHashIds = GameObject.FindGameObjectWithTag(Tags.animatorHashController).GetComponent<AnimatorHashIds>();
		
		// Making sure the rotation is controlled by Mecanim.
		_nav.updateRotation = false;
		
		// Creating an instance of the AnimatorSetup class and calling it's constructor.
		_enemyAnimatorSetup = new EnemyAnimatorSetup(_animator, _animatorHashIds);
		
		// Set the weights for the shooting and gun layers to 1.
		_animator.SetLayerWeight(1, 1f);
		_animator.SetLayerWeight(2, 1f);

        // We need to convert the angle for the deadzone from degrees to radians.
        _deadZoneAngleInRadian = _deadZoneAngleInDegree * Mathf.Deg2Rad;
	}
	
	
	void Update () 
	{
		// Calculate the parameters that need to be passed to the animator component.
		NavAnimUpdate();
	}
	
	
	void OnAnimatorMove()
    {
		// Set the NavMeshAgent's velocity to the change in position since the last frame, by the time it took for the last frame.
        _nav.velocity = _animator.deltaPosition / Time.deltaTime;
		
		// The gameobject's rotation is driven by the animation's rotation.
		transform.rotation = _animator.rootRotation;
    }
	
	
	void NavAnimUpdate()
	{
		// Create the parameters to pass to the helper function.
		float speed;
		float angle;
		
		// If the player is in sight...
		if(_enemySight.PlayerInSight && (_enemySight.PersonalLastSighting - transform.position).magnitude < _enemySight.ShootingDistance)
		{
            // ... the enemy should stop...
			speed = 0f;
			
			// ... and the angle to turn through is towards the player.
			angle = FindAngle(transform.forward, _player.position - transform.position, transform.up);
		}
		else
		{
			// Otherwise the speed is a projection of desired velocity on to the forward vector...
			speed = Vector3.Project(_nav.desiredVelocity, transform.forward).magnitude;

//			Debug.Log ("desired Velocity = " + nav.desiredVelocity);
			if(_enemySight.InPursuit)
			{
				Vector3 vecFromEnemyToHeardPosition = _enemySight.PersonalLastSighting - transform.position;
				angle = FindAngle(transform.forward,vecFromEnemyToHeardPosition, transform.up);
            }
			else
			// ... and the angle is the angle between forward and the desired velocity.
			angle = FindAngle(transform.forward, _nav.desiredVelocity, transform.up);


			// If the angle is within the deadZone...
			if(Mathf.Abs(angle) < _deadZoneAngleInRadian)
			{
				// ... set the direction to be along the desired direction and set the angle to be zero.
 				transform.LookAt(transform.position + _nav.desiredVelocity);
      			angle = 0f;
    		}
		}
		
		// Call the Setup function of the helper class with the given parameters.
		_enemyAnimatorSetup.Setup(speed, angle);
	}
	
	
	float FindAngle (Vector3 fromVector, Vector3 toVector, Vector3 upVector)
	{
		// If the vector the angle is being calculated to is 0...
		if(toVector == Vector3.zero)
			// ... the angle between them is 0.
			return 0f;
		
		// Create a float to store the angle between the facing of the enemy and the direction it's travelling.
		float angle = Vector3.Angle(fromVector, toVector);
		
		// Find the cross product of the two vectors (this will point up if the velocity is to the right of forward).
		Vector3 normal = Vector3.Cross(fromVector, toVector);
		
		// The dot product of the normal with the upVector will be positive if they point in the same direction.
		angle *= Mathf.Sign(Vector3.Dot(normal, upVector));
		
		// We need to convert the angle we've found from degrees to radians.
		angle *= Mathf.Deg2Rad;

		return angle;
	}
}
