using UnityEngine;
using System.Collections;
using System;

public class Battery : Interactable
{
    private Player player;

    void Start()
    {
        GameObject playerGO = GameObject.Find("Player");
        player = playerGO.GetComponent<Player>();
    }

    public override void activate()
    {
        player.numberOfBatteries++;
        Destroy(gameObject);
    }
}
