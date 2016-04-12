using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class WaitingInQueue : MonoBehaviour
{
    public float time;
    public Text timeText;
    public int queuedAs;
    public LobbyClient lobby;
    public GameObject queueUpScreen;

    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime;

        int minutes = (int)(time / 60);
        int seconds = ((int)time) % 60;

        string secString = seconds.ToString();
        if(seconds < 10)
        {
            secString = "0" + secString;
        }
        timeText.text = minutes.ToString() + ":" + secString;
    }

    public void cancel()
    {
        // send disconnect message to server
        CitaNet.NetworkMessage msg = new CitaNet.NetworkMessage();
        msg.setString("D", "");
        lobby.sendMessage(msg);
        gameObject.SetActive(false);
        queueUpScreen.SetActive(true);
    }

    public void networkMessageReceived(CitaNet.NetworkMessage msg)
    {
        if (gameObject.activeSelf)
        {
            string type;
            msg.getString("T", out type);
            if (type == "M")
            {
                // match found.
                string address;
                msg.getString("A", out address);

                // set game settings and switch scenes
                GameSettings.monsterAddress = address;
                GameSettings.settingsSetFromMenu = true;
                GameSettings.playingAs = (GameSettings.PlayModes)queuedAs;
                lobby.cleanUp();
                SceneManager.LoadScene("Main");
            }
        }
    }
}
