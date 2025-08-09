using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CarDriver))]
public class CarDriverAI : MonoBehaviour
{
    
    [SerializeField] public List<Transform> waypoints;
    [SerializeField] public float reachedTargetDistance = 5f;
    [SerializeField] public float reverseDistance = 10f;
    private int currentWaypoint = 0;
    public bool allowedToMove = true;

    private CarDriver driver;
    private Vector3 targetPosition;

    private void Awake()
    {
        driver = GetComponent<CarDriver>();
    }
    private void Update()
    {
        if (allowedToMove)
        {
            Move();
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
        driver.SetInputs(forwardAmount, turnAmount);
    }
}
