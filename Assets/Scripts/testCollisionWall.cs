using UnityEngine;
using System.Collections;

public class testCollisionWall : MonoBehaviour {

	private HashIds hash;
	private Animator playerAnimator;
	private GameObject player;
	private GameObject gameCam;
	private CapsuleCollider caps;

	void Awake()
	{
		hash = GameObject.FindGameObjectWithTag (DoneTags.gameController).GetComponent<HashIds> ();
		player = GameObject.FindGameObjectWithTag (DoneTags.player);
		playerAnimator = player.GetComponent<Animator> ();
		caps = player.GetComponent<CapsuleCollider>();
		gameCam = GameObject.FindGameObjectWithTag (DoneTags.camera);

	}
	
	void OnCollisionEnter(Collision collision) {

		//If the player collide and was in Locomotion
		if(collision.gameObject.CompareTag(DoneTags.player) && playerAnimator.GetCurrentAnimatorStateInfo(0).nameHash == hash.m_LocomotionIdState)
		{
			ContactPoint contact = collision.contacts[0];
			if (collision.relativeVelocity.magnitude > 1)
			{
//				Debug.Log ("scalar product = " + Vector3.Dot(collision.gameObject.transform.forward,contact.normal));
//				Debug.Log ("wall normal = " + contact.normal);
				CharacterControllerLogic characterControllerLogicScript = collision.gameObject.GetComponent<CharacterControllerLogic>();

//				//angle 150 with wall's normal vector
//				if(Vector3.Dot(collision.gameObject.transform.forward,contact.normal) < -0.7f)
//				{

//					//DOWN FACE
//					if(Vector3.Dot(contact.normal, Vector3.forward) < -0.9f)
//					{
//						Debug.Log ("DOWN FACE");
//						characterControllerLogicScript.CurrentCoverState = CoverState.onDownFace; 
//					}
//
//					//UP FACE (discard)
//					if(Vector3.Dot(contact.normal, Vector3.forward) > 0.9f)
//					{
//						Debug.Log ("UP FACE");
//					}
//
//					//LEFT FACE
//					if(Vector3.Dot(contact.normal, Vector3.left) > 0.9f)
//					{
//						Debug.Log ("LEFT FACE");
//						characterControllerLogicScript.CurrentCoverState = CoverState.OnLeftFace; 
//					}
//
//					//RIGHT FACE
//					if(Vector3.Dot(contact.normal, Vector3.left) < -0.9f)
//					{
//						Debug.Log ("RIGHT FACE");
//						characterControllerLogicScript.CurrentCoverState = CoverState.OnRightFace;
//					}

				if(Vector3.Dot(collision.gameObject.transform.forward,contact.normal) > 0.6f)
				{

					//DOWN FACE
					if(Vector3.Dot(contact.normal, Vector3.forward) > 0.9f)
					{
						Debug.Log ("DOWN FACE");
						characterControllerLogicScript.SavedCamPosition = gameCam.transform.position;
						characterControllerLogicScript.SavedCamRotation = gameCam.transform.rotation;
						playerAnimator.SetBool(hash.coverBool, true);
//						characterControllerLogicScript.VecToAlignTo = -1*contact.normal;
						characterControllerLogicScript.BoundingBoxMinX = collider.bounds.min.x + 0.15f;
						characterControllerLogicScript.BoundingBoxMaxX = collider.bounds.max.x - 0.15f;
						characterControllerLogicScript.PositionToPlaceTo = new Vector3(contact.point.x, player.transform.position.y, contact.point.z -0.23f);
						characterControllerLogicScript.CurrentCoverState = CoverState.onDownFace;

					}
					
					//UP FACE (discard)
					if(Vector3.Dot(contact.normal, Vector3.forward) < -0.9f)
					{
						Debug.Log ("UP FACE");

					}
					
					//LEFT FACE
					if(Vector3.Dot(contact.normal, Vector3.left) < -0.9f)
					{
						Debug.Log ("LEFT FACE");
						characterControllerLogicScript.SavedCamPosition = gameCam.transform.position;
						characterControllerLogicScript.SavedCamRotation = gameCam.transform.rotation;
						playerAnimator.SetBool(hash.coverBool, true);
//						characterControllerLogicScript.VecToAlignTo = -1*contact.normal;
						characterControllerLogicScript.BoundingBoxMinZ = collider.bounds.min.z + 0.15f;
						characterControllerLogicScript.BoundingBoxMaxZ = collider.bounds.max.z - 0.15f;
						characterControllerLogicScript.PositionToPlaceTo = new Vector3(contact.point.x - 0.23f, player.transform.position.y, contact.point.z);
						characterControllerLogicScript.CurrentCoverState = CoverState.OnLeftFace;

					}
					
					//RIGHT FACE
					if(Vector3.Dot(contact.normal, Vector3.left) > 0.9f)
					{
						Debug.Log ("RIGHT FACE");
						characterControllerLogicScript.SavedCamPosition = gameCam.transform.position;
						characterControllerLogicScript.SavedCamRotation = gameCam.transform.rotation;
						playerAnimator.SetBool(hash.coverBool, true);
//						characterControllerLogicScript.VecToAlignTo = -1*contact.normal;
						characterControllerLogicScript.BoundingBoxMinZ = collider.bounds.min.z + 0.15f;
						characterControllerLogicScript.BoundingBoxMaxZ = collider.bounds.max.z - 0.15f;
						characterControllerLogicScript.PositionToPlaceTo = new Vector3(contact.point.x + 0.23f, player.transform.position.y, contact.point.z);
						characterControllerLogicScript.CurrentCoverState = CoverState.OnRightFace;

					}


				}
			}
		}
		
	}
}
