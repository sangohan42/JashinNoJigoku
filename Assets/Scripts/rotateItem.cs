using UnityEngine;
using System.Collections;

public class rotateItem : MonoBehaviour {

	private float degreesPerSecond;
	private bool stopRotating;
	private GameObject keyOnRadar;
	private GameObject keyText;
	// Use this for initialization
	void Start () {
		degreesPerSecond = 160;
		stopRotating = false;
		foreach (Transform child in transform)
		{
			if(child.gameObject.name == "Radar")
			{
				keyOnRadar = child.gameObject;
			}
			else keyText = child.gameObject;
		}
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

			//Set boolean to TRUE
			other.gameObject.GetComponent<CharacterControllerLogic>().GotKey = true;

			//disable the key rendering
			renderer.enabled = false;

			//destroy key rendering on radar
			keyOnRadar.renderer.enabled = false;

			//Enable text and play animation
			keyText.renderer.enabled =true;

			animation.Play("objectCatching");

//			Destroy (this.gameObject);
//			StartCoroutine(Destroytimer ());
		}
	}

	void DestroyObject()
	{
		Destroy (this.gameObject);
	}
}

