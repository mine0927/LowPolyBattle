using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class Slot : MonoBehaviour, IPointerClickHandler,IDropHandler,IBeginDragHandler,IDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler
{
    public ItemData item;
    public int itemCount; // 획득한 아이템의 개수
    public RawImage itemImage;  // 아이템의 이미지
    public UI ui;

    private bool isEnter = false;
    
    [SerializeField]
    private TextMeshProUGUI text_Count;
    [SerializeField]
    private TextMeshProUGUI itemName;
    [SerializeField]
    private TextMeshProUGUI itemToolTip;

    // 아이템 이미지의 투명도 조절
    private void SetColor(float _alpha)
    {
        Color color = itemImage.color;
        color.a = _alpha;
        itemImage.color = color;
    }

    // 인벤토리에 새로운 아이템 슬롯 추가
    public void AddItem(ItemData _item, int _count = 1)
    {
        item = _item;
        itemCount = _count;
        itemImage.texture = item.Image;

        if (!(_item.Type == ItemData.ItemType.Weapon || _item.Type == ItemData.ItemType.Cloth))
        {
            text_Count.gameObject.SetActive(true);
            text_Count.text = itemCount.ToString();
        }
        else
        {
            text_Count.gameObject.SetActive(false);
            text_Count.text = "0";
        }
        SetColor(1);
    }


    // 해당 슬롯의 아이템 갯수 업데이트
    public void SetSlotCount(int _count)
    {
        itemCount += _count;
        text_Count.text = itemCount.ToString();

        if (itemCount <= 0)
            ClearSlot();
    }

    // 해당 슬롯 하나 삭제
    public void ClearSlot()
    {
        SetColor(0);
        item = null;
        itemCount = 0;
        itemImage.texture = null;
        text_Count.text = "0";
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (item != null)
        {
            DragSlot.instance.dragSlot = this;
            DragSlot.instance.DragSetImage(itemImage);
            DragSlot.instance.transform.position = eventData.position;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (item != null)
            DragSlot.instance.transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        DragSlot.instance.SetColor(0);
        DragSlot.instance.dragSlot = null;
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (DragSlot.instance.dragSlot != null)
            ChangeSlot();
    }

    public void ChangeSlot()
    {
        ItemData _tempItem = item;
        int _tempItemCount = itemCount;

        AddItem(DragSlot.instance.dragSlot.item, DragSlot.instance.dragSlot.itemCount);

        if (_tempItem != null)
            DragSlot.instance.dragSlot.AddItem(_tempItem, _tempItemCount);
        else
            DragSlot.instance.dragSlot.ClearSlot();

        if (ui.toolTip.gameObject.activeSelf == false)
        {
            ui.toolTip.gameObject.SetActive(true);
            isEnter = true;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (item != null)
        {
            isEnter = true;
            ui.toolTip.gameObject.SetActive(true);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        ui.toolTip.gameObject.SetActive(false);
        isEnter = false;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        ui.toolTip.gameObject.SetActive(false);
        if (eventData.button == PointerEventData.InputButton.Right && item != null)
        {
            if (item.Type != ItemData.ItemType.Weapon && item.Type != ItemData.ItemType.Cloth)
            {
                ui.equipment.gameObject.SetActive(false);
                return;
            }
            ui.equipment.slot = this;
            ui.equipment.transform.position = eventData.position;
            ui.equipment.gameObject.SetActive(true);

            var isWeapon = item.Type == ItemData.ItemType.Weapon;
            ui.equipment.panelWeapon.SetActive(isWeapon);
            ui.equipment.panelCloth.SetActive(!isWeapon);

            if (isWeapon)
            {
                WeaponData.WeaponType type = ((WeaponData)item).type;
                var isRight = false;
                if (ui.character.slot[6].weapon != null)
                {
                    switch (ui.character.slot[6].weapon.type)
                    {
                        case WeaponData.WeaponType.Axe:
                            isRight = (type == WeaponData.WeaponType.Shield);
                            break;
                        case WeaponData.WeaponType.Hammer:
                            isRight = (type == WeaponData.WeaponType.Shield);
                            break;
                        case WeaponData.WeaponType.Sword:
                            isRight = (type == WeaponData.WeaponType.Sword || type == WeaponData.WeaponType.Shield);
                            break;
                    }
                }
                var isLeft = (type != WeaponData.WeaponType.Shield);

                ui.equipment.btnLeft.SetActive(isLeft);
                ui.equipment.btnRight.SetActive(isRight);
            }
        }
        else
            ui.equipment.gameObject.SetActive(false);
    }

    private void FixedUpdate()
    {
        if (isEnter)
        {
            itemName.text = item.Name;
            itemToolTip.text = item.Tooltip;
            ui.toolTip.transform.position = Input.mousePosition;
        }
    }
}