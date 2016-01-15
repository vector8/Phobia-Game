using UnityEngine;
using System.Collections;
using System;

public class BookCaseDoor : Interactable
{
    bool m_Open = false;
    float m_LerpTime = 0.0f;
    float m_Timer = 0.0f;
    public float m_MAX_TIME;
    bool m_Activated = false;
    public Transform rotatePoint;
    float m_Angle = 90.0f;
    Vector3 m_OpenRotation;
    Vector3 m_ClosedRotation;
    Vector3 m_ClosedPosition;
    Vector3 m_OpenPosition;
    // Use this for initialization
    void Start()
    {
        m_ClosedRotation = transform.localEulerAngles;
        m_ClosedPosition = transform.localPosition;
        //Vector3 Temp = transform.position - rotatePoint.position;
        //Temp = Quaternion.Euler(0, 90, 0) * Temp;
        //m_OpenPosition = transform.InverseTransformDirection(Temp + rotatePoint.position);
        //Vector3 temp = (transform.position - rotatePoint.position).normalized;
        transform.RotateAround(rotatePoint.position, transform.up, m_Angle);
        m_OpenRotation = transform.localEulerAngles;
        m_OpenPosition = transform.localPosition;
        transform.RotateAround(rotatePoint.position, transform.up, -m_Angle);
    }

    // Update is called once per frame
    void Update()
    {
        if (m_Activated)
        {
            m_Timer += Time.deltaTime;
            m_LerpTime = m_Timer / m_MAX_TIME;
            if (m_Open)
            {
                transform.localEulerAngles = Vector3.Lerp(m_ClosedRotation, m_OpenRotation, m_LerpTime);
                transform.localPosition = Vector3.Lerp(m_ClosedPosition, m_OpenPosition, m_LerpTime);
            }
            else
            {
                transform.localEulerAngles = Vector3.Lerp(m_OpenRotation, m_ClosedRotation, m_LerpTime);
                transform.localPosition = Vector3.Lerp(m_OpenPosition, m_ClosedPosition, m_LerpTime);
            }
            if (m_Timer > m_MAX_TIME)
            {
                m_Timer = 0.0f;
                m_Activated = false;
            }

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
