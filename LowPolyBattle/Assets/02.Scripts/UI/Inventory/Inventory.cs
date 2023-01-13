using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    [SerializeField]
    public Slot[] slots;  // ½½·Ôµé ¹è¿­

    public void AcquireItem(ItemData _item, int _count = 1)
    {
        if (!(_item.Type == ItemData.ItemType.Weapon || _item.Type == ItemData.ItemType.Cloth))
        {
            for (int i = 0; i < slots.Length; i++)
            {
                if (slots[i].item != null)
                {
                    if (slots[i].item.Code == _item.Code)
                    {
                        slots[i].SetSlotCount(_count);
                        return;
                    }
                }
            }
        }

        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].item == null)
            {
                slots[i].AddItem(_item, _count);
                return;
            }
        }
    }
}