using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnGate : MonoBehaviour
{
    // Prefabs for gates
    public GameObject[] gatePrefabs;
    // Transform of the planet object
    Transform planet;
    // The player, which holds the information about the distance to gates
    public Player runningPlayer;

    // Values for determining creation and position of a new gate
    bool thereIsAGate;
    float radius = 25;
    float playerHeight = 0.25f;

    // Start is called before the first frame update
    void Start()
    {
        // Sets planet transform
        planet = transform;

        // Wait until needed to create a gate
        thereIsAGate = false;
    }

    // Update is called once per frame
    void Update()
    {
        // Checks the angle based on the last obstacle and checks if it is greater than
        // the designated gratable angle.  If it is, spawn a new obstacle
        if (runningPlayer.completedDistance >= runningPlayer.localDistanceToGate - 90
            && !thereIsAGate)
        {
            CreateGate();
            thereIsAGate = true;
        }
        else if (runningPlayer.completedDistance < runningPlayer.localDistanceToGate - 90)
        {
            thereIsAGate = false;
        }
    }

    // Creates the new gate
    void CreateGate()
    {
        // Create a new gate object in front of the planet
        GameObject theGate = Instantiate(gatePrefabs[Random.Range(0, gatePrefabs.Length)]);
        theGate.transform.SetParent(planet);
        theGate.transform.up = Vector3.forward;
        // Eight times the player's height seems to look good
        theGate.transform.position = planet.position + Vector3.forward * (radius + 8 * playerHeight);
    }
}
