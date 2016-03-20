using UnityEngine;
using System.Collections;

public class TutorialClown : MonoBehaviour
{
    private Player player;
    private MonsterController monsterController;
    private float inLightTimer = 0f;

    // Use this for initialization
    void Start()
    {
        player = FindObjectOfType<Player>();
        monsterController = FindObjectOfType<MonsterController>();
    }

    // Update is called once per frame
    void Update()
    {
        Light playerLight = player.controllerElements.flashlight;

        // check if we are hit by the player's flashlight
        if (playerLight.gameObject.activeSelf)
        {
            RaycastHit hit;
            Ray ray = new Ray(playerLight.transform.position, playerLight.transform.forward);

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.gameObject.GetInstanceID() == gameObject.GetInstanceID())
                {
                    inLightTimer += Time.deltaTime;

                    if (inLightTimer > monsterController.MAX_TIME_IN_LIGHT)
                    {
                        gameObject.SetActive(false);
                    }
                }
            }
        }
    }
}