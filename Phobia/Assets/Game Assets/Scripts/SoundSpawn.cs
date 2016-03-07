using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class SoundSpawn : MonoBehaviour
{
    AudioSource sound;

    // Use this for initialization
    void Start()
    {
        sound = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if(!sound.isPlaying)
        {
            // sound is finished, despawn
            Destroy(gameObject);
        }
    }
}
