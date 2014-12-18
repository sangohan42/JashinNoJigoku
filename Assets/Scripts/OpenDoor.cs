using UnityEngine;
using System.Collections;

public class OpenDoor : MonoBehaviour {

	private CharacterControllerLogic characterLogicScript;
	private bool isOpened;
	private checkEnemyStatus checkEnemyStatus;
	private SoundManager soundManager;
	// Use this for initialization
	void Start () {
		characterLogicScript = GameObject.FindGameObjectWithTag (DoneTags.player).GetComponent<CharacterControllerLogic> ();
		isOpened = false;
		checkEnemyStatus = GameObject.FindGameObjectWithTag (DoneTags.player).GetComponent<checkEnemyStatus> ();
		soundManager = GameObject.FindGameObjectWithTag (DoneTags.soundmanager).GetComponent<SoundManager> ();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnTriggerEnter(Collider collider)
	{
		if(collider.gameObject.CompareTag(DoneTags.player))
		{
			if(characterLogicScript.GotKey && checkEnemyStatus.isNotSeen())
			{
				while(animation.isPlaying)
				{
				}
				soundManager.playSound(soundName.SE_DoorOpen);
				animation.Play("openDoor");
				isOpened = true;
			}
		}
	}

	void OnTriggerExit(Collider collider)
	{
		if(collider.gameObject.CompareTag(DoneTags.player))
		{
			if(isOpened)
			{

				while(animation.isPlaying)
				{
				}
				soundManager.playSound(soundName.SE_DoorClose);

				animation.Play("closeDoor");
				isOpened = false;
			}

		}
	}

}
