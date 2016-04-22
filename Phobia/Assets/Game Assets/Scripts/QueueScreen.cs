using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class QueueScreen : MonoBehaviour
{
    public Button playButton, humanButton, monsterButton;

    public WaitingInQueue waitingInQueueScreen;
    public LobbyClient lobby;
    public GameObject optionsMenu;

    private int playingAs;

    public void humanButtonPressed()
    {
        playButton.interactable = true;
        humanButton.interactable = false;
        monsterButton.interactable = true;
        playingAs = 0;
    }

    public void monsterButtonPressed()
    {
        playButton.interactable = true;
        humanButton.interactable = true;
        monsterButton.interactable = false;
        playingAs = 1;
    }

    public void playButtonPressed()
    {
        waitingInQueueScreen.time = 0f;
        waitingInQueueScreen.queuedAs = playingAs;

        // send connect message to the server
        CitaNet.NetworkMessage msg = new CitaNet.NetworkMessage();
        msg.setString("T", "C");
        msg.setInt("P", playingAs);
        lobby.sendMessage(msg);

        gameObject.SetActive(false);
        waitingInQueueScreen.gameObject.SetActive(true);
    }

    public void optionsButtonPressed()
    {
        gameObject.SetActive(false);
        optionsMenu.SetActive(true);
    }

    public void exitButtonPressed()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#elif UNITY_STANDALONE
        Application.Quit();
#endif
    }
}
