﻿using UnityEngine;
using System.Collections;

/// <summary>
/// Joystick widget for NGUI.
/// </summary>
[RequireComponent(typeof(UIWidget))]
[RequireComponent(typeof(BoxCollider))]
public class UIJoystick : MonoBehaviour
{

    public UISprite joystickSprite;
    public Vector2 position;
    public bool isNormalize = false;
    public bool isKeyboardEmulation = true;
	public bool hasMovedBottom = false;

    bool lastKeyboardInput = false;
    int touchId = -10;
    public bool isPressed;
    bool isInitialized = false;
    UIWidget widget;
    Plane plane;
    Vector2 innerPosition;
    Vector2 centerPosition;
    Vector2 lastPosition;
    float radius;
	
	void Awake()
	{
	}

    void OnEnable()
    {
        widget = GetComponent<UIWidget>();
//		joystickSprite.enabled = false;

        if (SystemInfo.deviceType == DeviceType.Handheld)
        {
            isKeyboardEmulation = false;
        }
    }

    /// <summary>
    /// Initialize joysticks. This can't performed in OnEnable() or Start() because NGUI 
    /// </summary>
    void Initialize()
    {
//		joystickSprite.enabled = true;
        centerPosition = UICamera.mainCamera.WorldToScreenPoint(transform.position);

        radius = (widget.height) * 0.5f * Screen.height / widget.root.manualHeight;

        Transform trans = UICamera.currentCamera.transform;
        plane = new Plane(trans.rotation * Vector3.back, UICamera.lastHit.point);


        isInitialized = true;
    }

    void OnPress(bool pressed)
    {
        if (pressed)
        {
            if (isInitialized == false)
            {
                Initialize();
            }
            touchId = UICamera.currentTouchID;
            innerPosition = UICamera.currentTouch.pos;

            ApplyMovement();

            isPressed = true;
        }
        else
        {
            if (touchId == UICamera.currentTouchID)
            {
                touchId = -10;
                ClearPosition();
				isInitialized = false;
                isPressed = false;
//				joystickSprite.enabled = false;

            }
        }
    }

    void Update()
    {
        if (isKeyboardEmulation)
        {
            EmulateKeyboard();
        }

        if (isPressed)
        {
            innerPosition = UICamera.GetTouch(touchId).pos;

            ApplyMovement();
			
        }
    }

    void EmulateKeyboard()
    {
        if (isInitialized == false)
        {
            Initialize();
        }

        bool isKeyboardInput = false;
        Vector2 keyboardDelta = Vector2.zero;

        if (Input.GetKey(KeyCode.W))
        {
            keyboardDelta += new Vector2(0, 1);
            isKeyboardInput = true;
        }
        if (Input.GetKey(KeyCode.S))
        {
            keyboardDelta += new Vector2(0, -1);
            isKeyboardInput = true;
        }
        if (Input.GetKey(KeyCode.D))
        {
            keyboardDelta += new Vector2(1, 0);
            isKeyboardInput = true;
        }
        if (Input.GetKey(KeyCode.A))
        {
            keyboardDelta += new Vector2(-1, 0);
            isKeyboardInput = true;
        }

        if (isKeyboardInput == true)
        {
            innerPosition = centerPosition + keyboardDelta * radius;
            ApplyMovement();
        }
        else
        {
            if (lastKeyboardInput == true)
            {
                // Release joystick.
                ClearPosition();
            }
        }

        lastKeyboardInput = isKeyboardInput;
    }

    void ClearPosition()
    {
        position = Vector2.zero;
        innerPosition = centerPosition;

        ApplyMovement();
		
		hasMovedBottom = false;
    }

    void ApplyMovement()
    {
        Vector2 positionDelta = innerPosition - centerPosition;

        if (isNormalize == false)
        {
            if (positionDelta.x > radius)
            {
                positionDelta.x = radius;
            }
            if (positionDelta.x < -radius)
            {
                positionDelta.x = -radius;
            }
            if (positionDelta.y > radius)
            {
                positionDelta.y = radius;
            }
            if (positionDelta.y < -radius)
            {
                positionDelta.y = -radius;
            }
        }
        else
        {
            if (positionDelta.magnitude > radius)
            {
                positionDelta = positionDelta.normalized * radius;
            }
        }

        position.x = positionDelta.x / radius;
        position.y = positionDelta.y / radius;

        innerPosition = centerPosition + positionDelta;

        Ray ray = UICamera.currentCamera.ScreenPointToRay(innerPosition);
        float dist = 0.0f;

        if (plane.Raycast(ray, out dist))
        {
            Vector3 currentPosition = ray.GetPoint(dist);

            // Release joystick.
            joystickSprite.transform.position = currentPosition;
        }
    }
	
	
}
