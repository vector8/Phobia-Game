using UnityEngine;
using System.Collections;

public class OptionsMenu : MonoBehaviour
{
    public GameObject queueScreen;
    
    public void backButtonPressed()
    {
        gameObject.SetActive(false);
        queueScreen.SetActive(true);
    }

    public void mouseCtrlSchemeValueChanged(bool value)
    {
        if(value)
        {
            GameSettings.controlScheme = GameSettings.HumanControlSchemes.Mouse;
            GameSettings.settingsSetFromMenu = true;
        }
    }

    public void hydraCtrlSchemeValueChanged(bool value)
    {
        if (value)
        {
            GameSettings.controlScheme = GameSettings.HumanControlSchemes.Hydra;
            GameSettings.settingsSetFromMenu = true;
        }
    }
}
