using UnityEngine;
using System.Collections;

public static class GameSettings
{
    public enum PlayModes
    {
        Human,
        Monster
    }

    public enum HumanControlSchemes
    {
        Mouse,
        Hydra
    }

    public static bool settingsSetFromMenu = false;
    public static PlayModes playingAs;
    public static HumanControlSchemes controlScheme;
    public static string monsterAddress;
    public static int port = 8888;

    public static bool scoreNeedsUpdating = false;
    public static int lastScore;
}
