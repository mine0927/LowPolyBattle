using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartAnim : MonoBehaviour
{
    public GameObject[] models = new GameObject[6];
    private void Awake()
    {
        int num = Random.Range(0, 6);
        var obj1 = Instantiate(models[num]);
        obj1.transform.position = new Vector3(5.368f, 0f, -0.82f);
        obj1.transform.localEulerAngles = new Vector3(0f, 270f, 0f);

        int num2;
        while (true)
        {
            num2 = Random.Range(0, 6);
            if (num2 != num) break;
        }
        var obj2 = Instantiate(models[num2]);
        obj2.transform.position = new Vector3(-5.99f, 0f, 0f);
        obj2.transform.localEulerAngles = new Vector3(0f, 90f, 0f);
    }

}
