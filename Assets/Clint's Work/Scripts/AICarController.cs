using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BaseCarController))]
public class AICarController : MonoBehaviour
{
    public WaypointContainer wpContainer;
    private List<Transform> waypoints;
    private int currentWaypoint = 0;
    private BaseCarController carController;

    [SerializeField] public float range;
    public float currentAngle;
    public float gasInput;
    public bool isInsideBraking;
    public float maxAngle = 45f;
    public float maxSpeed = 120f;

    private void Start()
    {
        carController = GetComponent<BaseCarController>();
        waypoints = wpContainer.waypoints;
        currentWaypoint = 0;
    }
    private void Update()
    {
        if (Vector3.Distance(waypoints[currentWaypoint].position,transform.position) < range)
        {
            currentWaypoint++;
            if (currentWaypoint == waypoints.Count) currentWaypoint = 0;
        }
        Vector3 fwd = transform.TransformDirection(Vector3.forward);
        currentAngle = Vector3.Angle(transform.position, waypoints[currentWaypoint].transform.position - transform.position);
        if (currentAngle == 180) currentAngle -= 180;
        gasInput = Mathf.Clamp01(maxAngle - Mathf.Abs(carController.speed * 0.01f * currentAngle) / (maxAngle));
        if (isInsideBraking)
        {
            gasInput = -gasInput * (Mathf.Clamp01(carController.speed/maxSpeed)* 2 - 1f);
        }
        carController.SetInput(gasInput,currentAngle,0,0);
        Debug.DrawRay(transform.position, waypoints[currentWaypoint].position - transform.position,Color.yellow);
    }
}
