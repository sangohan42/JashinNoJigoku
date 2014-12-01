using UnityEngine;
using System.Collections;

public class DoneEnemyAI : MonoBehaviour
{
	public float patrolSpeed = 2f;							// The nav mesh agent's speed when patrolling.
	public float chaseSpeed = 5f;							// The nav mesh agent's speed when chasing.
	public float youjinSpeed = 5f;							// The nav mesh agent's speed when chasing.
	public float chaseWaitTime = 2f;						// The amount of time to wait when the last sighting is reached.
	public float patrolWaitTime = 1f;						// The amount of time to wait when the patrol way point is reached.
	public Transform[] patrolWayPoints;						// An array of transforms for the patrol route.

	private DoneEnemySight enemySight;						// Reference to the EnemySight script.
	private NavMeshAgent nav;								// Reference to the nav mesh agent.
	private Transform player;								// Reference to the player's transform.
	private DonePlayerHealth playerHealth;					// Reference to the PlayerHealth script.
	private DoneLastPlayerSighting lastPlayerSighting;		// Reference to the last global sighting of the player.
	private float chaseTimer;								// A timer for the chaseWaitTime.
	private float patrolTimer;								// A timer for the patrolWaitTime.
	private int wayPointIndex;								// A counter for the way point array.


	void Awake ()
	{
		// Setting up the references.
		enemySight = GetComponent<DoneEnemySight>();
		nav = GetComponent<NavMeshAgent>();
		player = GameObject.FindGameObjectWithTag(DoneTags.player).transform;
		playerHealth = player.GetComponent<DonePlayerHealth>();
		lastPlayerSighting = GameObject.FindGameObjectWithTag(DoneTags.gameController).GetComponent<DoneLastPlayerSighting>();

	}
	
	
	void Update ()
	{
		// If the player is in sight and is alive...
		if(enemySight.playerInSight && playerHealth.health > 0f)
			// ... shoot.
			Shooting();

		// If the player has been heard and isn't dead...
		else if(enemySight.personalLastSighting != lastPlayerSighting.resetPosition && playerHealth.health > 0f)
			//... youjin
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
		Vector3 sightingDeltaPos = enemySight.personalLastSighting - transform.position;
		Debug.Log ("Distance = " + sightingDeltaPos.magnitude);
		if(sightingDeltaPos.magnitude < enemySight.shootingDistance)
		{
			Debug.Log ("STOP ANIM");
			nav.Stop();
		}

		else
		{
			// If the last personal sighting of the player is not close...
			if(sightingDeltaPos.sqrMagnitude > 5f)
			nav.destination = enemySight.personalLastSighting;

			// Set the appropriate speed for the NavMeshAgent.
			nav.speed = chaseSpeed;
		}

	}

	void Chasing ()
	{
		//If the player was previously sighted
		if(enemySight.inPursuit)
		{
			// Create a vector from the enemy to the last sighting of the player.
			Vector3 sightingDeltaPos = enemySight.personalLastSighting - transform.position;

//			Debug.Log("Distance between player and enemy = " +sightingDeltaPos.sqrMagnitude);
			// If the last personal sighting of the player is not close...
			if(sightingDeltaPos.sqrMagnitude > 5f)
				// ... set the destination for the NavMeshAgent to the position of the player.
				nav.destination = enemySight.personalLastSighting;
			
			// Set the appropriate speed for the NavMeshAgent.
			nav.speed = chaseSpeed;
			
			// If near the last personal sighting...
			if(nav.remainingDistance < nav.stoppingDistance)
			{
				Debug.Log ("Increment timer");
				// ... increment the timer.
				chaseTimer += Time.deltaTime;
				
				// If the timer exceeds the wait time...
				if(chaseTimer >= chaseWaitTime)
				{
					enemySight.personalLastSighting = lastPlayerSighting.resetPosition;
					chaseTimer = 0f;
					enemySight.inPursuit = false;
					enemySight.inPatrol = true;
					Debug.Log ("Return patrolling");
				}
			}
			else
				// If not near the last sighting personal sighting of the player, reset the timer.
				chaseTimer = 0f;

		}

		//We are in Youjin Mode
		else
		{
			// Create a vector from the enemy to the last sighting of the player.
			Vector3 sightingDeltaPos = enemySight.personalLastSighting - transform.position;
			
			// If the last personal sighting of the player is not close...
			if(sightingDeltaPos.sqrMagnitude > 4f)
				// ... set the destination for the NavMeshAgent to the position of the player.
				nav.destination = enemySight.personalLastSighting;
			
			// Set the appropriate speed for the NavMeshAgent.
			nav.speed = youjinSpeed;
			
			// If near the last personal sighting...
			if(nav.remainingDistance < nav.stoppingDistance)
			{
				// ... increment the timer.
				chaseTimer += Time.deltaTime;
				
				// If the timer exceeds the wait time...
				if(chaseTimer >= chaseWaitTime)
				{
					enemySight.personalLastSighting = lastPlayerSighting.resetPosition;
					chaseTimer = 0f;
					enemySight.inPatrol = true;
				}
			}
			else
				// If not near the last sighting personal sighting of the player, reset the timer.
				chaseTimer = 0f;
			
		}

	}

	
	void Patrolling ()
	{
		// Set an appropriate speed for the NavMeshAgent.
		nav.speed = patrolSpeed;
		
		// If near the next waypoint or there is no destination...
		if(nav.destination == lastPlayerSighting.resetPosition || nav.remainingDistance < nav.stoppingDistance)
		{
			// ... increment the timer.
			patrolTimer += Time.deltaTime;
			
			// If the timer exceeds the wait time...
			if(patrolTimer >= patrolWaitTime)
			{
				// ... increment the wayPointIndex.
				if(wayPointIndex == patrolWayPoints.Length - 1)
					wayPointIndex = 0;
				else
					wayPointIndex++;
				
				// Reset the timer.
				patrolTimer = 0;
			}
		}
		else
			// If not near a destination, reset the timer.
			patrolTimer = 0;
		
		// Set the destination to the patrolWayPoint.
		nav.destination = patrolWayPoints[wayPointIndex].position;
	}
}
