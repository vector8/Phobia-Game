﻿using UnityEngine;
using System.Collections;

public class InteractionManager : MonoBehaviour
{
    private Interactable target = null;

    public GameObject interactionIcon;
    public Player player;


    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(new Vector2(Screen.width / 2f, Screen.height / 2f));

        if (Physics.Raycast(ray, out hit))
        {
            Interactable i = hit.collider.transform.gameObject.GetComponent<Interactable>();
            if (i != null && Vector3.Distance(this.transform.position, hit.point) < i.getActivationRange())
            {
                target = i;
                interactionIcon.SetActive(true);
            }
            else
            {
                target = null;
                interactionIcon.SetActive(false);
            }
        }


        if (player.usingHydra)
        {
            SixenseInput.Controller rightController = SixenseInput.GetController(SixenseHands.RIGHT);

            if(target != null && rightController.GetButtonDown(SixenseButtons.TRIGGER))
            {
                target.activate(false);
            }
        }
        else
        {
            if(target != null && Input.GetKeyDown(KeyCode.E))
            {
                target.activate(false);
            }
        }
    }
}
