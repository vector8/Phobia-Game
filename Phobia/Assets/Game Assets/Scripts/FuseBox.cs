using UnityEngine;
using System.Collections;

[RequireComponent (typeof(CitaNet.NetworkedObject))]
public class FuseBox : Interactable
{
    public GameObject[] fuses;
    public Player player;

    private int fusesActive = 0;

    // Use this for initialization
    protected override void Start()
    {
        base.Start();
        player = FindObjectOfType<Player>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public override void activate(bool fromNetwork)
    {
        base.activate(fromNetwork);

        for(int i = fusesActive; i < fusesActive + player.numberOfFuses; i++)
        {
            fuses[i].SetActive(true);
        }

        player.setFuseCount(0);
    }
}
