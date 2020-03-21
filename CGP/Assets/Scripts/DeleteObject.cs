using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeleteObject : MonoBehaviour
{
    public float zValue;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Delete(zValue);
    }

    void Delete(float zValue)
    {
        if (transform.position.z < zValue)
            Destroy(gameObject);
    }
}
