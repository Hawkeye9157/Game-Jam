using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CarDriver))]
public class CarDriverAI : MonoBehaviour
{
    
    [SerializeField] public List<Transform> waypoints;
    [SerializeField] public float reachedTargetDistance = 5f;
    [SerializeField] public float reverseDistance = 10f;
    private int currentWaypoint = 0;
    public bool allowedToMove = false;
    public float glitchTimer;
    public float minGlitchTime = 1f;
    public float maxGlitchTime = 5f;
    public float successRate = .50f;

    private CarDriver driver;
    private Vector3 targetPosition;

    public int lapCount = 0;
    public int checkpointCount = 0;

    //kay's stuff
    [Header("Particles")]
    public ParticleSystem[] wheelSmokeParticles; 

    private void Awake()
    {
        driver = GetComponent<CarDriver>();
    }
    private void Start()
    {
        allowedToMove = false;
        glitchTimer = Random.Range(minGlitchTime, maxGlitchTime); 

    }
    private void Update()
    {
        if (allowedToMove)
        {
            glitchTimer -= Time.deltaTime;
            if(glitchTimer <= 0)
            {
                glitchTimer = Random.Range(minGlitchTime, maxGlitchTime);
                ApplyGlitch();
            }
            Move();

            //smoke
            if (wheelSmokeParticles != null && wheelSmokeParticles.Length > 0)
            {
                bool shouldEmit = driver.GetSpeed() > 5f;
                foreach (var smoke in wheelSmokeParticles)
                {
                    var emission = smoke.emission;
                    emission.enabled = shouldEmit;
                }
            }
        }
        
    }
    private void Move()
    {
        if (waypoints[currentWaypoint] == null) return;
        targetPosition = waypoints[currentWaypoint].position;
        float forwardAmount = 0f;
        float turnAmount = 0f;

        //direction and distance to target
        Vector3 dirToTarget = (targetPosition - transform.position).normalized;
        float distanceToTarget = Vector3.Distance(transform.position, targetPosition);

        //Dot Product to determine if distance is in front or behind
        float dot = Vector3.Dot(transform.forward, dirToTarget);
        float angleToTarget = Vector3.SignedAngle(transform.forward, dirToTarget, Vector3.up);

        if (distanceToTarget > reachedTargetDistance)
        {
            //target is in front or behind
            if (dot > 0 || distanceToTarget < reverseDistance)
            {
                forwardAmount = 1f; //move forward
            }
            else
            {
                forwardAmount = -1f; //reverse
                angleToTarget *= -1f;
            }
            //Turn based on angle to target
            turnAmount = Mathf.Clamp(angleToTarget / 45f, -2f, 2f);
            if (turnAmount < -1f || turnAmount > 1f) { forwardAmount *= 0.75f; }

            //Apply Braking if close to target and moving too fast
            if (distanceToTarget < 30f && driver.GetSpeed() > 15f)
            {
                forwardAmount = -1f;
            }
        }
        else
        {
            //target reached -- switch to next target
            forwardAmount = 0f;
            driver.ClearTurnSpeed();
            //switch waypoint
            currentWaypoint++;
            if (currentWaypoint == waypoints.Count) currentWaypoint = 0;
        }
        float dontTurnDot = .995f - .015f * (Mathf.Clamp(distanceToTarget, 0f, 100f) / 100f);
        if (dot > dontTurnDot)
        {
            turnAmount = 0f;
            driver.ClearTurnSpeed();
        }
        driver.SetInputs(forwardAmount, turnAmount);

    }
    private void ApplyGlitch()
    {
        float chance = Random.Range(0f, 1f);
        Debug.Log("Chance: " + chance);
        if(chance > successRate)
        {
            //glitch forward
            transform.position += transform.forward * 10.0f;
            Debug.Log("Passed");
        }
        else
        {
            //glitch backward
            transform.position += -transform.forward * 10.0f;
            Debug.Log("Failed");
        }
    }
}
