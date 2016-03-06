using UnityEngine;
using CitaNet;

[RequireComponent (typeof(NetworkedObject))]
public class Fuse : Interactable
{
    private Player player;

    // Use this for initialization
    protected override void Start()
    {
        base.Start();

        player = FindObjectOfType<Player>();
    }

    protected override void customNetworkMessageHandler(NetworkMessage msg)
    {
        bool result;
        if (msg.getBool("Actvd", out result))
        {
            // don't activate here if it was activated remotely, just remove from game
            Destroy(gameObject);
        }
    }

    public override void activate(bool fromNetwork)
    {
        base.activate(fromNetwork);

        player.incrementFuseCount();
        Destroy(gameObject);
    }
}
