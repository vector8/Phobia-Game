using UnityEngine;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace CitaNet
{
    public class CitaNetManager : MonoBehaviour
    {
        private struct GameObjNetObjPair
        {
            public GameObject gObj;
            public NetworkedObject netObj;
        }

        public float artificialLag = 0f;
        private Dictionary<int, GameObjNetObjPair> networkedObjects = new Dictionary<int, GameObjNetObjPair>();
        private Queue<NetworkMessage> messageQueue = new Queue<NetworkMessage>();
        private bool initialized = false;
        private int maxID = 0;

        public static CitaNetManager getInstance()
        {
            return FindObjectOfType<CitaNetManager>();
        }

        void Start()
        {
            Application.runInBackground = true;
        }

        // Update is called once per frame
        void Update()
        {
            if (initialized)
            {
                if (Input.GetKeyDown(KeyCode.PageUp))
                {
                    artificialLag += 0.5f;
                }
                else if (Input.GetKeyDown(KeyCode.PageDown))
                {
                    artificialLag -= 0.5f;
                    if(artificialLag < 0f)
                    {
                        artificialLag = 0f;
                    }
                }

                bool received = CitaNetWrapper.hasReceived();
                checkErrors();
                if (received)
                {
                    string rawMessage = CitaNetWrapper.getLastReceivedMessage();

                    Debug.Log(rawMessage);

                    NetworkMessage msg = new NetworkMessage(rawMessage);

                    int instanceID;
                    if (msg.getInt(NetworkedObject.ID_KEY, out instanceID))
                    {
                        GameObjNetObjPair objs;
                        if (networkedObjects.TryGetValue(instanceID, out objs))
                        {
                            objs.netObj.receiveNetworkMessage(msg);
                        }
                    }
                }

                // process send queue
                while(messageQueue.Count > 0 && Time.time >= messageQueue.Peek().sendTime + artificialLag)
                {
                    print("sending message at " + Time.time);
                    NetworkMessage msg = messageQueue.Dequeue();
                    CitaNetWrapper.sendMsg(msg.ToString());
                }
            }
        }

        /**
         * This method should only be called in the Start() function.
         */
        public void registerNetworkedObject(GameObject gObj, ref NetworkedObject netObj)
        {
            //netObj.networkID = maxID;
            //maxID++;
            //print("Name: " + gObj.name + " ID: " + netObj.networkID);
            GameObjNetObjPair objs;
            objs.gObj = gObj;
            objs.netObj = netObj;
            networkedObjects.Add(netObj.networkID, objs);
        }

        private bool checkErrors()
        {
            if (CitaNetWrapper.hasError())
            {
                Debug.Log(CitaNetWrapper.getErrorMessage());
                return true;
            }

            return false;
        }

        public bool initAsServer(int port)
        {
            CitaNetWrapper.initialize(port, "");
            bool error = checkErrors();

            if (error)
            {
                CitaNetWrapper.cleanUp();
                return false;
            }
            else
            {
                initialized = true;
                return true;
            }
        }

        public bool initAsClient(int port, string serverAddress)
        {
            CitaNetWrapper.initialize(port, serverAddress);
            bool error = checkErrors();

            if (error)
            {
                CitaNetWrapper.cleanUp();
                return false;
            }
            else
            {
                initialized = true;
                return true;
            }
        }

        public void sendMessage(NetworkMessage msg)
        {
            msg.sendTime = Time.time;
            print("queuing message at " + msg.sendTime);
            messageQueue.Enqueue(msg);
        }

        void OnApplicationQuit()
        {
            if(initialized)
            {
                CitaNetWrapper.cleanUp();
            }
        }

        private static class CitaNetWrapper
        {
            private const string DLL_NAME = "CitaNet";

            [DllImport(DLL_NAME)]
            public static extern void initialize(int port, string serverAddress);

            [DllImport(DLL_NAME)]
            public static extern void sendMsg(string msg);

            [DllImport(DLL_NAME)]
            public static extern bool hasReceived();

            [DllImport(DLL_NAME)]
            private static extern System.IntPtr getLastReceived();

            [DllImport(DLL_NAME)]
            public static extern bool hasError();

            [DllImport(DLL_NAME)]
            private static extern System.IntPtr getError();

            [DllImport(DLL_NAME)]
            public static extern void cleanUp();

            public static string getLastReceivedMessage()
            {
                return Marshal.PtrToStringAnsi(getLastReceived());
            }

            public static string getErrorMessage()
            {
                return Marshal.PtrToStringAnsi(getError());
            }
        }
    }
}
