using UnityEngine;
using System.Collections;
using CitaNet;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public GameSettings.PlayModes playingAs;
    public GameSettings.HumanControlSchemes humanControlScheme;
    public string serverAddress;
    public int portNumber;

    public Player playerController;
    public MonsterController monsterController;

    private CitaNetManager citaNetMgr;

    // Use this for initialization
    void Start()
    {
        citaNetMgr = CitaNetManager.getInstance();

        if(GameSettings.settingsSetFromMenu)
        {
            playingAs = GameSettings.playingAs;
            humanControlScheme = GameSettings.controlScheme;
            serverAddress = GameSettings.monsterAddress;
            portNumber = GameSettings.port;
        }

        switch (playingAs)
        {
            case GameSettings.PlayModes.Human:
                citaNetMgr.initAsClient(portNumber, serverAddress);
                monsterController.setLocal(false);
                if (humanControlScheme == GameSettings.HumanControlSchemes.Mouse)
                {
                    playerController.setPlayMode(Player.PlayMode.Mouse);
                }
                else
                {
                    playerController.setPlayMode(Player.PlayMode.Hydra);
                }
                break;
            case GameSettings.PlayModes.Monster:
                monsterController.setLocal(true);
                // monster is always server because reasons
                citaNetMgr.initAsServer(portNumber);
                playerController.setPlayMode(Player.PlayMode.Remote);
                break;
            default:
                break;
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Period) && Input.GetKey(KeyCode.RightShift) && Input.GetKey(KeyCode.RightControl))
        {
            SceneManager.LoadScene("Main");
        }
    }
}
