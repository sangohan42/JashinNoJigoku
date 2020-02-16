using UnityEngine;

public enum CoverState {OnDownFace, OnRightFace, OnLeftFace, None};

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]
public class CharacterControllerLogic : MonoBehaviour 
{
    #region Variables (private)

    // Inspector serialized
    [SerializeField]
	private GameObject _radarCam;
    [SerializeField]
	private float _directionDampTime = 0.25f;
	[SerializeField]
	private float _speedDampTime = 0.05f;
    [SerializeField]
    private float _rotationSpeed = 15f;
    [SerializeField]
    private float _cameraFollowSpeed = 5f;
    [SerializeField]
    private Transform _cameraInCoverPos;
    [SerializeField]
    private Transform _cameraInLookingAroundPosRight;
    [SerializeField]
    private Transform _cameraInLookingAroundPosLeft;
    [SerializeField]
    private Transform _camInPanoramic;

    //const
    private const float COVER_SPEED = 0.9f;
    private const float LEVEL_MAX_WIDTH = 20;
    private const float LEVEL_MIN_WIDTH = 4;
    public const float JOYSTICK_INPUT_THRESHOLD = 0.15f;

    // Private global only
    private Animator _animator;
    private Rigidbody _rigidbody;
    private Transform _gamecamTransform;
    private AnimatorHashIds _animatorHashIds;
    private AnimatorStateInfo _stateInfo;
    private UIJoystick _uiJoystick;
    private CapsuleCollider _caps;
    private PlayerLifeHandler _playerLifeHandler;
    private SoundManager _soundManager;

    private float _joyX;
    private float _joyY;

    private float _speed;
    private float _direction;

    private Vector3 _initialRadarCameraRotation;
	private Vector3 _initialRadarCameraPosition;

    private Vector3 _initialCoverPos;
	private Vector3 _initialCoverRot;

	private Vector3 _lookAroundPosRight;
	private Vector3 _lookAroundRotRight;
	private Vector3 _lookAroundPosRightCopy;

    private Vector3 _lookAroundPosLeft;
	private Vector3 _lookAroundRotLeft;
	private Vector3 _lookAroundPosLeftCopy;

	private Vector3 _currentLookAroundPos;
	private Vector3 _currentLookAroundRot;

	private bool _isInCoverMode;
    private bool _isInPositioningCoverModeCam;
	private bool _isPlayerPlaced;
	private bool _isInModifyCoverPos;
    private bool _isPlayerCloseToBorder;
    private bool _inLookAroundMode;
    private bool _isInCoverLookingAround;
    private bool _hasBeenInCover;
    private bool _isInPanoramicView;

    private float _currentModifToCoverPos;
    private float _currentModifToPanoramicRotVertical;
    private Vector3 _camRotationWhenCloseToBorder;

    private int buildingMask;
	private int NPCMask;

    #endregion
    
	#region Properties (public)

    public Vector3 CameraRotation { get; set; } // The camera rotation in normal mode
    public Vector3 CameraPosition { get; set; } // The camera position in normal mode
    public CoverState CurrentCoverState { get; set; }
    public Vector3 VecToAlignTo { get; set; }
    public Vector3 PositionToPlaceTo { get; set; }
    public Vector3 SavedCamPosition { get; set; }
    public Quaternion SavedCamRotation { get; set; }
    public Vector3 CoverPos { get; set; }
    public Vector3 CoverRot { get; set; }
    public bool InCrouchCoverMode { get; set; }
    public float CoverModeColliderBoundingBoxMinX { get; set; }
    public float CoverModeColliderBoundingBoxMaxX { get; set; }
    public float CoverModeColliderBoundingBoxMinZ { get; set; }
    public float CoverModeColliderBoundingBoxMaxZ { get; set; }
    public bool IsPursued { get; set; }
    public bool GotKey { get; set; }
    public int FoundShoukoNb { get; set; }
    public int TotalShoukoNb { get; set; }
    public int NumberOfEnemyInPursuitMode { get; set; }
    public float ElapsedTimeSinceLevelStartup { get; set; }
    public bool IsCloseToEnemy { get; set; }
    public Animator CloseEnemyAnimator { get; set; }
    public GameObject CloseEnemy { get; set; }
    #endregion

    #region Unity event functions

    /// Use this for initialization.
    void Start() 
	{
        _animator = GetComponent<Animator>();
        if (_animator.layerCount >= 2)
        {
            _animator.SetLayerWeight(1, 1);
        }
        _rigidbody = GetComponent<Rigidbody>();
        _animatorHashIds = GameObject.FindGameObjectWithTag(Tags.animatorHashController).GetComponent<AnimatorHashIds>();
        _uiJoystick = GameObject.FindGameObjectWithTag(Tags.joystick).GetComponent<UIJoystick>();

        _gamecamTransform = Camera.main.transform;
        _caps = GetComponent<CapsuleCollider>();

		CameraRotation = _gamecamTransform.eulerAngles;
		CameraPosition = _gamecamTransform.position - transform.position;
        CoverPos = _cameraInCoverPos.localPosition;
        CoverRot = _cameraInCoverPos.localEulerAngles;
        IsPursued = false;
        CurrentCoverState = CoverState.None;
        GotKey = false;
        NumberOfEnemyInPursuitMode = 0;
        FoundShoukoNb = 0;
        TotalShoukoNb = GameObject.FindGameObjectsWithTag(Tags.shouko).Length;
        ElapsedTimeSinceLevelStartup = 0;
        IsCloseToEnemy = false;

        _initialRadarCameraRotation = _radarCam.transform.eulerAngles;
		_initialRadarCameraPosition = _radarCam.transform.position - transform.position;

		_initialCoverPos = CoverPos;
		_initialCoverRot = CoverRot;

		_lookAroundPosRight = _cameraInLookingAroundPosRight.localPosition;
		_lookAroundRotRight = _cameraInLookingAroundPosRight.localEulerAngles;
		_lookAroundPosRightCopy = _lookAroundPosRight;

		_lookAroundPosLeft = _cameraInLookingAroundPosLeft.localPosition;
		_lookAroundRotLeft = _cameraInLookingAroundPosLeft.localEulerAngles;
		_lookAroundPosLeftCopy = _lookAroundPosLeft;
		_currentModifToPanoramicRotVertical = 0;

		_isInCoverMode = false;
		_isInPositioningCoverModeCam = false;
		_isPlayerPlaced = false;
		_inLookAroundMode = false;
		_isInCoverLookingAround = false;
		_hasBeenInCover = false;
		_isInModifyCoverPos = false;
		_currentModifToCoverPos = 0;

		_isInPanoramicView = false;

		_camRotationWhenCloseToBorder = new Vector3 (45f, 0, 0);

		_isPlayerCloseToBorder = false;

		buildingMask = 11;
        NPCMask = 1 << buildingMask;

		_playerLifeHandler = this.GetComponent<PlayerLifeHandler> ();

		_soundManager = GameObject.FindGameObjectWithTag (Tags.soundmanager).GetComponent<SoundManager> ();
			
	}

	private void CompensateForWalls(Vector3 fromObject)
	{
		Vector3 globalTargetPos = fromObject + Vector3.up + transform.forward *CoverPos.z;
		// Compensate for walls between camera
		RaycastHit wallHit = new RaycastHit();	

		if (Physics.Linecast(fromObject, globalTargetPos, out wallHit, NPCMask)) 
		{
			if(CurrentCoverState == CoverState.OnLeftFace || CurrentCoverState == CoverState.OnRightFace)
			{
				float modifValue = Mathf.Abs(wallHit.point.x - fromObject.x);
                CoverPos = new Vector3(CoverPos.x, CoverPos.y -0.5f, modifValue);
				_lookAroundPosRight.z = modifValue;
				_lookAroundPosLeft.z = modifValue;

			}
			else 
			{
				float modifValue = Mathf.Abs(wallHit.point.z - fromObject.z);
                CoverPos = new Vector3(CoverPos.x, CoverPos.y -0.5f, modifValue);
				_lookAroundPosRight.z = modifValue;
				_lookAroundPosLeft.z = modifValue;
			}
			_lookAroundPosRight.y -= 0.5f;
			_lookAroundPosLeft.y -= 0.5f;
			
		}
	}
	
	void LateUpdate()
	{
        //Reset Radar Camera Rotation and position
		Vector3 currentRadarEulerAngles = _radarCam.transform.eulerAngles;
        currentRadarEulerAngles = _initialRadarCameraRotation; 
		_radarCam.transform.eulerAngles = currentRadarEulerAngles;
		_radarCam.transform.position = transform.position+_initialRadarCameraPosition;

		if(!_isInPanoramicView)
		{
			//Reset Camera position and rotation if we are not in cover 
			if(CurrentCoverState == CoverState.None)
			{
				//If we have been in cover(the camera was attached to the player so we detach it
				if(_hasBeenInCover)
                    _gamecamTransform.parent = null;

				// The position to reach
				Vector3 standardPos = transform.position + CameraPosition;

					//We are not close to the border of the level
				if(transform.position.x < LEVEL_MAX_WIDTH && transform.position.x > LEVEL_MIN_WIDTH)
				{
					if(_isPlayerCloseToBorder)
					{
						_isPlayerCloseToBorder = false;
					}

					//Reset rotation
					Vector3 rot = _gamecamTransform.eulerAngles;
					rot = CameraRotation;

					if(!_hasBeenInCover)
					{
						// Lerp the camera's position between it's current position and it's new position.
						_gamecamTransform.position = Vector3.Lerp(_gamecamTransform.position, standardPos, _cameraFollowSpeed * Time.deltaTime);
						_gamecamTransform.eulerAngles = Vector3.Lerp(_gamecamTransform.eulerAngles,rot, _cameraFollowSpeed * Time.deltaTime);
					}

					//The transition from cover to normal mode had to be abrupt
					else 
					{
						_gamecamTransform.position = standardPos;
						_gamecamTransform.eulerAngles = rot;
					}

				}

				//We are close to the border of the level
				else
				{

					if(!_isPlayerCloseToBorder)
					{
						_isPlayerCloseToBorder = true;
					}

					standardPos.x = (transform.position.x > LEVEL_MAX_WIDTH) ? LEVEL_MAX_WIDTH : LEVEL_MIN_WIDTH;
					_gamecamTransform.position = Vector3.Lerp(_gamecamTransform.position, standardPos, _cameraFollowSpeed * Time.deltaTime);

					_gamecamTransform.eulerAngles = Vector3.Lerp(_gamecamTransform.eulerAngles,_camRotationWhenCloseToBorder, _cameraFollowSpeed * Time.deltaTime);

				}

				_hasBeenInCover = false;

				// If there is some axis input...
				if((_joyX != 0f || _joyY != 0f) && !_playerLifeHandler.PlayerIsDead)
				{
					// ... set the players rotation and set the speed parameter to 5.5f.
					Rotating(_joyX, _joyY);
				}
			}

			//We are positiong the character close to the wall so we save the camera position until it's correctly set
			else if(_isInPositioningCoverModeCam == false && _isInCoverMode == false && !_isInCoverLookingAround)
			{
				_gamecamTransform.position = SavedCamPosition;
				_gamecamTransform.rotation = SavedCamRotation;
			}

			else if (_isInCoverMode && !_inLookAroundMode)
			{
				_gamecamTransform.localPosition = CoverPos;
				_gamecamTransform.localEulerAngles = CoverRot;
			}
		}

	}
	
	/// Update is called once per frame.
	void Update() 
	{

		_stateInfo = _animator.GetCurrentAnimatorStateInfo(0);
		_direction = 0f;	
		float charSpeed = 0f;

        ElapsedTimeSinceLevelStartup += Time.deltaTime;

		//Get Joystick Vector
		_joyX = _uiJoystick.position.x;
		_joyY = _uiJoystick.position.y;

//		if (Input.GetButtonDown("PanoramicView") && joyX == 0 && joyY == 0)
//		{
//			if(!isInPanoramicView)
//			{
//				isInPanoramicView = true;
//				gamecam.transform.parent = transform;
//				gamecam.transform.localPosition = camPanoramicPosition;
//				gamecam.transform.localEulerAngles = camPanoramicRotation;
//			}
//			else 
//			{
//				isInPanoramicView = false;
//				gamecam.transform.parent = null;
//				currentModifToPanoramicRotVertical = 0;
//			}
//
//		}
		if(Input.GetButtonDown("PanoramicView"))
		{
			if(_joyX == 0 && _joyY == 0 && CurrentCoverState == CoverState.None && !_playerLifeHandler.PlayerIsDead )
			{
				if(!_isInPanoramicView)
				{
					_isInPanoramicView = true;
					_gamecamTransform.parent = transform;
					_gamecamTransform.localPosition = _camInPanoramic.position;
					_gamecamTransform.localEulerAngles = _camInPanoramic.rotation.eulerAngles;
					//				animator.SetBool(hashIdsScript.playerRaiseWeapon, true);
				}
				else 
				{
					_isInPanoramicView = false;
					_gamecamTransform.parent = null;
					_currentModifToPanoramicRotVertical = 0;
					_gamecamTransform.position = transform.position + CameraPosition;
					_gamecamTransform.eulerAngles = CameraRotation;
					//				animator.SetBool(hashIdsScript.playerRaiseWeapon, false);
					
				}
			}
		
			//Rolling
			else if(Mathf.Abs(_joyX) > 0.8f || Mathf.Abs(_joyY) >0.8f)
			{
                _animator.SetBool(_animatorHashIds.RollingBool, true);
			}
		}


		if(_animator.IsInTransition(0) && _animator.GetNextAnimatorStateInfo(0).nameHash == _animatorHashIds.RollingState)
		{
            _animator.SetBool(_animatorHashIds.RollingBool, false);
		}

		if(_animator.IsInTransition(0) && _animator.GetNextAnimatorStateInfo(0).nameHash == _animatorHashIds.StranglingState)
		{
            _animator.SetBool(_animatorHashIds.StranglingBool, false);
		}

		if(_animator.GetCurrentAnimatorStateInfo(0).nameHash == _animatorHashIds.StranglingState)
		{
			transform.forward = CloseEnemy.transform.forward;
			transform.position = CloseEnemy.transform.position- 0.7f*transform.forward;
		}

		if(!_isInPanoramicView)
		{

			//NOT in COVER
			if(CurrentCoverState == CoverState.None)
			{
				_isInCoverMode = false;
				_isPlayerPlaced = false;
				_isInCoverLookingAround = false;
//				camSwitchDamp = 12f;

				Vector3 stickDirection = new Vector3 (_joyX, 0, _joyY);
				charSpeed = stickDirection.magnitude;
				_speed = charSpeed;

                _animator.SetFloat(_animatorHashIds.SpeedFloat, _speed, _speedDampTime, Time.deltaTime);

				if (_speed < JOYSTICK_INPUT_THRESHOLD && Mathf.Abs(_joyX) < 0.05f)    // Dead zone
				{
                    _animator.SetFloat(_animatorHashIds.Direction, 0f);
                    _animator.SetFloat(_animatorHashIds.Angle, 0f);
				}

			}

			//IN COVER
			else
			{
				//We place the player close to the wall while we are in transition
				if(!_isPlayerPlaced)
				{
					_hasBeenInCover = true;
					_gamecamTransform.parent = transform;

                    _animator.SetFloat(_animatorHashIds.SpeedFloat, 0);
                    _animator.SetFloat(_animatorHashIds.Direction, 0);

					transform.forward = VecToAlignTo;
					transform.position = PositionToPlaceTo;
					_isPlayerPlaced = true;

					Vector3 currPos = transform.position;
					currPos.y = -0.02f;
					transform.position = currPos;

					//We verify that the coverPos is OK or if there is a wall between the player and the camera
					//If there is a wall we change the coverPos as well as the lookingAroundPos
					CompensateForWalls(transform.position);

					if(InCrouchCoverMode)
					{
						_caps.center = new Vector3(0,0.5f,0);
						_caps.height = 0.9f;
						_caps.radius = 0.3f;
					}

				}
				//We place the camera to the right position
				else if(_gamecamTransform.localPosition != CoverPos && !_inLookAroundMode)
				{
                    _animator.SetFloat(_animatorHashIds.SpeedFloat, 0);
                    _animator.SetFloat(_animatorHashIds.Direction, 0);

					_isInPositioningCoverModeCam = true;

					transform.forward = VecToAlignTo;
					transform.position = PositionToPlaceTo;

					Vector3 currPos = transform.position;
					currPos.y = -0.02f;
					transform.position = currPos;
					_gamecamTransform.localPosition = Vector3.Lerp(_gamecamTransform.localPosition, CoverPos, 13*Time.deltaTime);
					_gamecamTransform.localEulerAngles = Vector3.Lerp(_gamecamTransform.localEulerAngles, CoverRot, 13*Time.deltaTime);
                }
				else
				{
					_isInCoverMode = true;
					_isInPositioningCoverModeCam = false;
					transform.forward = VecToAlignTo;
//					transform.position = positionToPlaceTo;

					Vector3 currPos = transform.position;
					currPos.y = -0.02f;
					transform.position = currPos;

					if(_inLookAroundMode)
					{
						_gamecamTransform.localPosition = Vector3.Lerp(_gamecamTransform.localPosition, _currentLookAroundPos, 7f*Time.deltaTime);
						_gamecamTransform.localEulerAngles = Vector3.Lerp(_gamecamTransform.localEulerAngles, _currentLookAroundRot, 7f*Time.deltaTime);
					}

					switch(CurrentCoverState)
					{
						case CoverState.OnDownFace:
							if(_joyY > -0.2f)
							{
								_speed = Mathf.Abs (_joyX);
								_direction = _joyX;

								if(!_inLookAroundMode)
								{
									if(transform.position.x < CoverModeColliderBoundingBoxMinX && _direction <0)
									{
//										Debug.Log("ENTER IN LOOK AROUND");
										Vector3 temp = transform.position;
										temp.x = CoverModeColliderBoundingBoxMinX;
										transform.position = temp;

                                        PositionToPlaceTo = transform.position;
										_currentLookAroundPos = _lookAroundPosLeft;
										_currentLookAroundRot = _lookAroundRotLeft;
										_inLookAroundMode = true;
                                        _animator.SetBool(_animatorHashIds.LookingAroundBool, true);
										_isInCoverLookingAround = true;
									}

									else if(transform.position.x > CoverModeColliderBoundingBoxMaxX && _direction >0)
									{
										Vector3 temp = transform.position;
										temp.x = CoverModeColliderBoundingBoxMaxX;
										transform.position = temp;

                                        PositionToPlaceTo = transform.position;
										_currentLookAroundPos = _lookAroundPosRight;
										_currentLookAroundRot = _lookAroundRotRight;
										_inLookAroundMode = true;
                                        _animator.SetBool(_animatorHashIds.LookingAroundBool, true);
										_isInCoverLookingAround = true;
									}

									else 
									{
										_rigidbody.MovePosition(_rigidbody.position-1 *transform.right * 1.2f*Time.deltaTime*_direction*Mathf.Abs(_direction)*COVER_SPEED);
									}
								}

								else
								{
									//MIN BORDER
									if(_currentLookAroundPos == _lookAroundPosLeft)
									{

										//COME BACK TO NORMAL COVER
										if(_direction >=0)
										{

											_isInCoverMode = false;
                                            //camSwitchDamp = 7f;
                                            _animator.SetBool(_animatorHashIds.LookingAroundBool, false);
//											transform.position = positionToPlaceTo;
											_inLookAroundMode = false;
										}

									}

									//MAX BORDER
									else
									{
										//COME BACK TO NORMAL COVER
										if(_direction <=0)
										{
											_isInCoverMode = false;
                                            //camSwitchDamp = 7f;
                                            _animator.SetBool(_animatorHashIds.LookingAroundBool, false);
//											transform.position = positionToPlaceTo;
											_inLookAroundMode = false;
										}
									}
								}
							}

							else 
							{
								CurrentCoverState = CoverState.None;
                                _animator.SetBool(_animatorHashIds.CoverBool, false);
                                _animator.SetBool(_animatorHashIds.CrouchCoverBool, false);

                                CoverPos = _initialCoverPos;
								CoverRot = _initialCoverRot;
								_lookAroundPosRight = _lookAroundPosRightCopy;
								_lookAroundPosLeft = _lookAroundPosLeftCopy;
								_currentModifToCoverPos = 0;
								//testCollisionWall.inCoverMode = false;
								_rigidbody.constraints = RigidbodyConstraints.FreezeRotation;
								if(InCrouchCoverMode)
								{
									_caps.center = new Vector3(0,1f,0.04f);
									_caps.height = 2;
									_caps.radius = 0.28f;
									InCrouchCoverMode = false;
								}
							}

							break;

						case CoverState.OnLeftFace:
							if(_joyY > -0.2f)
							{
								_speed = Mathf.Abs (_joyX);
								_direction = _joyX;

								if(!_inLookAroundMode)
								{
									if(transform.position.z < CoverModeColliderBoundingBoxMinZ && _direction >0)
									{
//										Debug.Log ("HERE");
										Vector3 temp = transform.position;
										temp.z = CoverModeColliderBoundingBoxMinZ;
										transform.position = temp;

                                        PositionToPlaceTo = transform.position;
										_currentLookAroundPos = _lookAroundPosRight;
										_currentLookAroundRot = _lookAroundRotRight;
										_inLookAroundMode = true;
                                        _animator.SetBool(_animatorHashIds.LookingAroundBool, true);
										_isInCoverLookingAround = true;
									}
									
									else if(transform.position.z > CoverModeColliderBoundingBoxMaxZ && _direction <0)
									{
										Vector3 temp = transform.position;
										temp.z = CoverModeColliderBoundingBoxMaxZ;
										transform.position = temp;

                                        PositionToPlaceTo = transform.position;
										_currentLookAroundPos = _lookAroundPosLeft;
										_currentLookAroundRot = _lookAroundRotLeft;
										_inLookAroundMode = true;
                                        _animator.SetBool(_animatorHashIds.LookingAroundBool, true);
										_isInCoverLookingAround = true;
									}
									
									else 
									{
									_rigidbody.MovePosition(_rigidbody.position-1 *1.2f*transform.right * Time.deltaTime*_direction*Mathf.Abs(_direction)*COVER_SPEED);
									}
								}
								
								else
								{
								//MIN BORDER
									if(_currentLookAroundPos == _lookAroundPosRight)
									{

									//COME BACK TO NORMAL COVER
										if(_direction <=0)
										{
											_isInCoverMode = false;
                                            //											camSwitchDamp = 7f;
                                            _animator.SetBool(_animatorHashIds.LookingAroundBool, false);
//											transform.position = positionToPlaceTo;
											_inLookAroundMode = false;
										}
										
									}
									
									//MAX BORDER
									else
									{
										//COME BACK TO NORMAL COVER
										if(_direction >=0)
										{
											_isInCoverMode = false;
                                            //											camSwitchDamp = 7f;
                                            _animator.SetBool(_animatorHashIds.LookingAroundBool, false);
//											transform.position = positionToPlaceTo;
											_inLookAroundMode = false;
										}
									}
								}
							}

							else 
							{
								CurrentCoverState = CoverState.None;
                                _animator.SetBool(_animatorHashIds.CoverBool, false);
                                _animator.SetBool(_animatorHashIds.CrouchCoverBool, false);

                                CoverPos = _initialCoverPos;
								CoverRot = _initialCoverRot;
								_lookAroundPosRight = _lookAroundPosRightCopy;
								_lookAroundPosLeft = _lookAroundPosLeftCopy;
								_currentModifToCoverPos = 0;
								//testCollisionWall.inCoverMode = false;
								_rigidbody.constraints = RigidbodyConstraints.FreezeRotation;
								if(InCrouchCoverMode)
								{
									_caps.center = new Vector3(0,1f,0.04f);
									_caps.height = 2;
									_caps.radius = 0.28f;
									InCrouchCoverMode = false;
								}
							
							}
							break;

						case CoverState.OnRightFace:
							if(_joyY > -0.2f)
							{
								_speed = Mathf.Abs (_joyX);
								_direction = _joyX;

								if(!_inLookAroundMode)
								{
									if(transform.position.z < CoverModeColliderBoundingBoxMinZ && _direction <0)
									{
										//Debug.Log ("HERE");
										Vector3 temp = transform.position;
										temp.z = CoverModeColliderBoundingBoxMinZ;
										transform.position = temp;
										
										PositionToPlaceTo = transform.position;
										_currentLookAroundPos = _lookAroundPosLeft;
										_currentLookAroundRot = _lookAroundRotLeft;
										_inLookAroundMode = true;
                                        _animator.SetBool(_animatorHashIds.LookingAroundBool, true);
										_isInCoverLookingAround = true;
									}
									
									else if(transform.position.z > CoverModeColliderBoundingBoxMaxZ && _direction >0)
									{
										Vector3 temp = transform.position;
										temp.z = CoverModeColliderBoundingBoxMaxZ;
										transform.position = temp;

                                        PositionToPlaceTo = transform.position;
										_currentLookAroundPos = _lookAroundPosRight;
										_currentLookAroundRot = _lookAroundRotRight;
										_inLookAroundMode = true;
                                        _animator.SetBool(_animatorHashIds.LookingAroundBool, true);
										_isInCoverLookingAround = true;
									}
									
									else 
									{
									_rigidbody.MovePosition(_rigidbody.position-1 *1.2f*transform.right * Time.deltaTime*_direction*Mathf.Abs(_direction)*COVER_SPEED);
									}
								}
								
								else
								{
									//MIN BORDER
									if(_currentLookAroundPos == _lookAroundPosLeft)
									{
										//COME BACK TO NORMAL COVER
										if(_direction >=0)
										{
											_isInCoverMode = false;
                                            //camSwitchDamp = 7f;
                                            _animator.SetBool(_animatorHashIds.LookingAroundBool, false);
											transform.position = PositionToPlaceTo;
											_inLookAroundMode = false;
										}
										
									}
									
									//MAX BORDER
									else
									{
										//COME BACK TO NORMAL COVER
										if(_direction <=0)
										{
											_isInCoverMode = false;
                                            //camSwitchDamp = 7f;
                                            _animator.SetBool(_animatorHashIds.LookingAroundBool, false);
											transform.position = PositionToPlaceTo;
											_inLookAroundMode = false;
										}
									}
								}
							}

							else 
							{
								CurrentCoverState = CoverState.None;
                                _animator.SetBool(_animatorHashIds.CoverBool, false);
                                _animator.SetBool(_animatorHashIds.CrouchCoverBool, false);

                                CoverPos = _initialCoverPos;
								CoverRot = _initialCoverRot;
								_lookAroundPosRight = _lookAroundPosRightCopy;
								_lookAroundPosLeft = _lookAroundPosLeftCopy;
								_currentModifToCoverPos = 0;
								//testCollisionWall.inCoverMode = false;
								_rigidbody.constraints = RigidbodyConstraints.FreezeRotation;
								if(InCrouchCoverMode)
								{
									_caps.center = new Vector3(0,1f,0.04f);
									_caps.height = 2;
									_caps.radius = 0.28f;
									InCrouchCoverMode = false;
								}

							}
							break;

						default:
							break;
					}
                    //positionToPlaceTo = transform.position;
                    //Debug.Log ("direction = " + direction);

                    _animator.SetFloat(_animatorHashIds.SpeedFloat, _speed, _speedDampTime, Time.deltaTime);
                    _animator.SetFloat(_animatorHashIds.Direction, _direction, _directionDampTime, Time.deltaTime);
				}
			}
		}

		//In Panoramic view
		else
		{
			//If we are not dead
			if(!_playerLifeHandler.PlayerIsDead)RotateInPanoramic(_joyX, _joyY);

			//If we are dead we switch back to the normal camera mode
			else 
			{
				_isInPanoramicView = false;
				_gamecamTransform.parent = null;
				_currentModifToPanoramicRotVertical = 0;
				_gamecamTransform.position = transform.position + CameraPosition;
				_gamecamTransform.eulerAngles = CameraRotation;
			}
		}
	}
	
	void RotateInPanoramic(float horizontal, float vertical)
	{
		float nextValX = _currentModifToPanoramicRotVertical + vertical;

		//VERTICAL MOVEMENT
		if(Mathf.Abs(nextValX) <60)
		{
			_currentModifToPanoramicRotVertical = nextValX; 
			Vector3 copy = _gamecamTransform.localEulerAngles;
			copy.x -= vertical;
			_gamecamTransform.localEulerAngles = copy;
		}

		//HORIZONTAL MOVEMENT
		transform.rotation = Quaternion.AngleAxis(1.5f*horizontal, Vector3.up) * transform.rotation;
	}

	void Rotating (float horizontal, float vertical)
	{
		// Create a new vector of the horizontal and vertical inputs.
		Vector3 targetDirection = new Vector3(horizontal, 0f, vertical);
		
		// Create a rotation based on this new vector assuming that up is the global y axis.
		Quaternion targetRotation = Quaternion.LookRotation(targetDirection, Vector3.up);
		
		// Create a rotation that is an increment closer to the target rotation from the player's rotation.
		Quaternion newRotation = Quaternion.Lerp(_rigidbody.rotation, targetRotation, _rotationSpeed * Time.deltaTime);
		
		// Change the players rotation to this new rotation.
		_rigidbody.MoveRotation(newRotation);
	}
	
	/// Any code that moves the character needs to be checked against physics
	void FixedUpdate()
	{							
		AudioManagement ();
	}

	void AudioManagement ()
	{
		// If the player is currently in the run state...
		if(_animator.GetCurrentAnimatorStateInfo(0).nameHash == _animatorHashIds.LocomotionIdState)
		{
			// ... and if the footsteps are not playing...
			if(!GetComponent<AudioSource>().isPlaying)
				// ... play them.
				GetComponent<AudioSource>().Play();
		}
		else
			// Otherwise stop the footsteps.
			GetComponent<AudioSource>().Stop();
		
	}

	public bool IsInLocomotion()
	{
		return _stateInfo.nameHash == _animatorHashIds.LocomotionIdState;
	}

	// Subscribe to events
	void OnEnable(){
		
		EasyTouch.On_TouchStart += HandleOn_TouchStart;
		EasyTouch.On_TouchDown += HandleOn_TouchDown;
		EasyTouch.On_TouchUp += HandleOn_TouchUp;
		EasyTouch.On_DoubleTap += HandleOn_DoubleTap;
		EasyTouch.On_SimpleTap += HandleOnSimpleTap;
		
	}


	void OnDestroy()
	{
		EasyTouch.On_TouchStart -= HandleOn_TouchStart;
		EasyTouch.On_TouchDown -= HandleOn_TouchDown;
		EasyTouch.On_TouchUp -= HandleOn_TouchUp;
		EasyTouch.On_DoubleTap -= HandleOn_DoubleTap;
    }

    void HandleOnSimpleTap (Gesture gesture)
	{
		if (IsCloseToEnemy && !CloseEnemy.GetComponent<EnemySight>().IsDead && !_isInCoverMode)
		{
			transform.forward = CloseEnemy.transform.forward;
			//			closeEnemy.transform.forward = transform.forward;
			transform.position = CloseEnemy.transform.position- 0.66f*transform.forward;
			//			closeEnemy.transform.position = transform.position+ 0.4f*transform.forward;

			CloseEnemy.GetComponent<SphereCollider>().enabled = false;
			CloseEnemy.GetComponent<CapsuleCollider>().enabled = false;
			CloseEnemy.GetComponent<EnemySight>().IsDead = true;
            _animator.SetBool(_animatorHashIds.StranglingBool, true);
			CloseEnemyAnimator.SetBool(_animatorHashIds.IsStrangledBool, true);
			foreach (Transform child in CloseEnemy.transform)
			{
				if(child.name == "FOV" || child.name == "PositionPoint"|| child.name == "InterrogativePoint")
				{
					child.gameObject.SetActive(false);
				}
			}
//			soundManager.playSound(soundName.SE_EnemyHurt);
			_soundManager.PlaySoundLoop(soundName.SE_EnemyHurt,11, 0.25f, 0.4f);
//			playSoundAfter(soundName.SE_EnemyHurt, 1f);
		}
	}

	void HandleOn_DoubleTap (Gesture gesture)
	{
		if(_joyX == 0 && _joyY == 0 && CurrentCoverState == CoverState.None && !_playerLifeHandler.PlayerIsDead )
		{
			if(!_isInPanoramicView)
			{
				_isInPanoramicView = true;
				_gamecamTransform.parent = transform;
				_gamecamTransform.localPosition = _camInPanoramic.position;
                _gamecamTransform.localEulerAngles = _camInPanoramic.rotation.eulerAngles;
//				animator.SetBool(hashIdsScript.playerRaiseWeapon, true);
            }
			else 
			{
				_isInPanoramicView = false;
				_gamecamTransform.parent = null;
				_currentModifToPanoramicRotVertical = 0;
				_gamecamTransform.position = transform.position + CameraPosition;
				_gamecamTransform.eulerAngles = CameraRotation;
//				animator.SetBool(hashIdsScript.playerRaiseWeapon, false);

			}
		}

		//Rolling
		else if(Mathf.Abs(_joyX) > 0.8f || Mathf.Abs(_joyY) >0.8f)
		{
            _animator.SetBool(_animatorHashIds.RollingBool, true);
		}

	}
	
	void HandleOn_TouchStart (Gesture gesture)
	{
		if (CurrentCoverState != CoverState.None && !_inLookAroundMode)
		{
			_isInModifyCoverPos = true;
		}
	}
	
	void HandleOn_TouchDown (Gesture gesture)
	{
		if(_isInModifyCoverPos && CurrentCoverState != CoverState.None)
		{
			float nextVal = _currentModifToCoverPos + gesture.deltaPosition.x;
//			Debug.Log ("currentModifToCoverPos = " + currentModifToCoverPos);
			if(Mathf.Abs(nextVal) <90)
			{
				_currentModifToCoverPos = nextVal; 
				CoverPos = Quaternion.AngleAxis(gesture.deltaPosition.x, Vector3.up) * CoverPos;
                Vector3 newCoverRot = CoverRot;
                newCoverRot.y += gesture.deltaPosition.x;
                CoverRot = newCoverRot;
            }
		}
	}
	
	void HandleOn_TouchUp (Gesture gesture)
	{
		if (_isInModifyCoverPos)
		{
			_isInModifyCoverPos = false;
		}

	}
	
	#endregion
	
}
