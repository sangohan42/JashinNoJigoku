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

	private Vector3 cameraRotation;	// The camera rotation in normal mode
	private Vector3 cameraPosition; // The camera position in normal mode
	public float smooth = 1.5f;		// The relative speed at which the camera will catch up.

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
	private Vector3 coverPosCopy;
	private Vector3 coverRotCopy;

	private Transform CameraInLookingAroundPosRight;
	private Vector3 lookAroundPosRight;
	private Vector3 lookAroundRotRight;
	private Vector3 lookAroundPosRightCopy;

	private Transform CameraInLookingAroundPosLeft;
	private Vector3 lookAroundPosLeft;
	private Vector3 lookAroundRotLeft;
	private Vector3 lookAroundPosLeftCopy;

	private Vector3 currentLookAroundPos;
	private Vector3 currentLookAroundRot;
	private bool inLookAroundMode;

	private Vector3 savedCamPosition;
	private Quaternion savedCamRotation;
	
	private bool inCoverMode;
	private bool inPositioningCoverModeCam;
	private bool playerPlaced;
	private bool inModifyCoverPos;
	private float currentModifToCoverPos;

	private float boundingBoxMinX;
	private float boundingBoxMaxX;
	private float boundingBoxMinZ;
	private float boundingBoxMaxZ;

	private float camSwitchDamp;

	private bool hasBeenInLookingAround;
	private bool hasBeenInCover;
	
	private bool isPursued;
	private bool isInPanoramicView;
	public float maxAngleInPanoramicView = 120;
	private Vector3 camPanoramicPosition;
	private Vector3 camPanoramicRotation;
	private float currentModifToPanoramicRotVertical;
	
	private Vector3 camPositionWhenCloseToBorder;
	private Vector3 camRotationWhenCloseToBorder;
	private bool isPlayerCloseToBorder;

	public float levelMaxX = 20;
	public float levelMinX = 4;

	private int buildingMask;
	private int NPCMask;

	
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

	public Vector3 CameraPosition
	{
		get
		{
			return this.cameraPosition;
		}
		set
		{
			this.cameraPosition = value;
		}
	}

	public Vector3 CameraRotation
	{
		get
		{
			return this.cameraRotation;
		}
		set
		{
			this.cameraRotation = value;
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
		coverPosCopy = coverPos;
		coverRotCopy = coverRot;

		CameraInLookingAroundPosRight = GameObject.Find ("CameraInLookAroundRight").transform;
		lookAroundPosRight = CameraInLookingAroundPosRight.localPosition;
		lookAroundRotRight = CameraInLookingAroundPosRight.localEulerAngles;
		lookAroundPosRightCopy = lookAroundPosRight;

		CameraInLookingAroundPosLeft = GameObject.Find ("CameraInLookAroundLeft").transform;
		lookAroundPosLeft = CameraInLookingAroundPosLeft.localPosition;
		lookAroundRotLeft = CameraInLookingAroundPosLeft.localEulerAngles;
		lookAroundPosLeftCopy = lookAroundPosLeft;
		currentModifToPanoramicRotVertical = 0;

		inCoverMode = false;
		inPositioningCoverModeCam = false;
		playerPlaced = false;
		inLookAroundMode = false;
		hasBeenInLookingAround = false;
		hasBeenInCover = false;
		inModifyCoverPos = false;
		currentModifToCoverPos = 0;

		camSwitchDamp = 12f;
		isPursued = false;
		isInPanoramicView = false;
		camPanoramicPosition = GameObject.Find ("CameraPanoramic").transform.position;
		camPanoramicRotation = Vector3.zero;

		camRotationWhenCloseToBorder = new Vector3 (32, 0, 0);

		isPlayerCloseToBorder = false;

		buildingMask = 11;

		NPCMask = 1 << buildingMask;

	}

	private void CompensateForWalls(Vector3 fromObject, ref Vector3 toTarget)
	{
		Vector3 globalTargetPos = fromObject + Vector3.up + transform.forward *toTarget.z;
//		Debug.Log ("Global Target Pos = " + globalTargetPos);
		// Compensate for walls between camera
		RaycastHit wallHit = new RaycastHit();	
//		Debug.DrawRay(fromObject + Vector3.up, globalTargetPos, Color.red);
//		Debug.DrawRay(fromObject + Vector3.up, fromObject + Vector3.up + transform.forward, Color.cyan);
//
//		Debug.Break ();
		if (Physics.Linecast(fromObject, globalTargetPos, out wallHit, NPCMask)) 
		{
//			Debug.Log("wallHit.point = " + wallHit.point);
//			Debug.DrawRay(wallHit.point, wallHit.normal, Color.gray);
			if(currentCoverState == CoverState.OnLeftFace || currentCoverState == CoverState.OnRightFace)
			{
				float modifValue = Mathf.Abs(wallHit.point.x - fromObject.x);
				toTarget = new Vector3(toTarget.x, toTarget.y -0.5f, modifValue);
				lookAroundPosRight.z = modifValue;
				lookAroundPosLeft.z = modifValue;

			}
			else 
			{
				float modifValue = Mathf.Abs(wallHit.point.z - fromObject.z);
				toTarget = new Vector3(toTarget.x, toTarget.y -0.5f, modifValue);
				lookAroundPosRight.z = modifValue;
				lookAroundPosLeft.z = modifValue;
			}
			lookAroundPosRight.y -= 0.5f;
			lookAroundPosLeft.y -= 0.5f;
			
		}
	}
	
	void LateUpdate()
	{

		//Reset Radar Camera Rotation and position
		Vector3 rot2 = radarCam.transform.eulerAngles;
		rot2 = radarCameraRotation; 
		radarCam.transform.eulerAngles = rot2;
		radarCam.transform.position = transform.position+radarCameraPosition;

		if(!isInPanoramicView)
		{
			//Reset Camera position and rotation if we are not in cover 
			if(currentCoverState == CoverState.nil)
			{
				//If we have been in cover(the camera was attached to the player so we detach it
				if(hasBeenInCover)gamecam.transform.parent = null;

				// The position to reach
				Vector3 standardPos = transform.position + cameraPosition;

					//We are not close to the border of the level
				if(transform.position.x < levelMaxX && transform.position.x > levelMinX)
				{
					if(isPlayerCloseToBorder)
					{
						isPlayerCloseToBorder = false;
					}

					//Reset rotation
					Vector3 rot = gamecam.transform.eulerAngles;
					rot = cameraRotation;

					if(!hasBeenInCover)
					{
						// Lerp the camera's position between it's current position and it's new position.
						gamecam.transform.position = Vector3.Lerp(gamecam.transform.position, standardPos, smooth * Time.deltaTime);
						gamecam.transform.eulerAngles = Vector3.Lerp(gamecam.transform.eulerAngles,rot, smooth * Time.deltaTime);
					}

					//The transition from cover to normal mode had to be abrupt
					else 
					{
						gamecam.transform.position = standardPos;
						gamecam.transform.eulerAngles = rot;
					}

				}

				//We are close to the border of the level
				else
				{

					if(!isPlayerCloseToBorder)
					{
						isPlayerCloseToBorder = true;
	//					camPositionWhenCloseToBorder = gamecam.transform.position;
					}

					standardPos.x = (transform.position.x > levelMaxX) ? levelMaxX : levelMinX;
					gamecam.transform.position = Vector3.Lerp(gamecam.transform.position, standardPos, smooth * Time.deltaTime);

					gamecam.transform.eulerAngles = Vector3.Lerp(gamecam.transform.eulerAngles,camRotationWhenCloseToBorder, smooth * Time.deltaTime);

				}

				hasBeenInCover = false;

				//If we still are in idle (not in pivot, not in locomotion, not in Sneak)
				if(stateInfo.nameHash == hashIdsScript.m_IdleState || stateInfo.nameHash == hashIdsScript.m_sneakingState 
				   || stateInfo.nameHash == hashIdsScript.m_LocomotionIdState)
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

		if (Input.GetButtonDown("PanoramicView") && joyX == 0 && joyY == 0)
		{
//			Debug.Log ("TOUCH");
			if(!isInPanoramicView)
			{
				isInPanoramicView = true;
				gamecam.transform.parent = transform;
				gamecam.transform.localPosition = camPanoramicPosition;
				gamecam.transform.localEulerAngles = camPanoramicRotation;
			}
			else 
			{
				isInPanoramicView = false;
				gamecam.transform.parent = null;
				currentModifToPanoramicRotVertical = 0;
			}

		}

		if(!isInPanoramicView)
		{

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

//				if (charSpeed>=1f)
//				{
//					speed = Mathf.Lerp(speed, SPRINT_SPEED, Time.deltaTime);
//				}
//				else speed = charSpeed;

				speed = charSpeed;

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

			}

			//IN COVER
			else
			{
				//We place the player close to the wall while we are in transition
				if(!playerPlaced)
				{
					hasBeenInCover = true;
					gamecam.transform.parent = transform;

					animator.SetFloat(hashIdsScript.speedFloat, 0);
					animator.SetFloat(hashIdsScript.direction, 0);

//					switch(currentCoverState)
//					{
//					case CoverState.onDownFace:
//
//						transform.eulerAngles = new Vector3(0,180,0);
//						break;
//					case CoverState.OnLeftFace:
//						transform.eulerAngles = new Vector3(0,-90,0);
//						break;
//					case CoverState.OnRightFace:
//						transform.eulerAngles = new Vector3(0,90,0);
//						break;
//					}

					transform.forward = vecToAlignTo;
					transform.position = positionToPlaceTo;
					playerPlaced = true;

					//We verify that the coverPos is OK or if there is a wall between the player and the camera
					//If there is a wall we change the coverPos as well as the lookingAroundPos
					CompensateForWalls(transform.position, ref coverPos);

				}
				//We place the camera to the right position
				else if(gamecam.transform.localPosition != coverPos && !inLookAroundMode)
				{
					animator.SetFloat(hashIdsScript.speedFloat, 0);
					animator.SetFloat(hashIdsScript.direction, 0);

					inPositioningCoverModeCam = true;
//					switch(currentCoverState)
//					{
//					case CoverState.onDownFace:
//						transform.eulerAngles = new Vector3(0,180,0);
//						break;
//					case CoverState.OnLeftFace:
//						transform.eulerAngles = new Vector3(0,-90,0);
//						break;
//					case CoverState.OnRightFace:
//						transform.eulerAngles = new Vector3(0,90,0);
//						break;
//					}		

					transform.forward = vecToAlignTo;
					transform.position = positionToPlaceTo;
					gamecam.transform.localPosition = Vector3.Lerp(gamecam.transform.localPosition, coverPos, 15*Time.deltaTime);
					gamecam.transform.localEulerAngles = Vector3.Lerp(gamecam.transform.localEulerAngles, coverRot, 15*Time.deltaTime);
				}
				else
				{
					inCoverMode = true;
					inPositioningCoverModeCam = false;
					transform.forward = vecToAlignTo;
					transform.position = positionToPlaceTo;
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
//								transform.position += new Vector3(Time.deltaTime*direction*Mathf.Abs(direction)*COVER_SPEED,0, 0) ;
//								transform.position += -1 *transform.right * Time.deltaTime*direction*Mathf.Abs(direction)*COVER_SPEED;
//								transform.eulerAngles = new Vector3(0,180,0);
								if(transform.position.x <= boundingBoxMinX)
								{
									Vector3 temp = transform.position;
									temp.x = boundingBoxMinX;
									transform.position = temp;

									if(direction <0 && !inLookAroundMode)
									{
										currentLookAroundPos = lookAroundPosLeft;
										currentLookAroundRot = lookAroundRotLeft;
										inLookAroundMode = true;
										animator.SetBool(hashIdsScript.lookingAroundBool, true);
										hasBeenInLookingAround = true;
									}

									else if(direction >=0)
									{
										if(inLookAroundMode)
										{
											inCoverMode = false;
											camSwitchDamp = 7f;
											animator.SetBool(hashIdsScript.lookingAroundBool, false);
											inLookAroundMode = false;
										}
										transform.position += -1 *transform.right * Time.deltaTime*direction*Mathf.Abs(direction)*COVER_SPEED;
									}
								}
								else if(transform.position.x >= boundingBoxMaxX)
								{
									Vector3 temp = transform.position;
									temp.x = boundingBoxMaxX;
									transform.position = temp;

									if(direction >0 && !inLookAroundMode)
									{
										currentLookAroundPos = lookAroundPosRight;
										currentLookAroundRot = lookAroundRotRight;
										inLookAroundMode = true;
										animator.SetBool(hashIdsScript.lookingAroundBool, true);
										hasBeenInLookingAround = true;
									}

									else if(direction <=0)
									{
										if(inLookAroundMode)
										{
											inCoverMode = false;
											camSwitchDamp = 7f;
											animator.SetBool(hashIdsScript.lookingAroundBool, false);
											inLookAroundMode = false;
										}
										transform.position += -1 *transform.right * Time.deltaTime*direction*Mathf.Abs(direction)*COVER_SPEED;
									}

								}
								else 
								{
									transform.position += -1 *transform.right * Time.deltaTime*direction*Mathf.Abs(direction)*COVER_SPEED;

									if(inLookAroundMode)
									{
										inCoverMode = false;
										camSwitchDamp = 7f;
										animator.SetBool(hashIdsScript.lookingAroundBool, false);
										inLookAroundMode = false;
									}
									
								}
							}
							else 
							{
								currentCoverState = CoverState.nil;
								animator.SetBool(hashIdsScript.coverBool, false);
								animator.SetBool(hashIdsScript.crouchCoverBool, false);

								coverPos = coverPosCopy;
								coverRot = coverRotCopy;
								lookAroundPosRight = lookAroundPosRightCopy;
								lookAroundPosLeft = lookAroundPosLeftCopy;
								currentModifToCoverPos = 0;
								caps.center = new Vector3(0,1.03f,0);
								caps.height = 2.07f;
							}
							break;

						case CoverState.OnLeftFace:
							if(joyY > -0.4f)
							{
								speed = Mathf.Abs (joyX);
								direction = joyX;
								charAngle = 0;
//								transform.position += new Vector3(0, 0,-1*Time.deltaTime*direction*Mathf.Abs(direction)*COVER_SPEED);
//								transform.position += -1 *transform.right * Time.deltaTime*direction*Mathf.Abs(direction)*COVER_SPEED;

//								transform.eulerAngles = new Vector3(0,-90,0);
								if(transform.position.z <= boundingBoxMinZ)
								{
									Vector3 temp = transform.position;
									temp.z = boundingBoxMinZ;
									transform.position = temp;

									if(direction >0 && !inLookAroundMode)
									{
										currentLookAroundPos = lookAroundPosRight;
										currentLookAroundRot = lookAroundRotRight;
										inLookAroundMode = true;
										hasBeenInLookingAround = true;
										animator.SetBool(hashIdsScript.lookingAroundBool, true);
									}

									else if(direction <=0)
									{
										if(inLookAroundMode)
										{
											inCoverMode = false;
											camSwitchDamp = 7f;
											animator.SetBool(hashIdsScript.lookingAroundBool, false);
											inLookAroundMode = false;
										}
										transform.position += -1 *transform.right * Time.deltaTime*direction*Mathf.Abs(direction)*COVER_SPEED;
									}
								}
								else if(transform.position.z >= boundingBoxMaxZ)
								{
									Vector3 temp = transform.position;
									temp.z = boundingBoxMaxZ;
									transform.position = temp;

									if(direction <0 && !inLookAroundMode)
									{
										currentLookAroundPos = lookAroundPosLeft;
										currentLookAroundRot = lookAroundRotLeft;
										inLookAroundMode = true;
										hasBeenInLookingAround = true;
										animator.SetBool(hashIdsScript.lookingAroundBool, true);
									}

									else if(direction >=0)
									{
										if(inLookAroundMode)
										{
											inCoverMode = false;
											camSwitchDamp = 7f;
											animator.SetBool(hashIdsScript.lookingAroundBool, false);
											inLookAroundMode = false;
										}
										transform.position += -1 *transform.right * Time.deltaTime*direction*Mathf.Abs(direction)*COVER_SPEED;
									}
								}
								else 
								{
									transform.position += -1 *transform.right * Time.deltaTime*direction*Mathf.Abs(direction)*COVER_SPEED;

									if(inLookAroundMode)
									{
										inCoverMode = false;
										camSwitchDamp = 7f;
										animator.SetBool(hashIdsScript.lookingAroundBool, false);
										inLookAroundMode = false;
									}
									
								}

							}
							else 
							{
								currentCoverState = CoverState.nil;
								animator.SetBool(hashIdsScript.coverBool, false);
								animator.SetBool(hashIdsScript.crouchCoverBool, false);

								coverPos = coverPosCopy;
								coverRot = coverRotCopy;
								lookAroundPosRight = lookAroundPosRightCopy;
								lookAroundPosLeft = lookAroundPosLeftCopy;
								currentModifToCoverPos = 0;
								caps.center = new Vector3(0,1.03f,0);
								caps.height = 2.07f;
							}
							break;

						case CoverState.OnRightFace:
							if(joyY > -0.4f)
							{
								speed = Mathf.Abs (joyX);
								direction = joyX;
								charAngle = 0;
//								transform.position += new Vector3(0, 0,Time.deltaTime*direction*Mathf.Abs(direction)*COVER_SPEED);
//								transform.position += -1 *transform.right * Time.deltaTime*direction*Mathf.Abs(direction)*COVER_SPEED;

//								transform.eulerAngles = new Vector3(0,90,0);
								if(transform.position.z <= boundingBoxMinZ)
								{
									Vector3 temp = transform.position;
									temp.z = boundingBoxMinZ;
									transform.position = temp;

									if(direction <0 && !inLookAroundMode)
									{
										currentLookAroundPos = lookAroundPosLeft;
										currentLookAroundRot = lookAroundRotLeft;
										inLookAroundMode = true;
										hasBeenInLookingAround = true;
										animator.SetBool(hashIdsScript.lookingAroundBool, true);
									}

									else if(direction >=0)
									{
										if(inLookAroundMode)
										{
											inCoverMode = false;
											camSwitchDamp = 7f;
											animator.SetBool(hashIdsScript.lookingAroundBool, false);
											inLookAroundMode = false;
										}
										transform.position += -1 *transform.right * Time.deltaTime*direction*Mathf.Abs(direction)*COVER_SPEED;
									}
								}
								else if(transform.position.z >= boundingBoxMaxZ)
								{
									Vector3 temp = transform.position;
									temp.z = boundingBoxMaxZ;
									transform.position = temp;

									if(direction >0 &&!inLookAroundMode)
									{
										currentLookAroundPos = lookAroundPosRight;
										currentLookAroundRot = lookAroundRotRight;
										inLookAroundMode = true;
										hasBeenInLookingAround = true;
										animator.SetBool(hashIdsScript.lookingAroundBool, true);
									}

									else if(direction <=0)
									{
										if(inLookAroundMode)
										{
											inCoverMode = false;
											camSwitchDamp = 7f;
											animator.SetBool(hashIdsScript.lookingAroundBool, false);
											inLookAroundMode = false;
										}
										transform.position += -1 *transform.right * Time.deltaTime*direction*Mathf.Abs(direction)*COVER_SPEED;
									}
								}
								else 
								{
									transform.position += -1 *transform.right * Time.deltaTime*direction*Mathf.Abs(direction)*COVER_SPEED;

									if(inLookAroundMode)
									{
										inCoverMode = false;
										camSwitchDamp = 7f;
										animator.SetBool(hashIdsScript.lookingAroundBool, false);
										inLookAroundMode = false;
									}
									

								}
							}
							else 
							{
								currentCoverState = CoverState.nil;
								animator.SetBool(hashIdsScript.coverBool, false);
								animator.SetBool(hashIdsScript.crouchCoverBool, false);

								coverPos = coverPosCopy;
								coverRot = coverRotCopy;
								lookAroundPosRight = lookAroundPosRightCopy;
								lookAroundPosLeft = lookAroundPosLeftCopy;
								currentModifToCoverPos = 0;
								caps.center = new Vector3(0,1.03f,0);
								caps.height = 2.07f;
							}
							break;

						default:
							break;
					}
					positionToPlaceTo = transform.position;

					animator.SetFloat(hashIdsScript.speedFloat, speed, speedDampTime, Time.deltaTime);
					animator.SetFloat(hashIdsScript.direction, direction, directionDampTime, Time.deltaTime);
				}
			}
		}

		//In Panoramic view
		else
		{
			RotateInPanoramic(joyX, joyY);
		}

	}

	void RotateInPanoramic(float horizontal, float vertical)
	{
		float nextValX = currentModifToPanoramicRotVertical + vertical;

		//VERTICAL MOVEMENT
		if(Mathf.Abs(nextValX) <60)
		{
			currentModifToPanoramicRotVertical = nextValX; 
			Vector3 copy = gamecam.transform.localEulerAngles;
			copy.x -= vertical;
			gamecam.transform.localEulerAngles = copy;

//			gamecam.transform.localRotation = Quaternion.AngleAxis(vertical, -1*this.transform.right) * gamecam.transform.localRotation;
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

	public bool IsInLocomotion()
	{
		return stateInfo.nameHash == hashIdsScript.m_LocomotionIdState;
	}

	// Subscribe to events
	void OnEnable(){
		
		EasyTouch.On_TouchStart += HandleOn_TouchStart;
		EasyTouch.On_TouchDown += HandleOn_TouchDown;
		EasyTouch.On_TouchUp += HandleOn_TouchUp;
		EasyTouch.On_DoubleTap += HandleOn_DoubleTap;
		
	}

	void OnDestroy()
	{
		EasyTouch.On_TouchStart -= HandleOn_TouchStart;
		EasyTouch.On_TouchDown -= HandleOn_TouchDown;
		EasyTouch.On_TouchUp -= HandleOn_TouchUp;
		EasyTouch.On_DoubleTap -= HandleOn_DoubleTap;
	}

	void HandleOn_DoubleTap (Gesture gesture)
	{
		if(joyX == 0 && joyY == 0 && currentCoverState == CoverState.nil)
		{
			if(!isInPanoramicView)
			{
				isInPanoramicView = true;
				gamecam.transform.parent = transform;
				gamecam.transform.localPosition = camPanoramicPosition;
				gamecam.transform.localEulerAngles = camPanoramicRotation;
			}
			else 
			{
				isInPanoramicView = false;
				gamecam.transform.parent = null;
				currentModifToPanoramicRotVertical = 0;
				gamecam.transform.position = transform.position + cameraPosition;
				gamecam.transform.eulerAngles = cameraRotation;
			}
		}
	}
	
	void HandleOn_TouchStart (Gesture gesture)
	{
		if (currentCoverState != CoverState.nil && !inLookAroundMode)
		{
			inModifyCoverPos = true;
		}
	}
	
	void HandleOn_TouchDown (Gesture gesture)
	{
		if(inModifyCoverPos && currentCoverState != CoverState.nil)
		{
			float nextVal = currentModifToCoverPos + gesture.deltaPosition.x;
//			Debug.Log ("currentModifToCoverPos = " + currentModifToCoverPos);
			if(Mathf.Abs(nextVal) <90)
			{
				currentModifToCoverPos = nextVal; 
				coverPos = Quaternion.AngleAxis(gesture.deltaPosition.x, Vector3.up) * coverPos;
				coverRot.y += gesture.deltaPosition.x;
			}
		}
	}
	
	void HandleOn_TouchUp (Gesture gesture)
	{
		if (inModifyCoverPos)
		{
			inModifyCoverPos = false;

		}

	}
	
	#endregion
	
}
