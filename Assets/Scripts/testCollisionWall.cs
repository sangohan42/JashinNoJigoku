using UnityEngine;
using System.Collections;

public class testCollisionWall : MonoBehaviour {
	
	void OnCollisionEnter(Collision collision) {
		if(collision.gameObject.CompareTag(DoneTags.player))
		{
//			Debug.Log ("Collision magnitude = " + collision.relativeVelocity.magnitude);
			ContactPoint contact = collision.contacts[0];

			if (collision.relativeVelocity.magnitude > 3)
			{
				Debug.Log ("scalar product = " + Vector3.Dot(collision.gameObject.transform.forward,contact.normal));
				Debug.Log ("wall normal = " + contact.normal);
				CharacterControllerLogic characterControllerLogicScript = collision.gameObject.GetComponent<CharacterControllerLogic>();

				//angle 150 with wall's normal vector
				if(Vector3.Dot(collision.gameObject.transform.forward,contact.normal) < -0.7f)
				{

					//DOWN FACE
					if(Vector3.Dot(contact.normal, Vector3.forward) < -0.9f)
					{
						Debug.Log ("DOWN FACE");
						characterControllerLogicScript.CurrentCoverState = CoverState.onDownFace; 
					}

					//UP FACE (discard)
					if(Vector3.Dot(contact.normal, Vector3.forward) > 0.9f)
					{
						Debug.Log ("UP FACE");
					}

					//LEFT FACE
					if(Vector3.Dot(contact.normal, Vector3.left) > 0.9f)
					{
						Debug.Log ("LEFT FACE");
						characterControllerLogicScript.CurrentCoverState = CoverState.OnLeftFace; 
					}

					//RIGHT FACE
					if(Vector3.Dot(contact.normal, Vector3.left) < -0.9f)
					{
						Debug.Log ("RIGHT FACE");
						characterControllerLogicScript.CurrentCoverState = CoverState.OnRightFace;
					}

				}
			}
		}
		
	}
}
