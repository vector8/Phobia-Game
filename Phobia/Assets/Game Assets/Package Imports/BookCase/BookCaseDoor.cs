using UnityEngine;
using System.Collections;
using System;

public class BookCaseDoor : Interactable
{
    public float m_MAX_TIME;
    public Transform rotatePoint;
    public float openAngle = 90.0f;

    private bool m_Open = false;
    private float m_Timer = 0.0f;
    private bool m_Activated = false;

    // Use this for initialization
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (m_Activated)
        {
            m_Timer += Time.deltaTime;
            float u = m_Timer / m_MAX_TIME;
            if(u >= 1f)
            {
                u = 1f;
                m_Activated = false;
                m_Timer = 0f;
            }

            float yAngle;
            if (m_Open)
            {
                yAngle = Mathf.Lerp(0, openAngle, u);
            }
            else
            {
                yAngle = Mathf.Lerp(openAngle, 0, u);
            }

            Vector3 euler = rotatePoint.localEulerAngles;
            euler.y = yAngle;
            rotatePoint.localEulerAngles = euler;
        }
    }

    public override void activate()
    {
        if (m_Open)
        {
            m_Timer = 0.0f;
            m_Activated = true;
            m_Open = false;
        }
        else
        {
            m_Timer = 0.0f;
            m_Activated = true;
            m_Open = true;
        }
    }
}
