using UnityEngine;
using System.Collections;

public class JackPop : MonoBehaviour
{
    public float popBeginTime;
    public float popDuration;
    public Vector3 startPosition, endPosition;

    private float popTimer = 0f;



    // Update is called once per frame
    void Update()
    {
        popTimer += Time.deltaTime;

        if(popTimer >= popBeginTime)
        {
            transform.localPosition = Vector3.Lerp(startPosition, endPosition, (popTimer - popBeginTime) / popDuration);
        }
    }
}
