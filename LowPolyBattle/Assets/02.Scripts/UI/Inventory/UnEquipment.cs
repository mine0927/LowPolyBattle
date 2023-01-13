using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UnEquipment : MonoBehaviour
{
    public UI ui;
    public CharSlot slot;
    public bool isExit;
    // Start is called before the first frame update
    public void Dismount()
    {
        if (slot.cloth != null)
        {
            ClothData.ClothType type = slot.cloth.Cloth;
            ui.inventory.AcquireItem(slot.cloth);
            ui.player.StartSkin(type, slot.cloth.Code, false);
            slot.ClearSlot();
            
        }
        else
        {
            if(slot == ui.character.slot[6])
            {
                /*
                if (ui.character.slot[6] != null)
                {
                    ui.player.startWeapon(slot.weapon.Code, true, false);
                    ui.inventory.AcquireItem(ui.character.slot[6].weapon);
                    ui.character.slot[6].ClearSlot();
                }
                if (ui.character.slot[8] != null)
                {
                    ui.player.startWeapon(ui.character.slot[8].weapon.Code, false, false);
                    ui.inventory.AcquireItem(ui.character.slot[8].weapon);
                    ui.character.slot[8].ClearSlot();
                }
            }
            else
            {
                ui.player.startWeapon(ui.character.slot[8].weapon.Code, true, false);
                ui.player.startWeapon(ui.character.slot[6].weapon.Code, true, true);
                ui.player.startWeapon(ui.character.slot[6].weapon.Code, false, false);
                ui.inventory.AcquireItem(ui.character.slot[8].weapon);
                ui.character.slot[8].ClearSlot();
                */
            }
        }
        this.gameObject.SetActive(false);
    }

}
