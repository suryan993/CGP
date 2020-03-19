using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public Transform planet; // drag the planet here
    Transform playerRotationCore; // allows the player to tilt around the surface of the planet
    float radius = 520; // planet radius
    float currentVelocity = 6; // player speed - degrees per second
    float overallAcceleration;
    float maxVelocity = 100;
    float[] laneAngles = { 0.02f, 0.01f, 0.0f, -0.01f, -0.02f }; // z values for where to rotate to be in each lane
    int leftmostLane; // array index to show how far the player is allowed to move at present
    int rightmostLane; // all these ints are array indices
    int currentLane;
    int destinationLane;
    bool isChangingLeft = false;
    bool isChangingRight = false;
    float laneChangeVel = 6; // player speed in z rotating around the playerRotationCore
    public GameObject[] prefabs; // drag the item prefabs here
    int qntItems = 30; // how many items populate the scene
    float bornAngle = 0; // items born at this X angle
    float killAngle = 90; // items disappear after this angle
    float pathAngle = 10; // path angle from vertical
    float grassAngle = 45; // end of grass angle from vertical
    public GameObject[] pickupPrefabs; // drag the pickup prefabs here
    float pickupAngle = 7; // degrees between pickups
    float pickupHeight = 1; // pickup height above ground

    public GameObject[] items;
    GameObject lastPickup; // last pickup created

    void Start()
    {
        playerRotationCore = transform.parent;
        leftmostLane = 0; // The number of lanes can change depending on conditions during play
        rightmostLane = 4;
        currentLane = 2;
        overallAcceleration = 2; // Different conditions can have different default acceleration

        items = new GameObject[qntItems];
        // populate planet
        for (var i = 0; i < qntItems; i++)
        {
            // create a random item
            //GameObject item = Instantiate(prefabs[Random.Range(0, prefabs.Length)]);
            //MoveItem(item, Random.Range(bornAngle, killAngle)); // move it to a random position
            //item.transform.SetParent(planet); // child item to the planet
            //items[i] = item;
        }
        //lastPickup = CreatePickup();
    }

    void Update()
    {
        // Rotate planet according to player's velocity
        planet.transform.Rotate(-currentVelocity * Time.deltaTime, 0, 0); // rotate planet
        //animation.CrossFade("walk"); // play "walk" animation
        //animation.CrossFade("idle"); // else play "idle"

        // Accelerate regularly
        currentVelocity = currentVelocity + overallAcceleration * Time.deltaTime;
        if (currentVelocity > maxVelocity)
        {
            currentVelocity = maxVelocity;
        }

        //for (var i = 0; i < qntItems; i++)
        //{
        //    // if item passed the kill angle from Z axis...
        //    if (Vector3.Angle(items[i].transform.up, Vector3.forward) > killAngle)
        //    {
        //        // replant it at the born angle, at random position
        //        MoveItem(items[i], bornAngle);
        //    }
        //}
        //if (Vector3.Angle(lastPickup.transform.up, Vector3.forward) > pickupAngle)
        //{
        //    lastPickup = CreatePickup();
        //}

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

    // Move item to a random position in the grass, at elevation angleX
    void MoveItem(GameObject item, float angleX)
    {
        float angleY;
        if (Random.value < 0.5)
        { // randomly select left or right sides
            angleY = Random.Range(-grassAngle, -pathAngle);
        }
        else
        {
            angleY = Random.Range(pathAngle, grassAngle);
        }
        // calculate new item up direction
        var dir = Quaternion.Euler(0, angleY, 0) * Vector3.forward;
        dir = Quaternion.Euler(-angleX, 0, 0) * dir;
        // set item position and rotation
        item.transform.position = planet.transform.position + radius * dir;
        item.transform.rotation = Quaternion.FromToRotation(Vector3.up, dir);
        // set random size and rotation about Y 
        item.transform.Rotate(0, Random.Range(0, 360), 0);
        item.transform.localScale = Random.Range(0.4f, 1.5f) * Vector3.one;
        // set a random color
        Vector4 rndColor = Random.insideUnitSphere;
        Renderer rndr = item.GetComponentInChildren<Renderer>();
        rndr.material.color = rndColor;
    }

    GameObject CreatePickup()
    { // create pickup in front of the planet
        GameObject pickup = Instantiate(pickupPrefabs[Random.Range(0, pickupPrefabs.Length)]);
        pickup.transform.SetParent(planet);
        pickup.transform.up = Vector3.forward;
        pickup.transform.position = planet.position + Vector3.forward * (radius + pickupHeight);
        return pickup;
    }
}
