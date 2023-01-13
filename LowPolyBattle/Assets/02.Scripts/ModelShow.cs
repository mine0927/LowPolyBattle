using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ModelShow : MonoBehaviour
{
    public bool isTurn = false;
    public float rotateSpeed = 0;
    [HideInInspector] public Vector2 mousePos;
    [HideInInspector] public static int hairNum = 0;
    [HideInInspector] public static int skinNum = 0;
    [HideInInspector] public int clothNum = 0;
    [HideInInspector] public int weaponNum = 0;
    public GameObject[] hair = new GameObject[7];
    public GameObject[] skin = new GameObject[5];
    public GameObject[] cloth = new GameObject[9];

    // Update is called once per frame

    void Update()
    {
        if (isTurn) transform.Rotate(Vector3.up * rotateSpeed);
        if (Input.GetMouseButton(1))
        {
            mousePos = new Vector2(Input.GetAxis("Mouse X") * 10, 0);
            transform.Rotate(Vector3.up * mousePos.x * (-1));
        }
    }

    public void turnKey()
    {
        isTurn = !isTurn;
    }
    public void turnHair(bool isLeft)
    {
        PartChanger(hair, isLeft, ref hairNum);
    }
    public void turnSkin(bool isLeft)
    {
        PartChanger(skin, isLeft, ref skinNum);
    }

    public void turnCloth(bool isLeft)
    {
        PartChanger(cloth, isLeft, ref clothNum);
    }

    public void PartChanger(GameObject[] templist, bool isLeft, ref int num)
    {
        templist[num].SetActive(false);
        if (isLeft)
            num = num != 0 ? num - 1 : templist.Length - 1;
        else
            num = num != templist.Length - 1 ? num + 1 : 0;
        templist[num].SetActive(true);
    }
    public void UpdateText(TextMeshProUGUI text)
    {
        if (text.tag == "Red") text.text = hair[hairNum].name;
        else if (text.tag == "Orange") text.text = skin[skinNum].name;
    }
}
