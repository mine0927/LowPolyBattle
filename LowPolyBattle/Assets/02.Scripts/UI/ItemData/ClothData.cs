using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "ItemData/Cloth", order = 1)]
public class ClothData : EquipmentData
{
    public enum ClothType { Belt, Cloth, Crown, Glove, Hat, Helm, Shoe, ShoulderPad, None};
    [SerializeField] private ClothType cloth;
    [SerializeField] private float speed;
    [SerializeField] private int armor;

    public ClothType Cloth => cloth;
    public float Speed => speed;
    public int Armor => armor;


}