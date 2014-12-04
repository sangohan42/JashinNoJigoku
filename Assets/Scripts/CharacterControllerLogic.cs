using UnityEngine;
using System.Collections;

public enum CoverState {onDownFace, OnRightFace, OnLeftFace, nil};

/// #DESCRIPTION OF CLASS#
public class CharacterControllerLogic : MonoBehaviour 
{
	
	#region Variables (private)
	
	// Inspector serialized
	[SerializeField]
	private Animator animator;
	[SerializeField]
	private GameObject gamecam;
	[SerializeField]
	private GameObject radarCam;
	[SerializeField]
	private Camera mainCam;
	[SerializeField]
	private float rotationDegreePerSecond = 120f;
	[SerializeField]
	private float directionSpeed = 1.5f;
	[SerializeField]
	private float directionDampTime = 0.25f;
	[SerializeField]
	private float speedDampTime = 0.05f;
	[SerializeField]
	private float fovDampTime = 3f;
	[SerializeField]
	private HashIds hashIdsScript;
	
	
	// Private global only
	private float joyX = 0f;
	private float joyY = 0f;
	public float turnSmoothing = 15f;	// A smoothing value for turning the player.
	private AnimatorStateInfo stateInfo;
	private AnimatorTransitionInfo transInfo;
	private float speed = 0f;
	private float direction = 0f;
	private float charAngle = 0f;
	private const float SPRINT_SPEED = 2.0f;	
	private float COVER_SPEED = 0.9f;

	private const float SPRINT_FOV = 75.0f;
	private const float NORMAL_FOV = 60.0f;
	private float capsuleHeight;

	private Vector3 cameraRotation;
	private Vector3 cameraPosition;

	private Vector3 radarCameraRotation;
	private Vector3 radarCameraPosition;

	private UIJoystick uiJoystickScript;

	private CoverState currentCoverState;
	private Vector3 vecToAlignTo;
	private Vector3 positionToPlaceTo;
	private CapsuleCollider caps;

	private Transform CameraInCoverPos;
	private Vector3 coverPos;
	private Vector3 coverRot;

	private Transform CameraInLookingAroundPosRight;
	private Vector3 lookAroundPosRight;
	private Vector3 lookAroundRotRight;
	private Transform CameraInLookingAroundPosLeft;
	private Vector3 lookAroundPosLeft;
	private Vector3 lookAroundRotLeft;

	private Vector3 currentLookAroundPos;
	private Vector3 currentLookAroundRot;
	private bool inLookAroundMode;

	private Vector3 savedCamPosition;
	private Quaternion savedCamRotation;
	
	private bool inCoverMode;
	private bool inPositioningCoverModeCam;
	private bool playerPlaced;

	private float boundingBoxMinX;
	private float boundingBoxMaxX;
	private float boundingBoxMinZ;
	private float boundingBoxMaxZ;

	private float camSwitchDamp;

	private bool hasBeenInLookingAround;

	private bool isPursued;
	
	#endregion
		
	
	#region Properties (public)

	public Animator Animator
	{
		get
		{
			return this.animator;
		}
	}

	public float Speed
	{
		get
		{
			return this.speed;
		}
	}

	public CoverState CurrentCoverState
	{
		get
		{
			return this.currentCoverState;
		}
		set
		{
			this.currentCoverState = value;
		}
	}

	public Vector3 VecToAlignTo
	{
		get
		{
			return this.vecToAlignTo;
		}
		set
		{
			this.vecToAlignTo = value;
		}
	}

	public Vector3 PositionToPlaceTo
	{
		get
		{
			return this.positionToPlaceTo;
		}
		set
		{
			this.positionToPlaceTo = value;
		}
	}

	public Vector3 SavedCamPosition
	{
		get
		{
			return this.savedCamPosition;
		}
		set
		{
			this.savedCamPosition = value;
		}
	}

	public Quaternion SavedCamRotation
	{
		get
		{
			return this.savedCamRotation;
		}
		set
		{
			this.savedCamRotation = value;
		}
	}

	public float BoundingBoxMinX
	{
		get
		{
			return this.boundingBoxMinX;
		}
		set
		{
			this.boundingBoxMinX = value;
		}
	}

	public float BoundingBoxMaxX
	{
		get
		{
			return this.boundingBoxMaxX;
		}
		set
		{
			this.boundingBoxMaxX = value;
		}
	}

	public float BoundingBoxMinZ
	{
		get
		{
			return this.boundingBoxMinZ;
		}
		set
		{
			this.boundingBoxMinZ = value;
		}
	}
	public float BoundingBoxMaxZ
	{
		get
		{
			return this.boundingBoxMaxZ;
		}
		set
		{
			this.boundingBoxMaxZ = value;
		}
	}

	public bool IsPursued
	{
		get
		{
			return this.isPursued;
		}
		set
		{
			this.isPursued = value;
		}
	}
	
	public float LocomotionThreshold { get { return 0.15f; } }
	
	#endregion
	
	
	#region Unity event functions
	
	/// Use this for initialization.
	void Start() 
	{
		animator = GetComponent<Animator>();
		mainCam = Camera.main;
		gamecam = mainCam.transform.gameObject;
		radarCam = GameObject.Find ("RadarCamera");
		hashIdsScript = GameObject.FindGameObjectWithTag(DoneTags.gameController).GetComponent<HashIds> ();

		if(animator.layerCount >= 2)
		{
			animator.SetLayerWeight(1, 1);
		}		

		cameraRotation = gamecam.transform.eulerAngles;
		cameraPosition = gamecam.transform.position - transform.position;
		radarCameraRotation = radarCam.transform.eulerAngles;
		radarCameraPosition = radarCam.transform.position - transform.position;

		uiJoystickScript = GameObject.Find ("JoyStick").GetComponent<UIJoystick> ();

		currentCoverState = CoverState.nil;

		caps = GetComponent<CapsuleCollider>();

		CameraInCoverPos = GameObject.Find ("CameraInCoverPos").transform;
		coverPos = CameraInCoverPos.localPosition;
		coverRot = CameraInCoverPos.localEulerAngles;		

		CameraInLookingAroundPosRight = GameObject.Find ("CameraInLookAroundRight").transform;
		lookAroundPosRight = CameraInLookingAroundPosRight.localPosition;
		lookAroundRotRight = CameraInLookingAroundPosRight.localEulerAngles;

		CameraInLookingAroundPosLeft = GameObject.Find ("CameraInLookAroundLeft").transform;
		lookAroundPosLeft = CameraInLookingAroundPosLeft.localPosition;
		lookAroundRotLeft = CameraInLookingAroundPosLeft.localEulerAngles;

		inCoverMode = false;
		inPositioningCoverModeCam = false;
		playerPlaced = false;
		inLookAroundMode = false;
		hasBeenInLookingAround = false;

		camSwitchDamp = 12f;
		isPursued = false;
	}

	void LateUpdate()
	{

		//Reset Radar Camera Rotation and position
		Vector3 rot2 = radarCam.transform.eulerAngles;
		rot2 = radarCameraRotation; 
		radarCam.transform.eulerAngles = rot2;
		radarCam.transform.position = transform.position+radarCameraPosition;

		//Reset Camera position and rotation if we are not in cover and if we are not in transition from cover
		if(currentCoverState == CoverState.nil)
		{
			if(transInfo.nameHash != hashIdsScript.Cover_LocomotionTrans)
			{
				Vector3 rot = gamecam.transform.eulerAngles;
				rot = cameraRotation; 
				gamecam.transform.eulerAngles = rot;
				gamecam.transform.position = transform.position+cameraPosition;
			}

//			else
//			{
//				Debug.Log("ENTER HERE");
//
//				gamecam.transform.localPosition = Vector3.Lerp(gamecam.transform.localPosition, cameraPosition, 10f*Time.deltaTime);
//				gamecam.transform.localRotation = Quaternion.Lerp(gamecam.transform.localRotation, cameraRotationQuaternion, 10f*Time.deltaTime);
//			}

			//If we still are in idle (not in pivot, not in locomotion, not in Sneak)
			if(stateInfo.nameHash == hashIdsScript.m_IdleState || stateInfo.nameHash == hashIdsScript.m_sneakingState)
			{
				// If there is some axis input...
				if(joyX != 0f || joyY != 0f)
				{
					// ... set the players rotation and set the speed parameter to 5.5f.
					Rotating(joyX, joyY);
				}
			}
		}

		//We are positiong the character close to the wall so we save the camera position until it's correctly set
		else if(inPositioningCoverModeCam == false && inCoverMode == false && !hasBeenInLookingAround)
		{
			gamecam.transform.position = savedCamPosition;
			gamecam.transform.rotation = savedCamRotation;
		}

		else if (inCoverMode && !inLookAroundMode)
		{
			gamecam.transform.localPosition = coverPos;
			gamecam.transform.localEulerAngles = coverRot;
		}

//		else if(inLookAroundMode)
//		{
//			gamecam.transform.localPosition = lookAroundPos;
//			gamecam.transform.localEulerAngles = lookAroundRot;
//		}

	}
	
	/// Update is called once per frame.
	void Update() 
	{

		stateInfo = animator.GetCurrentAnimatorStateInfo(0);
		transInfo = animator.GetAnimatorTransitionInfo(0);
		
		charAngle = 0f;
		direction = 0f;	
		float charSpeed = 0f;

		//Get Joystick Vector
		joyX = uiJoystickScript.position.x;
		joyY = uiJoystickScript.position.y;

		//NOT in COVER
		if(currentCoverState == CoverState.nil)
		{
			inCoverMode = false;
			playerPlaced = false;
			hasBeenInLookingAround = false;
			camSwitchDamp = 12f;

			Vector3 stickDirection = new Vector3 (joyX, 0, joyY);
			Vector3 axisSign = Vector3.Cross(this.transform.forward, stickDirection);
			
			float angleRootToMove = Vector3.Angle(transform.forward, stickDirection) * (axisSign.y < 0 ? -1f : 1f);
			
			charSpeed = stickDirection.magnitude;

			direction = angleRootToMove * directionSpeed / 180f;
			
			charAngle = angleRootToMove;


//			Debug.Log ("Speed = " + speed);
			// Press B to sprint
			if (charSpeed>=1f)
			{
				speed = Mathf.Lerp(speed, SPRINT_SPEED, Time.deltaTime);
			}
			else speed = charSpeed;


			animator.SetFloat(hashIdsScript.speedFloat, speed, speedDampTime, Time.deltaTime);
			animator.SetFloat(hashIdsScript.direction, direction, directionDampTime, Time.deltaTime);
			
			if (speed > LocomotionThreshold)	// Dead zone
			{
				Animator.SetFloat(hashIdsScript.angle, charAngle);
			}

			if (speed < LocomotionThreshold && Mathf.Abs(joyX) < 0.05f)    // Dead zone
			{
				animator.SetFloat(hashIdsScript.direction, 0f);
				animator.SetFloat(hashIdsScript.angle, 0f);
			}

			animator.SetBool(hashIdsScript.sneakingBool, Input.GetButton("Sneak"));
		}

		//IN COVER
		else
		{
			//We place the player close to the wall while we are in transition
			if(!playerPlaced)
			{
				animator.SetFloat(hashIdsScript.speedFloat, 0);
				animator.SetFloat(hashIdsScript.direction, 0);

				switch(currentCoverState)
				{
				case CoverState.onDownFace:
					transform.eulerAngles = new Vector3(0,180,0);
					break;
				case CoverState.OnLeftFace:
					transform.eulerAngles = new Vector3(0,-90,0);
					break;
				case CoverState.OnRightFace:
					transform.eulerAngles = new Vector3(0,90,0);
					break;
				}
//				transform.forward = vecToAlignTo;
				transform.position = positionToPlaceTo;
				playerPlaced = true;

			}
			//We place the camera to the right position
			else if(gamecam.transform.localPosition != coverPos && !inLookAroundMode)
			{
				animator.SetFloat(hashIdsScript.speedFloat, 0);
				animator.SetFloat(hashIdsScript.direction, 0);

				inPositioningCoverModeCam = true;
				switch(currentCoverState)
				{
				case CoverState.onDownFace:
					transform.eulerAngles = new Vector3(0,180,0);
					break;
				case CoverState.OnLeftFace:
					transform.eulerAngles = new Vector3(0,-90,0);
					break;
				case CoverState.OnRightFace:
					transform.eulerAngles = new Vector3(0,90,0);
					break;
				}				
				//transform.position = positionToPlaceTo;
				gamecam.transform.localPosition = Vector3.Lerp(gamecam.transform.localPosition, coverPos, 12*Time.deltaTime);
				gamecam.transform.localEulerAngles = Vector3.Lerp(gamecam.transform.localEulerAngles, coverRot, 12*Time.deltaTime);

//				if(!hasBeenInLookingAround)
//				{
//					Debug.Log ("MODIF EULER");
//					gamecam.transform.localEulerAngles = Vector3.Lerp(gamecam.transform.localEulerAngles, coverRot, camSwitchDamp*Time.deltaTime);
//				}
			}

			else
			{
				inCoverMode = true;
				inPositioningCoverModeCam = false;

				if(inLookAroundMode)
				{
					gamecam.transform.localPosition = Vector3.Lerp(gamecam.transform.localPosition, currentLookAroundPos, 6f*Time.deltaTime);
					gamecam.transform.localEulerAngles = Vector3.Lerp(gamecam.transform.localEulerAngles, currentLookAroundRot, 6f*Time.deltaTime);
				}

				Vector3 stickDirection = new Vector3 (joyX, 0, joyY).normalized;
				Vector3 axisSign = Vector3.Cross(this.transform.forward, stickDirection);
				
				float angleRootToMove = Vector3.Angle(transform.forward, stickDirection) * (axisSign.y < 0 ? -1f : 1f);

				direction = angleRootToMove * directionSpeed / 180f;
				
				charAngle = angleRootToMove;

				switch(currentCoverState)
				{
					case CoverState.onDownFace:
						if(joyY > -0.4f)
						{
							speed = Mathf.Abs (joyX);
							direction = joyX;
							charAngle = 0;
							transform.position += new Vector3(Time.deltaTime*direction*Mathf.Abs(direction)*COVER_SPEED,0, 0) ;
							transform.eulerAngles = new Vector3(0,180,0);
							if(transform.position.x < boundingBoxMinX)
							{
								Vector3 temp = transform.position;
								temp.x = boundingBoxMinX;
								transform.position = temp;

								if(direction <0)
								{
									currentLookAroundPos = lookAroundPosLeft;
									currentLookAroundRot = lookAroundRotLeft;
									inLookAroundMode = true;
									animator.SetBool(hashIdsScript.lookingAroundBool, true);
									hasBeenInLookingAround = true;
								}
							}
							else if(transform.position.x > boundingBoxMaxX)
							{
								Vector3 temp = transform.position;
								temp.x = boundingBoxMaxX;
								transform.position = temp;

								if(direction >0)
								{
									currentLookAroundPos = lookAroundPosRight;
									currentLookAroundRot = lookAroundRotRight;
									inLookAroundMode = true;
									animator.SetBool(hashIdsScript.lookingAroundBool, true);
									hasBeenInLookingAround = true;
								}
							}
							else 
							{
								if(inLookAroundMode)
								{
									inCoverMode = false;
									camSwitchDamp = 7f;
								}
								animator.SetBool(hashIdsScript.lookingAroundBool, false);
								inLookAroundMode = false;
							}
						}
						else 
						{
							currentCoverState = CoverState.nil;
							animator.SetBool(hashIdsScript.coverBool, false);
//							caps.radius = 0.4f;
						}
						break;

					case CoverState.OnLeftFace:
						if(joyY > -0.4f)
						{
							speed = Mathf.Abs (joyX);
							direction = joyX;
							charAngle = 0;
							transform.position += new Vector3(0, 0,-1*Time.deltaTime*direction*Mathf.Abs(direction)*COVER_SPEED);
							transform.eulerAngles = new Vector3(0,-90,0);
							if(transform.position.z < boundingBoxMinZ)
							{
								Vector3 temp = transform.position;
								temp.z = boundingBoxMinZ;
								transform.position = temp;

								if(direction >0)
								{
									currentLookAroundPos = lookAroundPosRight;
									currentLookAroundRot = lookAroundRotRight;
									inLookAroundMode = true;
									hasBeenInLookingAround = true;
									animator.SetBool(hashIdsScript.lookingAroundBool, true);
								}
							}
							else if(transform.position.z > boundingBoxMaxZ)
							{
								Vector3 temp = transform.position;
								temp.z = boundingBoxMaxZ;
								transform.position = temp;

								if(direction <0)
								{
									currentLookAroundPos = lookAroundPosLeft;
									currentLookAroundRot = lookAroundRotLeft;
									inLookAroundMode = true;
									hasBeenInLookingAround = true;
									animator.SetBool(hashIdsScript.lookingAroundBool, true);
								}
							}
							else 
							{
								if(inLookAroundMode)
								{
									inCoverMode = false;
									camSwitchDamp = 7f;
								}
								animator.SetBool(hashIdsScript.lookingAroundBool, false);
								inLookAroundMode = false;
							}

						}
						else 
						{
							currentCoverState = CoverState.nil;
							animator.SetBool(hashIdsScript.coverBool, false);
//							caps.radius = 0.4f;
						}
						break;

					case CoverState.OnRightFace:
						if(joyY > -0.4f)
						{
							speed = Mathf.Abs (joyX);
							direction = joyX;
							charAngle = 0;
							transform.position += new Vector3(0, 0,Time.deltaTime*direction*Mathf.Abs(direction)*COVER_SPEED);
							transform.eulerAngles = new Vector3(0,90,0);
							if(transform.position.z < boundingBoxMinZ)
							{
								Vector3 temp = transform.position;
								temp.z = boundingBoxMinZ;
								transform.position = temp;

								if(direction <0)
								{
									currentLookAroundPos = lookAroundPosLeft;
									currentLookAroundRot = lookAroundRotLeft;
									inLookAroundMode = true;
									hasBeenInLookingAround = true;
									animator.SetBool(hashIdsScript.lookingAroundBool, true);
								}
							}
							else if(transform.position.z > boundingBoxMaxZ)
							{
								Vector3 temp = transform.position;
								temp.z = boundingBoxMaxZ;
								transform.position = temp;

								if(direction >0)
								{
									currentLookAroundPos = lookAroundPosRight;
									currentLookAroundRot = lookAroundRotRight;
									inLookAroundMode = true;
									hasBeenInLookingAround = true;
									animator.SetBool(hashIdsScript.lookingAroundBool, true);
								}
							}
							else 
							{
								if(inLookAroundMode)
								{
									inCoverMode = false;
									camSwitchDamp = 7f;
								}
								animator.SetBool(hashIdsScript.lookingAroundBool, false);
								inLookAroundMode = false;
								
							}
						}
						else 
						{
							currentCoverState = CoverState.nil;
							animator.SetBool(hashIdsScript.coverBool, false);
//							caps.radius = 0.4f;
						}
						break;

					default:
						break;
				}
				animator.SetFloat(hashIdsScript.speedFloat, speed, speedDampTime, Time.deltaTime);
				animator.SetFloat(hashIdsScript.direction, direction, directionDampTime, Time.deltaTime);
			}
		}

	}

	void Rotating (float horizontal, float vertical)
	{
		// Create a new vector of the horizontal and vertical inputs.
		Vector3 targetDirection = new Vector3(horizontal, 0f, vertical);
		
		// Create a rotation based on this new vector assuming that up is the global y axis.
		Quaternion targetRotation = Quaternion.LookRotation(targetDirection, Vector3.up);
		
		// Create a rotation that is an increment closer to the target rotation from the player's rotation.
		Quaternion newRotation = Quaternion.Lerp(rigidbody.rotation, targetRotation, turnSmoothing * Time.deltaTime);
		
		// Change the players rotation to this new rotation.
		rigidbody.MoveRotation(newRotation);
	}
	
	/// Any code that moves the character needs to be checked against physics
	void FixedUpdate()
	{							
		// Rotate character model if stick is tilted right or left, but only if character is moving in that direction
		if (IsInLocomotion()&& ((direction >= 0 && joyX >= 0) || (direction < 0 && joyX < 0)))
		{
//			Debug.Log ("HERE");
			Vector3 rotationAmount = Vector3.Lerp(Vector3.zero, new Vector3(0f, rotationDegreePerSecond * (joyX < 0f ? -1f : 1f), 0f), Mathf.Abs(joyX));
			Quaternion deltaRotation = Quaternion.Euler(rotationAmount * Time.deltaTime);
        	this.transform.rotation = (this.transform.rotation * deltaRotation);
		}		


		AudioManagement ();
	}

	void AudioManagement ()
	{
		// If the player is currently in the run state...
		if(animator.GetCurrentAnimatorStateInfo(0).nameHash == hashIdsScript.m_LocomotionIdState)
		{
			// ... and if the footsteps are not playing...
			if(!audio.isPlaying)
				// ... play them.
				audio.Play();
		}
		else
			// Otherwise stop the footsteps.
			audio.Stop();
		
	}
	
	#endregion
	
	
	#region Methods


    public bool IsInLocomotion()
    {
		return stateInfo.nameHash == hashIdsScript.m_LocomotionIdState;
    }

	#endregion Methods
}
