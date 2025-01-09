using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Character : MonoBehaviour
{

    Rigidbody rb;

    [SerializeField] WheelCollider frontRight;
    [SerializeField] WheelCollider frontLeft;
    [SerializeField] WheelCollider backRight;
    [SerializeField] WheelCollider backLeft;

    [SerializeField] float acceleration;
    [SerializeField] float breakingForce;

    float wheelAccel;
    float currentBreakForce;

    [SerializeField] float maxTurnAngle;
    float currentTurnAngle;

    [SerializeField] float maxTiltAngle;

    [SerializeField] GameObject projectile;
    [SerializeField] float projectileVerticalForce;
    [SerializeField] float projectileHorizontalForce;

    Vector3 projectileTarget;

    public GameObject linePrefab;
    GameObject currentLine;

    LineRenderer lineRenderer;
    public List<Vector2> linePointPositions;


    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        SlingshotAttack();

        RotateInFrame(maxTiltAngle);

        if (Input.GetMouseButtonDown(0))
        {
            CreateLine();
        }
        if (Input.GetMouseButton(0))
        {
            Vector2 tempPointPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            if (Vector2.Distance(tempPointPosition, linePointPositions[linePointPositions.Count -1]) > .1f)
            {
                UpdateLineFunction(tempPointPosition);
            }
            
        }

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

    void SlingshotAttack()
    {
        // right now only happens when space is pressed - later remove so it's called when the 
        if (Input.GetKeyDown(KeyCode.Space))
        {
            //get the mouse position in world
            projectileTarget = GetMousePositionInWord();

            // instantiate "projectile"
            GameObject _projectile = Instantiate(projectile, transform.position, Quaternion.identity);

            // chariot current velocity
            Vector3 currentChariotVelocity = rb.velocity;

            // launch projectile
            _projectile.GetComponent<Rigidbody>().AddForce(/*transform.forward*/(projectileTarget - transform.position).normalized * projectileHorizontalForce + new Vector3(0, projectileVerticalForce, 0) + currentChariotVelocity, ForceMode.Impulse);

            // destroy after time
            Destroy(_projectile, 5);
        }
    }

    float ClampAngle(float angle, float from, float to)
    {
        // accepts e.g. -80, 80
        if (angle < 0f) angle = 360 + angle;
        if (angle > 180f) return Mathf.Max(angle, 360 + from);
        return Mathf.Min(angle, to);
    }

    void RotateInFrame(float _maxTiltAngle)
    {
        Vector3 currentRotation = transform.rotation.eulerAngles;
        currentRotation.z = ClampAngle(currentRotation.z, -_maxTiltAngle, _maxTiltAngle);

        transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, currentRotation.z);

    }

    Vector3 GetMousePositionInWord()
    {
        Vector3 screenPos = Input.mousePosition;
        Ray ray = Camera.main.ScreenPointToRay(screenPos);

        // plane is only used if the player doesn't aim at a collider
        Plane plane = new Plane(Vector3.down, 2);

        Vector3 _projectileTarget = Vector3.zero;

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

    void CreateLine()
    {
        Destroy(currentLine);
        currentLine = Instantiate(linePrefab, transform);
        lineRenderer = currentLine.GetComponent<LineRenderer>();
        
        linePointPositions.Clear();
        linePointPositions.Add(Camera.main.ScreenToWorldPoint(Input.mousePosition));
        linePointPositions.Add(Camera.main.ScreenToWorldPoint(Input.mousePosition));
        lineRenderer.SetPosition(0, linePointPositions[0]);
        lineRenderer.SetPosition(0, linePointPositions[1]);
    }

    void UpdateLineFunction(Vector2 newPointPosition)
    {
        linePointPositions.Add(newPointPosition);
        lineRenderer.positionCount++;
        lineRenderer.SetPosition(lineRenderer.positionCount - 1, newPointPosition);
    }

}
