using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SO_CountableItem : SO_Item
{
    [SerializeField] private int maxAmount = 200;
    public int MaxAmount => maxAmount;
 
}
