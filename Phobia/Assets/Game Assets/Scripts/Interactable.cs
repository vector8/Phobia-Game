using UnityEngine;
using CitaNet;

[RequireComponent (typeof(NetworkedObject))]
public abstract class Interactable : MonoBehaviour
{
    public const float STANDARD_ACTIVATION_RANGE = 10f;

    protected NetworkedObject netObj;

    protected virtual void Start()
    {
        netObj = GetComponent<NetworkedObject>();
        netObj.customNetworkMessageFunc = customizeNetworkMessage;
        netObj.customNetworkMessageHandler = customNetworkMessageHandler;
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
        if(!fromNetwork)
        {
            netObj.sendNetworkUpdate();
        }
    }

    public virtual float getActivationRange()
    {
        return STANDARD_ACTIVATION_RANGE;
    }
}