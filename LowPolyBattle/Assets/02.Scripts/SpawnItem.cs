using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnItem : MonoBehaviourPunCallbacks
{
    public GameObject[] items = new GameObject[68];
    private GameObject itemList;
    private void Start()
    {
        itemList = GameObject.Find("ItemList");
        PhotonView pv = GetComponent<PhotonView>();
        if (pv.IsMine) pv.RPC(nameof(CreateItem), RpcTarget.MasterClient);
    }

    [PunRPC]
    IEnumerator CreateItem()
    {
        WaitForSeconds wfs = new WaitForSeconds(5f);
        GameObject obj;
        while (true)
        {
            int i = Random.Range(0, 66);
            if (transform.childCount < 40)
            {
                obj = PhotonNetwork.Instantiate(items[i].name, itemList.transform.position, Quaternion.identity);
                obj.transform.SetParent(itemList.transform);
                obj.transform.position = new Vector3(Random.Range(-100,100), 100, Random.Range(-100,100));
            }
            yield return wfs;
        }
    }
}
