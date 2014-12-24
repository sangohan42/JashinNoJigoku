using UnityEngine;
using System.Collections;

public class collideWithItem : MonoBehaviour {

	private rotateItem rotateItemScript;
	// Use this for initialization
	void Start () {
		rotateItemScript = this.GetComponentInChildren<rotateItem> ();
	}
	
	void OnTriggerEnter(Collider other)
	{
		if(other.gameObject.CompareTag(DoneTags.player))
		{
			rotateItemScript.stopRotation();
			collider.enabled = false;
		}
	}
}
