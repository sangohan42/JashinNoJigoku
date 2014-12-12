using UnityEngine;
using System.Collections;

public class stageClear : MonoBehaviour {

	private CharacterControllerLogic characterLogicScript;

	void Start()
	{
		characterLogicScript = GameObject.FindGameObjectWithTag (DoneTags.player).GetComponent<CharacterControllerLogic> ();

	}

	void OnTriggerEnter(Collider collider)
	{
		if(collider.gameObject.CompareTag(DoneTags.player) && !characterLogicScript.IsPursued)
		{
			Application.LoadLevel("testScene");
		}
	}
}
