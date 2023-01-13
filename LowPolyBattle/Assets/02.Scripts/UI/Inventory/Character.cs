using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    // Start is called before the first frame update
    public CharSlot[] slot = new CharSlot[9];
    public UI ui;

    public void Setting(ClothData cloth, WeaponData weapon, Slot slots, bool leftHand)
    {
        slots.ClearSlot();

        if (cloth != null)
        {
            SettingCloth(cloth);
        }
        else
        {
            SettingWeapon(weapon, leftHand);
        }
    }


    private void SettingCloth(ClothData cloth)
    {
        int num = 0;
        switch (cloth.Cloth)
        {
            case ClothData.ClothType.Cloth: num = 1; break;
            case ClothData.ClothType.Belt: num = 2; break;
            case ClothData.ClothType.Shoe: num = 3; break;
            case ClothData.ClothType.Glove: num = 4; break;
            case ClothData.ClothType.ShoulderPad: num = 5; break;
        }
        if (slot[num].cloth != null)
        {
            ui.player.StartSkin(slot[num].cloth.Cloth, slot[num].cloth.Code, false);
            ui.inventory.AcquireItem(slot[num].cloth);
        }

        slot[num].addCloth(cloth);
        ui.player.StartSkin(cloth.Cloth, cloth.Code, true);
    }

    private void SettingWeapon(WeaponData weapon, bool useLeft)
    {
        // 캐릭터/인벤토리 UI설정
        if (useLeft && slot[6].weapon != null)
        {
            ui.inventory.AcquireItem(slot[6].weapon);
            slot[6].ClearSlot();
        }
        if (slot[8].weapon != null)
        {
            ui.inventory.AcquireItem(slot[8].weapon);
            slot[8].ClearSlot();
        }
        slot[useLeft ? 6 : 8].addWeapon(weapon);

        // 모델 설정
        var isLeft = slot[6].weapon != null;
        var isRight = slot[8].weapon != null;

        ui.player.startWeapon(0,false,false); // isActive가 false이면 앞 두 개의 값은 의미없음
        if(isRight)
        {
            if(slot[8].weapon.type != WeaponData.WeaponType.Shield)
            {
                
                ui.player.startWeapon(slot[6].weapon.Code, true, true);
                ui.player.startWeapon(slot[8].weapon.Code, false, true);
            }
            else
            {
                ui.player.startWeapon(slot[8].weapon.Code, true, true);
                if (isLeft) ui.player.startWeapon(slot[6].weapon.Code, false, true);
            }
        }
        else
        {
            if (slot[6].weapon.type != WeaponData.WeaponType.Wand)
                ui.player.startWeapon(slot[6].weapon.Code, true, true);
            else
                ui.player.startWeapon(slot[6].weapon.Code, false, true);
        }
    }
}
