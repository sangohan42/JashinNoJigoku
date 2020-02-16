using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Animation))]
public class DoorOpener : MonoBehaviour {

	private CharacterControllerLogic _characterControllerLogic;
	private bool _isOpened;
	private EnemyStatusChecker _checkEnemyStatus;
	private SoundManager _soundManager;
    private Animation _animation;

	// Use this for initialization
	void Start ()
    {
		_characterControllerLogic = GameObject.FindGameObjectWithTag (Tags.player).GetComponent<CharacterControllerLogic> ();
		_isOpened = false;
		_checkEnemyStatus = GameObject.FindGameObjectWithTag (Tags.player).GetComponent<EnemyStatusChecker> ();
		_soundManager = GameObject.FindGameObjectWithTag (Tags.soundmanager).GetComponent<SoundManager> ();
        _animation = GetComponent<Animation>();
    }

	void OnTriggerEnter(Collider collider)
	{
		if(collider.gameObject.CompareTag(Tags.player))
		{
			if(_characterControllerLogic.GotKey && _checkEnemyStatus.isNotSeen())
			{
                _soundManager.PlaySound(soundName.SE_DoorAccessGranted);
				_soundManager.PlaySound(soundName.SE_DoorOpen);
                _animation.Play("openDoor");
				_isOpened = true;
			}
			else
			{
				_soundManager.PlaySound(soundName.SE_DoorAccessRefused);
			}
		}
	}

	void OnTriggerExit(Collider collider)
	{
		if(collider.gameObject.CompareTag(Tags.player))
		{
			if(_isOpened)
			{

				while(_animation.isPlaying)
				{
				}

				_soundManager.PlaySound(soundName.SE_DoorClose);

                _animation.Play("closeDoor");
				_isOpened = false;
			}

		}
	}

}
