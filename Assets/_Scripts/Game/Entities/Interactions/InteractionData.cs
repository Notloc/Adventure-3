using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionData
{
    public float timer = 0f;
    public float progress = 0f;

    public void Reset()
    {
        timer = 0f;
        progress = 0f;
    }
}
