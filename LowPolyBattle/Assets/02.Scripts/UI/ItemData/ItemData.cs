using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ItemData : ScriptableObject
{
    [SerializeField] private string _name;

    [Multiline] [SerializeField] private string toolTip;
    public enum ItemType { Weapon, Cloth };
    [SerializeField] private ItemType type;
    [SerializeField] private short code;
    [SerializeField] private Texture2D image;
    [SerializeField] private GameObject dropItem;

    public string Name => _name; // 아이템 이름
    public ItemType Type => type;
    public string Tooltip => toolTip; // 아이템 설명
    public short Code => code; // 아이템 ID
    public Texture2D Image => image; // 아이템 이미지
    public GameObject DropItem => dropItem; // 드랍할 아이템 오브젝트

}