using UnityEngine;
using System.Collections;

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
//	[SerializeField]
//	private float jumpMultiplier = 1f;
//	[SerializeField]
//	private CapsuleCollider capCollider;
//	[SerializeField]
//	private float jumpDist = 1f;
	
	
	// Private global only
	private float leftX = 0f;
	private float leftY = 0f;
	private AnimatorStateInfo stateInfo;
	private AnimatorTransitionInfo transInfo;
	private float speed = 0f;
	private float direction = 0f;
	private float charAngle = 0f;
	private const float SPRINT_SPEED = 2.0f;	
	private const float SPRINT_FOV = 75.0f;
	private const float NORMAL_FOV = 60.0f;
	private float capsuleHeight;

	private Vector3 cameraRotation;
	private Vector3 cameraPosition;

	private Vector3 radarCameraRotation;
	private Vector3 radarCameraPosition;

	private UIJoystick uiJoystickScript;
	

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
	}

	void LateUpdate()
	{
		//Reset Camera Rotation and position
		Vector3 rot = gamecam.transform.eulerAngles;
		rot = cameraRotation; 
		gamecam.transform.eulerAngles = rot;
		gamecam.transform.position = transform.position+cameraPosition;

		//Reset Radar Camera Rotation and position
		Vector3 rot2 = radarCam.transform.eulerAngles;
		rot2 = radarCameraRotation; 
		radarCam.transform.eulerAngles = rot2;
		radarCam.transform.position = transform.position+radarCameraPosition;
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
		float joyX = uiJoystickScript.position.x;
		float joyY = uiJoystickScript.position.y;

		Vector3 stickDirection = new Vector3 (joyX, 0, joyY);
		Vector3 axisSign = Vector3.Cross(this.transform.forward, stickDirection);
		
		float angleRootToMove = Vector3.Angle(transform.forward, stickDirection) * (axisSign.y < 0 ? -1f : 1f);
		
		charSpeed = stickDirection.magnitude;

		direction = angleRootToMove * directionSpeed / 180f;
		
		charAngle = angleRootToMove;

		// Press B to sprint
		if (Input.GetButton("Sprint"))
		{
			speed = Mathf.Lerp(speed, SPRINT_SPEED, Time.deltaTime);
//			mainCam.fieldOfView = Mathf.Lerp(mainCam.fieldOfView, SPRINT_FOV, fovDampTime * Time.deltaTime);
		}
		else
		{
			speed = charSpeed;
//			mainCam.fieldOfView = Mathf.Lerp(mainCam.fieldOfView, NORMAL_FOV, fovDampTime * Time.deltaTime);		
		}

		animator.SetFloat(hashIdsScript.speedFloat, speed, speedDampTime, Time.deltaTime);
		animator.SetFloat(hashIdsScript.direction, direction, directionDampTime, Time.deltaTime);
		
		if (speed > LocomotionThreshold)	// Dead zone
		{
			Animator.SetFloat(hashIdsScript.angle, charAngle);
		}

		if (speed < LocomotionThreshold && Mathf.Abs(leftX) < 0.05f)    // Dead zone
		{
			animator.SetFloat(hashIdsScript.direction, 0f);
			animator.SetFloat(hashIdsScript.angle, 0f);
		}

		animator.SetBool(hashIdsScript.sneakingBool, Input.GetButton("Sneak"));
		
	}
	
	/// Any code that moves the character needs to be checked against physics
	void FixedUpdate()
	{							
		// Rotate character model if stick is tilted right or left, but only if character is moving in that direction
		if (IsInLocomotion()&& ((direction >= 0 && leftX >= 0) || (direction < 0 && leftX < 0)))
		{
//			Debug.Log ("HERE");
			Vector3 rotationAmount = Vector3.Lerp(Vector3.zero, new Vector3(0f, rotationDegreePerSecond * (leftX < 0f ? -1f : 1f), 0f), Mathf.Abs(leftX));
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
