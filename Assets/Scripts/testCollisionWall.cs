using UnityEngine;
using System.Collections;

public class testCollisionOnWall : MonoBehaviour {
	
	void OnCollisionEnter(Collision collision) {
		if(collision.gameObject.CompareTag(DoneTags.player))
		{
			foreach (ContactPoint contact in collision.contacts) {
				Debug.DrawRay(contact.point, contact.normal, Color.white);
			}
			if (collision.relativeVelocity.magnitude > 2)
				audio.Play();
		}
		
	}
}
