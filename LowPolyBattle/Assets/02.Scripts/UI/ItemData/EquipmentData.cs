using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentData : ItemData
{
    [SerializeField] private int maxDurability;

    public int MaxDurability => maxDurability;
}