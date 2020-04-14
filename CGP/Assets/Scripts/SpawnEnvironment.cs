using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SpawnEnvironment : MonoBehaviour
{
    // Prefabs for obstacles that slow down the player when hit
    public GameObject[] environmentPrefabs;
    // Transform of the planet object
    Transform planet;

    // values for determining position and movement of objects
    float radius = 25;
    float playerHeight = 0.25f;
    float[] laneAngles = { 0.02f, 0.01f, 0.0f, -0.01f, -0.02f };
    float maxAngleLeft = 0.3f;
    float minAngleLeft = 0.05f;
    float maxAngleRight = -0.05f;
    float minAngleRight = -0.3f;
    float environmentObjectAngle = 8;

    // the newest obstacle in the scene
    GameObject lastEnvironmentObject;

    int completedLevels;

    // Start is called before the first frame update
    void Start()
    {
        // sets planet transform and creates first obstacle
        planet = transform;
        lastEnvironmentObject = CreateEnvironmentObject();
        completedLevels = GameObject.Find("Player").GetComponent<Player>().completedLevels;
    }

    // Update is called once per frame
    void Update()
    {
        if (lastEnvironmentObject == null || Vector3.Angle(lastEnvironmentObject.transform.up, Vector3.forward) > environmentObjectAngle)
        {
            lastEnvironmentObject = CreateEnvironmentObject();
        }
    }

    float GetEnvironmentSpawnAngle()
    {
        int side = Random.Range(0, 2);
        if(side == 0)
        {
            return Random.Range(minAngleLeft,maxAngleLeft);
        } else
        {
            return Random.Range(minAngleRight, maxAngleRight);
        }

    }

    // creates a new obstacle
    GameObject CreateEnvironmentObject()
    {
        // Create a new spawnable object in front of the planet
        GameObject spawnObj = null;
        spawnObj = Instantiate(environmentPrefabs[Random.Range(0, environmentPrefabs.Length)]);
        spawnObj.tag = "Environment";

        spawnObj.transform.SetParent(planet);
        spawnObj.transform.up = Vector3.forward;
        spawnObj.transform.position = planet.position + Vector3.forward * (radius-0.1f);

        spawnObj.transform.RotateAround(planet.position, Vector3.up, 120 * GetEnvironmentSpawnAngle());
        return spawnObj;
    }


    public void DestroyAllEnvironment()
    {
        GameObject[] allEnvironmentObjects = GameObject.FindGameObjectsWithTag("Environment");
        foreach (GameObject environmentObject in allEnvironmentObjects)
        {
            Destroy(environmentObject);
        }
    }
}
