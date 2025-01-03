using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{

    [SerializeField] WheelCollider frontRight;
    [SerializeField] WheelCollider frontLeft;
    [SerializeField] WheelCollider backRight;
    [SerializeField] WheelCollider backLeft;

    public float acceleration;
    public float breakingForce;

    float currentAccel;
    float currentBreakForce;

    public float maxTurnAngle;
    float currentTurnAngle;

    void FixedUpdate()
    {
        // input for forwards
        if (Input.GetKey(KeyCode.W))
        {
            currentAccel = acceleration;
        }
        else
        {
            currentAccel = 0;
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

        frontRight.motorTorque = currentAccel;
        frontLeft.motorTorque = currentAccel;

        frontRight.brakeTorque = currentBreakForce;
        frontLeft.brakeTorque = currentBreakForce;
        backRight.brakeTorque = currentBreakForce;
        backLeft.brakeTorque = currentBreakForce;

        float ForwardDirection = Input.GetAxisRaw("Vertical");

        currentTurnAngle = maxTurnAngle * Input.GetAxis("Horizontal");

        frontRight.steerAngle = currentTurnAngle;
        frontLeft.steerAngle = currentTurnAngle;

    }

}
