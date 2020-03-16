using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public Transform planet; // drag the planet here
    float radius =520; // planet radius
    float vel = 6; // player speed - degrees per second
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
        items = new GameObject[qntItems];
        // populate planet
        for (var i = 0; i < qntItems; i++)
        {
            // create a random item
            GameObject item = Instantiate(prefabs[Random.Range(0, prefabs.Length)]);
            MoveItem(item, Random.Range(bornAngle, killAngle)); // move it to a random position
            item.transform.SetParent(planet); // child item to the planet
            items[i] = item;
        }
        lastPickup = CreatePickup();
    }

    void Update()
    {
        // rotate planet according to up/down arrow keys
        float vAxis = Input.GetAxis("Vertical");
        if (vAxis > 0.1)
        { // if moving forward...
            planet.transform.Rotate(-vAxis * vel * Time.deltaTime, 0, 0); // rotate planet
            //animation.CrossFade("walk"); // play "walk" animation
        }
        else
        {
            //animation.CrossFade("idle"); // else play "idle"
        }
        for (var i = 0; i < qntItems; i++)
        {
            // if item passed the kill angle from Z axis...
            if (Vector3.Angle(items[i].transform.up, Vector3.forward) > killAngle)
            {
                // replant it at the born angle, at random position
                MoveItem(items[i], bornAngle);
            }
        }
        if (Vector3.Angle(lastPickup.transform.up, Vector3.forward) > pickupAngle)
        {
            lastPickup = CreatePickup();
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
