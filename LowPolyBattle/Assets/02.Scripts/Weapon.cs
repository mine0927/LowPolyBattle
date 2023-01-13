using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class Weapon : MonoBehaviour
{
    public WeaponData data;
    public PlayerControl player;

    private void Awake()
    {
        gameObject.tag = player.tag;
    }
}
