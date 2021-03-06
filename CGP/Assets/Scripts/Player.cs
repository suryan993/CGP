﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class Player : MonoBehaviour
{
    public Transform planet; // drag the planet here
    Transform playerRotationCore; // allows the player to tilt around the surface of the planet
    //float radius = 25; // planet radius
    float currentVelocity; // player speed - degrees per second
    float overallAcceleration;
    float minVelocity = 5;
    float maxVelocity = 50;
    public SpeedInfo speedDisplay;
    float[] laneAngles = { 0.02f, 0.01f, 0.0f, -0.01f, -0.02f }; // z values for where to rotate to be in each lane
    int leftmostLane; // array index to show how far the player is allowed to move at present
    int rightmostLane; // all these ints are array indices
    int currentLane;
    int destinationLane;
    bool isChangingLeft = false;
    bool isChangingRight = false;
    float laneChangeVel = 6; // player speed in z rotating around the playerRotationCore
    //public GameObject[] prefabs; // drag the item prefabs here
    //int qntItems = 30; // how many items populate the scene
    //float bornAngle = 0; // items born at this X angle
    //float killAngle = 90; // items disappear after this angle
    //float pathAngle = 10; // path angle from vertical
    //float grassAngle = 45; // end of grass angle from vertical
    public GameObject[] gratablePrefabs; // drag the gratable prefabs here
    //float gratableAngle = 7; // degrees between gratable objects
    //float playerHeight = 0.25f; // general height above ground where the player can be found
    //GameObject lastGratable; // last gratable object created
    public Text distanceText;
    public float displayDistance;

    public Text speedText;

    [System.NonSerialized]
    public GameObject[] items;

    // values used for determining range and time used by the gates
    public float topRange;
    public float botRange;
    float requiredRangeDif = 0.3f;
    float oldTopRange;

    public int distanceToGate;
    [System.NonSerialized]
    public int localDistanceToGate;
    [System.NonSerialized]
    public float completedDistance;
    int minRandDistance = 180;
    int maxRandDistance = 360;

    // How well the player has done
    int completedLevels;

    void Start()
    {
        playerRotationCore = transform.parent;
        leftmostLane = 0; // The number of lanes can change depending on conditions during play
        rightmostLane = 4;
        currentLane = 2;
        overallAcceleration = 2; // Different conditions can have different default acceleration
        currentVelocity = minVelocity;

        // Set default display and get the distance for the first gate from the UI
        speedDisplay.MapToRange(botRange, topRange);
        localDistanceToGate = distanceToGate; // This should be larger then 90 at least
        displayDistance = localDistanceToGate;
        speedDisplay.SetRange();
        completedDistance = 0;
        completedLevels = 0;
    }

    void Update()
    {
        displayDistance = (displayDistance - (currentVelocity * Time.deltaTime));
        distanceText.text = ((int)displayDistance).ToString();
        speedText.text = ((int)currentVelocity).ToString();
        // Rotate planet according to player's velocity and mark the amount of distance completed
        planet.transform.Rotate(-currentVelocity * Time.deltaTime, 0, 0);
        completedDistance += currentVelocity * Time.deltaTime;
        //animation.CrossFade("walk"); // play "walk" animation
        //animation.CrossFade("idle"); // else play "idle"

        // Accelerate regularly
        currentVelocity = currentVelocity + overallAcceleration * Time.deltaTime;
        if (currentVelocity > maxVelocity)
        {
            currentVelocity = maxVelocity;
        }

        // Checks and changes the gates
        HandleGates();

        // Check for and handle lane changes
        float hAxis = Input.GetAxis("Horizontal");
        if (!isChangingLeft && !isChangingRight)
        {
            if (hAxis < -0.1 && currentLane > leftmostLane)
            {
                destinationLane = currentLane - 1;
                isChangingLeft = true;
            }
            else if (hAxis > 0.1 && currentLane < rightmostLane)
            {
                destinationLane = currentLane + 1;
                isChangingRight = true;
            }

        }
        else if (isChangingLeft)
        {
            if (playerRotationCore.rotation.z >= laneAngles[destinationLane])
            {
                currentLane = destinationLane;
                isChangingLeft = false;
            }
            else // Rotate player toward the new lane
            {
                playerRotationCore.Rotate(0, 0, laneChangeVel * Time.deltaTime);
            }
        }
        else if (isChangingRight)
        {
            if (playerRotationCore.rotation.z <= laneAngles[destinationLane])
            {
                currentLane = destinationLane;
                isChangingRight = false;
            }
            else // Rotate player toward the new lane
            {
                playerRotationCore.Rotate(0, 0, -laneChangeVel * Time.deltaTime);
            }
        }
    }

    // Handles logic determining whether to change gates or end the game
    void HandleGates()
    {
        speedDisplay.DisplayCurrentSpeed(currentVelocity, minVelocity, maxVelocity);

        // checks if current localDistanceToGate is equal to the current distance 
        if (completedDistance >= localDistanceToGate)
        {
            // checks if the current speed of the player (ascertained from the slider value) 
            // is within the top and bottom ranges.  If so, player has met the threshold for the next level
            if (speedDisplay.slider.value > botRange && speedDisplay.slider.value < topRange)
            {
                completedLevels++;

                // Update the limits on speed values for the next level
                ManipulateRangeForNewLevel();

                // map the range from the slider values to the height of the slider
                // this is used to properly position the bars along the slider to match the new range
                speedDisplay.MapToRange(botRange, topRange);

                // Changes the bars to match the new positions
                speedDisplay.SetRange();

                // Reset completedDistance and update localDistanceToGate for the next level
                completedDistance = 0;
                ManipulateDistanceForNewLevel();
            }
            else // if the current speed is NOT within range, player loses
            {
                Debug.Log("DEFEAT"); // bool or function to signify end of game
                SceneManager.LoadScene("GameOver");
            }
        }
    }

    // Increases the range for the gate
    void ManipulateRangeForNewLevel()
    {
        // Store the completed level's value for subsequent distance code, to maintain fairness
        oldTopRange = topRange;

        botRange = Random.Range(0.0f, 1.0f - requiredRangeDif);
        topRange = botRange + requiredRangeDif;
    }

    // Increases the range for the gate
    void ManipulateDistanceForNewLevel()
    {
        localDistanceToGate = (int)Random.Range(minRandDistance, maxRandDistance);

        // If the player must accelerate, give even more distance to complete the level
        if (topRange > oldTopRange)
        {
            localDistanceToGate = (int)(localDistanceToGate * topRange / oldTopRange);
        }
        displayDistance = localDistanceToGate;
        Debug.Log("Distance to next gate: " + localDistanceToGate);
    }

    // Move item to a random position in the grass, at elevation angleX
    void MoveItem(GameObject item, float angleX)
    {
        //float angleY;
        //if (Random.value < 0.5)
        //{ // randomly select left or right sides
        //    angleY = Random.Range(-grassAngle, -pathAngle);
        //}
        //else
        //{
        //    angleY = Random.Range(pathAngle, grassAngle);
        //}
        //// calculate new item up direction
        //var dir = Quaternion.Euler(0, angleY, 0) * Vector3.forward;
        //dir = Quaternion.Euler(-angleX, 0, 0) * dir;
        //// set item position and rotation
        //item.transform.position = planet.transform.position + radius * dir;
        //item.transform.rotation = Quaternion.FromToRotation(Vector3.up, dir);
        //// set random size and rotation about Y 
        //item.transform.Rotate(0, Random.Range(0, 360), 0);
        //item.transform.localScale = Random.Range(0.4f, 1.5f) * Vector3.one;
        //// set a random color
        //Vector4 rndColor = Random.insideUnitSphere;
        //Renderer rndr = item.GetComponentInChildren<Renderer>();
        //rndr.material.color = rndColor;
    }

    void OnTriggerEnter(Collider objectHit)
    {
        GratableObject objectQualities = objectHit.GetComponent<GratableObject>();
        if (objectQualities != null)
        {
            currentVelocity = currentVelocity - objectQualities.slowdownOnHit;
            if (currentVelocity < minVelocity)
            {
                currentVelocity = minVelocity;
            }
        }
        objectQualities.EmitParticlesAndDestroy(planet);
    }
}