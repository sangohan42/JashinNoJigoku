using UnityEngine;
using System.Collections;

public class testCollisionWall : MonoBehaviour {

	private HashIds hash;
	private Animator playerAnimator;
	private GameObject player;
	private CapsuleCollider caps;

	void Awake()
	{
		hash = GameObject.FindGameObjectWithTag (DoneTags.gameController).GetComponent<HashIds> ();
		player = GameObject.FindGameObjectWithTag (DoneTags.player);
		playerAnimator = player.GetComponent<Animator> ();
		caps = player.GetComponent<CapsuleCollider>();

	}
	
	void OnCollisionEnter(Collision collision) {

		//If the player collide and was in Locomotion
		if(collision.gameObject.CompareTag(DoneTags.player) && playerAnimator.GetCurrentAnimatorStateInfo(0).nameHash == hash.m_LocomotionIdState)
		{
			ContactPoint contact = collision.contacts[0];
			if (collision.relativeVelocity.magnitude > 3)
			{
				Debug.Log ("scalar product = " + Vector3.Dot(collision.gameObject.transform.forward,contact.normal));
				Debug.Log ("wall normal = " + contact.normal);
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

				if(Vector3.Dot(collision.gameObject.transform.forward,contact.normal) > 0.9f)
				{

					//DOWN FACE
					if(Vector3.Dot(contact.normal, Vector3.forward) > 0.9f)
					{
						Debug.Log ("DOWN FACE");
						characterControllerLogicScript.CurrentCoverState = CoverState.onDownFace; 
//						caps.radius = 0.25f;
						playerAnimator.SetBool(hash.coverBool, true);
						characterControllerLogicScript.VecToAlignTo = -1*contact.normal;
						characterControllerLogicScript.PositionToPlaceTo = new Vector3(contact.point.x, player.transform.position.y, contact.point.z -0.13f);

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
						characterControllerLogicScript.CurrentCoverState = CoverState.OnLeftFace;
//						caps.radius = 0.25f;
						playerAnimator.SetBool(hash.coverBool, true);
						characterControllerLogicScript.VecToAlignTo = -1*contact.normal;
						characterControllerLogicScript.PositionToPlaceTo = new Vector3(contact.point.x - 0.13f, player.transform.position.y, contact.point.z);

					}
					
					//RIGHT FACE
					if(Vector3.Dot(contact.normal, Vector3.left) > 0.9f)
					{
						Debug.Log ("RIGHT FACE");
						characterControllerLogicScript.CurrentCoverState = CoverState.OnRightFace;
//						caps.radius = 0.25f;
						playerAnimator.SetBool(hash.coverBool, true);
						characterControllerLogicScript.VecToAlignTo = -1*contact.normal;
						characterControllerLogicScript.PositionToPlaceTo = new Vector3(contact.point.x + 0.13f, player.transform.position.y, contact.point.z);

					}


				}
			}
		}
		
	}
}
