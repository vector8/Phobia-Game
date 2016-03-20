using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using CitaNet;

[RequireComponent(typeof(NetworkedObject))]
public class Player : MonoBehaviour
{
    [System.Serializable]
    public struct ControllerElements
    {
        public GameObject controllerGO;
        public Transform controllerTransform;
        public NumberDisplayController batteryNumberUI;
        public SpriteRenderer batteryFillUI, batteryOutlineUI;
        public GameObject batteryFillScalePivot;
        public Light flashlight;
        public FlashlightController flashlightController;
        public GameObject handUI;
        public GameObject fuseUI;
        public NumberDisplayController fuseNumberUI;
        public Image damageOverlay;
        public GameObject winOverlay, loseOverlay;
        public MonoBehaviour controllerScript;
        public AudioSource flashlightAudioSource;
    }

    public enum PlayMode
    {
        Mouse,
        Hydra,
        Remote
    }

    [Header("Controller Elements")]
    public ControllerElements mouseElements;
    public ControllerElements hydraElements;
    public ControllerElements controllerElements;

    [Header("Remote Objects")]
    public GameObject remoteHuman;
    public GameObject remoteFlashlight;
    public GameObject remoteFlashlight_light;

    public PlayMode playMode
    {
        get; private set;
    }

    [Header("Constants")]
    public float MAX_BATTERY_LEVEL = 100f;
    public float BATTERY_RELOAD_TIME = 3f;
    public float BATTERY_RELOAD_AMOUNT = 50f;
    public float HEALTH_REGEN_DELAY = 3f;
    public float HEALTH_REGEN_PER_SECOND = 50f;
    public float MAX_HEALTH = 100f;

    [Header("Sounds")]
    public AudioClip flashLightClick;
    public AudioClip flashLightFlicker;
    public float volumeScale;

    [Header("Misc")]
    public float batteryLevel;
    public float batteryDrainRate = 1f;
    public bool flashlightOn = true;
    public bool useFlashlightBatteryDimming = false;
    public int numberOfBatteries = 0;
    public int numberOfFuses = 0;
    public float networkUpdateDelay;
    public bool dead = false;
    public bool won = false;
    public float health;

    [Header("Tutorial")]
    public GameObject tutorial1Text;
    public GameObject tutorial2Text;
    public GameObject tutorialArea;
    private bool reloaded = false;

    private float batteryFlashTimer;
    private const float BATTERY_FLASH_DURATION = 0.5f;
    private bool batteryRed = false;
    private float batteryFlickerTimer = 0f;
    private bool reloading = false;
    private float reloadTimer = 0f;
    private float healthRegenDelayTimer = 0f;

    // networking stuff
    private NetworkedObject netObj;
    private float networkUpdateTimer = 0f;
    private Vector3 prevPosSent = new Vector3();
    private Vector3 prevRotSent = new Vector3();

    // Use this for initialization
    void Start()
    {
        health = MAX_HEALTH;

        // init network stuff
        netObj = GetComponent<NetworkedObject>();
        netObj.customNetworkMessageFunc = customizeNetworkMessage;
        netObj.customNetworkMessageHandler = customNetworkMessageHandler;
    }

    public void setPlayMode(PlayMode mode)
    {
        playMode = mode;

        switch (playMode)
        {
            case PlayMode.Mouse:
                controllerElements = mouseElements;
                mouseElements.controllerGO.SetActive(true);
                break;
            case PlayMode.Hydra:
                controllerElements = hydraElements;
                hydraElements.controllerGO.SetActive(true);
                break;
            case PlayMode.Remote:
                remoteHuman.SetActive(true);
                break;
            default:
                break;
        }

        if (playMode != PlayMode.Remote)
        {
            // Disable mouse cursor if this is a build
#if UNITY_EDITOR
            Cursor.visible = true;
#elif UNITY_STANDALONE
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
#endif
            tutorialArea.SetActive(true);
        }
        else
        {
            tutorialArea.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (playMode != PlayMode.Remote && !dead && !won)
        {
            if(healthRegenDelayTimer > 0f)
            {
                healthRegenDelayTimer -= Time.deltaTime;
            }

            if(health < MAX_HEALTH && healthRegenDelayTimer <= 0f)
            {
                health += HEALTH_REGEN_PER_SECOND * Time.deltaTime;
                setDamageOverlayAlpha();
            }

            checkForReload();
            if (!reloading)
            {
                updateFlashlight();
            }

            networkUpdateTimer += Time.deltaTime;
            if (networkUpdateTimer >= networkUpdateDelay &&
                    (controllerElements.controllerTransform.position != prevPosSent || controllerElements.controllerTransform.rotation.eulerAngles.y != prevRotSent.y ||
                    Camera.main.transform.rotation.eulerAngles.x != prevRotSent.x))
            {
                networkUpdateTimer = 0f;
                netObj.sendNetworkUpdate();
                prevPosSent = controllerElements.controllerTransform.position;
                prevRotSent.y = controllerElements.controllerTransform.rotation.eulerAngles.y;
                prevRotSent.x = Camera.main.transform.rotation.eulerAngles.x;
            }
        }
    }

    private void customizeNetworkMessage(ref NetworkMessage msg)
    {
        msg.setFloat("PX", controllerElements.controllerTransform.position.x);
        msg.setFloat("PY", controllerElements.controllerTransform.position.y - 3.3f); // too slow! (controllerElements.controller.GetComponent<CharacterController>().height / 2f));
        msg.setFloat("PZ", controllerElements.controllerTransform.position.z);
        msg.setFloat("RY", controllerElements.controllerTransform.rotation.eulerAngles.y);
        msg.setFloat("RX", Camera.main.transform.rotation.eulerAngles.x);
        msg.setBool("F", controllerElements.flashlight.gameObject.activeSelf);
        msg.setBool("D", dead);
        msg.setBool("W", won);
    }

    private void customNetworkMessageHandler(NetworkMessage msg)
    {
        float xPos, yPos, zPos, xRot, yRot;
        bool remoteFlashlightOn;

        msg.getFloat("PX", out xPos);
        msg.getFloat("PY", out yPos);
        msg.getFloat("PZ", out zPos);
        msg.getFloat("RX", out xRot);
        msg.getFloat("RY", out yRot);
        msg.getBool("F", out remoteFlashlightOn);
        msg.getBool("D", out dead);
        msg.getBool("W", out won);

        remoteHuman.transform.position = new Vector3(xPos, yPos, zPos);
        remoteHuman.transform.rotation = Quaternion.Euler(new Vector3(0f, yRot, 0f));
        remoteFlashlight.transform.localRotation = Quaternion.Euler(new Vector3(xRot, 0f, 0f));
        remoteFlashlight_light.SetActive(remoteFlashlightOn);

        //Vector3 euler = transform.rotation.eulerAngles;
        //euler.y = yRot;
        //transform.rotation = Quaternion.Euler(euler);

        //euler = m_Camera.transform.rotation.eulerAngles;
        //euler.x = xRot;
        //m_Camera.transform.rotation = Quaternion.Euler(euler);
    }

    private void updateFlashlight()
    {
        if (batteryLevel > 0f)
        {
            // If flashlight button has been pressed
            if ((playMode == PlayMode.Hydra && controllerElements.flashlightController.m_controller.GetButtonDown(SixenseButtons.TWO)) || (playMode == PlayMode.Mouse && Input.GetKeyDown(KeyCode.Q)))
            {
                // Toggle flashlight
                flashlightOn = !flashlightOn;
                controllerElements.flashlight.gameObject.SetActive(flashlightOn);

                // update remote host with new flashlight info.
                networkUpdateTimer = 0f;
                netObj.sendNetworkUpdate();

                tutorial1Text.SetActive(false);

                controllerElements.flashlightAudioSource.PlayOneShot(flashLightClick, volumeScale);
            }

            if (flashlightOn)
            {
                batteryLevel -= batteryDrainRate * Time.deltaTime;

                if(batteryLevel <= 50f && !reloaded && numberOfBatteries > 0)
                {
                    tutorial2Text.SetActive(true);
                }
                else
                {
                    tutorial2Text.SetActive(false);
                }

                updateBatteryUI();
            }
        }
        else
        {
            batteryLevel = 0f;
            controllerElements.flashlight.gameObject.SetActive(false);
            flashlightOn = false;
        }

        if (batteryLevel < 20f && batteryLevel > 0f)
        {
            batteryFlashTimer += Time.deltaTime;
            if (batteryFlashTimer >= BATTERY_FLASH_DURATION)
            {
                batteryRed = !batteryRed;
                batteryFlashTimer = 0f;

                if (batteryRed)
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

            if (flashlightOn && batteryLevel <= 10f)
            {
                batteryFlickerTimer -= Time.deltaTime;

                // flicker effect.
                if (batteryFlickerTimer <= 0f)
                {
                    controllerElements.flashlight.gameObject.SetActive(!controllerElements.flashlight.gameObject.activeSelf);
                    float timeFactor = batteryLevel / 10f;
                    if (controllerElements.flashlight.gameObject.activeSelf)
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
        if (reloading)
        {
            // reloading animation and increment reload timer
            reloadTimer += Time.deltaTime;

            if (reloadTimer >= BATTERY_RELOAD_TIME)
            {
                decrementBatteryCount();
                batteryLevel += BATTERY_RELOAD_AMOUNT;
                reloadTimer = 0f;
                reloading = false;
                controllerElements.flashlight.gameObject.SetActive(flashlightOn);
                updateBatteryUI();

                // update remote host with new flashlight info.
                networkUpdateTimer = 0f;
                netObj.sendNetworkUpdate();
            }
        }
        else if (numberOfBatteries > 0 && batteryLevel < MAX_BATTERY_LEVEL - BATTERY_RELOAD_AMOUNT &&
            ((playMode == PlayMode.Hydra && controllerElements.flashlightController.m_controller.GetButtonDown(SixenseButtons.FOUR)) || (playMode == PlayMode.Mouse && Input.GetKeyDown(KeyCode.R))))
        // TODO: figure out which button is appropriate for hydra
        {
            // reload
            reloaded = true;
            tutorial2Text.SetActive(false);
            reloading = true;
            controllerElements.flashlight.gameObject.SetActive(false);

            // update remote host with new flashlight info.
            networkUpdateTimer = 0f;
            netObj.sendNetworkUpdate();
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

    public void incrementFuseCount()
    {
        numberOfFuses++;
        controllerElements.fuseNumberUI.setNumber(numberOfFuses);
        controllerElements.fuseUI.SetActive(numberOfFuses > 0);
    }

    public void decrementFuseCount()
    {
        numberOfFuses--;
        controllerElements.fuseNumberUI.setNumber(numberOfFuses);
        controllerElements.fuseUI.SetActive(numberOfFuses > 0);
    }

    public void setFuseCount(int count)
    {
        numberOfFuses = count;
        controllerElements.fuseNumberUI.setNumber(numberOfFuses);
        controllerElements.fuseUI.SetActive(numberOfFuses > 0);
    }

    public void takeDamage(float damage)
    {
        health -= damage;
        healthRegenDelayTimer = HEALTH_REGEN_DELAY;
        setDamageOverlayAlpha();
        if (health <= 0f)
        {
            dead = true;
            controllerElements.loseOverlay.SetActive(true);
            controllerElements.controllerScript.enabled = false;
            networkUpdateTimer = 0f;
            netObj.sendNetworkUpdate();
        }
    }

    public void win()
    {
        won = true;
        controllerElements.winOverlay.SetActive(true);
        controllerElements.controllerScript.enabled = false;
        networkUpdateTimer = 0f;
        netObj.sendNetworkUpdate();
    }

    private void setDamageOverlayAlpha()
    {
        Color c = controllerElements.damageOverlay.color;
        c.a = 1f - (health / MAX_HEALTH);
        controllerElements.damageOverlay.color = c;
    }
}
