using System;
using System.Collections.Generic;
using UnityEngine;

public class CarController : MonoBehaviour
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
    public Vector3 _centerOfMass;

   

    public List<Wheel> wheels;

    private float moveInput;
    private float steerInput;
    private Rigidbody carRb;
    private float currentAcceleration;
    private bool isBoosting;

    private void Start()
    {
        carRb = GetComponent<Rigidbody>();
        carRb.centerOfMass = _centerOfMass;
        currentAcceleration = normalMaxAcceleration;

    }

    private void Update()
    {
        GetInputs();
        AnimateWheels();
        
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
        
        currentAcceleration = isBoosting ? normalMaxAcceleration * boostMultiplier : normalMaxAcceleration;

        foreach (var wheel in wheels)
        {
            
            wheel.wheelCollider.motorTorque = moveInput * 1000f * currentAcceleration * Time.fixedDeltaTime;
        }


        if (isBoosting)
        {
            Debug.Log("BOOST ACTIVATED! Current speed: " + carRb.linearVelocity.magnitude);
        }
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
        }
        else
        {
            foreach (var wheel in wheels)
            {
                wheel.wheelCollider.brakeTorque = 0;
            }
        }
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