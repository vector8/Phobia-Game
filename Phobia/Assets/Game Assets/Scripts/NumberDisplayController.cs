using UnityEngine;
using System.Collections;

public class NumberDisplayController : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;

    public Sprite[] numberSprites;

    public void setNumber(int n)
    {
        if (n < 0 || n > 9)
            return;

        spriteRenderer.sprite = numberSprites[n];
    }
}
