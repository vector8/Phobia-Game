using UnityEngine;
using System.Collections;

public class BoxTop_S : MonoBehaviour
{
    public float m_rotateVelocity;
    public float m_TimerSync;
    // Use this for initialization
    void Start()
    {
        m_TimerSync = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.rotation.z < 0.90f && Time.time > m_TimerSync + 11.2f)
        {
            transform.Rotate(transform.forward, m_rotateVelocity * Time.deltaTime);
        }
    }
}
