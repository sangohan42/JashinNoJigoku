using UnityEngine;

[RequireComponent(typeof(Animation))]
public class ItemRotater : MonoBehaviour {

	private float _degreesPerSecond;
	private bool _stopRotating;
	private GameObject _onRadarGO;
	private GameObject _textGO;
	private bool _isAKey;
	private CharacterControllerLogic _characterControllerLogic;
	private SoundManager _soundManager;

	// Use this for initialization
	void Start ()
    {
        _degreesPerSecond = 160;
		_stopRotating = false;
		foreach (Transform child in transform)
		{
			if(child.gameObject.name == "Radar")
			{
				_onRadarGO = child.gameObject;
			}
			else
                _textGO = child.gameObject;
		}
		if (CompareTag (Tags.key))
            _isAKey = true;
		else
            _isAKey = false;
		_characterControllerLogic = GameObject.FindGameObjectWithTag (Tags.player).GetComponent<CharacterControllerLogic> ();
		_soundManager = GameObject.FindGameObjectWithTag (Tags.soundmanager).GetComponent<SoundManager> ();
	}
	
	// Update is called once per frame
	void Update ()
    {
		if(!_stopRotating)
            transform.Rotate(_degreesPerSecond * Time.deltaTime * Vector3.up, Space.Self);
        else 
		{
			Vector3 localEulerAngles = transform.localEulerAngles;
            localEulerAngles.y = 0f;
			transform.localEulerAngles = localEulerAngles;
		}
	}

	public void StopRotation()
	{
		//Stop rotating
		_stopRotating = true;
		
		//if we grab a key we set the boolean to true to be able to open the door
		if(_isAKey)
            _characterControllerLogic.GotKey = true;
		//else we grab a shoukou
		else
            _characterControllerLogic.FoundShoukoNb ++;
		
		//disable the item rendering
		GetComponent<Renderer>().enabled = false;
		
		//destroy item rendering on radar
		_onRadarGO.GetComponent<Renderer>().enabled = false;

        //Enable text and play animation
        _textGO.GetComponent<Renderer>().enabled = true;
		
		_soundManager.PlaySound(soundName.SE_GrabObject);
		
		GetComponent<Animation>().Play("objectCatching");
	}

    void DestroyObject()
	{
		Destroy (this.gameObject);
	}
}

