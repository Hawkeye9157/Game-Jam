using UnityEngine;

public class BrakeingZone : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        AICarController car = other.GetComponent<AICarController>();
        if (car)
        {
            car.isInsideBraking = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        AICarController car = other.GetComponent<AICarController>();
        if (car)
        {
            car.isInsideBraking = false;
        }
    }
}
