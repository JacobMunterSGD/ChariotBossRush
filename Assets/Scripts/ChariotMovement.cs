using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Cinemachine;

public class ChariotMovement : MonoBehaviour
{

    Rigidbody rb;

    [Header("Wheels")]
    [SerializeField] WheelCollider frontRight;
    [SerializeField] WheelCollider frontLeft;
    [SerializeField] WheelCollider backRight;
    [SerializeField] WheelCollider backLeft;

    [Header("Speed")]
    [SerializeField] float acceleration;
    [SerializeField] float breakingForce;
    [SerializeField] float backwardsAcceleration;

    float wheelAccel;
    float currentBreakForce;

    [SerializeField] float maxTurnAngle;
    float currentTurnAngle;

    [SerializeField] float maxTiltAngle;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {

        TiltClamp(maxTiltAngle);


    }
    void FixedUpdate()
    {
        ChariotAccelerateAndBraking();
        ChariotTurning();
    }

    void ChariotAccelerateAndBraking()
    {
        // input for forwards
        if (Input.GetKey(KeyCode.W))
        {
            wheelAccel = acceleration;
        }
        // reverse
        else if (Input.GetKey(KeyCode.LeftShift))
        {
            wheelAccel = -backwardsAcceleration;
        }
        else
        {
            wheelAccel = 0;
        }

        // braking
        if (Input.GetKey(KeyCode.S))
        {
            currentBreakForce = breakingForce;
        }
        else
        {
            currentBreakForce = 0;
        }

        // accelerate
        frontRight.motorTorque = wheelAccel;
        frontLeft.motorTorque = wheelAccel;
        backRight.motorTorque = wheelAccel;
        backLeft.motorTorque = wheelAccel;

        // break
        frontRight.brakeTorque = currentBreakForce;
        frontLeft.brakeTorque = currentBreakForce;
        backRight.brakeTorque = currentBreakForce;
        backLeft.brakeTorque = currentBreakForce;
    }

    void ChariotTurning()
    {
        // turning input
        currentTurnAngle = maxTurnAngle * Input.GetAxis("Horizontal");

        // Turn wheels based on input
        frontRight.steerAngle = currentTurnAngle;
        frontLeft.steerAngle = currentTurnAngle;
    }

    void TiltClamp(float _maxTiltAngle)
    {
        Vector3 currentRotation = transform.rotation.eulerAngles;
        currentRotation.z = ClampAngle(currentRotation.z, -_maxTiltAngle, _maxTiltAngle);

        transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, currentRotation.z);

        float ClampAngle(float angle, float from, float to)
        {
            // accepts e.g. -80, 80
            if (angle < 0f) angle = 360 + angle;
            if (angle > 180f) return Mathf.Max(angle, 360 + from);
            return Mathf.Min(angle, to);
        }
    }
}
