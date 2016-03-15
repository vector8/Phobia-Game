using UnityEngine;
using System.Collections;

public class FlameLight : MonoBehaviour
{
    public Light target;
    public Vector2 positionRange;
    public Vector2 intensityRange;

    //private Vector3 originalPosition;

    void Start()
    {
        //originalPosition = target.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 newPosition = new Vector3(positionRange.x, positionRange.x, positionRange.x);
        target.transform.localPosition = newPosition + (positionRange.y - positionRange.x) * 
            new Vector3(Mathf.Pow(Mathf.PerlinNoise(Time.time, 0f), Mathf.PerlinNoise(Time.time, 0f)), 
                        Mathf.Pow(Mathf.PerlinNoise(Time.time, 0.2f), Mathf.PerlinNoise(Time.time, 0.2f)), 
                        Mathf.Pow(Mathf.PerlinNoise(Time.time, 0.4f), Mathf.PerlinNoise(Time.time, 0.4f)));
        target.intensity = intensityRange.x + (intensityRange.y - intensityRange.x) * Mathf.Pow(Mathf.PerlinNoise(Time.time, 0f), Mathf.PerlinNoise(Time.time, 0f));
    }
}
