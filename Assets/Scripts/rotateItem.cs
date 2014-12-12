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
			other.gameObject.GetComponent<CharacterControllerLogic>().GotKey = true;
			Destroy (this.gameObject);
//			StartCoroutine(Destroytimer ());
		}
	}

	IEnumerator Destroytimer()
	{
		yield return new WaitForSeconds (animation["GetItem"].length);
		Destroy (this.gameObject);
	}
}

