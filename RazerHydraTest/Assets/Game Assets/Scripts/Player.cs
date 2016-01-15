using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Player : MonoBehaviour
{
    public Transform controllerToFollow;

    public const float MAX_BATTERY_LEVEL = 73f;
    public float batteryLevel;
    public float batteryDrainRate = 1f;
    public Image batteryFillUI;
    public Image batteryOutlineUI;
    public GameObject hydraFlashlight;
    public GameObject mouseFlashlight;

    public FlashlightController flashlightController;
    private bool usingHydra = true;

    private float batteryFlashTimer;
    private const float BATTERY_FLASH_DURATION = 0.5f;
    private bool batteryRed = false;

    // Use this for initialization
    void Start()
    {
        GameObject temp = GameObject.Find("HydraController");
        if(temp == null)
        {
            temp = GameObject.Find("MouseController");
            usingHydra = false;
        }

        controllerToFollow = temp.transform;

        batteryLevel = MAX_BATTERY_LEVEL;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = controllerToFollow.position;
        transform.rotation = controllerToFollow.rotation;
        transform.localScale = controllerToFollow.localScale;

        updateFlashlight();
    }

    private void updateFlashlight()
    {
        if(batteryLevel > 0f)
        {
            bool flashlightOn = false;

            if(usingHydra)
            {
                if (flashlightController.m_controller.GetButtonDown(SixenseButtons.TWO))
                {
                    // Toggle flashlight
                    hydraFlashlight.SetActive(!hydraFlashlight.activeSelf);
                }
                flashlightOn = hydraFlashlight.activeSelf;
            }
            else // using mouse + keyboard
            {
                if (Input.GetKeyDown(KeyCode.Q))
                {
                    // Toggle flashlight
                    mouseFlashlight.SetActive(!mouseFlashlight.activeSelf);
                }
                flashlightOn = mouseFlashlight.activeSelf;
            }

            if(flashlightOn)
            {
                batteryLevel -= batteryDrainRate * Time.deltaTime;

                Vector2 dimensions = batteryFillUI.rectTransform.sizeDelta;
                dimensions.x = batteryLevel;
                batteryFillUI.rectTransform.sizeDelta = dimensions;
            }
        }
        else
        {
            batteryLevel = 0f;
            mouseFlashlight.SetActive(false);
            hydraFlashlight.SetActive(false);
        }

        if(batteryLevel < 20f && batteryLevel > 0f)
        {
            batteryFlashTimer += Time.deltaTime;
            if(batteryFlashTimer >= BATTERY_FLASH_DURATION)
            {
                batteryRed = !batteryRed;
                batteryFlashTimer = 0f;
                
                if(batteryRed)
                {
                    batteryOutlineUI.color = Color.red;
                    batteryFillUI.color = Color.red;
                }
                else
                {
                    batteryOutlineUI.color = Color.white;
                    batteryFillUI.color = Color.white;
                }
            }
        }
        else
        {
            //batteryIndicator.SetActive(true);
            batteryRed = false;
            batteryFillUI.color = Color.white;
            batteryOutlineUI.color = Color.white;
            batteryFlashTimer = 0f;
        }
    }
}
