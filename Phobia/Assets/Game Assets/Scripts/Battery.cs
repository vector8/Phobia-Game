using UnityEngine;
using CitaNet;

[RequireComponent (typeof(CitaNet.NetworkedObject))]
public class Battery : Interactable
{
    private Player player;

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

        if(player != null && player.numberOfBatteries < 9)
        {
            player.incrementBatteryCount();
            Destroy(gameObject);
        }
        else
        {
            // TODO: tell the player that they cant hold anymore batteries.
        }
    }
}
