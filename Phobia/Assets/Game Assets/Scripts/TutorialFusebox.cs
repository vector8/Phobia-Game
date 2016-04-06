using UnityEngine;
using System.Collections;

public class TutorialFusebox : Interactable
{
    public GameObject fuse;
    public Player player;
    public Door exitDoor;

    private AudioSource audioSource;

    // Use this for initialization
    protected override void Start()
    {
        base.Start();
        player = FindObjectOfType<Player>();
        audioSource = GetComponent<AudioSource>();
    }

    public override void activate(bool fromNetwork)
    {
        base.activate(fromNetwork);

        if(player.numberOfFuses > 0)
        {
            audioSource.Play();
            fuse.SetActive(true);
            player.setFuseCount(0);
            activatable = false;
            exitDoor.activate(false);
        }
    }
}
