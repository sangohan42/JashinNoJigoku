using UnityEngine;

[RequireComponent(typeof(EnemySight))]
[RequireComponent(typeof(NavMeshAgent))]
public class EnemyAI : MonoBehaviour
{
    [SerializeField]
    public float _patrolSpeed = 2f;                          // The nav mesh agent's speed when patrolling.
    [SerializeField]
    public float _chaseSpeed = 5f;                           // The nav mesh agent's speed when chasing.
    [SerializeField]
    public float _youjinSpeed = 1.5f;                          // The nav mesh agent's speed when chasing.
    [SerializeField]
    public float _chaseWaitTime = 2f;                        // The amount of time to wait when the last sighting is reached.
    [SerializeField]
    public float _patrolWaitTime = 1f;                       // The amount of time to wait when the patrol way point is reached.
    [SerializeField]
    public Transform[] _patrolWayPoints;						// An array of transforms for the patrol route.

	private EnemySight _enemySight;						            // Reference to the EnemySight script.
	private NavMeshAgent _nav;								        // Reference to the nav mesh agent.
	private Transform _player;								        // Reference to the player's transform.
	private PlayerLifeHandler _playerLifeHandler;					// Reference to the PlayerHealth script.
	private LastPlayerSighting _lastPlayerSighting;                 // Reference to the last global sighting of the player.
    private float _chaseTimer;								        // A timer for the chaseWaitTime.
	private float _patrolTimer;								        // A timer for the patrolWaitTime.
	private int _currentWayPointIndex;					            // An index for the current way point

	void Awake ()
	{
		// Setting up the references.
		_enemySight = GetComponent<EnemySight>();
		_nav = GetComponent<NavMeshAgent>();
		_player = GameObject.FindGameObjectWithTag(Tags.player).transform;
		_playerLifeHandler = _player.GetComponent<PlayerLifeHandler>();
        _lastPlayerSighting = LastPlayerSighting.Instance;
    }


    void Update ()
	{
        // If the player is in sight and is alive...
        if (_enemySight.PlayerInSight && _playerLifeHandler.Health > 0f)
			// ... shoot.
			Shooting();

        // If the player has been heard and isn't dead...
		else if( !_enemySight.InPatrol && _playerLifeHandler.Health > 0f)
			//... youjin or in pursuit without the player in sight
			Chasing();

		// Otherwise...
		else 
			// ... patrol.
			Patrolling();
	}
	
	
	void Shooting ()
	{
		// Stop the enemy where it is.
		// Create a vector from the enemy to the last sighting of the player.
		Vector3 sightingDeltaPos = _enemySight.PersonalLastSighting - transform.position;
//		Debug.Log ("Distance = " + sightingDeltaPos.magnitude);
		if(sightingDeltaPos.magnitude <= _enemySight.ShootingDistance)
		{
			_nav.speed = 0;
		}
        else
		{
			// If the last personal sighting of the player is not close...
//			if(sightingDeltaPos.sqrMagnitude > 5f)
			_nav.destination = _enemySight.PersonalLastSighting;

			// Set the appropriate speed for the NavMeshAgent.
			_nav.speed = _chaseSpeed;
		}

	}

	void Chasing ()
	{
		//If the player was previously sighted
		if(_enemySight.InPursuit)
		{
            // Create a vector from the enemy to the last sighting of the player.
            Vector3 sightingDeltaPos = _enemySight.PersonalLastSighting - transform.position;

            // ... set the destination for the NavMeshAgent to the last known position of the player.
            _nav.destination = _enemySight.PersonalLastSighting;

            // Set the appropriate speed for the NavMeshAgent.
            _nav.speed = _chaseSpeed;

            // If near the last personal sighting...
			if(_nav.remainingDistance < _nav.stoppingDistance)
			{
				Debug.Log ("Increment timer");
				// ... increment the timer.
				_chaseTimer += Time.deltaTime;
				
				// If the timer exceeds the wait time...
				if(_chaseTimer >= _chaseWaitTime)
				{
					_enemySight.PersonalLastSighting = _lastPlayerSighting.ResetPosition;
					_chaseTimer = 0f;
					_enemySight.InPursuit = false;
					_enemySight.InPatrol = true;
                }
            }
			else
				// If not near the last sighting personal sighting of the player, reset the timer.
				_chaseTimer = 0f;
        }

		//We are in Youjin Mode
		else
		{
            // Create a vector from the enemy to the last sighting of the player.
            Vector3 sightingDeltaPos = _enemySight.PersonalLastSighting - transform.position;
			
			// If the last personal sighting of the player is not close...
			if(sightingDeltaPos.sqrMagnitude > 5f)
				// ... set the destination for the NavMeshAgent to the position of the player.
				_nav.destination = _enemySight.PersonalLastSighting;
			
			// Set the appropriate speed for the NavMeshAgent.
			_nav.speed = _youjinSpeed;
			
			// If near the last personal sighting...
			if(_nav.remainingDistance < _nav.stoppingDistance)
			{
				// ... increment the timer.
				_chaseTimer += Time.deltaTime;
				
				// If the timer exceeds the wait time...
				if(_chaseTimer >= _chaseWaitTime)
				{
					_enemySight.PersonalLastSighting = _lastPlayerSighting.ResetPosition;
					_chaseTimer = 0f;
					_enemySight.InPatrol = true;
				}
			}
			else
				// If not near the last sighting personal sighting of the player, reset the timer.
				_chaseTimer = 0f;
        }
    }

	void Patrolling ()
	{
        // Set an appropriate speed for the NavMeshAgent.
        _nav.speed = _patrolSpeed;
		
		// If near the next waypoint or there is no destination...
		if(_nav.destination == _lastPlayerSighting.ResetPosition || _nav.remainingDistance < _nav.stoppingDistance)
		{
			// ... increment the timer.
			_patrolTimer += Time.deltaTime;
			
			// If the timer exceeds the wait time...
			if(_patrolTimer >= _patrolWaitTime)
			{
				// ... increment the wayPointIndex.
				if(_currentWayPointIndex == _patrolWayPoints.Length - 1)
					_currentWayPointIndex = 0;
				else
					_currentWayPointIndex++;
				
				// Reset the timer.
				_patrolTimer = 0;
			}
		}
		else
			// If not near a destination, reset the timer.
			_patrolTimer = 0;
		
		// Set the destination to the patrolWayPoint.
		_nav.destination = _patrolWayPoints[_currentWayPointIndex].position;
	}
}
