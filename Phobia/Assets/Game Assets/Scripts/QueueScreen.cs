using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class QueueScreen : MonoBehaviour
{
    public Button playButton, humanButton, monsterButton;

    // Update is called once per frame
    void Update()
    {

    }

    public void humanButtonPressed()
    {
        playButton.interactable = true;
        humanButton.interactable = false;
        monsterButton.interactable = true;
    }

    public void monsterButtonPressed()
    {
        playButton.interactable = true;
        humanButton.interactable = true;
        monsterButton.interactable = false;
    }

    public void playButtonPressed()
    {
        
    }
}
