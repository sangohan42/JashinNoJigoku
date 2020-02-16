using UnityEngine;
using System.Collections;

public class UIGameOver : MonoBehaviour
{
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
