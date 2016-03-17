using UnityEngine;
using System.Collections;
using System;

[RequireComponent (typeof(CitaNet.NetworkedObject))]
public class Drawer : Interactable
{
    public AudioClip m_AudioOpen;
    public AudioClip m_AudioClose;
    public float m_TotalTime;
    public float openPosition;

    private bool m_open = false;
    private AudioSource cached_AS;
    private float m_Timer;
    private bool m_Activated = false;
    private float m_LerpTime;
    private Vector3 m_OpenPosition;
    private Vector3 m_ClosedPosition;

    // Use this for initialization
    protected override void Start()
    {
        base.Start();

        cached_AS = GetComponent<AudioSource>();
        m_ClosedPosition = transform.localPosition;
        m_OpenPosition = transform.localPosition;
        m_OpenPosition.z = openPosition;
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

    public override void activate(bool fromNetwork)
    {
        base.activate(fromNetwork);

        if (m_open)
        {
            cached_AS.clip = m_AudioClose;
            cached_AS.Play();
            m_Timer = 0.0f;
            m_Activated = true;
            m_open = false;
        }
        else
        {
            cached_AS.clip = m_AudioOpen;
            cached_AS.Play();
            m_Timer = 0.0f;
            m_Activated = true;
            m_open = true;
        }
    }
}
