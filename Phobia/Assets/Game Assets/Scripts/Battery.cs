﻿using UnityEngine;
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
        if(player.numberOfBatteries < 9)
        {
            player.incrementBatteryCount();
            Destroy(gameObject);
        }
        else
        {
            // TODO: tell the player that they cant hold anymore batteries.
        }
    }
}
