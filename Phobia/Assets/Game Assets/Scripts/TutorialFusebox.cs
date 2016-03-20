using UnityEngine;
using System.Collections;

public class TutorialFusebox : Interactable
{
    public GameObject fuse;
    public Player player;
    public Door exitDoor;

    // Use this for initialization
    protected override void Start()
    {
        base.Start();
        player = FindObjectOfType<Player>();
    }

    public override void activate(bool fromNetwork)
    {
        base.activate(fromNetwork);

        if(player.numberOfFuses > 0)
        {
            fuse.SetActive(true);
            player.setFuseCount(0);
            activatable = false;
            exitDoor.activate(false);
        }
    }
}
