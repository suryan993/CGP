using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class Player : MonoBehaviour
{
    public AudioSource CHEESE;
    public Transform planet; // drag the planet here
    Transform playerRotationCore; // allows the player to tilt around the surface of the planet
    //float radius = 25; // planet radius
    float currentVelocity; // player speed - degrees per second
    float overallAcceleration;
    float minVelocity = 5;
    float maxVelocity = 50;
    float minAcceleration = 0.15f;
    float maxAcceleration = 15f;
    public SpeedInfo speedDisplay;
    public AccelerationInfo accelerationDisplay;
    float[] laneAngles = { 0.02f, 0.01f, 0.0f, -0.01f, -0.02f }; // z values for where to rotate to be in each lane
    int leftmostLane; // array index to show how far the player is allowed to move at present
    int rightmostLane; // all these ints are array indices
    int currentLane;
    int destinationLane;
    bool isChangingLeft = false;
    bool isChangingRight = false;
    bool accelerationBlock = false;
    float laneChangeVel = 6; // player speed in z rotating around the playerRotationCore
    float pickupDelay = 0f;
    bool togglePickups = false;
    public int lives = 3;
    bool gateChecked = false;
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

    public float points = 0;
    public Text pointsText;

    public Text livesText;

    public int config_points_cheese = 10;
    public int config_points_gates = 100;
    public int config_points_distance = 1;

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
    int minRandDistance = 140;
    int maxRandDistance = 360;

    Queue<int> keyBuffer;
    public int config_key_buffer_count = 3;

    // How well the player has done
    public int completedLevels;

    void Start()
    {
        playerRotationCore = transform.parent;
        leftmostLane = 0; // The number of lanes can change depending on conditions during play
        rightmostLane = 4;
        currentLane = 2;
        overallAcceleration = 1.5f; // Different conditions can have different default acceleration
        currentVelocity = minVelocity;

        // Set default display and get the distance for the first gate from the UI
        speedDisplay.MapToRange(botRange, topRange);
        localDistanceToGate = distanceToGate; // This should be larger then 90 at least
        displayDistance = localDistanceToGate;
        speedDisplay.SetRange();
        completedDistance = 0;
        completedLevels = 0;
        keyBuffer = new Queue<int>();
    }

    void Update()
    {
        //Debug.Log("Key Buffer Count " + keyBuffer.Count);
        if (togglePickups)
        {
            if(pickupDelay < 0)
            {
                ToggleSpawnPickups();
                togglePickups = false;

            }
            else
            {
                pickupDelay -= Time.deltaTime;
            }
        }
        //Display distance and speed
        displayDistance = (displayDistance - (currentVelocity * Time.deltaTime));
        distanceText.text = ((int)displayDistance).ToString();
        speedText.text = ((int)currentVelocity).ToString();

        livesText.text = "x"+lives.ToString();

        //Add points and display
        points += Time.deltaTime * currentVelocity * config_points_distance;
        pointsText.text = ((int)points).ToString();

        // Rotate planet according to player's velocity and mark the amount of distance completed
        planet.transform.Rotate(-currentVelocity * Time.deltaTime, 0, 0);
        completedDistance += currentVelocity * Time.deltaTime;
        //animation.CrossFade("walk"); // play "walk" animation
        //animation.CrossFade("idle"); // else play "idle"

        // Accelerate regularly
        if (!accelerationBlock)
        {
            currentVelocity = currentVelocity + overallAcceleration * Time.deltaTime;
            if (currentVelocity > maxVelocity)
            {
                currentVelocity = maxVelocity;
            }
        }

        // Checks and changes the gates
        HandleGates();
        accelerationDisplay.DisplayCurrentAcceleration(overallAcceleration, minAcceleration, maxAcceleration);

        // Check for and handle lane changes

        if (Input.GetKeyDown(KeyCode.LeftArrow)){
            queueKeyDownLeft();
        }

        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            queueKeyDownRight();
        }

        if (!isChangingLeft && !isChangingRight && keyBuffer.Count > 0)
        {
            if (keyBuffer.Peek() == -1)
            {
                if (currentLane > leftmostLane)
                {
                    keyBuffer.Dequeue();
                    destinationLane = currentLane - 1;
                    isChangingLeft = true;
                } else
                {
                    keyBuffer.Dequeue();
                }
            }
            else if (keyBuffer.Peek() == 1)
            {
                if (currentLane < rightmostLane)
                {
                    keyBuffer.Dequeue();
                    destinationLane = currentLane + 1;
                    isChangingRight = true;
                } else
                {
                    keyBuffer.Dequeue();
                }
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

        /*        float hAxis = Input.GetAxis("Horizontal"); //Old Input Handling
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
                }*/
    }

    // Handles logic determining whether to change gates or end the game
    void HandleGates()
    {
        speedDisplay.DisplayCurrentSpeed(currentVelocity, minVelocity, maxVelocity);

        // checks if current localDistanceToGate is equal to the current distance 
        if (completedDistance >= localDistanceToGate)
        {
            accelerationBlock = false;
            // checks if the current speed of the player (ascertained from the slider value) 
            // is within the top and bottom ranges.  If so, player has met the threshold for the next level
            if ((speedDisplay.slider.value > botRange && speedDisplay.slider.value < topRange) || gateChecked)
            {
                Debug.Log(topRange);
                Debug.Log(speedDisplay.slider.value);
                Debug.Log(botRange);
               
                points += config_points_gates;
                completedLevels++;
                overallAcceleration = overallAcceleration + (.1f * completedLevels);
                maxVelocity += (.5f * completedLevels);
                minVelocity += (.5f * completedLevels);
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
                ToggleSpawnPickups();
                pickupDelay = 1.0f; //Timer to disable pickups
                togglePickups = true;
                DestroyAllGratables();
                gateChecked = false;
            }
            else // if the current speed is NOT within range, player loses
            {
                gateChecked = true;
                lives--;
                Debug.Log("Lives : " + lives);
                if (lives <= 0)
                {
                    Debug.Log("DEFEAT"); // bool or function to signify end of game
                    SceneManager.LoadScene("GameOver");
                }
            }
        }
    }

    void queueKeyDownLeft()
    {
        if(keyBuffer.Count <= config_key_buffer_count)
        {
            keyBuffer.Enqueue(-1);
        }
    }

    void queueKeyDownRight()
    {
        if (keyBuffer.Count <= config_key_buffer_count)
        {
            keyBuffer.Enqueue(1);
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
        if (objectHit.CompareTag("Gratable"))
            HandleGraterHit(objectHit);
        else if (objectHit.CompareTag("PickUp"))
            HandlePickUpHit(objectHit);

    }

    void HandleGraterHit(Collider objectHit)
    {
        CHEESE.Play();
        GratableObject objectQualities = objectHit.GetComponent<GratableObject>();
        if (objectQualities != null)
        {
            accelerationBlock = false;
            points += config_points_cheese;
            currentVelocity = currentVelocity - objectQualities.slowdownOnHit;
            if (currentVelocity < minVelocity)
            {
                currentVelocity = minVelocity;
            }
        }
        objectQualities.EmitParticlesAndDestroy(planet);
    }

    void HandlePickUpHit(Collider pickUpHit)
    {
        PowerUp powerUp = pickUpHit.GetComponent<PowerUp>();
        if(powerUp != null)
        {
            if (powerUp.powerUPType == PowerUpType.Accelerate)
            {
                overallAcceleration += powerUp.powerUpValue;
                accelerationBlock = false;
            } else if(powerUp.powerUPType == PowerUpType.Decelerate)
            {
                overallAcceleration -= powerUp.powerUpValue;
                accelerationBlock = false;
            } else
            {
                accelerationBlock = true;
            }
            if (overallAcceleration < minAcceleration)
                overallAcceleration = minAcceleration;
            if (overallAcceleration > maxAcceleration)
                overallAcceleration = maxAcceleration;
        }
        powerUp.EmitParticlesAndDestroy(planet);
    }

    public void ToggleSpawnPickups()
    {
        Debug.Log("Spawn Toggled");
        SpawnGratable script = (SpawnGratable)GameObject.Find("Planet").GetComponent<SpawnGratable>();
        script.ToggleSpawnPickups();
    }

    public void DestroyAllGratables()
    {
        SpawnGratable script = (SpawnGratable)GameObject.Find("Planet").GetComponent<SpawnGratable>();
        script.DestroyAllGratables();
    }
}