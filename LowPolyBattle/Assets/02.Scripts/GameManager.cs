using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : MonoBehaviourPunCallbacks
{
    [SerializeField] private RectTransform canvas;
    [SerializeField] private PlayerSlotManager manager;
    void Start()
    {
        Transform point = GameObject.Find("SpawnPoint").transform;

        var TeamParaent = PhotonNetwork.Instantiate("Panel - Team", new Vector3(0, 0, 0), new Quaternion(0, 0, 0, 0), 0);
        TeamParaent.transform.SetParent(canvas, false);
        PhotonNetwork.Instantiate("Player", point.position, new Quaternion(0, 0, 0, 0), 0);
        manager.GetLock((int)PhotonNetwork.CurrentRoom.MaxPlayers);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        int idx = -1;
        for(int i=0; i<6; i++)
        {
            if (manager.slots[i].order == otherPlayer.ActorNumber)
                idx = i;
        }
        manager.GetName(idx, null);
        manager.GetOrder(idx, -1);
        manager.GetOwner(idx, false);
        manager.GetTeam(idx, false, "Red");
        manager.GetReady(idx, false, false);
    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        for (int i = 0; i < 6; i++)
        {
            if (newMasterClient.ActorNumber == manager.slots[i].order)
            {
                manager.GetOwner(i, true);
                manager.GetReady(i, true, false);
                ReadyStart ready = FindObjectOfType<ReadyStart>();
                ready.btnReady.SetActive(false);
                ready.btnStart.SetActive(true);
                break;
            }
        }
    }
}
