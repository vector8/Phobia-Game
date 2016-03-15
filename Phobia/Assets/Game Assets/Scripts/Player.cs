using UnityEngine;
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
    }

    public enum PlayMode
    {
        Mouse,
        Hydra,
        Remote
    }

    public ControllerElements mouseElements, hydraElements;
    public ControllerElements controllerElements;
    public GameObject remoteHuman, remoteFlashlight, remoteFlashlight_light;

    public PlayMode playMode
    {
        get; private set;
    }

    public float MAX_BATTERY_LEVEL = 100f;
    public float BATTERY_RELOAD_TIME = 3f;
    public float BATTERY_RELOAD_AMOUNT = 50f;

    public float batteryLevel;
    public float batteryDrainRate = 1f;
    public bool flashlightOn = true;
    public bool useFlashlightBatteryDimming = false;
    public int numberOfBatteries = 0;
    public int numberOfFuses = 0;
    public float networkUpdateDelay;

    private float batteryFlashTimer;
    private const float BATTERY_FLASH_DURATION = 0.5f;
    private bool batteryRed = false;
    private float batteryFlickerTimer = 0f;
    private bool reloading = false;
    private float reloadTimer = 0f;

    // networking stuff
    private NetworkedObject netObj;
    private float networkUpdateTimer = 0f;

    // Use this for initialization
    void Start()
    {
        batteryLevel = MAX_BATTERY_LEVEL;

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
#endif
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (playMode != PlayMode.Remote)
        {
            checkForReload();
            if (!reloading)
            {
                updateFlashlight();
            }

            networkUpdateTimer += Time.deltaTime;
            if (networkUpdateTimer >= networkUpdateDelay)
            {
                networkUpdateTimer = 0f;
                netObj.sendNetworkUpdate();
            }
        }
    }

    private void customizeNetworkMessage(ref NetworkMessage msg)
    {
        msg.setFloat("PosX", controllerElements.controllerTransform.position.x);
        msg.setFloat("PosY", controllerElements.controllerTransform.position.y - 3.3f); // too slow! (controllerElements.controller.GetComponent<CharacterController>().height / 2f));
        msg.setFloat("PosZ", controllerElements.controllerTransform.position.z);
        msg.setFloat("RotY", controllerElements.controllerTransform.rotation.eulerAngles.y);
        msg.setFloat("RotX", Camera.main.transform.rotation.eulerAngles.x);
        msg.setBool("Flshlt", controllerElements.flashlight.gameObject.activeSelf);
    }

    private void customNetworkMessageHandler(NetworkMessage msg)
    {
        float xPos, yPos, zPos, xRot, yRot;
        bool remoteFlashlightOn;

        msg.getFloat("PosX", out xPos);
        msg.getFloat("PosY", out yPos);
        msg.getFloat("PosZ", out zPos);
        msg.getFloat("RotX", out xRot);
        msg.getFloat("RotY", out yRot);
        msg.getBool("Flshlt", out remoteFlashlightOn);

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
            }

            if (flashlightOn)
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
}
