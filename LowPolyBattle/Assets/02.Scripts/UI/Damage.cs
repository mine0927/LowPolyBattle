using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Damage : MonoBehaviour
{
    public float count = 4f;
    private Transform trans;
    private bool isTrue = false;

    void Update()
    {
        if (isTrue)
        {
            transform.LookAt(trans);
        }
        if (count >= 0)
        {
            transform.position += new Vector3(0, Time.deltaTime, 0);
            count -= Time.deltaTime;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    public void isLook(Transform tr)
    {
        isTrue = true;
        trans = tr;
    }
}
