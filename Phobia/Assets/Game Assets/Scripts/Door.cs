using UnityEngine;
using System.Collections;
using System;

public class Door : Interactable
{

    public float MAX_TIME;
    public Transform rotatePoint;
    public float openAngle = 90.0f;

    private bool open = false;
    private float timer = 0.0f;
    private bool activated = false;

    // Update is called once per frame
    void Update()
    {
        if (activated)
        {
            timer += Time.deltaTime;
            float u = timer / MAX_TIME;
            if (u >= 1f)
            {
                u = 1f;
                activated = false;
                timer = 0f;
            }

            float yAngle;
            if (open)
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
        if (open)
        {
            timer = 0.0f;
            activated = true;
            open = false;
        }
        else
        {
            timer = 0.0f;
            activated = true;
            open = true;
        }
    }
}
