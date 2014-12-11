using UnityEngine;
using System.Collections;

public class rotateItem : MonoBehaviour {

	private float degreesPerSecond;
	// Use this for initialization
	void Start () {
		degreesPerSecond = 160;
	}
	
	// Update is called once per frame
	void Update () {
		transform.Rotate(Vector3.up* degreesPerSecond*Time.deltaTime, Space.Self); 
	}

	void OnTriggerEnter(Collider other)
	{
		if(other.gameObject.CompareTag(DoneTags.player))
		{
			Destroy (this.gameObject);
		}
	}
}

