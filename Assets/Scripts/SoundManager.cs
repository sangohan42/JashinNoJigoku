using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum soundName {BGM_Title, BGM_InGame, BGM_Result, SE_GameOver, SE_GameStart, SE_DoorOpen, SE_DoorClose, SE_GrabObject};

public class SoundManager : SingletonMonoBehaviour<SoundManager>{
	
		public enum storeStageName{
		testScene = 0,
		GameClear,
		GameOver
	}
	
	public delegate void volumeOnOffHandler(bool onVolume);
//	public static event volumeOnOffHandler _volumeSwitch;
	
	private bool volumeOff;

	private AudioClip BGM_Title {get; set;}
	private AudioClip BGM_InGame {get; set;}
	private AudioClip BGM_Result {get; set;}
	
	private AudioClip SE_GameOver {get; set;}
	private AudioClip SE_GameStart {get; set;}
	private AudioClip SE_DoorOpen {get; set;}
	private AudioClip SE_DoorClose {get; set;}
	private AudioClip SE_GrabObject {get; set;}

	public AudioSource[] audioSource {get; set;}
	
	bool[] sourceIsFree ;
	
	UISprite volumeUISprite;
	public storeStageName storeStageNameType ;
	
	public void Awake ()
	{
		if (this != Instance) {
			Destroy (this.gameObject);
			//Debug.Log ("Destroy Sound Manager :Start");
			return;
		}
		else{
			Debug.Log ("Dont destroy on load");
			DontDestroyOnLoad (this.gameObject);
		}
		
		//Debug.Log (" AWAKE");
		//volumeOff will be updated in the onStart function of getScore after reading the playerPrefs
		volumeOff = System.Convert.ToBoolean(PlayerPrefs.GetString("volumeOff", "false"));
	
//		checkIcon();
		
		//Debug.Log ("In Awake, VolumeOff = " + volumeOff);
	}
	
		
	public void initAudioSource()
	{
		audioSource = gameObject.GetComponents<AudioSource>();
		sourceIsFree = new bool[3];
		for(int i = 0; i <3; i++)
		{
			sourceIsFree[i] = true;
		}
	}
	
	public int getFreeAudioSourceIndex()
	{
		for(int i = 0; i <3; i++)
		{
			if(sourceIsFree[i] == true)return i;
		}
		return -1;
	}


	void Start () {
		initAudioSource();

//		BGM_Title = Resources.Load("Sounds/BGM_Title", typeof(AudioClip)) as AudioClip;
		BGM_InGame = Resources.Load("Sounds/BGM_InGame", typeof(AudioClip)) as AudioClip;
//		BGM_Result = Resources.Load("Sounds/BGM_Result", typeof(AudioClip)) as AudioClip;
		
		SE_GameOver = Resources.Load("Sounds/endgame", typeof(AudioClip)) as AudioClip;
		SE_DoorOpen = Resources.Load("Sounds/DoorOpen", typeof(AudioClip)) as AudioClip;
		SE_DoorClose = Resources.Load("Sounds/DoorClose", typeof(AudioClip)) as AudioClip;
		SE_GrabObject = Resources.Load("Sounds/grabObject2", typeof(AudioClip)) as AudioClip;
//		Jingle_GameStart = Resources.Load("Sounds/Jingle_GameStart", typeof(AudioClip)) as AudioClip;
//		SE_Enter = Resources.Load("Sounds/SE_Enter", typeof(AudioClip)) as AudioClip;
//		SE_Hit_Pin = Resources.Load("Sounds/SE_Hit_Pin", typeof(AudioClip)) as AudioClip;
//		SE_Pin_Down = Resources.Load("Sounds/SE_Pin_Down", typeof(AudioClip)) as AudioClip;
//		SE_Pin_Down2 = Resources.Load("Sounds/SE_Pin_Down2", typeof(AudioClip)) as AudioClip;
//		SE_Rolling_Loop = Resources.Load("Sounds/SE_Rolling_Loop", typeof(AudioClip)) as AudioClip;
//		SE_Rolling_Launch = Resources.Load("Sounds/SE_Rolling_Launch", typeof(AudioClip)) as AudioClip;
//		SE_Special = Resources.Load("Sounds/SE_Special", typeof(AudioClip)) as AudioClip;
//		SE_Throw = Resources.Load("Sounds/SE_Throw", typeof(AudioClip)) as AudioClip;

		//initialize
		if(Application.loadedLevelName == "testScene"){
			Debug.Log("Play Background Music");
			switchPlayBGM(soundName.BGM_InGame);
		}
		
		//audioSource[1].clip = SE_Enter;
	}
	
//	public void checkIcon()
//	{
//		if(!volumeOff)
//		{
//			GameObject.Find ("VolumeBtn").GetComponent<UISprite>().spriteName = "title_icon_speaker_on";
//		}
//		
//		else
//		{
//			GameObject.Find ("VolumeBtn").GetComponent<UISprite>().spriteName = "title_icon_speaker_off";
//		}	
//	}

	public void OnLevelWasLoaded(int level) 
	{
		if (this != Instance)
		{ 
			Destroy (this.gameObject); 
			return;
		}
		print ("OnLevelWasLoaded: " + "level" + level);
		switch(level){
			case(0)://GameScene
				//switchPlayBGM(soundName.BGM_Title);	
				StartCoroutine(switchPlayBGMLate(soundName.BGM_InGame));
				//checkIcon();
				break;
			case(1)://GameClear
				switchPlayBGM(soundName.BGM_InGame);	
				//StartCoroutine(switchPlayBGMLate(soundName.BGM_InGame));
				storeStageNameType = storeStageName.testScene;
				break;
			case(2)://GameOver
				//switchPlayBGM(soundName.BGM_Result);
				StartCoroutine(switchPlayBGMLate(soundName.BGM_InGame));
				storeStageNameType = storeStageName.GameOver;
				break;
		}
	}
	
	IEnumerator switchPlayBGMLate(soundName sound)
	{
		yield return new WaitForSeconds(0.05f);
		switchPlayBGM(sound);
	}

	public void volumeOnOff(bool val){
		if(val){// if volume off
			audioSource[0].volume = 0.8f;
			if(audioSource[0].clip == null) audioSource[0].clip = getBGMClip(soundName.BGM_Title);	
			audioSource[0].loop = true;
			audioSource[0].Play();	
			print("volumeOn");
			volumeOff = false;
		}
		else{
			audioSource[0].Pause ();
			print("volumeOff");
			volumeOff = true;
		}
	}
	
	public void switchPlayBGM(soundName sound){
		
		if(volumeOff) return;
		else
		{
			audioSource[0].volume = 0.3f;
			audioSource[0].clip = getBGMClip(sound);
			audioSource[0].loop = true;
			audioSource[0].Play ();	
			//Debug.Log ("Background Sound switched");
		}
	}

	public void stopBGM(){
		audioSource[0].Stop();
	}
	
	public AudioClip getAudioClip(soundName sound)
	{
		switch(sound)
		{
			case soundName.SE_GameOver:
			return SE_GameOver;

			case soundName.SE_GameStart:
			return SE_GameStart;

			case soundName.SE_DoorOpen:
			return SE_DoorOpen;

			case soundName.SE_DoorClose:
			return SE_DoorClose;

			case soundName.SE_GrabObject:
			return SE_GrabObject;

			default:
			Debug.Log ("Please Enter the name of an audio clip");
			return null;
		}
	}
	
	public AudioClip getBGMClip(soundName sound)
	{
		switch(sound)
		{
			case soundName.BGM_Title:
			Debug.Log ("Set Title Background sound");
			return BGM_Title;
			
			case soundName.BGM_InGame:
			Debug.Log ("Set Ingame Background sound");
			return BGM_InGame;
			
			case soundName.BGM_Result :
			Debug.Log ("Set Result Background sound");
			return BGM_Result;
			
			default:
			Debug.Log ("Please Enter the name of a Background clip");
			return null;
		}
	}
	
	IEnumerator soundFinishPlaying(float waitTime, int indexAudioSource)
	{
		yield return new WaitForSeconds(waitTime);
		sourceIsFree[indexAudioSource] = true;
	}

	public bool playSound(soundName sound)
	{
		if(volumeOff) return true;


		//get free audio source index
		int index = getFreeAudioSourceIndex();
		
		//no free audio source
		if(index ==-1)return false;
			
		//the current audio source become busy
		sourceIsFree[index] = false;
		
		audioSource[index+1].volume = 0.8f;
		audioSource[index+1].clip = getAudioClip(sound);
		audioSource[index+1].Play();
		StartCoroutine(soundFinishPlaying(audioSource[index+1].clip.length, index));
		
		return true;

	}
//
//	public bool playSoundLoop(soundName sound)
//	{
//		if(volumeOff) return true;
//
//		audioSource[1].volume = 0.8f;
//		audioSource[1].clip = getAudioClip(sound);
//		audioSource[1].loop = true;
//		audioSource[1].Play();
//
//		return true;
//
//	}
//
//	public bool playOneShot(soundName sound)
//	{
//		if(volumeOff) return true;
//		//If we ask to play the finish sound we stop the background sound to avoid strange blending
//		if(sound == soundName.Jingle_GameFinish)
//		{
//			audioSource[0].Stop();
//		}
//
//		if(audioSource[1].isPlaying)audioSource[1].Stop();
//
//		if(sound == soundName.SE_Rolling_Launch)
//		{
//			audioSource[1].volume = 4.5f;
//		}
//		else if(sound == soundName.SE_Hit_Pin)
//		{
//			audioSource[1].volume = 0.3f;
//		}
//		audioSource[1].PlayOneShot ( getAudioClip(sound));
//
//		return true;
//	}
	
	#region Delegate
	void OnEnable(){
		
	}
	void OnDisable(){
		unSubscribeEvent();
	}
	void OnDestroy(){
		unSubscribeEvent();
	}
	void unSubscribeEvent(){
		
	}
	#endregion

	void Update () {

	}
}
