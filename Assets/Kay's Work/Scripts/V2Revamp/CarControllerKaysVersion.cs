using System;
using System.Collections.Generic;
using UnityEngine;

public class CarControllerKv : MonoBehaviour 
{
    

    public enum Axel
    {
        Front,
        Rear
    }

    [Serializable]
    public struct Wheel
    {
        public GameObject wheelModel;
        public WheelCollider wheelCollider;
        public Axel axel;
    }

    public float normalMaxAcceleration = 1000.0f;
    public float breakAcceleration = 50.0f;
    public float boostMultiplier;
    public float turnSensitivity = 1.0f;
    public float maxSteerAngle = 30.0f;
    public float speedMultiplier = 1.0f;

    public Vector3 _centerOfMass;

   

    public List<Wheel> wheels;

    private float moveInput;
    private float steerInput;
    private Rigidbody carRb;
    private float currentAcceleration;
    private bool isBoosting;
    public float currentSpeed {  get; private set; }
    public static float CurrentCarSpeed { get; private set; }

    //kay's stuff for start game
    [Header("Particles")]
    public ParticleSystem[] wheelSmokeParticles;
    public bool controlsEnabled = true;

    public int lapCount = 0;
    public int checkpointCount = 0;


    private void Start()
    {
        carRb = GetComponent<Rigidbody>();
        carRb.centerOfMass = _centerOfMass;
        currentAcceleration = normalMaxAcceleration;

    } 

    //kays's method
    public void SetControlEnabled(bool enabled)
    {
        controlsEnabled = enabled;
    }

    private void Update()
    {
        if (!controlsEnabled) return;
        GetInputs();
        AnimateWheels();
        UpdateSpeed();

        // Control smoke particles
        if (wheelSmokeParticles != null && wheelSmokeParticles.Length > 0)
        {
            bool shouldEmit = currentSpeed > 10f && Mathf.Abs(steerInput) > 0.3f;
            foreach (var smoke in wheelSmokeParticles)
            {
                var emission = smoke.emission;
                emission.enabled = shouldEmit;
            }
        }

    }

    private void FixedUpdate()
    {
        Move();
        Steer();
        Break();
    }

    void GetInputs()
    {
        moveInput = Input.GetAxis("Vertical");
        steerInput = Input.GetAxis("Horizontal");

        // Check for boost input
        isBoosting = Input.GetKey(KeyCode.LeftShift);
    }

    void Move()
    {

        currentAcceleration = isBoosting ?
      normalMaxAcceleration * boostMultiplier :
      normalMaxAcceleration * speedMultiplier;

        // Calculate torque based on current speed (better acceleration curve)
        float speedFactor = Mathf.Clamp(1 - (carRb.linearVelocity.magnitude / 50f), 0.1f, 1f);
        float finalTorque = moveInput * currentAcceleration * speedFactor;

        foreach (var wheel in wheels)
        {
            wheel.wheelCollider.motorTorque = finalTorque;

            // Adjust wheel physics for better grip
            WheelFrictionCurve forwardFriction = wheel.wheelCollider.forwardFriction;
            forwardFriction.stiffness = isBoosting ? 3f : 1.5f;
            wheel.wheelCollider.forwardFriction = forwardFriction;

        }

        //Debug.Log("BOOST ACTIVATED! Current speed: " + carRb.linearVelocity.magnitude);
    }

    void Steer()
    {
        foreach (var wheel in wheels)
        {
            if (wheel.axel == Axel.Front)
            {
                var _steerAngle = steerInput * turnSensitivity * maxSteerAngle;
                wheel.wheelCollider.steerAngle = Mathf.Lerp(wheel.wheelCollider.steerAngle, _steerAngle, 0.6f);
            }
        }
    }

    void Break()
    {
       
        if (Input.GetKey(KeyCode.Space))
        {
            foreach (var wheel in wheels)
            {
                wheel.wheelCollider.brakeTorque = breakAcceleration * 10000;
            }

            if(carRb.linearVelocity.magnitude < 0.1f)
            {
                carRb.linearVelocity = Vector3.zero;
                carRb.angularVelocity = Vector3.zero;

               
                foreach (var wheel in wheels)
                {
                    wheel.wheelCollider.brakeTorque = Mathf.Infinity;
                    wheel.wheelCollider.motorTorque = 0f;
                }

            }
        }
        else
        {
            foreach (var wheel in wheels)
            {
                wheel.wheelCollider.brakeTorque = 0;
            }
        }
    }

    void UpdateSpeed()
    {
        currentSpeed = carRb.linearVelocity.magnitude < 0.1f ? 0f : carRb.linearVelocity.magnitude; 
        CurrentCarSpeed = currentSpeed;
        
    }


    void AnimateWheels()
    {
        foreach (var wheel in wheels)
        {
            Quaternion rot;
            Vector3 pos;
            wheel.wheelCollider.GetWorldPose(out pos, out rot);
            wheel.wheelModel.transform.position = pos;
            wheel.wheelModel.transform.rotation = rot;
        }
    }
}