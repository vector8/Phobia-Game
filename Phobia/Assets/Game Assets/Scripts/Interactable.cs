using UnityEngine;
using System.Collections;
using System;

public abstract class Interactable : MonoBehaviour
{
    public const float STANDARD_ACTIVATION_RANGE = 10f;

    public abstract void activate();

    public virtual float getActivationRange()
    {
        return STANDARD_ACTIVATION_RANGE;
    }
}