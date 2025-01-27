using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Cinemachine;

public class Character : MonoBehaviour
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

    [Header("Projectile")]
    [SerializeField] GameObject projectile;
    [SerializeField] float projectileVerticalForce;
    [SerializeField] float projectileHorizontalForceMultiplier;

    [SerializeField] float slingWindUpSpeed;
    [SerializeField] float slingSlowDownSpeed;
    float slingSpeed;

    Vector3 projectileTarget;

    Plane plane;

    [Header("Wheels")]
    [SerializeField] CinemachineVirtualCamera virtualCamera;
    
    float fixedDeltaTime;

    [SerializeField] float decreaseFOVWhileSlowedSpeed;
    [SerializeField] float SlowmoMultiplier;
    float startingFOV;

    Vector3 lastMousePos;

    


    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        this.fixedDeltaTime = Time.fixedDeltaTime;
        startingFOV = virtualCamera.m_Lens.FieldOfView;

        plane = new Plane(transform.forward, transform.position);

    }

    private void Update()
    {
        //SlingshotAttack();

        TiltClamp(maxTiltAngle);

        if (Input.GetMouseButtonDown(0))
        {

        }
        if (Input.GetMouseButton(0))
        {
            Vector3 tempPointPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            //if (Vector2.Distance(tempPointPosition, linePointPositions[linePointPositions.Count -1]) > .1f)
            //{
                //UpdateLineFunction(tempPointPosition);
            //}
        }
        if (Input.GetMouseButtonUp(0))
        {

            SlingshotAttack();

        }

        if (Input.GetMouseButton(1))
        {
            WindUpSling();
        }
        if (Input.GetMouseButtonUp(1)) print(slingSpeed); // remove later

        if (slingSpeed - slingSlowDownSpeed > 0) slingSpeed -= slingSlowDownSpeed;
        else slingSpeed = 0;


        SlowTimeOnMouseDown();

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

    void WindUpSling()
    {
        if (lastMousePos == null) return; 

        Vector3 differenceInPosSinceLastFrame = lastMousePos - Input.mousePosition;

        float increaseSlingBy = differenceInPosSinceLastFrame.magnitude * slingWindUpSpeed;

        slingSpeed += increaseSlingBy * Time.deltaTime;

        lastMousePos = Input.mousePosition;
    }

    void SlingshotAttack()
    {

        //get the mouse position in world
        projectileTarget = GetMousePositionInWord();

        // instantiate "projectile"
        GameObject _projectile = Instantiate(projectile, transform.position, Quaternion.identity);

        // chariot current velocity
        Vector3 currentChariotVelocity = rb.velocity;

        // launch projectile
        _projectile.GetComponent<Rigidbody>().AddForce(/*transform.forward*/(projectileTarget - transform.position).normalized * slingSpeed * projectileHorizontalForceMultiplier + new Vector3(0, projectileVerticalForce, 0) + currentChariotVelocity, ForceMode.Impulse);

        // destroy after time
        Destroy(_projectile, 5);

        slingSpeed = 0;

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

    Vector3 GetMousePositionInWord()
    {
        Vector2 screenPos = Input.mousePosition; // remove this later
        Ray ray = Camera.main.ScreenPointToRay(screenPos);

        // plane is only used if the player doesn't aim at a collider
        Plane plane = new Plane(Vector3.down, 2);

        Vector3 _projectileTarget = transform.forward;

        if (Physics.Raycast(ray, out RaycastHit hitData))
        {
            _projectileTarget = hitData.point;
        }
        else if (plane.Raycast(ray, out float distance))
        {
            _projectileTarget = ray.GetPoint(distance);
        }

        return _projectileTarget;
    }

    Vector3 GetAverageMousePointInLine(List<Vector2> _mousePointsInCurrentLine)
    {
        float x = 0;
        float y = 0;

        foreach (Vector2 vec in _mousePointsInCurrentLine)
        {
            x += vec.x;
            y += vec.y;
        }

        Vector2 averagePoint = new Vector2(x / _mousePointsInCurrentLine.Count, y / _mousePointsInCurrentLine.Count);
        return averagePoint;

    } // wip

    void SlowTimeOnMouseDown()
    {
        if (Input.GetMouseButton(0))
        {
            Time.timeScale = SlowmoMultiplier;
            virtualCamera.m_Lens.FieldOfView -= Time.deltaTime * decreaseFOVWhileSlowedSpeed;
        }
        else
        {
            Time.timeScale = 1f;
            virtualCamera.m_Lens.FieldOfView = startingFOV;
        }

        Time.fixedDeltaTime = this.fixedDeltaTime * Time.timeScale;
    }
}
