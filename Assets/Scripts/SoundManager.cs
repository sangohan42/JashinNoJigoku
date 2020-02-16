using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public enum soundName {BGM_Title, BGM_InGame, BGM_Result, SE_GameOver, SE_GameStart, SE_DoorOpen, SE_DoorClose, SE_GrabObject,
	SE_EnemyHurt, SE_EnemyDie, SE_DoorAccessGranted, SE_DoorAccessRefused, SE_EnemyYoujin1, SE_EnemyYoujin2, SE_EnemyYoujin3,
	SE_EnemyYoujin4, SE_EnemyYoujin5};

[RequireComponent(typeof(AudioSource))]
public class SoundManager : SingletonMonoBehaviour<SoundManager>
{
    private bool _volumeOff;

    private AudioClip BGM_Title {get; set;}
	private AudioClip BGM_InGame {get; set;}
	private AudioClip BGM_Result {get; set;}
	
	private AudioClip SE_GameOver {get; set;}
	private AudioClip SE_GameStart {get; set;}
	private AudioClip SE_DoorOpen {get; set;}
	private AudioClip SE_DoorClose {get; set;}
	private AudioClip SE_GrabObject {get; set;}
	private AudioClip SE_EnemyHurt {get; set;}
	private AudioClip SE_EnemyDie {get; set;}
	private AudioClip SE_DoorAccessGranted {get; set;}
	private AudioClip SE_DoorAccessRefused {get; set;}
	private AudioClip SE_EnemyYoujin1 {get; set;}
	private AudioClip SE_EnemyYoujin2 {get; set;}
	private AudioClip SE_EnemyYoujin3 {get; set;}
	private AudioClip SE_EnemyYoujin4 {get; set;}
	private AudioClip SE_EnemyYoujin5 {get; set;}

    public AudioSource[] audioSource {get; set;}

    private bool[] sourceIsFree ;
	
	private UISprite volumeUISprite;
	
	public void Awake ()
	{
		if (this != Instance) {
			Destroy (this.gameObject);
			//Debug.Log ("Destroy Sound Manager :Start");
			return;
		}
		else
        {
			Debug.Log ("Dont destroy on load");
			DontDestroyOnLoad (this.gameObject);
		}
		
        _volumeOff = false;
    }
    
	void Start () {
		InitAudioSource();

		BGM_InGame = Resources.Load("Sounds/BGM_InGame", typeof(AudioClip)) as AudioClip;
		
		SE_GameOver = Resources.Load("Sounds/endgame", typeof(AudioClip)) as AudioClip;
		SE_DoorOpen = Resources.Load("Sounds/DoorOpen", typeof(AudioClip)) as AudioClip;
		SE_DoorClose = Resources.Load("Sounds/DoorClose", typeof(AudioClip)) as AudioClip;
		SE_GrabObject = Resources.Load("Sounds/grabObject2", typeof(AudioClip)) as AudioClip;
		SE_EnemyHurt = Resources.Load("Sounds/EnemyHurt2", typeof(AudioClip)) as AudioClip;
		SE_EnemyDie = Resources.Load("Sounds/EnemyDie(2)", typeof(AudioClip)) as AudioClip;
		SE_DoorAccessGranted = Resources.Load("Sounds/AccessGranted", typeof(AudioClip)) as AudioClip;
		SE_DoorAccessRefused = Resources.Load("Sounds/AccessRefused", typeof(AudioClip)) as AudioClip;
		SE_EnemyYoujin1 = Resources.Load("Sounds/Youjin0", typeof(AudioClip)) as AudioClip;
		SE_EnemyYoujin2 = Resources.Load("Sounds/Youjin1(2)", typeof(AudioClip)) as AudioClip;
		SE_EnemyYoujin3 = Resources.Load("Sounds/Youjin2(2)", typeof(AudioClip)) as AudioClip;
		SE_EnemyYoujin4 = Resources.Load("Sounds/Youjin3(2)", typeof(AudioClip)) as AudioClip;
		SE_EnemyYoujin5 = Resources.Load("Sounds/Youjin4(2)", typeof(AudioClip)) as AudioClip;
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnLevelFinishedLoading;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnLevelFinishedLoading;
    }

    void OnLevelFinishedLoading(Scene scene, LoadSceneMode mode)
    {
        if (this != Instance)
        {
            Destroy(this.gameObject);
            return;
        }
        print("OnLevelWasLoaded: " + "level " + scene.name);
        switch (scene.name)
        {
            case ("testScene")://GameScene
                StartCoroutine(SwitchPlayBGMLate(soundName.BGM_InGame));
                //checkIcon();
                break;
            case ("GameClear")://GameClear
                SwitchPlayBGM(soundName.BGM_InGame);
                break;
            case ("GameOver")://GameOver
                StartCoroutine(SwitchPlayBGMLate(soundName.BGM_Result));
                break;
        }
    }

    public void InitAudioSource()
    {
        audioSource = gameObject.GetComponents<AudioSource>();
        sourceIsFree = new bool[audioSource.Length];
        for (int i = 0; i < audioSource.Length; i++)
        {
            sourceIsFree[i] = true;
        }
    }

    public int GetFreeAudioSourceIndex()
    {
        for (int i = 0; i < 3; i++)
        {
            if (sourceIsFree[i])
                return i;
        }
        return -1;
    }

    IEnumerator SwitchPlayBGMLate(soundName sound)
	{
		yield return new WaitForSeconds(0.05f);
		SwitchPlayBGM(sound);
	}

	public void VolumeOnOff(bool val)
    {
		if(val)
        {// if volume on
			audioSource[0].volume = 0.8f;
			if(audioSource[0].clip == null)
                audioSource[0].clip = GetBGMClip(soundName.BGM_InGame);	
			audioSource[0].loop = true;
			audioSource[0].Play();	
			print("volumeOn");
			_volumeOff = false;
		}
		else
        {
			audioSource[0].Pause ();
			print("volumeOff");
			_volumeOff = true;
		}
	}
	
	public void SwitchPlayBGM(soundName sound){
		
		if(_volumeOff)
            return;
		else
		{
			audioSource[0].volume = 0.3f;
			audioSource[0].clip = GetBGMClip(sound);
			audioSource[0].loop = true;
			audioSource[0].Play ();	
			Debug.Log ("Background Sound switched");
		}
	}

	public void StopBGM()
    {
		audioSource[0].Stop();
	}
	
	public AudioClip GetAudioClip(soundName sound)
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

			case soundName.SE_EnemyHurt:
			return SE_EnemyHurt;

			case soundName.SE_EnemyDie:
			return SE_EnemyDie;

			case soundName.SE_DoorAccessGranted:
			return SE_DoorAccessGranted;

			case soundName.SE_DoorAccessRefused:
			return SE_DoorAccessRefused;

			case soundName.SE_EnemyYoujin1:
			return SE_EnemyYoujin1;

			case soundName.SE_EnemyYoujin2:
			return SE_EnemyYoujin2;

			case soundName.SE_EnemyYoujin3:
			return SE_EnemyYoujin3;

			case soundName.SE_EnemyYoujin4:
			return SE_EnemyYoujin4;

			case soundName.SE_EnemyYoujin5:
			return SE_EnemyYoujin5;
			
			default:
			Debug.Log ("Please Enter the name of an audio clip");
			return null;
		}
	}
	
	public AudioClip GetBGMClip(soundName sound)
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
	
	IEnumerator SoundFinishPlaying(float waitTime, int indexAudioSource)
	{
		yield return new WaitForSeconds(waitTime);
		sourceIsFree[indexAudioSource] = true;
	}

	public bool PlaySound(soundName sound)
	{
		if(_volumeOff)
            return true;

        //get free audio source index
		int index = GetFreeAudioSourceIndex();
		
		//no free audio source
		if(index ==-1)return false;
			
		//the current audio source become busy
		sourceIsFree[index] = false;
		
		audioSource[index+1].volume = 0.8f;
		audioSource[index+1].clip = GetAudioClip(sound);
		audioSource[index+1].Play();
		StartCoroutine(SoundFinishPlaying(audioSource[index+1].clip.length, index));
		
		return true;

	}

	public bool PlaySoundLoop(soundName sound, int timesToPlay, float timeBetweenSounds,float firstTimeDelay)
	{
		if(_volumeOff)
            return true;

		StartCoroutine(PlaySoundLoopRoutine(sound, timesToPlay, timeBetweenSounds, firstTimeDelay));

		return true;
	}

	public IEnumerator PlaySoundLoopRoutine(soundName sound, int timesToPlay, float timeBetweenSounds, float firstTimeDelay)
    {
		yield return new WaitForSeconds (firstTimeDelay);
		audioSource[1].volume = 0.8f;
		audioSource[1].clip = GetAudioClip(sound);
		for(int i = 1; i <= timesToPlay; i++){
			yield return new WaitForSeconds(timeBetweenSounds);

			audioSource[1].Play();
		}
		yield return new WaitForSeconds(timeBetweenSounds);
		audioSource [1].clip = GetAudioClip (soundName.SE_EnemyDie);
		audioSource[1].Play();

	}
	
	public void StopPlayLoop()
	{
		audioSource [1].Stop ();
	}
}
