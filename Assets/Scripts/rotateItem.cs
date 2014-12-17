using UnityEngine;
using System.Collections;

public class rotateItem : MonoBehaviour {

	private float degreesPerSecond;
	private bool stopRotating;
	private GameObject OnRadar;
	private GameObject Text;
	private bool isAKey;
	private CharacterControllerLogic ccl;

	// Use this for initialization
	void Start () {
		degreesPerSecond = 160;
		stopRotating = false;
		foreach (Transform child in transform)
		{
			if(child.gameObject.name == "Radar")
			{
				OnRadar = child.gameObject;
			}
			else Text = child.gameObject;
		}
		if (this.CompareTag (DoneTags.key))isAKey = true;
		else isAKey = false;
		ccl = GameObject.FindGameObjectWithTag (DoneTags.player).GetComponent<CharacterControllerLogic> ();
	}
	
	// Update is called once per frame
	void Update () {
		if(!stopRotating)transform.Rotate(Vector3.up* degreesPerSecond*Time.deltaTime, Space.Self); 

		else 
		{
			Vector3 eulerAngl = transform.localEulerAngles;
			eulerAngl.y = 90f;
			transform.localEulerAngles = eulerAngl;
		}
	}

	void OnTriggerEnter(Collider other)
	{
		if(other.gameObject.CompareTag(DoneTags.player))
		{
			//Stop rotating
			stopRotating = true;

			//if we are a key then the player got it
			if(isAKey)ccl.GotKey = true;
			else ccl.ShoukoNb ++;

			//disable the item rendering
			renderer.enabled = false;

			//destroy item rendering on radar
			OnRadar.renderer.enabled = false;

			//Enable text and play animation
			Text.renderer.enabled = true;

			animation.Play("objectCatching");

		}
	}

	void DestroyObject()
	{
		Destroy (this.gameObject);
	}
}

