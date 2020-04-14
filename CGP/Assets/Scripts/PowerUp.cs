using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PowerUpType
{
    Accelerate = 0,
    Decelerate,
    Constant
}


public class PowerUp : MonoBehaviour
{

    public PowerUpType powerUPType;
    public float powerUpValue = 0.05f; // The effect when the player strikes this obstacle
    public float spinSpeed; // How quickly the object rotates in the game world, if at all
    public ParticleSystem powerUpParticles;

    void Start()
    {
        if (powerUPType == PowerUpType.Accelerate)
            powerUpValue *= 1;
        else if (powerUPType == PowerUpType.Decelerate)
            powerUpValue *= -1;
        else
            powerUpValue = 0.0f;
    }

    void Update()
    {
        // Rotate in place
        transform.Rotate(0, spinSpeed * Time.deltaTime, 0);
    }

    public void EmitParticlesAndDestroy(Transform planet)
    {
        
        ParticleSystem particles = Instantiate(powerUpParticles, transform.position, Quaternion.identity);
        //particles.transform.SetParent(planet);
        Destroy(gameObject);
    }
}
