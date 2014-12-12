using UnityEngine;
using System.Collections;

public class OpenDoor : MonoBehaviour {

	private CharacterControllerLogic characterLogicScript;
	// Use this for initialization
	void Start () {
		characterLogicScript = GameObject.FindGameObjectWithTag (DoneTags.player).GetComponent<CharacterControllerLogic> ();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnTriggerEnter(Collider collider)
	{
		if(collider.gameObject.CompareTag(DoneTags.player))
		{
			if(characterLogicScript.GotKey && !characterLogicScript.IsPursued)
			{
				while(animation.isPlaying)
				{
				}
				animation.Play("openDoor");
			}
		}
	}

	void OnTriggerExit(Collider collider)
	{
		if(collider.gameObject.CompareTag(DoneTags.player))
		{
			while(animation.isPlaying)
			{
			}

			animation.Play("closeDoor");

		}
	}

}
