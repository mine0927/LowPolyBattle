using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;
using PlayFab.ClientModels;
using PlayFab;
using TMPro;
using System.Collections.Generic;

public class LobbyManager : MonoBehaviourPunCallbacks
{
    //실행하는 게임의 버전
    private readonly string version = "1.0";
    
    public TMP_InputField roomName;
    public TextMeshProUGUI roomCount;
    public Transform scrollContent;
    private GameObject roomItem;
    private Dictionary<string, GameObject> rooms = new Dictionary<string, GameObject>();
    private void Awake()
    {
        //마스터 클라이언트의 씬 자동 동기화
        PhotonNetwork.AutomaticallySyncScene = true;
        //게임의 버전 설정
        PhotonNetwork.GameVersion = version;
        //포톤 서버 접속

        roomItem = Resources.Load<GameObject>("Panel - Room");

        if(PhotonNetwork.IsConnected == false)
            PhotonNetwork.ConnectUsingSettings();
    }

    private void Start()
    {
        UserDataManager manager = UserDataManager.GetInstance();
        var request = new GetPlayerProfileRequest { PlayFabId = manager.userId };
        PlayFabClientAPI.GetPlayerProfile(request, ResultSkin, manager.ErrorMsg);
        
    }

    private void ResultSkin(GetPlayerProfileResult result)
    {
        PhotonNetwork.NickName = result.PlayerProfile.DisplayName;
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        GameObject tempRoom = null;

        foreach(var roomInfo in roomList)
        {
            if(roomInfo.RemovedFromList == true)
            {
                rooms.TryGetValue(roomInfo.Name, out tempRoom);
                Destroy(tempRoom);
                rooms.Remove(roomInfo.Name);
            }
            else
            {
                if(rooms.ContainsKey(roomInfo.Name) == false)
                {
                    GameObject roomPrefab = Instantiate(roomItem, scrollContent);
                    roomPrefab.GetComponent<RoomData>().RoomInfo = roomInfo;
                    rooms.Add(roomInfo.Name, roomPrefab);
                }
                else
                {
                    rooms.TryGetValue(roomInfo.Name, out tempRoom);
                    tempRoom.GetComponent<RoomData>().RoomInfo = roomInfo;
                }
            }
        }
    }

    public void OnMakeRoomClick()
    {
        RoomOptions ro = new RoomOptions();
        int.TryParse(roomCount.text, out var count);
        ro.MaxPlayers = (byte)count;
        ro.IsOpen = true;
        ro.IsVisible = true;

        PhotonNetwork.CreateRoom(roomName.text, ro);
    }
    //포톤 서버에 접속 후 호출되는 함수
    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby();
    }

    //룸 생성이 완료된 후 호출되는 함수
    public override void OnCreatedRoom()
    {
        if (PhotonNetwork.IsMasterClient)
            PhotonNetwork.LoadLevel("04ReadyRoom");
    }

    public void RoomCount(bool isUp)
    {
        var count = int.Parse(roomCount.text);
        if (isUp)
        {
            if (count != 6) count ++;
        }
        else
        {
            if (count != 2) count--;
        }
        roomCount.text = count.ToString();
    }
}
