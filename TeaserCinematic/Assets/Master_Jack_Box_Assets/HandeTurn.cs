using UnityEngine;
using System.Collections;

public class HandeTurn : MonoBehaviour
{
    public const float DURATION = 30.5f;//18f;

    public float turnVelocity;
    public float timer;

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;

        if(timer < DURATION)
            transform.Rotate(transform.forward, turnVelocity * Time.deltaTime);
    }
}
