using UnityEngine;
using System.Collections;

public class CCTVPlayerDetector : MonoBehaviour
{
    private GameObject _player;								// Reference to the player.
    private LastPlayerSighting _lastPlayerSighting;		// Reference to the global last sighting of the player.

    void Awake ()
	{
		// Setting up the references.
		_player = GameObject.FindGameObjectWithTag(Tags.player);
		_lastPlayerSighting = LastPlayerSighting.Instance;
	}
    
	void OnTriggerStay (Collider other)
	{
		// If the colliding gameobject is the player...
		if(other.gameObject == _player)
		{
			// ... raycast from the camera towards the player.
			Vector3 relPlayerPos = _player.transform.position - transform.position;
			RaycastHit hit;
			
			if(Physics.Raycast(transform.position, relPlayerPos, out hit))
				// If the raycast hits the player...
				if(hit.collider.gameObject == _player)
					// ... set the last global sighting of the player to the player's position.
					_lastPlayerSighting.Position = _player.transform.position;
		}
	}
}
