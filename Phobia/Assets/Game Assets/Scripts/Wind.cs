using UnityEngine;
using System.Collections;

public class Wind : MonoBehaviour
{
    private float lifetime = 3f;
    private float MAX_EXTINGUISH_DISTANCE = 40f;
    private GameObject[] candles;
    private float timer = 0f;

    // Use this for initialization
    void Start()
    {
        candles = GameObject.FindGameObjectsWithTag("Candle");
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;

        foreach(GameObject g in candles)
        {
            if(Vector3.Distance(transform.position, g.transform.position) < MAX_EXTINGUISH_DISTANCE)
            {
                Candle c = g.GetComponent<Candle>();
                c.extinguishFlame();
            }
        }

        if (timer > lifetime)
        {
            Destroy(gameObject);
        }
    }
}
