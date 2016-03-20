using UnityEngine;
using CitaNet;

public abstract class Interactable : MonoBehaviour
{
    public const float STANDARD_ACTIVATION_RANGE = 10f;
    public bool activatable = true;
    public bool activatableByMonster = true;
    public bool usesNetwork = true;

    protected NetworkedObject netObj;

    protected virtual void Start()
    {
        if(usesNetwork)
        {
            netObj = GetComponent<NetworkedObject>();
            netObj.customNetworkMessageFunc = customizeNetworkMessage;
            netObj.customNetworkMessageHandler = customNetworkMessageHandler;
        }
    }

    protected virtual void customizeNetworkMessage(ref NetworkMessage msg)
    {
        msg.setBool("Actvd", true);
    }

    protected virtual void customNetworkMessageHandler(NetworkMessage msg)
    {
        bool result;
        if(msg.getBool("Actvd", out result))
        {
            activate(true);
        }
    }

    public virtual void activate(bool fromNetwork)
    {
        if(!fromNetwork && usesNetwork)
        {
            netObj.sendNetworkUpdate();
        }
    }

    public virtual float getActivationRange()
    {
        return STANDARD_ACTIVATION_RANGE;
    }
}