using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private float distance;
    private float speed;
    private float _deltaTime;
    [HideInInspector] public Transform targetTr;

    private void Start()
    {
        distance = Vector3.Distance(transform.position, targetTr.position);
    }

    private void FixedUpdate()
    {
        _deltaTime += Time.deltaTime;

        speed = _deltaTime;
        if(_deltaTime < 1.5f)
        {
            transform.Translate(transform.forward * speed, Space.World);
        }
        else
        {
            transform.position = Vector3.LerpUnclamped(transform.position, targetTr.position, speed / distance);
        }

        Vector3 directionVec = targetTr.position - transform.position;
        Quaternion qua = Quaternion.LookRotation(directionVec);
        transform.rotation = Quaternion.Slerp(transform.rotation, qua, Time.deltaTime * 2f);
    }
}
