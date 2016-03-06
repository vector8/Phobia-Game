using UnityEngine;
using System.Collections;

namespace CitaNet
{
    public class NetworkedObject : MonoBehaviour
    {
        public const string INSTANCE_ID_KEY = "InstID";

        public int networkID;

        public delegate void NetworkMessageCustomizer(ref NetworkMessage msg);
        public delegate void CustomNetworkMessageHandler(NetworkMessage msg);

        public NetworkMessageCustomizer customNetworkMessageFunc;
        public CustomNetworkMessageHandler customNetworkMessageHandler;

        private CitaNetManager citaNetMgr;

        void Start()
        {
            citaNetMgr = CitaNetManager.getInstance();
            citaNetMgr.registerNetworkedObject(gameObject, this);
        }

        private NetworkMessage getNetworkMessage()
        {
            NetworkMessage msg = new NetworkMessage();
            msg.setInt(INSTANCE_ID_KEY, networkID);
            if(customNetworkMessageFunc != null)
            {
                customNetworkMessageFunc(ref msg);
            }
            else    // Default logic - just position
            {
                msg.setFloat("PosX", transform.position.x);
                msg.setFloat("PosY", transform.position.y);
                msg.setFloat("PosZ", transform.position.z);
            }
            return msg;
        }

        public void sendNetworkUpdate()
        {
            NetworkMessage msg = getNetworkMessage();

            citaNetMgr.sendMessage(msg);
        }

        public void receiveNetworkMessage(NetworkMessage msg)
        {
            if(customNetworkMessageHandler != null)
            {
                customNetworkMessageHandler(msg);
            }
            else
            {
                float x = 0, y = 0, z = 0;
                bool success = true;
                success = success && msg.getFloat("PosX", out x) && msg.getFloat("PosY", out y) && msg.getFloat("PosZ", out z);

                if(success)
                {
                    transform.position = new Vector3(x, y, z);
                }
            }
        }
    }
}