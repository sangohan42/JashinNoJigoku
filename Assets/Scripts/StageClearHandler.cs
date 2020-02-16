using System;
using UnityEngine;
using System.Collections;

public class StageClearHandler : MonoBehaviour {

	private CharacterControllerLogic _characterLogicScript;
	void Start()
	{
		_characterLogicScript = GameObject.FindGameObjectWithTag (Tags.player).GetComponent<CharacterControllerLogic> ();
	}

	void OnTriggerEnter(Collider collider)
	{
		if(collider.gameObject.CompareTag(Tags.player))
		{
			collider.GetComponent<Rigidbody>().useGravity = false;
			PlayerPrefs.SetInt("FoundShoukoNumber", _characterLogicScript.FoundShoukoNb);
			PlayerPrefs.SetInt("Spotted", _characterLogicScript.SpottedCount);
			PlayerPrefs.SetInt ("TotalShoukoNumber", _characterLogicScript.TotalShoukoNb);
			PlayerPrefs.SetFloat("Duration", _characterLogicScript.ElapsedTimeSinceLevelStartup);

			StartCoroutine(PlayStageEndSound(() => {
                FadeManager.Instance.LoadLevel("GameClear", 0.35f);
            }));
        }
	}

	IEnumerator PlayStageEndSound( Action OnStageEndSoundPlayed )
	{
        // No Sound for now

        if (OnStageEndSoundPlayed != null)
            OnStageEndSoundPlayed.Invoke();

		yield return null;
	}
}
