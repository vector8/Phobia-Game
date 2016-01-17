using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Player : MonoBehaviour
{
    public Transform controllerToFollow;

    public const float MAX_BATTERY_LEVEL = 73f;
    public const float RELOAD_TIME = 3f;

    public float batteryLevel;
    public float batteryDrainRate = 1f;
    public Image batteryFillUI;
    public Image batteryOutlineUI;
    public Light hydraFlashlight;
    public Light mouseFlashlight;
    public FlashlightController flashlightController;
    public bool flashlightOn = true;
    public bool useFlashlightBatteryDimming = false;
    public int numberOfBatteries = 0;

    private bool usingHydra = true;
    private Light flashlight;
    private float batteryFlashTimer;
    private const float BATTERY_FLASH_DURATION = 0.5f;
    private bool batteryRed = false;
    private float batteryFlickerTimer = 0f;
    private bool reloading = false;
    private float reloadTimer = 0f;

    // Use this for initialization
    void Start()
    {
        flashlight = hydraFlashlight;
        GameObject temp = GameObject.Find("HydraController");
        if(temp == null)
        {
            temp = GameObject.Find("MouseController");
            usingHydra = false;
            flashlight = mouseFlashlight;
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
            if((usingHydra && flashlightController.m_controller.GetButtonDown(SixenseButtons.TWO)) || (!usingHydra && Input.GetKeyDown(KeyCode.Q)))
            {
                // Toggle flashlight
                flashlightOn = !flashlightOn;
                flashlight.gameObject.SetActive(flashlightOn);
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
            flashlight.gameObject.SetActive(false);
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
                    batteryOutlineUI.color = Color.red;
                    batteryFillUI.color = Color.red;
                }
                else
                {
                    batteryOutlineUI.color = Color.white;
                    batteryFillUI.color = Color.white;
                }
            }

            if (useFlashlightBatteryDimming)
            {
                flashlight.intensity = (batteryLevel / 20f) * 2f;
            }

            if(flashlightOn && batteryLevel <= 10f)
            {
                batteryFlickerTimer -= Time.deltaTime;

                // flicker effect.
                if(batteryFlickerTimer <= 0f)
                {
                    flashlight.gameObject.SetActive(!flashlight.gameObject.activeSelf);
                    float timeFactor = batteryLevel / 10f;
                    if(flashlight.gameObject.activeSelf)
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
            batteryFillUI.color = Color.white;
            batteryOutlineUI.color = Color.white;
            batteryFlashTimer = 0f;
            flashlight.intensity = 2f;
            flashlight.gameObject.SetActive(flashlightOn && batteryLevel > 0f);
        }
    }

    private void checkForReload()
    {
        if(reloading)
        {
            // reloading animation and increment reload timer
            reloadTimer += Time.deltaTime;

            if(reloadTimer >= RELOAD_TIME)
            {
                numberOfBatteries--;
                batteryLevel += 25;
                reloadTimer = 0f;
                reloading = false;
                flashlight.gameObject.SetActive(flashlightOn);
                updateBatteryUI();
            }
        }
        else if(numberOfBatteries > 0 &&  batteryLevel < 48f && // TODO: figure out which button is appropriate here
            ((usingHydra && flashlightController.m_controller.GetButtonDown(SixenseButtons.FOUR)) || (!usingHydra && Input.GetKeyDown(KeyCode.R))))
        {
            // reload
            reloading = true;
            flashlight.gameObject.SetActive(false);
        }
    }

    private void updateBatteryUI()
    {
        // update UI
        Vector2 dimensions = batteryFillUI.rectTransform.sizeDelta;
        dimensions.x = batteryLevel;
        batteryFillUI.rectTransform.sizeDelta = dimensions;
    }
}
