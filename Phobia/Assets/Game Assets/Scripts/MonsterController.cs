using UnityEngine;
using System.Collections.Generic;

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

    public Camera topDownCam, transitionCam;
    public GameObject topDownParent, fpsParent;
    public GameObject firstPersonController, interactionIcon;
    public float scrollSpeedMultiplier = 1f;
    public MonsterAbilities currentAbility = MonsterAbilities.None;
    public bool isFirstPerson = false;

    public StringPrefabDictEntry[] monsterSpawnPrefabs;
    public Dictionary<string, GameObject> monsterSpawnPrefabsDict;

    private const float MIN_ORTHO_SIZE = 3f, MAX_ORTHO_SIZE = 50f, ORTHO_SIZE_DIFF = 47f;
    private const float MIN_ORTHO_SPEED_MULTIPLIER = 3.3333f, MAX_ORTHO_SPEED_MULTIPLIER = 1.2f;
    private const float TRANSITION_DURATION = 1f;

    private bool transitioning = false;
    private float transitionTimer = 0f;

    private Transform origin, target;
    private Interactable targetInteractable = null;


    // Use this for initialization
    void Start()
    {
        monsterSpawnPrefabsDict = new Dictionary<string, GameObject>();
        foreach (StringPrefabDictEntry e in monsterSpawnPrefabs)
        {
            monsterSpawnPrefabsDict.Add(e.name, e.prefab);
        }
    }

    // Update is called once per frame
    void Update()
    {
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

            if (Mathf.Approximately(direction.sqrMagnitude, 0))
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

            if (!Mathf.Approximately(direction.sqrMagnitude, 0))
            {
                direction.Normalize();

                Vector3 pos = topDownCam.transform.position;
                float u = (topDownCam.orthographicSize - MIN_ORTHO_SIZE) / ORTHO_SIZE_DIFF;
                float tempSpeed = Mathf.Lerp(MIN_ORTHO_SPEED_MULTIPLIER, MAX_ORTHO_SPEED_MULTIPLIER, u) * topDownCam.orthographicSize;
                pos -= direction * scrollSpeedMultiplier * Time.deltaTime * tempSpeed;
                topDownCam.transform.position = pos;
            }

            if (Input.GetMouseButtonDown(1))
            {
                currentAbility = MonsterAbilities.None;
            }

            if (currentAbility != MonsterAbilities.None && Input.GetMouseButtonDown(0))
            {
                Vector3 placementPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                placementPosition.y = 1f;

                switch (currentAbility)
                {
                    case MonsterAbilities.Sound:
                        {
                            GameObject go = Instantiate<GameObject>(monsterSpawnPrefabsDict["PopGoesTheWeasel"]);
                            go.transform.position = placementPosition;
                        }
                        break;
                    case MonsterAbilities.Trap:
                        {
                            GameObject go = Instantiate<GameObject>(monsterSpawnPrefabsDict["JackInTheBox"]);
                            go.transform.position = placementPosition;
                        }
                        break;
                    case MonsterAbilities.Morph:
                        transitionCam.transform.position = topDownCam.transform.position;
                        transitionCam.transform.rotation = topDownCam.transform.rotation;
                        transitioning = true;
                        firstPersonController.transform.position = placementPosition;
                        firstPersonController.transform.rotation = Quaternion.identity;
                        transitionTimer = 0f;
                        origin = topDownCam.transform;
                        target = firstPersonController.transform;

                        transitionCam.gameObject.SetActive(true);
                        topDownParent.SetActive(false);

                        break;
                    default:
                        break;
                }

                currentAbility = MonsterAbilities.None;
            }
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.Q)) // OR dead OR timesUp)
            {
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
            }

            // check for interaction
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(new Vector2(Screen.width / 2f, Screen.height / 2f));

            if (Physics.Raycast(ray, out hit))
            {
                Interactable i = hit.collider.transform.gameObject.GetComponent<Interactable>();
                if (i != null && Vector3.Distance(firstPersonController.transform.position, hit.point) < i.getActivationRange())
                {
                    targetInteractable = i;
                    interactionIcon.SetActive(true);
                }
                else
                {
                    targetInteractable = null;
                    interactionIcon.SetActive(false);
                }
            }

            if (targetInteractable != null && Input.GetKeyDown(KeyCode.E))
            {
                targetInteractable.activate(false);
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
