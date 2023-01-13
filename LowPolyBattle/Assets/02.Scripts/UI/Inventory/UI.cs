using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Photon.Pun;

public class UI : MonoBehaviour, IPointerClickHandler 
{
    public PhotonView pv;
    public Inventory inventory;
    public Character character;
    public Equipment equipment;
    public UnEquipment unEquipment;
    public Image toolTip;
    public PlayerControl player;
    public void playerCom()
    {
        
        foreach (PlayerControl Player in FindObjectsOfType<PlayerControl>())
        {
            if (Player.photonView.IsMine) player = Player;
        }
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        equipment.gameObject.SetActive(false);
        unEquipment.gameObject.SetActive(false);
    }
}
