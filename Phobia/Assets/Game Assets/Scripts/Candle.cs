using UnityEngine;
using System.Collections;

public class Candle : MonoBehaviour
{
    public float REIGNITION_TIME = 10f;
    public GameObject flame;

    private float reignitionTimer = 0f;

    // Update is called once per frame
    void Update()
    {
        if(!flame.activeSelf)
        {
            reignitionTimer += Time.deltaTime;

            if(reignitionTimer >= REIGNITION_TIME)
            {
                flame.SetActive(true);
                reignitionTimer = 0f;
            }
        }
    }

    public void extinguishFlame()
    {
        flame.SetActive(false);
        reignitionTimer = 0f;
    }
}
