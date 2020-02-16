using UnityEngine;
using System.Collections;

public class ScoreDisplayer : MonoBehaviour {

	private UILabel shoukoNb;
	private UILabel spottedNb;
	private UILabel gameDuration;

	// Use this for initialization
	void Start () {
		shoukoNb = GameObject.Find ("Shouko").GetComponent<UILabel> ();
		spottedNb = GameObject.Find ("Spotted").GetComponent<UILabel> ();
		gameDuration = GameObject.Find ("GameDuration").GetComponent<UILabel> ();
		setLabels ();
	}

	void setLabels()
	{
		shoukoNb.text = "Shouko " + PlayerPrefs.GetInt ("FoundShoukoNumber") + "/" + PlayerPrefs.GetInt ("TotalShoukoNumber");
		spottedNb.text = "Spotted " + PlayerPrefs.GetInt ("Spotted");
		gameDuration.text = "Duration " + FormatedTimeString(PlayerPrefs.GetFloat ("Duration"));
	}

	private string FormatedTimeString (float input) {
		int seconds;
		int minutes;
		
		minutes = Mathf.FloorToInt(input / 60f);
		seconds = Mathf.FloorToInt(input - minutes * 60f);
		string r = (minutes < 10) ? "0" + minutes.ToString() : minutes.ToString();
		r += ":";
		r += (seconds < 10) ? "0" + seconds.ToString() : seconds.ToString();
		return r;
	}
}
