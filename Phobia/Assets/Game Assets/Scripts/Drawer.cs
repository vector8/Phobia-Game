using UnityEngine;
using System.Collections;
using System;

public class Drawer : Interactable
{
    bool m_open = false;
    float m_Range = 10f;
    public AudioClip m_Open;
    public AudioClip m_Close;
    AudioSource cached_AS;
    // Use this for initialization
    void Start()
    {
        cached_AS = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public override void activate()
    {
        if (m_open)
        {
            cached_AS.clip = m_Close;
            cached_AS.Play();
            transform.position -= transform.forward * 1.3f;
            m_open = false;
        }
        else
        {
            cached_AS.clip = m_Open;
            cached_AS.Play();
            transform.position += transform.forward * 1.3f;
            m_open = true;
        }
    }

    public override float getActivationRange()
    {
        return m_Range;
    }
}
