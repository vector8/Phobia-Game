using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour
{
    [System.Serializable]
    public struct ControllerElements
    {
        public GameObject controller;
        public NumberDisplayController batteryNumberUI;
        public SpriteRenderer batteryFillUI, batteryOutlineUI;
        public GameObject batteryFillScalePivot;
        public Light flashlight;
        public FlashlightController flashlightController;
        public GameObject handUI;
    }

    public ControllerElements mouseElements, hydraElements;
    public ControllerElements controllerElements;

    public bool usingHydra;

    public float MAX_BATTERY_LEVEL = 100f;
    public float BATTERY_RELOAD_TIME = 3f;
    public float BATTERY_RELOAD_AMOUNT = 35f;

    public float batteryLevel;
    public float batteryDrainRate = 1f;
    public bool flashlightOn = true;
    public bool useFlashlightBatteryDimming = false;
    public int numberOfBatteries = 0;

    private Transform controllerToFollow;
    private float batteryFlashTimer;
    private const float BATTERY_FLASH_DURATION = 0.5f;
    private bool batteryRed = false;
    private float batteryFlickerTimer = 0f;
    private bool reloading = false;
    private float reloadTimer = 0f;

    // Use this for initialization
    void Start()
    {
        if(usingHydra)
        {
            controllerElements = hydraElements;
            mouseElements.controller.SetActive(false);
            controllerToFollow = controllerElements.controller.transform;
        }
        else
        {
            controllerElements = mouseElements;
            hydraElements.controller.SetActive(false);
            controllerToFollow = controllerElements.controller.transform;
        }

        batteryLevel = MAX_BATTERY_LEVEL;

        // Disable mouse cursor if this is a build
#if UNITY_EDITOR
        Cursor.visible = true;
#elif UNITY_STANDALONE
        Cursor.visible = false;
#endif
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = controllerToFollow.position;
        transform.rotation = controllerToFollow.rotation;
        transform.localScale = controllerToFollow.localScale;

        checkForReload();
        if(!reloading)
        {
            updateFlashlight();
        }
    }

    private void updateFlashlight()
    {
        if(batteryLevel > 0f)
        {
            // If flashlight button has been pressed
            if((usingHydra && controllerElements.flashlightController.m_controller.GetButtonDown(SixenseButtons.TWO)) || (!usingHydra && Input.GetKeyDown(KeyCode.Q)))
            {
                // Toggle flashlight
                flashlightOn = !flashlightOn;
                controllerElements.flashlight.gameObject.SetActive(flashlightOn);
            }

            if(flashlightOn)
            {
                batteryLevel -= batteryDrainRate * Time.deltaTime;

                updateBatteryUI();
            }
        }
        else
        {
            batteryLevel = 0f;
            controllerElements.flashlight.gameObject.SetActive(false);
            flashlightOn = false;
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
                    controllerElements.batteryOutlineUI.color = Color.red;
                    controllerElements.batteryFillUI.color = Color.red;
                }
                else
                {
                    controllerElements.batteryOutlineUI.color = Color.white;
                    controllerElements.batteryFillUI.color = Color.white;
                }
            }

            if (useFlashlightBatteryDimming)
            {
                controllerElements.flashlight.intensity = (batteryLevel / 20f) * 2f;
            }

            if(flashlightOn && batteryLevel <= 10f)
            {
                batteryFlickerTimer -= Time.deltaTime;

                // flicker effect.
                if(batteryFlickerTimer <= 0f)
                {
                    controllerElements.flashlight.gameObject.SetActive(!controllerElements.flashlight.gameObject.activeSelf);
                    float timeFactor = batteryLevel / 10f;
                    if(controllerElements.flashlight.gameObject.activeSelf)
                    {
                        batteryFlickerTimer = timeFactor * Random.Range(0.5f, 1.5f);
                    }
                    else
                    {
                        batteryFlickerTimer = (1 - timeFactor) * Random.Range(0.1f, 0.5f);
                    }
                }
            }
        }
        else
        {
            batteryRed = false;
            controllerElements.batteryFillUI.color = Color.white;
            controllerElements.batteryOutlineUI.color = Color.white;
            batteryFlashTimer = 0f;
            controllerElements.flashlight.intensity = 2f;
            controllerElements.flashlight.gameObject.SetActive(flashlightOn && batteryLevel > 0f);
        }
    }

    private void checkForReload()
    {
        if(reloading)
        {
            // reloading animation and increment reload timer
            reloadTimer += Time.deltaTime;

            if(reloadTimer >= BATTERY_RELOAD_TIME)
            {
                decrementBatteryCount();
                batteryLevel += BATTERY_RELOAD_AMOUNT;
                reloadTimer = 0f;
                reloading = false;
                controllerElements.flashlight.gameObject.SetActive(flashlightOn);
                updateBatteryUI();
            }
        }
        else if(numberOfBatteries > 0 &&  batteryLevel < MAX_BATTERY_LEVEL - BATTERY_RELOAD_AMOUNT && 
            ((usingHydra && controllerElements.flashlightController.m_controller.GetButtonDown(SixenseButtons.FOUR)) || (!usingHydra && Input.GetKeyDown(KeyCode.R))))
                                        // TODO: figure out which button is appropriate for hydra
        {
            // reload
            reloading = true;
            controllerElements.flashlight.gameObject.SetActive(false);
        }
    }

    private void updateBatteryUI()
    {
        // update UI
        Vector3 scale = controllerElements.batteryFillScalePivot.transform.localScale;
        scale.x = batteryLevel / MAX_BATTERY_LEVEL;
        controllerElements.batteryFillScalePivot.transform.localScale = scale;
    }

    public void incrementBatteryCount()
    {
        numberOfBatteries++;
        controllerElements.batteryNumberUI.setNumber(numberOfBatteries);
    }

    public void decrementBatteryCount()
    {
        numberOfBatteries--;
        controllerElements.batteryNumberUI.setNumber(numberOfBatteries);
    }
}
