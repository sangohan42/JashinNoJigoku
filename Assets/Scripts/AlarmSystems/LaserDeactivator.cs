using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class LaserDeactivator : MonoBehaviour
{
    [SerializeField]
    private GameObject _laser;				// Reference to the laser that can we turned off at this switch.
    [SerializeField]
    private Material _deactivateLaserMat;	// The screen's material to show the laser has been deactivated.
    
	private GameObject _player;				// Reference to the player.
    
	void Awake ()
	{
		// Setting up the reference.
		_player = GameObject.FindGameObjectWithTag(Tags.player);
	}
    
	void OnTriggerStay (Collider other)
	{
		// If the colliding gameobject is the player...
		if(other.gameObject == _player)
			// ... and the switch button is pressed...
			if(Input.GetButton("Switch"))
				// ... deactivate the laser.
				LaserDeactivation();
	}
	
	
	void LaserDeactivation ()
	{
		// Deactivate the laser GameObject.
		_laser.SetActive(false);
		
		// Store the renderer component of the screen.
		Renderer screen = transform.Find("prop_switchUnit_screen_001").GetComponent<Renderer>();
		
		// Change the material of the screen to the unlocked material.
		screen.material = _deactivateLaserMat;
		
		// Play switch deactivation audio clip.
		GetComponent<AudioSource>().Play();
	}
}
