using Photon.Pun;
using UnityEngine;
public class Equipment : MonoBehaviour
{
    public UI ui;
    public Slot slot;
    public GameObject panelCloth;
    public GameObject panelWeapon;
    public GameObject btnLeft;
    public GameObject btnRight;

    public void UseCloth()
    {
        ui.character.Setting((ClothData)slot.item, null, slot, false);
        this.gameObject.SetActive(false);
    }

    public void UseWeapon(bool isLeft)
    {
        ui.character.Setting(null, (WeaponData)slot.item, slot, isLeft);
        this.gameObject.SetActive(false);
    }

    public void Delete()
    {
        PhotonNetwork.Instantiate(slot.item.DropItem.name, ui.player.transform.position, Quaternion.identity);
        slot.ClearSlot();
        this.gameObject.SetActive(false);
    }

}
