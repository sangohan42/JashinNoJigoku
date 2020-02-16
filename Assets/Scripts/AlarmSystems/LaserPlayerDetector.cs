using UnityEngine;

// Not Currently used
public class LaserPlayerDetector : MonoBehaviour
{
    private GameObject _player;								// Reference to the player.
    private LastPlayerSighting _lastPlayerSighting;		    // Reference to the global last sighting of the player.

    void Awake ()
    {
		// Setting up references.
		_player = GameObject.FindGameObjectWithTag(Tags.player);
		_lastPlayerSighting = LastPlayerSighting.Instance;
    }

    void OnTriggerStay(Collider other)
    {
		// If the beam is on...
        if(GetComponent<Renderer>().enabled)
			// ... and if the colliding gameobject is the player...
            if(other.gameObject == _player)
				// ... set the last global sighting of the player to the colliding object's position.
                _lastPlayerSighting.Position = other.transform.position;
    }
}