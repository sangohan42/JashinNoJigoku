using UnityEngine;

[RequireComponent(typeof(Animator))]
public class PlayerLifeHandler : MonoBehaviour
{
    [SerializeField]
    public float _health = 200f;						// How much health the player has left.
    [SerializeField]
    public float _resetAfterDeathTime = 3f;				// How much time from the player dying to the level reseting.
    [SerializeField]
    public AudioClip _gameOverClip;                     // The sound effect of gameover.
    [SerializeField]
    private UILabel _healthPointLabel;
    [SerializeField]
    private Transform _lifeBar;

    private Animator _animator;							// Reference to the animator component.
	private AnimatorHashIds _animatorHashIds;			// Reference to the HashIDs.
	private SceneFadeInOut _sceneFadeInOut;			    // Reference to the SceneFadeInOut script.
	private LastPlayerSighting _lastPlayerSighting;	    // Reference to the LastPlayerSighting script.
	private float _elapsedTimeSinceDeath;				// A timer for counting to the reset of the level once the player is dead.
	private float _maxHealthPoint;
	private string _maxHealthPointString;
	private SoundManager _soundManager;
	private bool _resetLevel;

    public float Health
    {
        get { return _health; }
        set { _health = value; }
    }              
    
    public bool PlayerIsDead { get; set; }              // A bool to show if the player is dead or not.

    void Awake ()
	{
		// Setting up the references.
		_animator = GetComponent<Animator>();
		_animatorHashIds = GameObject.FindGameObjectWithTag(Tags.animatorHashController).GetComponent<AnimatorHashIds>();
		_sceneFadeInOut = GameObject.FindGameObjectWithTag(Tags.fader).GetComponent<SceneFadeInOut>();
        _lastPlayerSighting = LastPlayerSighting.Instance;
        _maxHealthPointString = "/" + _health;
		_maxHealthPoint = _health;
		_soundManager = GameObject.FindGameObjectWithTag (Tags.soundmanager).GetComponent<SoundManager> ();
		_resetLevel = false;
	}
	
	
    void Update ()
	{
		if(_health <0)_health = 0;
		_healthPointLabel.text = Mathf.CeilToInt(_health) + _maxHealthPointString;
		Vector3 localScaleLifeBar = _lifeBar.localScale;
		localScaleLifeBar.x = _health / _maxHealthPoint;
		_lifeBar.localScale = localScaleLifeBar;

		// If health is less than or equal to 0...
		if(_health <= 0f)
		{
			// ... and if the player is not yet dead...
			if(!PlayerIsDead)
				// ... call the PlayerDying function.
				PlayerDying();
			else
			{
				// Otherwise, if the player is dead, call the PlayerDead and LevelReset functions.
				PlayerDead();
				LevelReset();
			}
		}
	}
	
	
	void PlayerDying ()
	{
		// The player is now dead.
		PlayerIsDead = true;

		// Set the animator's dead parameter to true also.
		_animator.SetBool(_animatorHashIds.DeadBool, PlayerIsDead);

		_soundManager.PlaySound (soundName.SE_GameOver);
        //		//Transition from animated to ragdolled
        //_animator.enabled = false; //disable animation
        //rigidbody.isKinematic = false; //allow the ragdoll RigidBodies to react to the environment
        //rigidbody.constraints = RigidbodyConstraints.None;
        //rigidbody.AddForce(-2f * transform.forward, ForceMode.VelocityChange);

        //Play the dying sound effect at the player's location.

        AudioSource.PlayClipAtPoint(_gameOverClip, transform.position);
    }
	
	
	void PlayerDead ()
	{
		// If the player is in the dying state then reset the dead parameter.
		if(_animator.GetCurrentAnimatorStateInfo(0).nameHash == _animatorHashIds.DyingState)
			_animator.SetBool(_animatorHashIds.DeadBool, false);
		
		// Disable the movement.
		_animator.SetFloat(_animatorHashIds.SpeedFloat, 0f);

		// Reset the player sighting to turn off the alarms.
        _lastPlayerSighting.Reset();
		
		// Stop the footsteps playing.
		GetComponent<AudioSource>().Stop();
//		soundManager.stopBGM ();
	}
	
	
	void LevelReset ()
	{
		// Increment the timer.
		_elapsedTimeSinceDeath += Time.deltaTime;
		
		//If the timer is greater than or equal to the time before the level resets...
		if(_elapsedTimeSinceDeath >= _resetAfterDeathTime && !_resetLevel)
		{
			_resetLevel = true;
			// ... fadeout and load the gameover scene.
            _sceneFadeInOut.StartFadeOut( () =>
            {
                // ... Load the GameOver scene (which will reload the level).
                FadeManager.Instance.LoadLevel("GameOver", 0.25f);
            } );
        }
	}
	
	
	public void TakeDamage (float amount)
    {
		// Decrement the player's health by amount.
        _health -= amount;
    }
}
