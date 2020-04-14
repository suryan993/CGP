using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeleteEnvironmentObject : MonoBehaviour
{
    public float zValue = -5;

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
