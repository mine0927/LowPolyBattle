using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

public class RoomData : MonoBehaviour
{
    private RoomInfo room;
    public TMP_Text roomName;
    public TMP_Text roomCount;
    public GameObject lockObj;

    public RoomInfo RoomInfo
    {
        get
        {
            return room;
        }
        set
        {
            room = value;
            roomName.text = room.Name;
            roomCount.text = $"({room.PlayerCount} / {room.MaxPlayers})";
            GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() => OnEnterRoom(room.Name));
        }
    }

    void OnEnterRoom(string roomName)
    {

        PhotonNetwork.JoinRoom(roomName);
    }
}
