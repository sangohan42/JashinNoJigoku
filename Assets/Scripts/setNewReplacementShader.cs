using UnityEngine;
using System.Collections;

public class setNewReplacementShader : MonoBehaviour {

	public Shader replacementShader;
	// Use this for initialization
	void Awake () {
		camera.SetReplacementShader (replacementShader,"RenderType");
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
