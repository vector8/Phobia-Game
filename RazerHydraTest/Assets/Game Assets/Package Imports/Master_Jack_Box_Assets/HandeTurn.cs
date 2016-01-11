using UnityEngine;
using System.Collections;

public class HandeTurn : MonoBehaviour
{
    public float turnVelocity;
    public float TimerSync;
    // Use this for initialization
    void Start()
    {
        TimerSync = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        if(TimerSync + 18f > Time.time)
        transform.Rotate(transform.forward, turnVelocity * Time.deltaTime);
    }
}
