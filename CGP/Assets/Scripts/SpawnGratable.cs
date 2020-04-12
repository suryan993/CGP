using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnGratable : MonoBehaviour
{
    // Prefabs for obstacles that slow down the player when hit
    public GameObject[] gratablePrefabs;
    // Transform of the planet object
    Transform planet;

    bool spawnPickups = true;

    // values for determining position and movement of objects
    float radius = 25;
    float playerHeight = 0.25f;
    float[] laneAngles = { 0.02f, 0.01f, 0.0f, -0.01f, -0.02f };
    float gratableAngle = 7;

    // the newest obstacle in the scene
    GameObject lastGratable;


    // Start is called before the first frame update
    void Start()
    {
        // sets planet transform and creates first obstacle
        planet = transform;
        lastGratable = CreateGratable();
    }

    // Update is called once per frame
    void Update()
    {
        // checks the angle based on the last obstacle and checks if it is greater than
        // the designated gratable angle.  If it is, spawn a new obstacle
        if (spawnPickups)
        {
            if (lastGratable == null || Vector3.Angle(lastGratable.transform.up, Vector3.forward) > gratableAngle)
            {
                lastGratable = CreateGratable();
            }
        }
    }

    // creates a new obstacle
    GameObject CreateGratable()
    { 
        // Create a new gratable object in front of the planet
        GameObject gratable = Instantiate(gratablePrefabs[Random.Range(0, gratablePrefabs.Length)]);
        gratable.tag = "Gratable";
        gratable.transform.SetParent(planet);
        gratable.transform.up = Vector3.forward;
        gratable.transform.position = planet.position + Vector3.forward * (radius + playerHeight);
        gratable.transform.RotateAround(planet.position, Vector3.up, 120 * laneAngles[Random.Range(0, laneAngles.Length)]);
        return gratable;
    }

    public void ToggleSpawnPickups()
    {
        if (spawnPickups)
        {
            spawnPickups = false;
        } else
        {
            spawnPickups = true;
        }
    }

    public void DestroyAllGratables()
    {
        GameObject[] allGratables = GameObject.FindGameObjectsWithTag("Gratable");
        foreach(GameObject gratable in allGratables)
        {
            Destroy(gratable);
        }
    }
}
