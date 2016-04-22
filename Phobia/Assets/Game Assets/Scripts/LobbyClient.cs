using UnityEngine;
using System.Collections.Generic;
using System;
using System.IO;
using System.Runtime.InteropServices;

public class LobbyClient : MonoBehaviour
{
    public int port = 8889;
    public string serverAddress;
    public WaitingInQueue waitingInQueueScreen;

    private bool initialized = false;

    void Start()
    {
        Application.runInBackground = true;

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        try
        {
            TextReader tr = new StreamReader("server.cfg");
            string address = tr.ReadLine();
            serverAddress = address.Trim();
            tr.Close();
        }
        catch (Exception e)
        {
            Debug.Log("Could not read from server.cfg");
        }

        CitaNetWrapper.initialize(port, serverAddress);
        bool error = checkErrors();

        if (error)
        {
            CitaNetWrapper.cleanUp();
        }
        else
        {
            initialized = true;
        }

        if (GameSettings.scoreNeedsUpdating)
        {
            CitaNet.NetworkMessage msg = new CitaNet.NetworkMessage();
            msg.setString("T", "U");
            msg.setInt("S", GameSettings.lastScore);
            sendMessage(msg);
            GameSettings.scoreNeedsUpdating = false;

            // temp code, retrieve scores list here since we dont have a high scores screen
            msg.setString("T", "G");
            sendMessage(msg);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (initialized)
        {
            bool received = CitaNetWrapper.hasReceived();
            checkErrors();
            if (received)
            {
                string rawMessage = CitaNetWrapper.getLastReceivedMessage();

                Debug.Log(rawMessage);

                CitaNet.NetworkMessage msg = new CitaNet.NetworkMessage(rawMessage);

                waitingInQueueScreen.networkMessageReceived(msg);
            }
        }
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

    public void sendMessage(CitaNet.NetworkMessage msg)
    {
        CitaNetWrapper.sendMsg(msg.ToString());
    }

    public void cleanUp()
    {
        if (initialized)
        {
            CitaNetWrapper.cleanUp();
        }
    }

    void OnApplicationQuit()
    {
        cleanUp();
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