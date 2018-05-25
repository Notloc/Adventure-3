using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/New Item")]
public class Item : ScriptableObject
{
    [SerializeField] bool stackable;
    public bool Stackable
    {
        get
        {
            return stackable;
        }
    }

    [SerializeField] int value;
    public int Value
    {
        get
        {
            return value;
        }
    }


}
