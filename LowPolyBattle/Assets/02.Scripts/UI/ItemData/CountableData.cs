using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CountableData : ItemData
{
    [SerializeField] private int maxAmount = 99;

    public int MaxAmount => maxAmount;

}
