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
    public FuseBox fuseBox;

    public Player playerController;
    public MonsterController monsterController;

    private float resetTimer = 0f;
    private const float RESET_DELAY = 5f;
    private float gameTimer = 0f;

    private const float TWENTY_MINS_IN_SECS = 1200;
    private const float FIFTEEN_MINS_IN_SECS = 900;
    private const int FUSE_BONUS = 1000;
    private const float MAX_TIME_BONUS = 20000;
    private const int WIN_BONUS = 5000;

    private CitaNetManager citaNetMgr;

    // Use this for initialization
    void Start()
    {
        citaNetMgr = CitaNetManager.getInstance();

        if (GameSettings.settingsSetFromMenu)
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
        if (playerController.dead || playerController.won)
        {
            resetTimer += Time.deltaTime;
            if (resetTimer >= RESET_DELAY)
            {
                citaNetMgr.cleanUp();
                GameSettings.scoreNeedsUpdating = true;
                int score = 0;
                if (playingAs == GameSettings.PlayModes.Human)
                {
                    // Calculate time bonus
                    if (gameTimer < TWENTY_MINS_IN_SECS)
                    {
                        if (playerController.won)
                        {
                            // lower time = greater score
                            score = (int)Mathf.Lerp(MAX_TIME_BONUS, 0, gameTimer / TWENTY_MINS_IN_SECS);
                            score += WIN_BONUS;
                        }
                        else // playerController.dead
                        {
                            // greater time = greater score
                            score = (int)Mathf.Lerp(0, MAX_TIME_BONUS, gameTimer / TWENTY_MINS_IN_SECS);
                        }
                    }
                    int fuseBonus = fuseBox.fusesActive * FUSE_BONUS;
                    score += fuseBonus;
                }
                else // playingAs == GameSettings.PlayModes.Monster
                {
                    if (playerController.dead)
                    {
                        if (gameTimer < FIFTEEN_MINS_IN_SECS)
                        {
                            // lower time = greater score
                            score = (int)Mathf.Lerp(MAX_TIME_BONUS, 0, gameTimer / FIFTEEN_MINS_IN_SECS);
                            score += WIN_BONUS;
                        }
                    }
                }

                GameSettings.lastScore = score;
                SceneManager.LoadScene("Menu");
            }
        }
        else
        {
            gameTimer += Time.deltaTime;
        }
    }
}
