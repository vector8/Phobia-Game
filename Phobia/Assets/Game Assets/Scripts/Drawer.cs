using UnityEngine;
using System.Collections;
using System;

public class Drawer : Interactable
{
    public AudioClip m_AudioOpen;
    public AudioClip m_AudioClose;
    public float m_TotalTime;

    private bool m_open = false;
    private AudioSource cached_AS;
    private float m_Timer;
    private bool m_Activated = false;
    private float m_LerpTime;
    private Vector3 m_OpenPosition;
    private Vector3 m_ClosedPosition;

    // Use this for initialization
    void Start()
    {
        cached_AS = GetComponent<AudioSource>();
        m_ClosedPosition = transform.localPosition;
        m_OpenPosition = transform.localPosition;
        m_OpenPosition.z = 1.67f;
    }

    // Update is called once per frame
    void Update()
    {
        if (m_Activated)
        {
            m_Timer += Time.deltaTime;


            m_LerpTime = m_Timer / m_TotalTime;

            if (m_open)
            {
                transform.localPosition = Vector3.Lerp(m_ClosedPosition, m_OpenPosition, m_LerpTime);
            }
            else
            {
                transform.localPosition = Vector3.Lerp(m_OpenPosition, m_ClosedPosition, m_LerpTime);
            }

            if (m_Timer > m_TotalTime)
            {
                m_Timer = 0.0f;
                m_Activated = false;
            }
        }


    }

    public override void activate()
    {
        if (m_open)
        {
            cached_AS.clip = m_AudioClose;
            cached_AS.Play();
            m_Timer = 0.0f;
            m_Activated = true;
            //transform.position -= transform.forward * 1.3f;
            m_open = false;
        }
        else
        {
            cached_AS.clip = m_AudioOpen;
            cached_AS.Play();
            m_Timer = 0.0f;
            m_Activated = true;
            //transform.position += transform.forward * 1.3f;
            m_open = true;
        }
    }
}
