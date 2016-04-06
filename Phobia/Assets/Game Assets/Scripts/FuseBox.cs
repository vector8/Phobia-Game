using UnityEngine;
using System.Collections;

[RequireComponent (typeof(CitaNet.NetworkedObject))]
public class FuseBox : Interactable
{
    public GameObject[] fuses;
    public Player player;

    private int fusesActive = 0;
    private AudioSource audioSource;

    // Use this for initialization
    protected override void Start()
    {
        base.Start();
        player = FindObjectOfType<Player>();
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public override void activate(bool fromNetwork)
    {
        base.activate(fromNetwork);
        
        if(player.numberOfFuses > 0)
        {
            audioSource.Play();
        }

        for(int i = fusesActive; i < fusesActive + player.numberOfFuses; i++)
        {
            fuses[i].SetActive(true);
        }

        fusesActive += player.numberOfFuses;

        if(fusesActive == fuses.Length)
        {
            player.win();
        }

        player.setFuseCount(0);
    }
}
