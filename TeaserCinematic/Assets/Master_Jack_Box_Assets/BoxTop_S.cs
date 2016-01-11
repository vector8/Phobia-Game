using UnityEngine;
using System.Collections;

public class BoxTop_S : MonoBehaviour
{
    public const float DURATION = 22.5f;//11.2f;

    public float m_rotateVelocity;

    private float timer = 0f;

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;

        if (transform.rotation.z < 0.90f && timer > DURATION)
        {
            transform.Rotate(transform.forward, m_rotateVelocity * Time.deltaTime);
        }
    }
}
