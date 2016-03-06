using UnityEngine;
using System.Collections;

[RequireComponent (typeof(CitaNet.NetworkedObject))]
public class FuseBox : Interactable
{

    // Use this for initialization
    protected override void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public override void activate(bool fromNetwork)
    {
        base.activate(fromNetwork);
    }
}
