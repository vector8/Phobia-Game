using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MonsterAbilityButton : MonoBehaviour
{
    public KeyCode hotkey;
    public MonsterController.MonsterAbilities ability;
    public Image icon;
    public Color inactiveColor, activeColor, disabledColor;
    public Text cooldownText;

    private MonsterController monster;
    private RectTransform rectTransform;

    void Start()
    {
        monster = FindObjectOfType<MonsterController>();
        rectTransform = GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void Update()
    {
        if (monster.abilityCooldownTimers[ability] < monster.abilityCooldowns[ability])
        {
            cooldownText.gameObject.SetActive(true);
            int timeLeft = (int)(monster.abilityCooldowns[ability] - monster.abilityCooldownTimers[ability]);
            cooldownText.text = timeLeft.ToString();
            icon.color = disabledColor;
        }
        else
        {
            cooldownText.gameObject.SetActive(false);
            if (Input.GetKeyDown(hotkey) || (Input.GetMouseButtonDown(0) && isMousePositionInRect()))
            {
                monster.currentAbility = ability;
                icon.color = activeColor;
            }
            else if (monster.currentAbility != ability)
            {
                icon.color = inactiveColor;
            }
        }
    }

    private bool isMousePositionInRect()
    {
        return rectTransform.offsetMin.x <= Input.mousePosition.x && rectTransform.offsetMax.x >= Input.mousePosition.x &&
            rectTransform.offsetMin.y <= Input.mousePosition.y && rectTransform.offsetMax.y >= Input.mousePosition.y;
    }
}
