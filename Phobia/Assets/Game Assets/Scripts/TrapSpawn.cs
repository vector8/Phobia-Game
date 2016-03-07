using UnityEngine;
using System.Collections;

public class TrapSpawn : MonoBehaviour
{
    public float duration;

    private float timer;

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;

        if(timer > duration)
        {
            Destroy(gameObject);
        }
    }
}
