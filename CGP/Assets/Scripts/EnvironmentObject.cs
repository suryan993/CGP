using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvironmentObject : MonoBehaviour
{
    public float spinSpeed; // How quickly the object rotates in the game world, if at all

    void Start()
    {
    }

    void Update()
    {
        // Rotate in place
        transform.Rotate(0, spinSpeed * Time.deltaTime, 0);
    }
}
