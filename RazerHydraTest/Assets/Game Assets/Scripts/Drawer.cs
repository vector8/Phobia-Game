using UnityEngine;
using System.Collections;
using System;

public class Drawer : Interactable
{
    bool m_open = false;
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public override void activate()
    {
        if (m_open)
        {
            transform.Translate(0f,0f,(gameObject.transform.forward.z * -0.5f));
            m_open = false;
        }
        else
        {
            transform.Translate(0f, 0f, gameObject.transform.forward.z * 0.5f);
            m_open = true;
        }
    }
}
