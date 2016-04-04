using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MonsterGroupButton : MonoBehaviour
{
    public KeyCode hotkey, hotkey2;
    public Image icon;
    public Color inactiveColor, activeColor;

    public GameObject toEnable, toDisable;

    private MonsterController monster;
    private RectTransform rectTransform;

    private bool mouseDownWasOnButton = false;

    void Start()
    {
        monster = FindObjectOfType<MonsterController>();
        rectTransform = GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(hotkey) || (hotkey2 != KeyCode.None && Input.GetKeyDown(hotkey2)) || (Input.GetMouseButtonDown(0) && isMousePositionInRect()))
        {
            icon.color = activeColor;

            if(!Input.GetKeyDown(hotkey))
            {
                mouseDownWasOnButton = true;
            }
        }

        else if(Input.GetKeyUp(hotkey) || (hotkey2 != KeyCode.None && Input.GetKeyUp(hotkey2)) || (mouseDownWasOnButton && Input.GetMouseButtonUp(0) && isMousePositionInRect()))
        {
            monster.currentAbility = MonsterController.MonsterAbilities.None;
            icon.color = inactiveColor;
            toDisable.SetActive(false);
            toEnable.SetActive(true);
        }
    }

    private bool isMousePositionInRect()
    {
        return rectTransform.offsetMin.x <= Input.mousePosition.x && rectTransform.offsetMax.x >= Input.mousePosition.x &&
            rectTransform.offsetMin.y <= Input.mousePosition.y && rectTransform.offsetMax.y >= Input.mousePosition.y;
    }
}
