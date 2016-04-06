using UnityEngine;
using System.Collections;

public class TeddyBear : MonoBehaviour
{
    public GameObject particles;
    private float despawnTimer = 0f;
    private float lifetime = 3f;
    private float triggerDistance = 12.5f;

    private bool triggered = false;

    private GameObject player;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        if(player == null)
        {
            player = GameObject.FindGameObjectWithTag("RemotePlayer");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(!triggered && Vector3.Distance(transform.position, player.transform.position) <= triggerDistance)
        {
            triggered = true;
            particles.SetActive(true);
            GetComponent<AudioSource>().Play();
        }

        if(triggered)
        {
            despawnTimer += Time.deltaTime;

            if(despawnTimer > lifetime)
            {
                Destroy(gameObject);
            }
        }
    }
}
