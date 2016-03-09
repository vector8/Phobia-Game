using UnityEngine;
using System.Collections;

public class InteractionManager : MonoBehaviour
{
    private Interactable target = null;

    public Player player;

    // Update is called once per frame
    void Update()
    {
        if(player.playMode != Player.PlayMode.Remote)
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(new Vector2(Screen.width / 2f, Screen.height / 2f));

            if (Physics.Raycast(ray, out hit))
            {
                Interactable i = hit.collider.transform.gameObject.GetComponent<Interactable>();
                if (i != null && Vector3.Distance(player.controllerElements.controllerTransform.position, hit.point) < i.getActivationRange())
                {
                    target = i;
                    player.controllerElements.handUI.SetActive(true);
                }
                else
                {
                    target = null;
                    player.controllerElements.handUI.SetActive(false);
                }
            }


            if (player.playMode == Player.PlayMode.Hydra)
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
}
