using System.Collections.Generic;
using UnityEngine;

public class GlitchKv : MonoBehaviour
{
    public CameraGlitchController cameraGlitch;

    [Header("Glitch Settings")]
    public float glitchForce = 10f;
    public float minInterval = 1f;
    public float maxInterval = 3f;
    public float maxGlitchDistance = 5f;
    public bool affectPlayers = true;
    

    [Header("Visual Effects")]
    public ParticleSystem glitchParticles;
    public AudioSource glitchSound;

    private float nextGlitchTime;
    private List<Rigidbody> allRigidbodies = new List<Rigidbody>();



    void Start()
    { 
        // Initialize with first glitch time
        SetNextGlitchTime();

        // Find all players in the scene
        if (affectPlayers)
        {
            GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
            foreach (GameObject player in players)
            {
                Rigidbody playerRb = player.GetComponent<Rigidbody>();
                if (playerRb != null)
                {
                    allRigidbodies.Add(playerRb);
                }
            }
        }
        else
        {
            Rigidbody rb = GetComponent<Rigidbody>();
            if (rb != null) allRigidbodies.Add(rb);
        }
        
    }

    void Update()
    {
        if (Time.time >= nextGlitchTime)
        {
            ApplyGlitchEffect();
            SetNextGlitchTime();
        }
    }

    void SetNextGlitchTime()
    {
        nextGlitchTime = Time.time + Random.Range(minInterval, maxInterval);
    }

    void ApplyGlitchEffect()
    {
        foreach (Rigidbody bodies in allRigidbodies)
        {
            if (bodies != null)
            {
                
                Vector3 glitchDirection = new Vector3(
                    Random.Range(-10f, 10f),
                    0,
                     Random.Range(-10f, 10f)
                ).normalized;

                
                float distanceFactor = Mathf.Clamp01(glitchForce / maxGlitchDistance);
                float actualForce = Mathf.Lerp(glitchForce * 0.5f, glitchForce, distanceFactor);

                // Apply the glitch
                bodies.MovePosition(bodies.position + glitchDirection * actualForce);

                // Visual/Audio effects
                if (glitchParticles != null)
                {
                    Instantiate(glitchParticles, bodies.position, Quaternion.identity);
                }


                
            }
        }
            // Trigger camera effect
                if (cameraGlitch != null)
                    cameraGlitch.TriggerGlitch();

                if (glitchSound != null)
                {
                    glitchSound.Play();
                }
    }
}
