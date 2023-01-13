using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Team : MonoBehaviour
{
    public PlayerControl player;
    public PlayerSlotManager manager;

    private void Start()
    {
        foreach(PlayerSlotManager slotManager in FindObjectsOfType<PlayerSlotManager>())
        {
            manager = slotManager;
        }
    }

    public void TeamSetting(string str)
    {
        player.SetTagRPC(str);
    }

    public void ReadySetting()
    {
        if (player != null)
        {
            bool isActive = manager.slots[player.roomCount].ready.text == "Ready" ? false : true;
            manager.GetReady(player.roomCount, isActive, true);
        }
    }

    public void StartSetting()
    {
        for(int i = 0; i < 6; i++)
        {
            if(manager.slots[i].order != 0 && manager.slots[i].ready.text == "Wait")
            {
                Debug.Log("아직 준비하지 않은 플레이어가 있습니다.");
                return;
            }
        }
        Photon.Realtime.Player[] players = PhotonNetwork.PlayerList;
    }
}
