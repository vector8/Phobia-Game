using UnityEngine;
using System.Collections;

public class MakeJackDisappear : MonoBehaviour
{
    public const float DISAPPEAR_TIME = 22.5f;

    private float timer = 0f;

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= DISAPPEAR_TIME)
            this.gameObject.SetActive(false);
    }
}
