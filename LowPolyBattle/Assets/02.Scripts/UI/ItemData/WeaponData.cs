using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Item", menuName = "ItemData/Weapon", order = 2)]
public class WeaponData : EquipmentData
{
    public enum WeaponType { None, Axe, Hammer, Sword, Shield, Bow, Wand};
    public enum WeaponMethod { Melee, Range};
    [SerializeField] private WeaponType weaponType;
    [SerializeField] private WeaponMethod weaponMethod;
    [SerializeField] private int minDam;
    [SerializeField] private int maxDam;
    [SerializeField] private float coolTime;
    [SerializeField] private int armor;

    public WeaponType type => weaponType;
    public WeaponMethod method => weaponMethod;
    public int MinDam => minDam;
    public int MaxDam => maxDam;
    public float CoolTime => coolTime;
    public int Armor => armor;
}