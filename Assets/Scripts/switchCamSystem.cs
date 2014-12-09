using UnityEngine;
using System.Collections;

public class switchCamSystem : MonoBehaviour {

	private Transform previousTransform;
	private Vector3 previousZoneCamPos;
	private Vector3 previousZoneCamRot;

	public Transform newTransform;
	private Vector3 newZoneCamPos;
	private Vector3 newZoneCamRot;

	private CharacterControllerLogic characterLogicScript;


	// Use this for initialization
	void Start () {
		characterLogicScript = GameObject.FindGameObjectWithTag (DoneTags.player).GetComponent<CharacterControllerLogic> ();

		previousTransform = GameObject.FindGameObjectWithTag (DoneTags.camera).transform;
		previousZoneCamPos = previousTransform.position;
		previousZoneCamRot = previousTransform.eulerAngles;

		newZoneCamPos = newTransform.position;
		newZoneCamRot = newTransform.eulerAngles;
	}
	
	void OnTriggerEnter(Collider other) {
		if(other.gameObject.CompareTag(DoneTags.player))
		{
			Debug.Log("ENTER");
			characterLogicScript.CameraPosition = newZoneCamPos;
			characterLogicScript.CameraRotation = newZoneCamRot;
		}
	}

	void OnTriggerExit(Collider other) {
		if(other.gameObject.CompareTag(DoneTags.player))
		{
			Debug.Log("EXIT");

			characterLogicScript.CameraPosition = previousZoneCamPos;
			characterLogicScript.CameraRotation = previousZoneCamRot;
		}
	}

	// Update is called once per frame
	void Update () {
	
	}
}
