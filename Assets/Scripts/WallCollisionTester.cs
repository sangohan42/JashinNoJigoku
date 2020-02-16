using UnityEngine;

[RequireComponent(typeof(Collider))]
public class WallCollisionTester : MonoBehaviour
{
    private enum Face { LEFT, RIGHT, DOWN };

    private AnimatorHashIds _hash;
	private Animator _playerAnimator;
	private GameObject _player;
	private CharacterControllerLogic _characterControllerLogicScript;
	private EnemyStatusChecker _enemyStatusChecker;
	private GameObject _gameCam;
	private CapsuleCollider _caps;
	private Vector3 _size;
	private bool _isAWall;

    private float _elapsedTimeSinceCollisionDetected;

	private Vector3 _currentCollidedSurfaceNormalVector;
    private Collider _wallCollider;

    private const float COLLISION_DURATION_BEFORE_STARTING_COVER = 0.5f;

    void Awake()
	{
		_hash = GameObject.FindGameObjectWithTag (Tags.animatorHashController).GetComponent<AnimatorHashIds> ();
		_player = GameObject.FindGameObjectWithTag (Tags.player);
		_playerAnimator = _player.GetComponent<Animator> ();
		_gameCam = GameObject.FindGameObjectWithTag (Tags.camera);
		_characterControllerLogicScript = _player.GetComponent<CharacterControllerLogic> ();
		_enemyStatusChecker = _player.GetComponent<EnemyStatusChecker> ();
		_caps = _player.GetComponent<CapsuleCollider> ();
        _elapsedTimeSinceCollisionDetected = 0;
		_currentCollidedSurfaceNormalVector = Vector3.zero;
		_isAWall = !CompareTag (Tags.box);
	}

	void Start()
    {
        _wallCollider = GetComponent<Collider>();

        if (_wallCollider is MeshCollider)
		{
            Bounds colliderBounds = _wallCollider.bounds;
            _size = new Vector3 (colliderBounds.max.x - colliderBounds.min.x, colliderBounds.max.y - colliderBounds.min.y, colliderBounds.max.z - colliderBounds.min.z);
        }
        else if (_wallCollider is BoxCollider)
		{
            _size = ((BoxCollider)_wallCollider).size;
		}
        else
        {
            Debug.LogError(("This script should be placed on items using Mesh/Box Collider"));
        }
    }

	void OnCollisionStay(Collision collision)
    {
		if(collision.gameObject.CompareTag(Tags.player) && _characterControllerLogicScript.CurrentCoverState == CoverState.None && _enemyStatusChecker.isNotSeen())
		{
			ContactPoint contact = collision.contacts[0];

			RaycastHit hit;
			Vector3 startPos = _player.transform.position + _caps.center.y*transform.up;

			if(Physics.Raycast(startPos, contact.point -startPos, out hit, 1f))
			{
				if(_currentCollidedSurfaceNormalVector != -1*hit.normal)
				{
                    _elapsedTimeSinceCollisionDetected = 0;
					_currentCollidedSurfaceNormalVector = -1*hit.normal;
				}
			}

			//We have the right orientation
			if(Vector3.Dot(collision.gameObject.transform.forward,_currentCollidedSurfaceNormalVector) > 0.75f && collision.relativeVelocity.magnitude >1)
			{
                _elapsedTimeSinceCollisionDetected += Time.deltaTime;


                //If we were COLLISION_DURATION_BEFORE_STARTING_COVER second in the good orientation
                if (_elapsedTimeSinceCollisionDetected > COLLISION_DURATION_BEFORE_STARTING_COVER)
				{
                    //DOWN FACE
					if(Vector3.Dot(_currentCollidedSurfaceNormalVector, Vector3.forward) > 0.707106)
					{
						Debug.Log ("DOWN FACE");
						_player.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionZ;

						_characterControllerLogicScript.SavedCamPosition = _gameCam.transform.position;
						_characterControllerLogicScript.SavedCamRotation = _gameCam.transform.rotation;

						if(_isAWall)
                        {
							_playerAnimator.SetBool(_hash.CoverBool, true);
							_characterControllerLogicScript.PositionToPlaceTo = new Vector3(contact.point.x, _player.transform.position.y, contact.point.z -0.24f);
						}
                        else
                        {
							_playerAnimator.SetBool(_hash.CrouchCoverBool, true);
							_characterControllerLogicScript.PositionToPlaceTo = new Vector3(contact.point.x, _player.transform.position.y, contact.point.z -0.42f);
							_characterControllerLogicScript.InCrouchCoverMode = true;
							_characterControllerLogicScript.CoverPos += new Vector3(0, -0.9f,0);
						}
						_characterControllerLogicScript.VecToAlignTo = -1*_currentCollidedSurfaceNormalVector;
						_characterControllerLogicScript.CurrentCoverState = CoverState.OnDownFace;
						CalculateBounds(Face.DOWN, -1*_currentCollidedSurfaceNormalVector);
					}

					//LEFT FACE
					else if(Vector3.Dot(_currentCollidedSurfaceNormalVector, -1*Vector3.left) > 0.707106)
					{
						Debug.Log ("LEFT FACE");
						_player.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionX;

						_characterControllerLogicScript.SavedCamPosition = _gameCam.transform.position;
						_characterControllerLogicScript.SavedCamRotation = _gameCam.transform.rotation;

						if(_isAWall){
							_playerAnimator.SetBool(_hash.CoverBool, true);
							_characterControllerLogicScript.PositionToPlaceTo = new Vector3(contact.point.x - 0.24f, _player.transform.position.y, contact.point.z);

						}

						else{
							_characterControllerLogicScript.PositionToPlaceTo = new Vector3(contact.point.x - 0.42f, _player.transform.position.y, contact.point.z);
							_playerAnimator.SetBool(_hash.CrouchCoverBool, true);
							_characterControllerLogicScript.InCrouchCoverMode = true;
							_characterControllerLogicScript.CoverPos += new Vector3(0, -0.9f,0);
						}
						_characterControllerLogicScript.VecToAlignTo = -1*_currentCollidedSurfaceNormalVector;
						_characterControllerLogicScript.CurrentCoverState = CoverState.OnLeftFace;
						CalculateBounds(Face.LEFT, -1*_currentCollidedSurfaceNormalVector);

					}
					
					//RIGHT FACE
					else if(Vector3.Dot(_currentCollidedSurfaceNormalVector, Vector3.left) > 0.707106)
					{
						Debug.Log ("RIGHT FACE");
						_player.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionX;

						_characterControllerLogicScript.SavedCamPosition = _gameCam.transform.position;
						_characterControllerLogicScript.SavedCamRotation = _gameCam.transform.rotation;

						if(_isAWall){
							_playerAnimator.SetBool(_hash.CoverBool, true);
							_characterControllerLogicScript.PositionToPlaceTo = new Vector3(contact.point.x + 0.24f, _player.transform.position.y, contact.point.z);

						}

						else{
							_playerAnimator.SetBool(_hash.CrouchCoverBool, true);
							_characterControllerLogicScript.PositionToPlaceTo = new Vector3(contact.point.x + 0.42f, _player.transform.position.y, contact.point.z);
							_characterControllerLogicScript.InCrouchCoverMode = true;
							_characterControllerLogicScript.CoverPos += new Vector3(0, -0.9f,0);
						}

						_characterControllerLogicScript.VecToAlignTo = -1*_currentCollidedSurfaceNormalVector;
						_characterControllerLogicScript.CurrentCoverState = CoverState.OnRightFace;
						CalculateBounds(Face.RIGHT, -1*_currentCollidedSurfaceNormalVector);
                    }
				}
			}
		}
	}

	void OnCollisionExit(Collision collision) 
	{
        _elapsedTimeSinceCollisionDetected = 0;
		_currentCollidedSurfaceNormalVector = Vector3.zero;
	}

    void CalculateBounds(Face currFace, Vector3 normal)
	{
		//We get the angle to first know how the object is rotated;
		float angle = transform.localEulerAngles.y;
		if(angle >180)
            angle -= 360f;

		switch(currFace)
		{
		case Face.DOWN:
			//If ANGLE < 0 WE NEED TO CHECK MIN X
			//If ANGLE > 0 WE NEED TO CHECK MAX X
			if(angle <0)
			{
				float val = Mathf.Sin((-1*angle) * Mathf.PI / 180f) * _size.z;

				_characterControllerLogicScript.CoverModeColliderBoundingBoxMinX = _wallCollider.bounds.min.x + val + 0.17f;
				_characterControllerLogicScript.CoverModeColliderBoundingBoxMaxX = _wallCollider.bounds.max.x - 0.17f;
			}
			else
			{
				float val = Mathf.Sin((angle) * Mathf.PI / 180f)* _size.z;
				_characterControllerLogicScript.CoverModeColliderBoundingBoxMinX = _wallCollider.bounds.min.x + 0.17f;
				_characterControllerLogicScript.CoverModeColliderBoundingBoxMaxX = _wallCollider.bounds.max.x - val - 0.17f;
			}

			break;
		case Face.LEFT:
			//If ANGLE < 0 WE NEED TO CHECK MAX Z
			//If ANGLE > 0 WE NEED TO CHECK MIN Z
			if(angle <0)
			{
				float val = Mathf.Sin((-1*angle) * Mathf.PI / 180f) * _size.x;

				_characterControllerLogicScript.CoverModeColliderBoundingBoxMinZ = _wallCollider.bounds.min.z + 0.17f;
				_characterControllerLogicScript.CoverModeColliderBoundingBoxMaxZ = _wallCollider.bounds.max.z - val - 0.17f;
			}
			else
			{
				float val = Mathf.Sin((angle) * Mathf.PI / 180f) * _size.x;
				
				_characterControllerLogicScript.CoverModeColliderBoundingBoxMinZ = _wallCollider.bounds.min.z + val+ 0.17f;
				_characterControllerLogicScript.CoverModeColliderBoundingBoxMaxZ = _wallCollider.bounds.max.z - 0.17f;
			}

			break;
		case Face.RIGHT:
			//If ANGLE < 0 WE NEED TO CHECK MIN Z
			//If ANGLE > 0 WE NEED TO CHECK MAX Z
			if(angle <0)
			{
				float val = Mathf.Sin((-1*angle) * Mathf.PI / 180f) * _size.x;
				
				_characterControllerLogicScript.CoverModeColliderBoundingBoxMinZ = _wallCollider.bounds.min.z + val + 0.17f;
				_characterControllerLogicScript.CoverModeColliderBoundingBoxMaxZ = _wallCollider.bounds.max.z - 0.17f;
			}
			else
			{
				float val = Mathf.Sin((angle) * Mathf.PI / 180f) * _size.x;
				
				_characterControllerLogicScript.CoverModeColliderBoundingBoxMinZ = _wallCollider.bounds.min.z + 0.17f;
				_characterControllerLogicScript.CoverModeColliderBoundingBoxMaxZ = _wallCollider.bounds.max.z - val - 0.17f;
			}

			break;
		}
    }
}
