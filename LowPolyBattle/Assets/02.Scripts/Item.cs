using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    public ItemData itemData;
    public float rotateSpeed = 0.7f;

    private void Update()
    {
        transform.Rotate(Vector3.up * rotateSpeed);
        if (Physics.Raycast(transform.position + (Vector3.up * 0.5f), Vector3.down, 1f) == false)
            transform.position += Vector3.down * 0.5f;
        if (transform.position.y < -20f)
            Destroy(this.gameObject);
    }

    public void Destroy()
    {
        Destroy(this.gameObject);
    }

}
