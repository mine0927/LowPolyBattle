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

    public string Name => _name; // ������ �̸�
    public ItemType Type => type;
    public string Tooltip => toolTip; // ������ ����
    public short Code => code; // ������ ID
    public Texture2D Image => image; // ������ �̹���
    public GameObject DropItem => dropItem; // ����� ������ ������Ʈ

}