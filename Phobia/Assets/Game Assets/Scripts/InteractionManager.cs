using UnityEngine;
using System.Collections;

public class InteractionManager : MonoBehaviour
{
    private Interactable target = null;

    public GameObject interactionIcon;

    // Update is called once per frame
    void Update()
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(new Vector2(Screen.width / 2f, Screen.height / 2f));

        if (Physics.Raycast(ray, out hit))
        {
            Interactable i = hit.collider.transform.gameObject.GetComponent<Interactable>();
            if (i != null && Vector3.Distance(this.transform.position, i.transform.position) < i.getActivationRange())
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

        if (target != null && Input.GetKeyDown(KeyCode.E))
        {
            target.activate();
        }
    }
}
