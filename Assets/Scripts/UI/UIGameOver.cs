using UnityEngine;
using System.Collections;

public class UIGameOver : MonoBehaviour {


	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
	
	}


	void OnPress(bool isDown)
	{
		if(isDown)
		{
			switch(gameObject.name)
			{
				case "RetryLabel":
					FadeManager.Instance.LoadLevel("testScene", 0.25f);		
				break;
			}
		}
	}
}
