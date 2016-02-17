using UnityEngine;
using System.Collections;

public class MonsterController : MonoBehaviour
{
    public Camera topDownCam, transitionCam;
    public GameObject firstPersonController, interactionIcon;
    public float scrollSpeedMultiplier = 1f;

    private const float MIN_ORTHO_SIZE = 3f, MAX_ORTHO_SIZE = 50f, ORTHO_SIZE_DIFF = 47f;
    private const float MIN_ORTHO_SPEED_MULTIPLIER = 3.3333f, MAX_ORTHO_SPEED_MULTIPLIER = 1.2f;
    private const float TRANSITION_DURATION = 1f;

    private bool isFirstPerson = false;
    private bool placementMode = false;
    private bool transitioning = false;
    private float transitionTimer = 0f;

    private Transform origin, target;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if(transitioning)
        {
            transitionTimer += Time.deltaTime;
            if(transitionTimer > TRANSITION_DURATION)
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
        else if(!isFirstPerson)
        {
            topDownCam.orthographicSize -= Input.mouseScrollDelta.y;
            topDownCam.orthographicSize = Mathf.Clamp(topDownCam.orthographicSize, MIN_ORTHO_SIZE, MAX_ORTHO_SIZE);

            Vector3 direction = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"));

            if (!Mathf.Approximately(direction.sqrMagnitude, 0))
            {
                direction.Normalize();

                Vector3 pos = topDownCam.transform.position;
                float u = (topDownCam.orthographicSize - MIN_ORTHO_SIZE) / ORTHO_SIZE_DIFF;
                float tempSpeed = Mathf.Lerp(MIN_ORTHO_SPEED_MULTIPLIER, MAX_ORTHO_SPEED_MULTIPLIER, u) * topDownCam.orthographicSize;
                print(tempSpeed);
                pos -= direction * scrollSpeedMultiplier * Time.deltaTime * tempSpeed;
                topDownCam.transform.position = pos;
            }

            if(Input.GetKeyDown(KeyCode.Q))
            {
                placementMode = true;
            }

            if(placementMode && Input.GetMouseButtonDown(0))
            {
                Vector3 placementPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                placementPosition.y = 1f;
                placementMode = false;
                transitionCam.transform.position = topDownCam.transform.position;
                transitionCam.transform.rotation = topDownCam.transform.rotation;
                transitioning = true;
                firstPersonController.transform.position = placementPosition;
                firstPersonController.transform.rotation = Quaternion.identity;
                transitionTimer = 0f;
                origin = topDownCam.transform;
                target = firstPersonController.transform;

                transitionCam.gameObject.SetActive(true);
                topDownCam.gameObject.SetActive(false);
            }
        }
        else
        {
            if(Input.GetKeyDown(KeyCode.Q)) // OR dead OR timesUp)
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
                firstPersonController.SetActive(false);
                
                // reset hand UI just in case the monster was aiming at an interactable
                interactionIcon.SetActive(false);
            }
        }
    }

    private void setFirstPersonMode(bool b)
    {
        isFirstPerson = b;
        transitionCam.gameObject.SetActive(false);
        topDownCam.gameObject.SetActive(!isFirstPerson);
        firstPersonController.SetActive(isFirstPerson);
    }
}
