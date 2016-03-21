using UnityEngine;
using System.Collections;

public class TutorialExit : MonoBehaviour
{
    public EntryDoor door;
    public GameObject tutorialArea;

    void OnTriggerEnter(Collider c)
    {
        if (c.tag == "Player")
        {
            door.activate(false);

            // disable the tutorial area to prevent fuckery
            tutorialArea.SetActive(false);
        }
    }
}
