using System;
using UnityEngine;
using System.Collections;

public class StageClearHandler : MonoBehaviour {

	private CharacterControllerLogic _characterLogicScript;
	private SoundManager _soundManager;
	void Start()
	{
		_characterLogicScript = GameObject.FindGameObjectWithTag (Tags.player).GetComponent<CharacterControllerLogic> ();
		_soundManager = GameObject.FindGameObjectWithTag (Tags.soundmanager).GetComponent<SoundManager> ();
	}

	void OnTriggerEnter(Collider collider)
	{
		if(collider.gameObject.CompareTag(Tags.player))
		{
			collider.GetComponent<Rigidbody>().useGravity = false;
			PlayerPrefs.SetInt("FoundShoukoNumber", _characterLogicScript.FoundShoukoNb);
			PlayerPrefs.SetInt("Spotted", _characterLogicScript.NumberOfEnemyInPursuitMode);
			PlayerPrefs.SetInt ("TotalShoukoNumber", _characterLogicScript.TotalShoukoNb);
			PlayerPrefs.SetFloat("Duration", _characterLogicScript.ElapsedTimeSinceLevelStartup);

			StartCoroutine(PlayStageEndSound(() => {
                FadeManager.Instance.LoadLevel("GameClear", 0.35f);
            }));

		}
	}

	IEnumerator PlayStageEndSound( Action OnStageEndSoundPlayed )
	{
        _soundManager.PlaySound(soundName.BGM_Result);
        yield return new WaitForSeconds(5);

        if (OnStageEndSoundPlayed != null)
            OnStageEndSoundPlayed.Invoke();

		yield return null;
	}
}
