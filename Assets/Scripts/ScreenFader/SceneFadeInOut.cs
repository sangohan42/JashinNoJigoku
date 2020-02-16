using System;
using UnityEngine;
using System.Collections;

[RequireComponent(typeof(GUITexture))]
public class SceneFadeInOut : MonoBehaviour
{
	public float fadeSpeed = 1.5f;			// Speed that the screen fades to and from black.

    private bool _isFadeIn;
    private bool _isFadeout;

    private Action _onFadeOutfinished;

    private GUITexture _fadingtexture;

    void Awake ()
	{
        _fadingtexture = GetComponent<GUITexture>();

        _isFadeIn = true;
        _isFadeout = false;

        // Set the texture so that it is the the size of the screen and covers it.
        _fadingtexture.pixelInset = new Rect(0f, 0f, Screen.width, Screen.height);
    }

    void Update()
    {
        if(_isFadeIn)
            FadeIn();
        else if(_isFadeout)
            FadeOut();
    }

    void FadeIn()
    {
        // Fade the texture to clear.
        FadeToClear();

        // If the texture is almost clear...
        if (_fadingtexture.color.a <= 0.05f)
        {
            // ... set the colour to clear and disable the GUITexture.
            _fadingtexture.color = Color.clear;
            _fadingtexture.enabled = false;
            _isFadeIn = false;
        }
    }

    void FadeToClear ()
	{
        // Lerp the colour of the texture between itself and transparent.
        _fadingtexture.color = Color.Lerp(_fadingtexture.color, Color.clear, fadeSpeed * Time.deltaTime);
	}

    void FadeOut()
    {
        FadeToBlack();
        // If the screen is almost black...
        if (_fadingtexture.color.a >= 0.95f )
        {
            if(_onFadeOutfinished != null )
                _onFadeOutfinished.Invoke();
        }
	}

    void FadeToBlack()
    {
        // Lerp the colour of the texture between itself and black.
        _fadingtexture.color = Color.Lerp(_fadingtexture.color, Color.black, fadeSpeed * Time.deltaTime);
    }

    public void StartFadeOut (Action onFadeOutFinished )
	{
        // Make sure the texture is enabled.
        _fadingtexture.enabled = true;
        _isFadeout = true;
        _onFadeOutfinished = onFadeOutFinished;
    }
}
