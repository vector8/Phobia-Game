using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class TopdownUIController : MonoBehaviour
{
    public MonsterController monsterController;

    [System.Serializable]
    public struct StringImageDictEntry
    {
        public string name;
        public Image image;
    }
    public StringImageDictEntry[] buttons;
    
    public Color inactiveColor, activeColor;

    private Dictionary<string, Image> uiButtons;
    private MonsterController.MonsterAbilities lastAbility = MonsterController.MonsterAbilities.None;

    void Start()
    {
        uiButtons = new Dictionary<string, Image>();
        foreach(StringImageDictEntry e in buttons)
        {
            uiButtons.Add(e.name, e.image);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(!monsterController.isFirstPerson)
        {
            if(Input.GetKeyDown(KeyCode.Alpha1))
            {
                monsterController.currentAbility = MonsterController.MonsterAbilities.Sound;
            }
            else if(Input.GetKeyDown(KeyCode.Alpha2))
            {
                monsterController.currentAbility = MonsterController.MonsterAbilities.Trap;
            }
            else if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                monsterController.currentAbility = MonsterController.MonsterAbilities.Morph;
            }

            if(lastAbility != monsterController.currentAbility)
            {
                foreach (Image i in uiButtons.Values)
                {
                    i.color = inactiveColor;
                }

                // highlight which ability is currently in use
                switch (monsterController.currentAbility)
                {
                    case MonsterController.MonsterAbilities.Sound:
                        uiButtons["Sound"].color = activeColor;
                        break;
                    case MonsterController.MonsterAbilities.Trap:
                        uiButtons["Trap"].color = activeColor;
                        break;
                    case MonsterController.MonsterAbilities.Morph:
                        uiButtons["Morph"].color = activeColor;
                        break;
                    default:
                        break;
                }

                lastAbility = monsterController.currentAbility;
            }
        }
    }
}
