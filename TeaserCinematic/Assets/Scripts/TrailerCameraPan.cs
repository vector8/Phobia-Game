using UnityEngine;
using System.Collections;

public class TrailerCameraPan : MonoBehaviour
{
    public const float CAMERA_PAN_DURATION = 20.5f;

    public Vector3 startPos, endPos;

    public Transform lookAtTarget;

    private float timer = 0f;

    // Use this for initialization
    void Start()
    {
        this.transform.position = startPos;
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;

        if (timer > CAMERA_PAN_DURATION)
            timer = CAMERA_PAN_DURATION;

        Vector3 currentPos = Vector3.Lerp(startPos, endPos, timer / CAMERA_PAN_DURATION);
        transform.position = currentPos;

        transform.LookAt(lookAtTarget);
    }
}