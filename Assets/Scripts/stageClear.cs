using UnityEngine;
using System.Collections;

public class stageClear : MonoBehaviour {

	private CharacterControllerLogic characterLogicScript;
	private SoundManager soundManager;
	void Start()
	{
		characterLogicScript = GameObject.FindGameObjectWithTag (DoneTags.player).GetComponent<CharacterControllerLogic> ();
		soundManager = GameObject.FindGameObjectWithTag (DoneTags.soundmanager).GetComponent<SoundManager> ();
	}

	void OnTriggerEnter(Collider collider)
	{
		if(collider.gameObject.CompareTag(DoneTags.player))
		{
			//Application.LoadLevel("testScene");
			collider.rigidbody.useGravity = false;
			PlayerPrefs.SetInt("Shouko", characterLogicScript.ShoukoNb);
			PlayerPrefs.SetInt("Spotted", characterLogicScript.HasBeenSeenNb);
			PlayerPrefs.SetInt ("availableShouko", characterLogicScript.AvailableShouko);
			PlayerPrefs.SetFloat("Duration", characterLogicScript.GameDuration);

			StartCoroutine(playEndSound());

		}
	}

	IEnumerator playEndSound()
	{
//		soundManager.playSound(soundName.SE_GameClear);
//		yield return new WaitForSeconds(5);

		FadeManager.Instance.LoadLevel("GameClear", 0.35f);
		yield return null;
	}
}
