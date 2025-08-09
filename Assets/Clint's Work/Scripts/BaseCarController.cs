using System;
using UnityEditor;
using UnityEngine;




[RequireComponent(typeof(Rigidbody)),RequireComponent(typeof(WheelColliders)),RequireComponent(typeof(WheelMeshes))]
public class BaseCarController : MonoBehaviour
{
    private Rigidbody rb;

    public WheelColliders colliders;
    public WheelMeshes wheelMeshes;
    public WheelParticles wheelParticles;

    public float gasInput;
    public float brakeInput;
    public float steeringInput;
    public GameObject smokePrefab;

    public float motorPower;
    public float brakePower;
    private float slipAngle;
    public float speed;
    public AnimationCurve steeringCurve;
    public float maxSteering = 70.0f;
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        InstantiateSmoke();
    }
    private void InstantiateSmoke()
    {
        if (smokePrefab)
        {
            wheelParticles.FRWheel = Instantiate(smokePrefab, colliders.FRWheel.transform.position - 
                (Vector3.up * colliders.FRWheel.radius), Quaternion.identity,colliders.FRWheel.transform)
                .GetComponent<ParticleSystem>();
            wheelParticles.FLWheel = Instantiate(smokePrefab, colliders.FLWheel.transform.position - 
                (Vector3.up * colliders.FLWheel.radius), Quaternion.identity,colliders.FLWheel.transform)
                .GetComponent<ParticleSystem>();
            wheelParticles.RRWheel = Instantiate(smokePrefab, colliders.RRWheel.transform.position - 
                (Vector3.up * colliders.RRWheel.radius), Quaternion.identity,colliders.RRWheel.transform)
                .GetComponent<ParticleSystem>();
            wheelParticles.RLWheel = Instantiate(smokePrefab, colliders.RLWheel.transform.position - 
                (Vector3.up * colliders.RLWheel.radius), Quaternion.identity,colliders.RLWheel.transform)
                .GetComponent<ParticleSystem>();
        }
        
    }
    private void Update()
    {
        speed = rb.linearVelocity.magnitude;
        ApplyMotor();
        ApplyBrake();
        CheckParticles();
        ApplyWheelPositions();
    }
    public void SetInput(float throttleIn,float steeringIn,float clutchIn, float handbrakeIn)
    {
        gasInput = throttleIn;
        ApplySteering(steeringIn);

        

    }
    void ApplyBrake()
    {
        colliders.FRWheel.brakeTorque = brakeInput * brakePower * 0.7f;
        colliders.FLWheel.brakeTorque = brakeInput * brakePower * 0.7f;
        colliders.RRWheel.brakeTorque = brakeInput * brakePower * 0.3f;
        colliders.RLWheel.brakeTorque = brakeInput * brakePower * 0.3f;
    }

    void ApplyMotor()
    {
        colliders.RRWheel.motorTorque = motorPower * -gasInput;
        colliders.RLWheel.motorTorque = motorPower * -gasInput;
    }

    void ApplySteering(float steeringAngle)
    {
        
        Debug.Log("Before Clamp" + steeringAngle);
        steeringAngle = Mathf.Clamp(steeringAngle, -maxSteering, maxSteering);
        Debug.Log("After Clamp " + steeringAngle);
        colliders.FRWheel.steerAngle = steeringAngle;
        colliders.FLWheel.steerAngle = steeringAngle;
    }

    private void ApplyWheelPositions()
    {
        UpdateWheel(colliders.FLWheel,wheelMeshes.FLWheel);
        UpdateWheel(colliders.FRWheel,wheelMeshes.FRWheel);
        UpdateWheel(colliders.RLWheel,wheelMeshes.RLWheel);
        UpdateWheel(colliders.RRWheel,wheelMeshes.RRWheel);
        
    }
    private void CheckParticles()
    {
        if (smokePrefab)
        {
            WheelHit[] wheelHits = new WheelHit[4];
            colliders.FRWheel.GetGroundHit(out wheelHits[0]);
            colliders.FLWheel.GetGroundHit(out wheelHits[1]);
            colliders.RRWheel.GetGroundHit(out wheelHits[2]);
            colliders.RLWheel.GetGroundHit(out wheelHits[3]);

            #region Particle start/stop
            float slipAllowance = 0.35f;
            if (Mathf.Abs(wheelHits[0].sidewaysSlip) + Mathf.Abs(wheelHits[0].forwardSlip) > slipAllowance)
            {
                wheelParticles.FRWheel.Play();
            }
            else
            {
                wheelParticles.FRWheel.Stop();
            }
            if (Mathf.Abs(wheelHits[1].sidewaysSlip) + Mathf.Abs(wheelHits[1].forwardSlip) > slipAllowance)
            {
                wheelParticles.FLWheel.Play();
            }
            else
            {
                wheelParticles.FLWheel.Stop();
            }
            if (Mathf.Abs(wheelHits[2].sidewaysSlip) + Mathf.Abs(wheelHits[2].forwardSlip) > slipAllowance)
            {
                wheelParticles.RRWheel.Play();
            }
            else
            {
                wheelParticles.RRWheel.Stop();
            }
            if (Mathf.Abs(wheelHits[3].sidewaysSlip) + Mathf.Abs(wheelHits[3].forwardSlip) > slipAllowance)
            {
                wheelParticles.RLWheel.Play();
            }
            else
            {
                wheelParticles.RLWheel.Stop();
            }
            #endregion
        }
    }
    private void UpdateWheel(WheelCollider collider, MeshRenderer wheelMesh)
    {
        Quaternion quat;
        Vector3 pos;
        collider.GetWorldPose(out pos, out quat);
        wheelMesh.transform.position = pos;
        wheelMesh.transform.rotation = quat;
    }
}
