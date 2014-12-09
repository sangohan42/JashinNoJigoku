using UnityEngine;
using System.Collections;

public class DoneEnemySight : MonoBehaviour
{
	public float fieldOfViewAngle = 90f;				// Number of degrees, centred on forward, for the enemy see.
	public float shootingDistance = 5;
	public bool playerInSight;							// Whether or not the player is currently sighted.
	public bool inPursuit;							// Whether or not the player is currently sighted.
	public bool inPatrol;
	public Vector3 personalLastSighting;				// Last place this enemy spotted the player.
	public Vector3 previousSighting;					// Where the player was sighted last frame.

	private NavMeshAgent nav;							// Reference to the NavMeshAgent component.
	private SphereCollider col;							// Reference to the sphere collider trigger component.
	private Animator anim;								// Reference to the Animator.
    private GameObject player;							// Reference to the player.
	private Animator playerAnim;						// Reference to the player's animator component.
	private DonePlayerHealth playerHealth;				// Reference to the player's health script.
	private HashIds hash;							// Reference to the HashIDs.
	private ParticleSystem interrogativePoint;
	private GameObject interrogativePointObject;
	public GameObject FOV;
	private bool resetFOVColor;
	
	public Texture FOV1;
	public Texture FOV2;

	void Awake ()
	{
		// Setting up the references.
		nav = GetComponent<NavMeshAgent>();
		col = GetComponent<SphereCollider>();
		anim = GetComponent<Animator>();
		player = GameObject.FindGameObjectWithTag(DoneTags.player);
		playerAnim = player.GetComponent<Animator>();
		playerHealth = player.GetComponent<DonePlayerHealth>();

		hash = GameObject.FindGameObjectWithTag(DoneTags.gameController).GetComponent<HashIds>();
		
		// Set the personal sighting and the previous sighting to the reset position.
		personalLastSighting = new Vector3 (1000, 1000, 1000);
		previousSighting = new Vector3 (1000, 1000, 1000);
		inPursuit = false;
		inPatrol = true;
		interrogativePointObject = transform.Find ("InterrogativePoint").gameObject;
		interrogativePoint = interrogativePointObject.GetComponent<ParticleSystem>();
		interrogativePointObject.SetActive (false);

		resetFOVColor = true;
	}
	
	
	void Update ()
	{
	
		// If the player is alive...
		if(playerHealth.health > 0f)
		{
			// ... set the animator parameter to whether the player is in sight or not.
			anim.SetBool(hash.playerInSightBool, playerInSight);
			anim.SetBool (hash.inPursuitBool, inPursuit);
			anim.SetBool (hash.inPatrolBool, inPatrol);

			int youJinLayerTransition = anim.GetAnimatorTransitionInfo(3).nameHash;
			int shootingLayerTransition = anim.GetAnimatorTransitionInfo(1).nameHash;

			//Display or undisplay the interrogation point
			if(youJinLayerTransition == hash.Empty_YoujinModeTrans && interrogativePointObject.activeSelf == false)
			{
				interrogativePointObject.SetActive(true);
				interrogativePoint.Play();
				anim.SetBool(hash.inYoujinBool, true);
			}

			else if((youJinLayerTransition == hash.WeaponRaise_WeaponLower || 
			         shootingLayerTransition == hash.Empty_WeaponRaiseTrans ||
			         shootingLayerTransition == hash.Empty_WeaponShootTrans) && 
			        interrogativePointObject.activeSelf == true)
			{
				interrogativePoint.Stop();
				interrogativePointObject.SetActive(false);
				anim.SetBool(hash.inYoujinBool, false);
			}

			if((shootingLayerTransition == hash.Empty_WeaponRaiseTrans ||
			   shootingLayerTransition == hash.Empty_WeaponShootTrans) && resetFOVColor)
			{
				FOV.renderer.material.SetTexture("_MainTex", FOV2);
				resetFOVColor = false;
			}

			else if(anim.GetBool(hash.inPatrolBool) == true && !resetFOVColor)
			{
				FOV.renderer.material.SetTexture("_MainTex",FOV1);
				resetFOVColor = true;
			}

		}

		//Player is Dead
		else
		{
			// ... set the animator parameter to false.
			anim.SetBool(hash.playerInSightBool, false);
			anim.SetBool (hash.inPursuitBool, false);
			anim.SetBool (hash.inPatrolBool, true);
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
        if(other.gameObject == player)
        {
			// By default the player is not in sight.
			playerInSight = false;

			anim.SetLayerWeight(3,1);

			// Create a vector from the enemy to the player and store the angle between it and forward.
            Vector3 direction = other.transform.position - transform.position;
			float angle = Vector3.Angle(direction, transform.forward);
			
			// If the angle between forward and where the player is, is less than half the angle of view...
			if(angle < fieldOfViewAngle * 0.5f)
			{
				RaycastHit hit;
				
				// ... and if a raycast towards the player hits something...
				if(Physics.Raycast(transform.position + transform.up, direction.normalized, out hit, col.radius))
				{
//					Debug.DrawLine(hit.point, hit.point + hit.normal, Color.green, 2, false);
					// ... and if the raycast hits the player...
					if(hit.collider.gameObject == player)
					{
						// ... the player is in sight.
						personalLastSighting = player.transform.position;
						playerInSight = true;
						inPursuit = true;
						inPatrol = false;
						anim.SetLayerWeight(3,0);

//						Debug.Log ("Player in sight");

					}
				}
			}
			
			// Store the name hashes of the current states.
			int playerLayerZeroStateHash = playerAnim.GetCurrentAnimatorStateInfo(0).nameHash;

			// If the player is walking, running
			if(playerLayerZeroStateHash == hash.m_LocomotionIdState)
			{
				// ... set the last personal sighting of the player to the player's current position.
				personalLastSighting = player.transform.position;
				inPatrol = false;

//					Debug.Log ("Player in Sphere Collider");
			}
        }
    }
	
	
	void OnTriggerExit (Collider other)
	{
		// If the player leaves the trigger zone...
		if(other.gameObject == player)
			// ... the player is not in sight.
			playerInSight = false;
	}
	
	
	float CalculatePathLength (Vector3 targetPosition)
	{
		// Create a path and set it based on a target position.
		NavMeshPath path = new NavMeshPath();
		if(nav.enabled)
			nav.CalculatePath(targetPosition, path);
		
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
