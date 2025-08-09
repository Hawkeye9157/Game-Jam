using UnityEngine;

public class Glitch : MonoBehaviour
{
    public float glitchForce = 10f; 
    public float glitchInterval = 2f; 
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        InvokeRepeating("ApplyRandomGlitch", glitchInterval, glitchInterval);
    }

    void ApplyRandomGlitch()
    {
        
        Vector3 randomDirection = transform.forward + new Vector3(
            Random.Range(0, 1000.5f),
            0,
           0
        ).normalized;

        
        
        transform.position += randomDirection * glitchForce;
    }
}
