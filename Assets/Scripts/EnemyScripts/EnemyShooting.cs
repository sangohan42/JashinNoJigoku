using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(SphereCollider))]
public class EnemyShooting : MonoBehaviour
{
    [SerializeField]
    private float _maximumDamage = 50f;					    // The maximum potential damage per shot.
    [SerializeField]
    private float _minimumDamage = 25f;					    // The minimum potential damage per shot.
    [SerializeField]
    private AudioClip _shotClip;							    // An audio clip to play when a shot happens.
    [SerializeField]
    private float _flashIntensity = 3f;					    // The intensity of the light when the shot happens.
    [SerializeField]
    private float _fadeSpeed = 10f;						    // How fast the light will fade after the shot.
    
	private Animator _animator;                             // Reference to the animator.
    private AnimatorHashIds _animatorHashIds;               // Reference to the HashIDs script.
    private LineRenderer _laserShotLineRenderer;	        // Reference to the laser shot line renderer.
	private Light _laserShotLight;						    // Reference to the laser shot light.
	private SphereCollider _playerDetectableSphereCollider;	// Reference to the sphere collider.
	private Transform _player;							    // Reference to the player's transform.
	private PlayerLifeHandler _playerLifeHandler;		    // Reference to the player's health.
	private bool _isShooting;								// A bool to say whether or not the enemy is currently shooting.
	private float _damageIntervalLength;					// Range between minimum and maximum damage.
	private CapsuleCollider _playerCapsuleCollider;         // Player capsule Collider

    void Awake ()
	{
        // Setting up the references.
        _animator = GetComponent<Animator>();
		_laserShotLineRenderer = GetComponentInChildren<LineRenderer>();
		_laserShotLight = _laserShotLineRenderer.gameObject.GetComponent<Light>();
		_playerDetectableSphereCollider = GetComponent<SphereCollider>();
		_player = GameObject.FindGameObjectWithTag(Tags.player).transform;
        _playerLifeHandler = _player.gameObject.GetComponent<PlayerLifeHandler>();
        _animatorHashIds = GameObject.FindGameObjectWithTag(Tags.animatorHashController).GetComponent<AnimatorHashIds>();
		
		// The line renderer and light are off to start.
		_laserShotLineRenderer.enabled = false;
		_laserShotLight.intensity = 0f;

        // The scaledDamage is the difference between the maximum and the minimum damage.
        _damageIntervalLength = _maximumDamage - _minimumDamage;

		_playerCapsuleCollider = GameObject.FindGameObjectWithTag (Tags.player).GetComponent<CapsuleCollider> ();
	}
	
	
	void Update ()
	{
		// Cache the current value of the shot curve.
		float shot = _animator.GetFloat(_animatorHashIds.ShotFloat);
		
		// If the shot curve is peaking and the enemy is not currently shooting...
		if(shot > 0.5f && !_isShooting)
			// ... shoot
			Shoot();
		
		// If the shot curve is no longer peaking...
		if(shot < 0.5f)
		{
			// ... the enemy is no longer shooting and disable the line renderer.
			_isShooting = false;
			_laserShotLineRenderer.enabled = false;
		}
		
		// Fade the light out.
		_laserShotLight.intensity = Mathf.Lerp(_laserShotLight.intensity, 0f, _fadeSpeed * Time.deltaTime);
	}
	
	
	void OnAnimatorIK (int layerIndex)
	{
		// Cache the current value of the AimWeight curve.
		float aimWeight = _animator.GetFloat(_animatorHashIds.AimWeightFloat);

        // Set the IK position of the right hand to the player's center.
        _animator.SetIKPosition(AvatarIKGoal.RightHand, _player.position + Vector3.up * _playerCapsuleCollider.center.y);

        // Set the weight of the IK compared to animation to that of the curve.
        _animator.SetIKPositionWeight(AvatarIKGoal.RightHand, aimWeight);
	}	
	
	
	void Shoot ()
	{
        // The enemy is shooting.
		_isShooting = true;
		
		// The fractional distance from the player, 1 is next to the player, 0 is the player is at the extent of the sphere collider.
		float fractionalDistance = (_playerDetectableSphereCollider.radius - Vector3.Distance(transform.position, _player.position)) / _playerDetectableSphereCollider.radius;
	
		// The damage is the scaled damage, scaled by the fractional distance, plus the minimum damage.
		float damage = _damageIntervalLength * fractionalDistance + _minimumDamage;

        // The player takes damage.
        _playerLifeHandler.TakeDamage(damage);
		
		// Display the shot effects.
		ShotEffects();
	}
	
	
	void ShotEffects ()
	{
		// Set the initial position of the line renderer to the position of the muzzle.
		_laserShotLineRenderer.SetPosition(0, _laserShotLineRenderer.transform.position);
		
		// Set the end position of the player's centre of mass.
		_laserShotLineRenderer.SetPosition(1, _player.position + Vector3.up * _playerCapsuleCollider.center.y);

		_laserShotLineRenderer.SetWidth (0.1f, 0.1f);
		// Turn on the line renderer.
		_laserShotLineRenderer.enabled = true;
		
		// Make the light flash.
		_laserShotLight.intensity = _flashIntensity;
		
		// Play the gun shot clip at the position of the muzzle flare.
		AudioSource.PlayClipAtPoint(_shotClip, _laserShotLight.transform.position);
	}
}
