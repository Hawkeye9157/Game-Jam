using NUnit.Framework;
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

    public float maxAcceleration = 30.0f;
    public float breakAcceleration = 50.0f;
    public float turnSensitivity = 1.0f;
    public float maxSteerAngle = 30.0f;

    public Vector3 _centerOfMass;



    public List<Wheel> wheels;

    float moveInput;
    float steerInput;

    private Rigidbody carRb;

    private void Start()
    {
        carRb = GetComponent<Rigidbody>();
        carRb.centerOfMass = _centerOfMass;
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
    }


    void Move()
    {
        foreach (var wheel in wheels)
        {
            wheel.wheelCollider.motorTorque = moveInput * 150 * maxAcceleration;
        }
    }

    void Steer()
    {
        foreach (var wheel in wheels)
        {
            if(wheel.axel == Axel.Front)
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
                wheel.wheelCollider.brakeTorque = breakAcceleration * 300;
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
