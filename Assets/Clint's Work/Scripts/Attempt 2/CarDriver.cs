using UnityEngine;

public class CarDriver : MonoBehaviour
{
    private float speed = 0f;
    private float turnSpeed = 0f;

    public void SetInputs(float fwd,float turn)
    {
        speed = fwd * 50f;
        turnSpeed = turn * 100f;
    }
    private void Update()
    {
        transform.position += transform.forward * speed * Time.deltaTime;
        transform.Rotate(0, turnSpeed * Time.deltaTime, 0);
    }
    public float GetSpeed() { return Mathf.Abs(speed); }
    public void ClearTurnSpeed() { turnSpeed = 0f; }
}
