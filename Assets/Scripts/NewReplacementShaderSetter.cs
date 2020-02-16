using UnityEngine;

[RequireComponent(typeof(Camera))]
public class NewReplacementShaderSetter : MonoBehaviour
{
	public Shader replacementShader;
	// Use this for initialization
	void Awake ()
    {
		GetComponent<Camera>().SetReplacementShader (replacementShader,"RenderType");
	}
}
