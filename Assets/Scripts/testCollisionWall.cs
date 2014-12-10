using UnityEngine;
using System.Collections;

public class testCollisionWall : MonoBehaviour {

	private HashIds hash;
	private Animator playerAnimator;
	private GameObject player;
	private CharacterControllerLogic characterControllerLogicScript;
	private GameObject gameCam;
	private CapsuleCollider caps;
	public bool isAWall = true;

	private enum Face{LEFT, RIGHT, DOWN};
	private Face currFace;
	private BoxCollider currCollider;
	void Awake()
	{
		hash = GameObject.FindGameObjectWithTag (DoneTags.gameController).GetComponent<HashIds> ();
		player = GameObject.FindGameObjectWithTag (DoneTags.player);
		playerAnimator = player.GetComponent<Animator> ();
		gameCam = GameObject.FindGameObjectWithTag (DoneTags.camera);
		characterControllerLogicScript = GameObject.FindGameObjectWithTag (DoneTags.player).GetComponent<CharacterControllerLogic> ();
		caps = player.GetComponent<CapsuleCollider> ();
		currCollider = (BoxCollider)collider;
	}
	
	void OnCollisionEnter(Collision collision) {

		//If the player collide and was in Locomotion
		if(collision.gameObject.CompareTag(DoneTags.player) && 
		   (playerAnimator.GetCurrentAnimatorStateInfo(0).nameHash == hash.m_LocomotionIdState || 
			 playerAnimator.GetCurrentAnimatorStateInfo(0).nameHash == hash.m_sneakingState) &&
		        !characterControllerLogicScript.IsPursued)
		{
			ContactPoint contact = collision.contacts[0];
			if (collision.relativeVelocity.magnitude > 1)
			{

				RaycastHit hit;
				Vector3 normalVector = Vector3.zero;
				if(Physics.Raycast(player.transform.position + caps.center.y*transform.up, player.transform.forward, out hit, 1f))
				{
					normalVector = hit.normal;
					Debug.Log ("Vector3.Dot(normalVector, Vector3.forward) = " + Vector3.Dot(normalVector, Vector3.forward));
				}

				if(Vector3.Dot(collision.gameObject.transform.forward,-1*normalVector) > 0.6f)
				{

					//DOWN FACE
					if(Vector3.Dot(normalVector, Vector3.forward) < -0.707f)
					{
						Debug.Log ("DOWN FACE");
						currFace = Face.DOWN;
						characterControllerLogicScript.SavedCamPosition = gameCam.transform.position;
						characterControllerLogicScript.SavedCamRotation = gameCam.transform.rotation;
						if(isAWall)
						{
							playerAnimator.SetBool(hash.coverBool, true);
							characterControllerLogicScript.PositionToPlaceTo = new Vector3(contact.point.x, player.transform.position.y, contact.point.z -0.2f);
						}
						else 
						{
							playerAnimator.SetBool(hash.crouchCoverBool, true);
							characterControllerLogicScript.PositionToPlaceTo = new Vector3(contact.point.x, player.transform.position.y, contact.point.z -0.42f);
							caps.center = new Vector3(0,0.5f,0);
							caps.height = 0.9f;
						}
						characterControllerLogicScript.VecToAlignTo = normalVector;
//						characterControllerLogicScript.BoundingBoxMinX = collider.bounds.min.x + 0.15f;
//						characterControllerLogicScript.BoundingBoxMaxX = collider.bounds.max.x - 0.15f;
						characterControllerLogicScript.CurrentCoverState = CoverState.onDownFace;
						calculateBounds(Face.DOWN, normalVector);
					}
					
					//UP FACE (discard)
					if(Vector3.Dot(normalVector, Vector3.forward) > 0.707f)
					{
						Debug.Log ("UP FACE");

					}
					
					//LEFT FACE
					if(Vector3.Dot(normalVector, Vector3.left) > 0.707f)
					{
						Debug.Log ("LEFT FACE");
						currFace = Face.LEFT;
						characterControllerLogicScript.SavedCamPosition = gameCam.transform.position;
						characterControllerLogicScript.SavedCamRotation = gameCam.transform.rotation;
						if(isAWall)
						{
							playerAnimator.SetBool(hash.coverBool, true);
							characterControllerLogicScript.PositionToPlaceTo = new Vector3(contact.point.x - 0.2f, player.transform.position.y, contact.point.z);
						}
						else 
						{
							characterControllerLogicScript.PositionToPlaceTo = new Vector3(contact.point.x - 0.42f, player.transform.position.y, contact.point.z);
							playerAnimator.SetBool(hash.crouchCoverBool, true);
							caps.center = new Vector3(0,0.5f,0);
							caps.height = 0.9f;
						}

						characterControllerLogicScript.VecToAlignTo = normalVector;
//						characterControllerLogicScript.BoundingBoxMinZ = collider.bounds.min.z + 0.15f;
//						characterControllerLogicScript.BoundingBoxMaxZ = collider.bounds.max.z - 0.15f;
						characterControllerLogicScript.CurrentCoverState = CoverState.OnLeftFace;
						calculateBounds(Face.LEFT, normalVector);


					}
					
					//RIGHT FACE
					if(Vector3.Dot(normalVector, Vector3.left) < -0.707f)
					{
						Debug.Log ("RIGHT FACE");
						currFace = Face.RIGHT;
						characterControllerLogicScript.SavedCamPosition = gameCam.transform.position;
						characterControllerLogicScript.SavedCamRotation = gameCam.transform.rotation;
						if(isAWall)
						{
							playerAnimator.SetBool(hash.coverBool, true);
							characterControllerLogicScript.PositionToPlaceTo = new Vector3(contact.point.x + 0.2f, player.transform.position.y, contact.point.z);
						}
						else 
						{
							playerAnimator.SetBool(hash.crouchCoverBool, true);
							characterControllerLogicScript.PositionToPlaceTo = new Vector3(contact.point.x + 0.42f, player.transform.position.y, contact.point.z);
							caps.center = new Vector3(0,0.5f,0);
							caps.height = 0.9f;
						}

						characterControllerLogicScript.VecToAlignTo = normalVector;
						characterControllerLogicScript.BoundingBoxMinZ = collider.bounds.min.z + 0.15f;
						characterControllerLogicScript.BoundingBoxMaxZ = collider.bounds.max.z - 0.15f;
						characterControllerLogicScript.CurrentCoverState = CoverState.OnRightFace;

					}


				}
			}
		}
		
	}

	void calculateBounds(Face currFace, Vector3 normal)
	{
		//We get the angle to first know how the object is rotated;
		float angle = transform.localEulerAngles.y;
		if(angle >180)angle = angle -360f;
		Debug.Log ("angle = " + angle);

		switch(currFace)
		{
		case Face.DOWN:
			//If ANGLE < 0 WE NEED TO CHECK MIN X
			//If ANGLE > 0 WE NEED TO CHECK MAX X
			if(angle <0)
			{
				float val = Mathf.Sin((-1*angle) * Mathf.PI / 180f) * currCollider.size.z;

				characterControllerLogicScript.BoundingBoxMinX = currCollider.bounds.min.x + val + 0.15f;
				characterControllerLogicScript.BoundingBoxMaxX = currCollider.bounds.max.x - 0.15f;
			}
			else
			{
				float val = Mathf.Sin(angle) * currCollider.size.z;
				characterControllerLogicScript.BoundingBoxMinX = currCollider.bounds.min.x + 0.15f;
				characterControllerLogicScript.BoundingBoxMaxX = currCollider.bounds.max.x - val - 0.15f;
			}

			break;
		case Face.LEFT:
			//If ANGLE < 0 WE NEED TO CHECK MAX Z
			//If ANGLE > 0 WE NEED TO CHECK MIN Z
			if(angle <0)
			{
				float val = Mathf.Sin((-1*angle) * Mathf.PI / 180f) * currCollider.size.x;

				characterControllerLogicScript.BoundingBoxMinZ = currCollider.bounds.min.z + 0.15f;
				characterControllerLogicScript.BoundingBoxMaxZ = currCollider.bounds.max.z - val - 0.15f;
			}
			else
			{
				float val = Mathf.Sin((angle) * Mathf.PI / 180f) * currCollider.size.x;
				
				characterControllerLogicScript.BoundingBoxMinZ = currCollider.bounds.min.z + val+ 0.15f;
				characterControllerLogicScript.BoundingBoxMaxZ = currCollider.bounds.max.z - 0.15f;
			}

			break;
		case Face.RIGHT:
			//If ANGLE < 0 WE NEED TO CHECK MIN Z
			//If ANGLE > 0 WE NEED TO CHECK MAX Z
			if(angle <0)
			{
				
			}
			else
			{
				
			}

			break;
		}

	}
}
