﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GratableObject : MonoBehaviour
{
    public float slowdownOnHit; // The effect when the player strikes this obstacle
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