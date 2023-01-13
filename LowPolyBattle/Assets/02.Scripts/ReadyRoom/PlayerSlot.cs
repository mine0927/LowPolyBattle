using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using PlayFab.ClientModels;
using UnityEngine.UI;

public class PlayerSlot : MonoBehaviour
{
    // Start is called before the first frame update
    public TextMeshProUGUI nickName;
    public TextMeshProUGUI ready;
    public GameObject owner;
    public Image team;
    public GameObject lockObj;
    public int order = -1;
}
