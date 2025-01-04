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

    [SerializeField] GameObject projectile;
    [SerializeField] float projectileVerticalForce;
    [SerializeField] float projectileHorizontalForce;


    private void Update()
    {
        SlingshotAttack();

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

        // accelerate
        frontRight.motorTorque = currentAccel;
        frontLeft.motorTorque = currentAccel;

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

    void SlingshotAttack()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            // instantiate "projectile"
            GameObject _projectile = Instantiate(projectile, transform.position, Quaternion.identity);

            // chariot current velocity
            Vector3 currentChariotVelocity = gameObject.GetComponent<Rigidbody>().velocity;

            // launch projectile
            _projectile.GetComponent<Rigidbody>().AddForce(transform.forward * projectileHorizontalForce + new Vector3(0, projectileVerticalForce, 0) + currentChariotVelocity, ForceMode.Impulse);

            // destroy after time
            Destroy(_projectile, 5);
        }
    }

}
