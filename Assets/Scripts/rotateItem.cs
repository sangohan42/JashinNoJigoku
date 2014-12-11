using UnityEngine;
using System.Collections;

public class rotateItem : MonoBehaviour {

	private float degreesPerSecond;
	// Use this for initialization
	void Start () {
		degreesPerSecond = 120;
	}
	
	// Update is called once per frame
	void Update () {
		transform.Rotate(Vector3.up* degreesPerSecond*Time.deltaTime, Space.Self); 
	}
}
