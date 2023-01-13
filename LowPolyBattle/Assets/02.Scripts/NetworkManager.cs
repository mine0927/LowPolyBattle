using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        if(newPlayer != PhotonNetwork.LocalPlayer)
            ((GameObject)PhotonNetwork.LocalPlayer.TagObject).GetComponent<PlayerControl>().Invoke();
    }
}
