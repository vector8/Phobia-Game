using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using CitaNet;

public class GameManager : MonoBehaviour
{
    public GameSettings.PlayModes playingAs;
    public GameSettings.HumanControlSchemes humanControlScheme;
    public string serverAddress;
    public int portNumber;

    public Player playerController;
    public MonsterController monsterController;

    private float resetTimer = 0f;
    private const float RESET_DELAY = 5f;

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
        if(playerController.dead || playerController.won)
        {
            resetTimer += Time.deltaTime;
            if(resetTimer >= RESET_DELAY)
            {
                citaNetMgr.cleanUp();
                SceneManager.LoadScene("Menu");
            }
        }
    }
}
