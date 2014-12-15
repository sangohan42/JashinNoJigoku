using UnityEngine;
using System.Collections;

public class UIGameOver : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}


	void OnPress(bool isDown)
	{
		switch(gameObject.name)
		{
			case "RetryLabel":
				FadeManager.Instance.LoadLevel("testScene", 0.25f);		
			break;
		}
	}
}
