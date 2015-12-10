
//
// Copyright (C) 2013 Sixense Entertainment Inc.
// All Rights Reserved
//

using UnityEngine;
using System.Collections;

public class RightHandController : MonoBehaviour
{
	public SixenseHands	m_hand;
	public SixenseInput.Controller m_controller = null;

    Vector3 m_baseOffset;
    float m_sensitivity = 0.001f; // Sixense units are in mm
    bool m_bInitialized;

	float 		m_fLastTriggerVal;
	Vector3		m_initialPosition;
	Quaternion 	m_initialRotation;


	protected void Start() 
	{
		// get the Animator
		m_initialRotation = transform.localRotation;
		m_initialPosition = transform.localPosition;
	}


	protected void Update()
	{
		if ( m_controller == null )
		{
			m_controller = SixenseInput.GetController( m_hand );
		}
        else
        {
            handleInput();
        }
    }
	
	
	// Updates the animated object from controller input.
    protected void handleInput()
	{
        // stuff here

        bool bResetHandPosition = false;

        if (IsControllerActive(m_controller) && m_controller.GetButtonDown(SixenseButtons.START))
        {
            bResetHandPosition = true;
        }

        if (m_bInitialized)
        {
            bool bControllerActive = IsControllerActive(m_controller);

            if (bControllerActive)
            {
                transform.localPosition = (m_controller.Position - m_baseOffset) * m_sensitivity;
                transform.localRotation = m_controller.Rotation * InitialRotation;
            }

            else
            {
                // use the inital position and orientation because the controller is not active
                transform.localPosition = InitialPosition;
                transform.localRotation = InitialRotation;
            }
        }

        if (bResetHandPosition)
        {
            m_bInitialized = true;

            m_baseOffset = Vector3.zero;

            // Get the base offset assuming forward facing down the z axis of the base
            m_baseOffset += m_controller.Position;

            m_baseOffset /= 2;
        }
	}

    /** returns true if a controller is enabled and not docked */
	private bool IsControllerActive(SixenseInput.Controller controller)
	{
		return ( controller != null && controller.Enabled && !controller.Docked );
	}

	public Quaternion InitialRotation
	{
		get { return m_initialRotation; }
	}
	
	public Vector3 InitialPosition
	{
		get { return m_initialPosition; }
	}

    public float getHydraJoystickX()
    {
        if(IsControllerActive(m_controller))
        {
            return m_controller.JoystickX;
        }
        else
        {
            return 0f;
        }
    }

    public float getHydraJoystickY()
    {
        if(IsControllerActive(m_controller))
        {
            return m_controller.JoystickY;
        }
        else
        {
            return 0f;
        }
    }
}