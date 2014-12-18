﻿using UnityEngine;
using System.Collections;

public class testCollisionWall : MonoBehaviour {

	private HashIds hash;
	private Animator playerAnimator;
	private GameObject player;
	private CharacterControllerLogic characterControllerLogicScript;
	private checkEnemyStatus checkEnemy;
	private GameObject gameCam;
	private CapsuleCollider caps;
	private enum Face{LEFT, RIGHT, DOWN};
	private BoxCollider currCollider;
	private Vector3 size;
	private bool isAWall;
//	private Vector3 boundMax;
//	private Vector3 boundMin;

	private float timeCollided;
	public static bool inCoverMode;

	private Vector3 normalVector;

	void Awake()
	{
		hash = GameObject.FindGameObjectWithTag (DoneTags.gameController).GetComponent<HashIds> ();
		player = GameObject.FindGameObjectWithTag (DoneTags.player);
		playerAnimator = player.GetComponent<Animator> ();
		gameCam = GameObject.FindGameObjectWithTag (DoneTags.camera);
		characterControllerLogicScript = player.GetComponent<CharacterControllerLogic> ();
		checkEnemy = player.GetComponent<checkEnemyStatus> ();
		caps = player.GetComponent<CapsuleCollider> ();
		timeCollided = 0;
		inCoverMode = false;
		normalVector = Vector3.zero;
		isAWall = !(this.CompareTag (DoneTags.box));
	}

	void Start()
	{
		if(collider is MeshCollider)
		{
			size = new Vector3 (collider.bounds.max.x - collider.bounds.min.x, collider.bounds.max.y - collider.bounds.min.y, collider.bounds.max.z - collider.bounds.min.z);

		}

		else 
		{
			currCollider = (BoxCollider)collider;
//			boundMax = currCollider.bounds.max;
//			boundMin = currCollider.bounds.min;
			size = currCollider.size;
		}

	}

	void OnCollisionStay(Collision collision) {
		if(collision.gameObject.CompareTag(DoneTags.player) && !inCoverMode && checkEnemy.isNotSeen())
		{
			ContactPoint contact = collision.contacts[0];
//			if(normalVector != contact.normal)
//			{
//				Debug.Log ("RESET");
//				timeCollided=0;
//				normalVector = contact.normal;
//			}
//			normalVector = contact.normal;
//			Debug.Log ("IS A WALL = " + isAWall);
			RaycastHit hit;
			Vector3 startPos = player.transform.position + caps.center.y*transform.up;

			if(Physics.Raycast(startPos, contact.point -startPos, out hit, 1f))
			{
				if(normalVector != -1*hit.normal)
				{
					timeCollided = 0;
					normalVector = -1*hit.normal;
				}
			}

			//We have the right orientation
			if(Vector3.Dot(collision.gameObject.transform.forward,normalVector) > 0.75f && collision.relativeVelocity.magnitude >1)
			{
				timeCollided += Time.deltaTime;


				//If we were 1 second in the good orientation
				if(timeCollided > 0.5f)
				{
//					Debug.Log ("IS A WALL = " + isAWall + " and normal = " + normalVector);
//
//					Debug.Log ("collision point = " + contact.point);
//					Debug.Log ("collision point Raycast = " + hit.point);
//
//					Debug.Break ();

					inCoverMode = true;


					//DOWN FACE
					if(Vector3.Dot(normalVector, Vector3.forward) > 0.707106)
					{
						Debug.Log ("DOWN FACE");
						player.rigidbody.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionZ;

						characterControllerLogicScript.SavedCamPosition = gameCam.transform.position;
						characterControllerLogicScript.SavedCamRotation = gameCam.transform.rotation;

						if(isAWall){
							playerAnimator.SetBool(hash.coverBool, true);
							characterControllerLogicScript.PositionToPlaceTo = new Vector3(contact.point.x, player.transform.position.y, contact.point.z -0.22f);
						}

						else{
							playerAnimator.SetBool(hash.crouchCoverBool, true);
							characterControllerLogicScript.PositionToPlaceTo = new Vector3(contact.point.x, player.transform.position.y, contact.point.z -0.42f);
							characterControllerLogicScript.InCrouchCoverMode = true;
							characterControllerLogicScript.CoverPos += new Vector3(0, -0.9f,0);
						}
						characterControllerLogicScript.VecToAlignTo = -1*normalVector;
						characterControllerLogicScript.CurrentCoverState = CoverState.onDownFace;
						calculateBounds(Face.DOWN, -1*normalVector);
					}

					//LEFT FACE
					else if(Vector3.Dot(normalVector, -1*Vector3.left) > 0.707106)
					{
						Debug.Log ("LEFT FACE");
						player.rigidbody.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionX;

						characterControllerLogicScript.SavedCamPosition = gameCam.transform.position;
						characterControllerLogicScript.SavedCamRotation = gameCam.transform.rotation;

						if(isAWall){
							playerAnimator.SetBool(hash.coverBool, true);
							characterControllerLogicScript.PositionToPlaceTo = new Vector3(contact.point.x - 0.22f, player.transform.position.y, contact.point.z);

						}

						else{
							characterControllerLogicScript.PositionToPlaceTo = new Vector3(contact.point.x - 0.42f, player.transform.position.y, contact.point.z);
							playerAnimator.SetBool(hash.crouchCoverBool, true);
							characterControllerLogicScript.InCrouchCoverMode = true;
							characterControllerLogicScript.CoverPos += new Vector3(0, -0.9f,0);
						}
						characterControllerLogicScript.VecToAlignTo = -1*normalVector;
						characterControllerLogicScript.CurrentCoverState = CoverState.OnLeftFace;
						calculateBounds(Face.LEFT, -1*normalVector);

					}
					
					//RIGHT FACE
					else if(Vector3.Dot(normalVector, Vector3.left) > 0.707106)
					{
						Debug.Log ("RIGHT FACE");
						player.rigidbody.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionX;

						characterControllerLogicScript.SavedCamPosition = gameCam.transform.position;
						characterControllerLogicScript.SavedCamRotation = gameCam.transform.rotation;

						if(isAWall){
							playerAnimator.SetBool(hash.coverBool, true);
							characterControllerLogicScript.PositionToPlaceTo = new Vector3(contact.point.x + 0.22f, player.transform.position.y, contact.point.z);

						}

						else{
							playerAnimator.SetBool(hash.crouchCoverBool, true);
							characterControllerLogicScript.PositionToPlaceTo = new Vector3(contact.point.x + 0.42f, player.transform.position.y, contact.point.z);
							characterControllerLogicScript.InCrouchCoverMode = true;
							characterControllerLogicScript.CoverPos += new Vector3(0, -0.9f,0);
						}

						characterControllerLogicScript.VecToAlignTo = -1*normalVector;
						characterControllerLogicScript.CurrentCoverState = CoverState.OnRightFace;
						calculateBounds(Face.RIGHT, -1*normalVector);
						
					}
				}
			}
		}
	}

	void OnCollisionExit(Collision collision) 
	{
		timeCollided = 0;
		normalVector = Vector3.zero;
	}
	
//	void OnCollisionEnter(Collision collision) {
//
//		//If the player collide and was in Locomotion
//		if(collision.gameObject.CompareTag(DoneTags.player)&& !characterControllerLogicScript.IsPursued)
//		{
//			ContactPoint contact = collision.contacts[0];
//			RaycastHit hit;
//			Vector3 normalVector = Vector3.zero;
//			if(Physics.Raycast(player.transform.position + caps.center.y*transform.up, player.transform.forward, out hit, 1f))
//			{
//				normalVector = hit.normal;
//			}
//
//			if(Vector3.Dot(collision.gameObject.transform.forward,-1*normalVector) > 0.6f)
//			{
//				hasFirstCollided = true;
//				//DOWN FACE
//				if(Vector3.Dot(normalVector, Vector3.forward) < -0.707f)
//				{
//					Debug.Log ("DOWN FACE");
//					currFace = Face.DOWN;
//					characterControllerLogicScript.SavedCamPosition = gameCam.transform.position;
//					characterControllerLogicScript.SavedCamRotation = gameCam.transform.rotation;
//					if(isAWall)
//					{
//						playerAnimator.SetBool(hash.coverBool, true);
//						characterControllerLogicScript.PositionToPlaceTo = new Vector3(contact.point.x, player.transform.position.y, contact.point.z -0.2f);
//					}
//					else 
//					{
//						playerAnimator.SetBool(hash.crouchCoverBool, true);
//						characterControllerLogicScript.PositionToPlaceTo = new Vector3(contact.point.x, player.transform.position.y, contact.point.z -0.42f);
////							caps.center = new Vector3(0,0.65f,-0.05f);
////							caps.height = 1f;
////							caps.radius = 0.36f;
//						characterControllerLogicScript.CoverPos += new Vector3(0, -0.9f,0);
//
//					}
//					characterControllerLogicScript.VecToAlignTo = normalVector;
////						characterControllerLogicScript.BoundingBoxMinX = collider.bounds.min.x + 0.15f;
////						characterControllerLogicScript.BoundingBoxMaxX = collider.bounds.max.x - 0.15f;
//					characterControllerLogicScript.CurrentCoverState = CoverState.onDownFace;
//					calculateBounds(Face.DOWN, normalVector);
//				}
//				
//				//UP FACE (discard)
//				if(Vector3.Dot(normalVector, Vector3.forward) > 0.707f)
//				{
//					Debug.Log ("UP FACE");
//
//				}
//				
//				//LEFT FACE
//				if(Vector3.Dot(normalVector, Vector3.left) > 0.707f)
//				{
//					Debug.Log ("LEFT FACE");
//					currFace = Face.LEFT;
//					characterControllerLogicScript.SavedCamPosition = gameCam.transform.position;
//					characterControllerLogicScript.SavedCamRotation = gameCam.transform.rotation;
//					if(isAWall)
//					{
//						playerAnimator.SetBool(hash.coverBool, true);
//						characterControllerLogicScript.PositionToPlaceTo = new Vector3(contact.point.x - 0.2f, player.transform.position.y, contact.point.z);
//					}
//					else 
//					{
//						characterControllerLogicScript.PositionToPlaceTo = new Vector3(contact.point.x - 0.42f, player.transform.position.y, contact.point.z);
//						playerAnimator.SetBool(hash.crouchCoverBool, true);
//						caps.center = new Vector3(0,0.5f,0);
//						caps.height = 0.9f;
//						caps.radius = 0.36f;
//
//						characterControllerLogicScript.CoverPos += new Vector3(0, -0.8f,0);
//
//					}
//
//					characterControllerLogicScript.VecToAlignTo = normalVector;
////						characterControllerLogicScript.BoundingBoxMinZ = collider.bounds.min.z + 0.15f;
////						characterControllerLogicScript.BoundingBoxMaxZ = collider.bounds.max.z - 0.15f;
//					characterControllerLogicScript.CurrentCoverState = CoverState.OnLeftFace;
//					calculateBounds(Face.LEFT, normalVector);
//
//
//				}
//				
//				//RIGHT FACE
//				if(Vector3.Dot(normalVector, Vector3.left) < -0.707f)
//				{
//					Debug.Log ("RIGHT FACE");
//					currFace = Face.RIGHT;
//					characterControllerLogicScript.SavedCamPosition = gameCam.transform.position;
//					characterControllerLogicScript.SavedCamRotation = gameCam.transform.rotation;
//					if(isAWall)
//					{
//						playerAnimator.SetBool(hash.coverBool, true);
//						characterControllerLogicScript.PositionToPlaceTo = new Vector3(contact.point.x + 0.2f, player.transform.position.y, contact.point.z);
//					}
//					else 
//					{
//						playerAnimator.SetBool(hash.crouchCoverBool, true);
//						characterControllerLogicScript.PositionToPlaceTo = new Vector3(contact.point.x + 0.42f, player.transform.position.y, contact.point.z);
//						caps.center = new Vector3(0,0.5f,0);
//						caps.height = 0.9f;
//						caps.radius = 0.36f;
//
//						characterControllerLogicScript.CoverPos += new Vector3(0, -0.8f,0);
//					}
//
//					characterControllerLogicScript.VecToAlignTo = normalVector;
////						characterControllerLogicScript.BoundingBoxMinZ = collider.bounds.min.z + 0.15f;
////						characterControllerLogicScript.BoundingBoxMaxZ = collider.bounds.max.z - 0.15f;
//					characterControllerLogicScript.CurrentCoverState = CoverState.OnRightFace;
//					calculateBounds(Face.RIGHT, normalVector);
//
//				}
//
//			}
//		}
//		
//	}

	void calculateBounds(Face currFace, Vector3 normal)
	{
		//We get the angle to first know how the object is rotated;
		float angle = transform.localEulerAngles.y;
		if(angle >180)angle = angle -360f;
//		Debug.Log ("angle = " + angle);

		switch(currFace)
		{
		case Face.DOWN:
			//If ANGLE < 0 WE NEED TO CHECK MIN X
			//If ANGLE > 0 WE NEED TO CHECK MAX X
			if(angle <0)
			{
				float val = Mathf.Sin((-1*angle) * Mathf.PI / 180f) * size.z;

				characterControllerLogicScript.BoundingBoxMinX = collider.bounds.min.x + val + 0.17f;
				characterControllerLogicScript.BoundingBoxMaxX = collider.bounds.max.x - 0.17f;
			}
			else
			{
				float val = Mathf.Sin((angle) * Mathf.PI / 180f)* size.z;
				characterControllerLogicScript.BoundingBoxMinX = collider.bounds.min.x + 0.17f;
				characterControllerLogicScript.BoundingBoxMaxX = collider.bounds.max.x - val - 0.17f;
			}

			break;
		case Face.LEFT:
			//If ANGLE < 0 WE NEED TO CHECK MAX Z
			//If ANGLE > 0 WE NEED TO CHECK MIN Z
			if(angle <0)
			{
				float val = Mathf.Sin((-1*angle) * Mathf.PI / 180f) * size.x;

				characterControllerLogicScript.BoundingBoxMinZ = collider.bounds.min.z + 0.17f;
				characterControllerLogicScript.BoundingBoxMaxZ = collider.bounds.max.z - val - 0.17f;
			}
			else
			{
				float val = Mathf.Sin((angle) * Mathf.PI / 180f) * size.x;
				
				characterControllerLogicScript.BoundingBoxMinZ = collider.bounds.min.z + val+ 0.17f;
				characterControllerLogicScript.BoundingBoxMaxZ = collider.bounds.max.z - 0.17f;
			}

			break;
		case Face.RIGHT:
			//If ANGLE < 0 WE NEED TO CHECK MIN Z
			//If ANGLE > 0 WE NEED TO CHECK MAX Z
			if(angle <0)
			{
				float val = Mathf.Sin((-1*angle) * Mathf.PI / 180f) * size.x;
				
				characterControllerLogicScript.BoundingBoxMinZ = collider.bounds.min.z + val + 0.17f;
				characterControllerLogicScript.BoundingBoxMaxZ = collider.bounds.max.z - 0.17f;
			}
			else
			{
				float val = Mathf.Sin((angle) * Mathf.PI / 180f) * size.x;
				
				characterControllerLogicScript.BoundingBoxMinZ = collider.bounds.min.z + 0.17f;
				characterControllerLogicScript.BoundingBoxMaxZ = collider.bounds.max.z - val - 0.17f;
			}

			break;
		}

	}
}
