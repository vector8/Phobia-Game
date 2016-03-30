﻿using UnityEngine;
using System.Collections.Generic;
using CitaNet;

public class MonsterController : MonoBehaviour
{
    public enum MonsterAbilities
    {
        None,
        Sound,
        Trap,
        Morph
    }

    [System.Serializable]
    public struct StringPrefabDictEntry
    {
        public string name;
        public GameObject prefab;
    }

    public enum MonsterNetworkMessageType
    {
        SoundSpawned = 0,
        TrapSpawned,
        MorphSpawned,
        MorphDespawned,
        MorphMoved,
        PlayerHit
    }

    public float MORPH_COOLDOWN;
    public float MAX_TIME_IN_LIGHT;
    public float MAX_MORPH_TIME;
    public float MELEE_RANGE;
    public float MELEE_DAMAGE;
    public Vector4 TOP_DOWN_CAMERA_EXTENTS;

    public Camera topDownCam, transitionCam;
    public GameObject topDownParent, fpsParent, monsterLight;
    public GameObject firstPersonController, interactionIcon;
    public CharacterController characterController;
    public float scrollSpeedMultiplier = 1f;
    public MonsterAbilities currentAbility = MonsterAbilities.None;
    public bool isFirstPerson = false;
    public bool isLocal;
    public float networkUpdateDelay;
    public bool allowMouseMove;
    public float morphCooldownTimer = 0f;
    public float inLightTimer = 0f;
    public float morphDurationTimer = 0f;

    public Player player;
    public Light playerLight;
    public Collider entryLightCollider;

    public StringPrefabDictEntry[] monsterSpawnPrefabs;
    public Dictionary<string, GameObject> monsterSpawnPrefabsDict;

    public GameObject winOverlayFPS, winOverlayTopdown, loseOverlayFPS, loseOverlayTopdown;

    private const float MIN_ORTHO_SIZE = 3f, MAX_ORTHO_SIZE = 50f, ORTHO_SIZE_DIFF = 47f;
    private const float MIN_ORTHO_SPEED_MULTIPLIER = 3.3333f, MAX_ORTHO_SPEED_MULTIPLIER = 1.2f;
    private const float TRANSITION_DURATION = 1f;

    private bool transitioning = false;
    private float transitionTimer = 0f;

    private Transform origin, target;

    private bool dead = false;

    private bool gameOver = false;

    // networking
    [Header("Networking")]
    public float deadReckoningDistanceThreshold = 1f;
    public float deadReckoningAngleThreshold = 15f;
    public float deadReckoningCorrectionTime = 1f;
    public bool lerpDeadReckoningCorrections = true;
    private NetworkedObject netObj;
    private NetworkMessage networkMessageToSend;
    private GameObject remoteMorphSpawned = null;
    private Vector3 lastPosition = new Vector3();
    private Vector3 deadReckoningTargetPosition = new Vector3();
    private Vector3 deadReckoningTargetRotation = new Vector3();
    private Vector3 lastRotation = new Vector3();
    private Vector3 lastVelocity = new Vector3();
    private float lastSendTime;
    private bool deadReckoningNeedsCorrection = false;
    private float deadReckoningCorrectionTimer = 0f;

    private void customizeNetworkMessage(ref NetworkMessage msg)
    {
        msg = networkMessageToSend;
        lastSendTime = Time.time;
        lastPosition = firstPersonController.transform.position;
        lastVelocity = characterController.velocity;
        lastRotation.y = firstPersonController.transform.rotation.eulerAngles.y;
    }

    private void customNetworkMessageHandler(NetworkMessage msg)
    {
        int msgType;
        msg.getInt("T", out msgType);

        MonsterNetworkMessageType type = (MonsterNetworkMessageType)msgType;

        string name;
        msg.getString("N", out name);
        Vector3 position = new Vector3();
        Vector3 orientation = new Vector3();
        msg.getFloat("PX", out position.x);
        msg.getFloat("PY", out position.y);
        msg.getFloat("PZ", out position.z);
        msg.getFloat("VX", out lastVelocity.x);
        msg.getFloat("VY", out lastVelocity.y);
        msg.getFloat("VZ", out lastVelocity.z);
        msg.getFloat("RY", out orientation.y);

        switch (type)
        {
            case MonsterNetworkMessageType.SoundSpawned:
                GameObject sound = Instantiate(monsterSpawnPrefabsDict[name]);
                sound.transform.position = position;
                break;
            case MonsterNetworkMessageType.TrapSpawned:
                GameObject trap = Instantiate(monsterSpawnPrefabsDict[name]);
                trap.transform.position = position;
                break;
            case MonsterNetworkMessageType.MorphSpawned:
                remoteMorphSpawned = Instantiate(monsterSpawnPrefabsDict[name]);
                remoteMorphSpawned.transform.position = position;
                break;
            case MonsterNetworkMessageType.MorphDespawned:
                if (remoteMorphSpawned != null)
                {
                    Destroy(remoteMorphSpawned);
                }
                break;
            case MonsterNetworkMessageType.MorphMoved:
                if (remoteMorphSpawned != null)
                {
                    lastPosition = remoteMorphSpawned.transform.position;
                    lastRotation = remoteMorphSpawned.transform.rotation.eulerAngles;
                    deadReckoningTargetPosition = position;
                    deadReckoningTargetRotation = orientation;
                    deadReckoningNeedsCorrection = true;
                    deadReckoningCorrectionTimer = 0f;
                }
                break;
            case MonsterNetworkMessageType.PlayerHit:
                player.takeDamage(MELEE_DAMAGE);
                break;
            default:
                break;
        }
    }

    private void buildNetworkMessage(MonsterNetworkMessageType type, string nameOfSpawn = "", Vector3 position = new Vector3(), Vector3 orientation = new Vector3())
    {
        networkMessageToSend = new NetworkMessage();
        networkMessageToSend.setInt(NetworkedObject.ID_KEY, netObj.networkID);
        networkMessageToSend.setInt("T", (int)type);
        networkMessageToSend.setString("N", nameOfSpawn);
        networkMessageToSend.setFloat("PX", position.x);
        networkMessageToSend.setFloat("PY", position.y);
        networkMessageToSend.setFloat("PZ", position.z);
        networkMessageToSend.setFloat("VX", characterController.velocity.x);
        networkMessageToSend.setFloat("VY", characterController.velocity.y);
        networkMessageToSend.setFloat("VZ", characterController.velocity.z);
        networkMessageToSend.setFloat("RY", orientation.y);
    }

    public void setLocal(bool local)
    {
        isLocal = local;

        topDownParent.SetActive(isLocal);
        monsterLight.SetActive(isLocal);
        entryLightCollider.gameObject.SetActive(isLocal);

        if (!isLocal)
        {
            fpsParent.SetActive(false);
        }
    }

    // Use this for initialization
    void Start()
    {
        morphCooldownTimer = MORPH_COOLDOWN;

        monsterSpawnPrefabsDict = new Dictionary<string, GameObject>();
        foreach (StringPrefabDictEntry e in monsterSpawnPrefabs)
        {
            monsterSpawnPrefabsDict.Add(e.name, e.prefab);
        }

        // init network stuff
        netObj = GetComponent<NetworkedObject>();
        netObj.customNetworkMessageFunc = customizeNetworkMessage;
        netObj.customNetworkMessageHandler = customNetworkMessageHandler;
    }

    // Update is called once per frame
    void Update()
    {
        if (isLocal)
        {
            if(!gameOver)
            {
                if (player.won)
                {
                    loseOverlayFPS.SetActive(true);
                    loseOverlayTopdown.SetActive(true);
                    gameOver = true;
                    return;
                }
                else if (player.dead)
                {
                    winOverlayFPS.SetActive(true);
                    winOverlayTopdown.SetActive(true);
                    gameOver = true;
                    return;
                }

                updateLocal();
            }
        }
        else if(remoteMorphSpawned != null)
        {
            if (deadReckoningNeedsCorrection)
            {
                if (lerpDeadReckoningCorrections)
                {
                    deadReckoningCorrectionTimer += Time.deltaTime;

                    if (deadReckoningCorrectionTimer >= deadReckoningCorrectionTime)
                    {
                        deadReckoningCorrectionTimer = deadReckoningCorrectionTime;
                        deadReckoningNeedsCorrection = false;
                    }

                    float u = deadReckoningCorrectionTimer / deadReckoningCorrectionTime;

                    remoteMorphSpawned.transform.position = Vector3.Lerp(lastPosition, deadReckoningTargetPosition, u);
                    remoteMorphSpawned.transform.rotation = Quaternion.Euler(0f, Mathf.LerpAngle(lastRotation.y, deadReckoningTargetRotation.y, u), 0f);
                }
                else
                {
                    remoteMorphSpawned.transform.position = deadReckoningTargetPosition;
                    remoteMorphSpawned.transform.rotation = Quaternion.Euler(0f, deadReckoningTargetRotation.y, 0f);
                    deadReckoningNeedsCorrection = false;
                }
            }
            else
            {
                remoteMorphSpawned.transform.position += Time.deltaTime * lastVelocity;
            }
        }
    }

    private void updateLocal()
    {
        if(morphCooldownTimer < MORPH_COOLDOWN)
        {
            morphCooldownTimer += Time.deltaTime;
        }

        if (transitioning)
        {
            transitionTimer += Time.deltaTime;
            if (transitionTimer > TRANSITION_DURATION)
            {
                transitionTimer = 0f;
                transitioning = false;
                setFirstPersonMode(!isFirstPerson);
            }
            else
            {
                float u = transitionTimer / TRANSITION_DURATION;
                transitionCam.transform.position = Vector3.Lerp(origin.position, target.position, u);
                transitionCam.transform.rotation = Quaternion.Slerp(origin.rotation, target.rotation, u);
            }
        }
        else if (!isFirstPerson)
        {
            topDownCam.orthographicSize -= Input.mouseScrollDelta.y;
            topDownCam.orthographicSize = Mathf.Clamp(topDownCam.orthographicSize, MIN_ORTHO_SIZE, MAX_ORTHO_SIZE);

            Vector3 direction = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"));

            if (allowMouseMove && Mathf.Approximately(direction.sqrMagnitude, 0))
            {
                // try to get scroll info based on mouse position
                Vector3 mPos = Input.mousePosition;
                float marginX = 0.05f * Camera.main.pixelWidth, marginY = 0.05f * Camera.main.pixelHeight;
                if (mPos.x < marginX)
                {
                    direction.x = -1f;
                }
                else if (mPos.x > (Camera.main.pixelWidth - marginX))
                {
                    direction.x = 1f;
                }

                if (mPos.y < marginY)
                {
                    direction.z = -1f;
                }
                else if (mPos.y > (Camera.main.pixelHeight - marginY))
                {
                    direction.z = 1f;
                }
            }

            direction.Normalize();

            Vector3 pos = topDownCam.transform.position;
            float u = (topDownCam.orthographicSize - MIN_ORTHO_SIZE) / ORTHO_SIZE_DIFF;
            float tempSpeed = Mathf.Lerp(MIN_ORTHO_SPEED_MULTIPLIER, MAX_ORTHO_SPEED_MULTIPLIER, u) * topDownCam.orthographicSize;
            pos -= direction * scrollSpeedMultiplier * Time.deltaTime * tempSpeed;
            pos.x = Mathf.Clamp(pos.x, TOP_DOWN_CAMERA_EXTENTS.x, TOP_DOWN_CAMERA_EXTENTS.y);
            pos.z = Mathf.Clamp(pos.z, TOP_DOWN_CAMERA_EXTENTS.z, TOP_DOWN_CAMERA_EXTENTS.w);
            topDownCam.transform.position = pos;

            if (Input.GetMouseButtonDown(1))
            {
                currentAbility = MonsterAbilities.None;
            }

            if (currentAbility != MonsterAbilities.None && Input.GetMouseButtonDown(0))
            {
                RaycastHit hit;
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                bool failed = false;

                if (Physics.Raycast(ray, out hit))
                {
                    if (hit.collider.gameObject.tag == "Floor")
                    {
                        Vector3 placementPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                        placementPosition.y = 1f;

                        switch (currentAbility)
                        {
                            case MonsterAbilities.Sound:
                                {
                                    GameObject go = Instantiate<GameObject>(monsterSpawnPrefabsDict["PopGoesTheWeasel"]);
                                    go.transform.position = placementPosition;

                                    buildNetworkMessage(MonsterNetworkMessageType.SoundSpawned, "PopGoesTheWeasel", placementPosition);
                                    netObj.sendNetworkUpdate();
                                }
                                break;
                            case MonsterAbilities.Trap:
                                {
                                    GameObject go = Instantiate<GameObject>(monsterSpawnPrefabsDict["JackInTheBox"]);
                                    go.transform.position = placementPosition;

                                    buildNetworkMessage(MonsterNetworkMessageType.TrapSpawned, "JackInTheBox", placementPosition);
                                    netObj.sendNetworkUpdate();
                                }
                                break;
                            case MonsterAbilities.Morph:
                                if (morphCooldownTimer >= MORPH_COOLDOWN)
                                {
                                    morphCooldownTimer = 0f;
                                    transitionCam.transform.position = topDownCam.transform.position;
                                    transitionCam.transform.rotation = topDownCam.transform.rotation;
                                    transitioning = true;
                                    firstPersonController.transform.position = placementPosition + new Vector3(0f, 2.5f, 0f);
                                    firstPersonController.transform.rotation = Quaternion.identity;
                                    transitionTimer = 0f;
                                    origin = topDownCam.transform;
                                    target = firstPersonController.transform;

                                    transitionCam.gameObject.SetActive(true);
                                    topDownParent.SetActive(false);

                                    buildNetworkMessage(MonsterNetworkMessageType.MorphSpawned, "Clown", firstPersonController.transform.position);
                                    netObj.sendNetworkUpdate();

                                    Cursor.lockState = CursorLockMode.Locked;
                                    Cursor.visible = false;
                                }
                                else
                                {
                                    failed = true;
                                }
                                break;
                            default:
                                break;
                        }
                    }
                    else
                    {
                        failed = true;
                    }
                }
                else
                {
                    failed = true;
                }

                if(failed)
                {
                    // TODO: give the player some kind of feedback that they cant do this

                }

                currentAbility = MonsterAbilities.None;
            }
        }
        else // isFirstPerson
        {
            morphDurationTimer += Time.deltaTime;

            if(firstPersonController.GetComponent<Collider>().bounds.Intersects(entryLightCollider.bounds))
            {
                inLightTimer += Time.deltaTime;

                if (inLightTimer > MAX_TIME_IN_LIGHT)
                {
                    dead = true;
                    inLightTimer = 0f;
                }
            }
            // check if we are in the player's flashlight cone
            else if(morphDurationTimer < MAX_MORPH_TIME && (playerLight.gameObject.activeSelf))
            {
                Vector3 diff = firstPersonController.transform.position - playerLight.transform.position;
                Vector3 lightAxis = playerLight.transform.forward * playerLight.range;
                float dot = Vector3.Dot(diff, lightAxis);

                if (dot / (diff.magnitude * playerLight.range) > Mathf.Cos(Mathf.Deg2Rad * playerLight.spotAngle / 2f) &&   // we are in the infinite cone
                    dot / playerLight.range < playerLight.range)    // we are in range
                {
                    // check if we are in line of sight
                    RaycastHit hit;
                    Ray ray = new Ray(playerLight.transform.position, firstPersonController.transform.position - playerLight.transform.position);

                    if (Physics.Raycast(ray, out hit))
                    {
                        if(hit.collider.gameObject.GetInstanceID() == firstPersonController.GetInstanceID())
                        {
                            inLightTimer += Time.deltaTime;

                            if(inLightTimer > MAX_TIME_IN_LIGHT)
                            {
                                dead = true;
                                inLightTimer = 0f;
                            }
                        }
                    }
                }
            }

            if (Input.GetKeyDown(KeyCode.Q) || dead || morphDurationTimer > MAX_MORPH_TIME)
            {
                morphDurationTimer = 0f;
                dead = false;
                transitioning = true;
                transitionTimer = 0f;
                Vector3 newTopDownPos = firstPersonController.transform.position;
                newTopDownPos.y = topDownCam.transform.position.y;
                topDownCam.transform.position = newTopDownPos;
                origin = firstPersonController.transform;
                target = topDownCam.transform;
                transitionCam.transform.position = firstPersonController.transform.position;
                transitionCam.transform.rotation = firstPersonController.transform.rotation;

                transitionCam.gameObject.SetActive(true);
                fpsParent.SetActive(false);

                // reset hand UI just in case the monster was aiming at an interactable
                interactionIcon.SetActive(false);

                buildNetworkMessage(MonsterNetworkMessageType.MorphDespawned);
                netObj.sendNetworkUpdate();

                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            else
            {
                // predict where the remote host sees us based on the last velocity sent
                Vector3 predictedPosition = lastPosition + (Time.time - lastSendTime) * lastVelocity;
                if (Vector3.Distance(firstPersonController.transform.position, predictedPosition) > deadReckoningDistanceThreshold ||
                    Mathf.Abs(Mathf.DeltaAngle(firstPersonController.transform.rotation.eulerAngles.y, lastRotation.y)) > deadReckoningAngleThreshold)
                {
                    Vector3 pos = firstPersonController.transform.position;
                    pos.y -= 2.5f; // too slow to get the actual controller's height.. otherwise i need to store it and meh.
                    buildNetworkMessage(MonsterNetworkMessageType.MorphMoved, "", pos, firstPersonController.transform.rotation.eulerAngles);
                    netObj.sendNetworkUpdate();
                }

                // check for interaction
                RaycastHit hit;
                Ray ray = Camera.main.ScreenPointToRay(new Vector2(Screen.width / 2f, Screen.height / 2f));

                if (Physics.Raycast(ray, out hit))
                {
                    Interactable i = hit.collider.gameObject.GetComponent<Interactable>();
                    if (i != null && i.activatable && i.activatableByMonster && hit.distance < i.getActivationRange())
                    {
                        interactionIcon.SetActive(true);
                        if (Input.GetKeyDown(KeyCode.E))
                        {
                            i.activate(false);
                        }
                    }
                    else
                    {
                        interactionIcon.SetActive(false);
                    }

                    Player p = hit.collider.transform.parent.GetComponent<Player>();
                    if(p != null && hit.distance <= MELEE_RANGE)
                    {
                        // TODO: set attack icon active
                        if(Input.GetMouseButtonDown(0))
                        {
                            buildNetworkMessage(MonsterNetworkMessageType.PlayerHit);
                            netObj.sendNetworkUpdate();
                        }
                    }
                }
                else
                {
                    // set all interaction icons inactive
                    interactionIcon.SetActive(false);
                }
            }
        }
    }

    private void setFirstPersonMode(bool b)
    {
        isFirstPerson = b;
        transitionCam.gameObject.SetActive(false);
        topDownParent.SetActive(!isFirstPerson);
        fpsParent.SetActive(isFirstPerson);
    }
}
