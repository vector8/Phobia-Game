using UnityEngine;
using System.Collections;

public class EntryScreen : MonoBehaviour
{
    public GameObject queueScreen;

    // Update is called once per frame
    void Update()
    {
        if(Input.anyKey)
        {
            queueScreen.SetActive(true);
            gameObject.SetActive(false);
        }
    }
}
