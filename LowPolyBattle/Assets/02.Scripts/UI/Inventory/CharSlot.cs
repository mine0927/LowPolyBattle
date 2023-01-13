using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class CharSlot : MonoBehaviour, IPointerClickHandler
{
    public ClothData cloth;
    public WeaponData weapon;
    public RawImage itemImage;
    public UI ui;

    public void addCloth(ClothData _cloth)
    {
        cloth = _cloth;
        itemImage.texture = cloth.Image; 
        SetColor(1);
        
    }
    public void addWeapon(WeaponData _weapon)
    {
        weapon = _weapon;
        itemImage.texture = weapon.Image;
        SetColor(1);
    }
    private void SetColor(float _alpha)
    {
        Color color = itemImage.color;
        color.a = _alpha;
        itemImage.color = color;
    }
    public void ClearSlot()
    {
        SetColor(0);
        cloth = null;
        weapon = null;
        itemImage.texture = null;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        ui.toolTip.gameObject.SetActive(false);
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            if (cloth != null || weapon != null)
            {
                ui.unEquipment.slot = this;
                ui.unEquipment.transform.position = eventData.position;
                ui.unEquipment.gameObject.SetActive(true);

            }
        }
        else
        {
            ui.equipment.gameObject.SetActive(false);
        }
    }
}
