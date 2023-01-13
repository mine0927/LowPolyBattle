using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerSlotManager : MonoBehaviour
{
    // Start is called before the first frame update
    public PlayerSlot[] slots  = new PlayerSlot[6];
    public PhotonView pv;
    public bool isStart = false;
    public StartData startData;
    public void GetName(int no, string name)
    {
        pv.RPC(nameof(SetName), RpcTarget.AllBufferedViaServer, no, name);
    }

    public void GetReady(int no, bool isReady, bool isActive)
    {
        pv.RPC(nameof(SetReady), RpcTarget.AllBufferedViaServer, no, isReady, isActive);
    }

    public void GetOrder(int no, int order)
    {
        pv.RPC(nameof(SetOrder), RpcTarget.AllBufferedViaServer, no, order);
    }

    public void GetLock(int no)
    {
        pv.RPC(nameof(SetLock), RpcTarget.AllBufferedViaServer, no);
    }

    public void GetTeam(int no, bool isActive, string color)
    {
        pv.RPC(nameof(SetTeam), RpcTarget.AllBufferedViaServer, no, isActive, color);
    }

    public void GetOwner(int no, bool isActive)
    {
        pv.RPC(nameof(SetOwner), RpcTarget.AllBufferedViaServer, no, isActive);
    }
    [PunRPC]
    private void SetName(int no, string name)
    {
        slots[no].nickName.text = name;
    }

    [PunRPC]
    private void SetReady(int no, bool isReady, bool isActive)
    {
        slots[no].ready.gameObject.SetActive(isActive);
        if (isReady || slots[no].owner.activeSelf)
            slots[no].ready.text = "Ready";
        else
            slots[no].ready.text = "Wait";
    }

    [PunRPC]
    private void SetOrder(int no, int order)
    {
        slots[no].order = order;
    }
    
    [PunRPC]
    private void SetLock(int no)
    {
        for(int i = 5; i> no - 1; i--)
        {
            slots[i].lockObj.SetActive(true);
        }
    }

    [PunRPC]
    private void SetTeam(int no, bool isActive, string color)
    {
        slots[no].team.gameObject.SetActive(isActive);
    }

    [PunRPC]
    private void SetOwner(int no, bool isActive)
    {
        slots[no].owner.SetActive(isActive);
    }

    public void SetStart()
    {
        Dictionary<Color, int> color = new Dictionary<Color, int>();
        Color checkColor = Color.black;
        int count = 0;
        for(int i = 0; i< 6; i++)
        {
            if(slots[i].order != -1 && slots[i].ready.text != "Ready")
            {
                Debug.Log("전원 레디해야 시작할 수 있습니다.");
                return;
            }
            else if(slots[i].order != -1)
            {
                checkColor = slots[i].team.color;
                if (color.ContainsKey(checkColor))
                    color[checkColor] = color[checkColor] + 1;
                else
                {
                    color.Add(checkColor, 1);
                    count++;
                }
            }
        }
        /*
        if(count == 1)
        {
            Debug.Log("팀 하나인 상태로 게임을 시작할 수 없습니다.");
            return;
        }
        */
        foreach(int value in color.Values)
        {
            if(value != color[checkColor])
            {
                Debug.Log("팀 인원수가 균등하지 않습니다.");
                return;
            }
        }
        isStart = true;
    }
}
