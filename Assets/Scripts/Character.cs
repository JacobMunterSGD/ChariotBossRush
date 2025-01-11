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

    float wheelAccel;
    float currentBreakForce;

    [SerializeField] float maxTurnAngle;
    float currentTurnAngle;

    [SerializeField] float maxTiltAngle;

    [Header("Projectile")]
    [SerializeField] GameObject projectile;
    [SerializeField] float projectileVerticalForce;
    [SerializeField] float projectileHorizontalForce;

    Vector3 projectileTarget;

    [Header("Line")]
    [SerializeField] GameObject linePrefab;
    GameObject currentLine;

    LineRenderer lineRenderer;
    [SerializeField] List<Vector3> linePointPositions;
    List<Vector2> mousePointsInCurrentLine = new List<Vector2>();

    Plane plane;

    [Header("Wheels")]
    [SerializeField] CinemachineVirtualCamera virtualCamera;
    
    float fixedDeltaTime;

    [SerializeField] float decreaseFOVWhileSlowedSpeed;
    [SerializeField] float SlowmoMultiplier;
    float startingFOV;

    //test remove later
    public GameObject testBall;


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
            CreateLine();
        }
        if (Input.GetMouseButton(0))
        {
            Vector3 tempPointPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            //if (Vector2.Distance(tempPointPosition, linePointPositions[linePointPositions.Count -1]) > .1f)
            //{
                //UpdateLineFunction(tempPointPosition);
            //}
            PlaneLineTest();
        }
        if (Input.GetMouseButtonUp(0))
        {
            Vector2 currentTargetScreenPoint = GetAverageMousePointInLine(mousePointsInCurrentLine);
            projectileTarget = GetMousePositionInWord(currentTargetScreenPoint);

            SlingshotAttack();

        }

        

        SlowTimeOnMouseDown();

    }
    void FixedUpdate()
    {
        ChariotAccelerateAndBraking();
        ChariotTurning();

    }

    void PlaneLineTest()
    {
        plane = new Plane(transform.forward, transform.position);

        Vector3 screenPosition = Input.mousePosition;
        Ray ray = Camera.main.ScreenPointToRay(screenPosition);

        if (plane.Raycast(ray, out float distance))
        {
            testBall.transform.position = ray.GetPoint(distance);

            linePointPositions.Add(ray.GetPoint(distance));
            lineRenderer.positionCount++;
            lineRenderer.SetPosition(lineRenderer.positionCount - 1, ray.GetPoint(distance));

        }

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

        //get the mouse position in world
        //projectileTarget = GetMousePositionInWord();

        // instantiate "projectile"
        GameObject _projectile = Instantiate(projectile, transform.position, Quaternion.identity);

        // chariot current velocity
        Vector3 currentChariotVelocity = rb.velocity;

        // launch projectile
        _projectile.GetComponent<Rigidbody>().AddForce(/*transform.forward*/(projectileTarget - transform.position).normalized * projectileHorizontalForce + new Vector3(0, projectileVerticalForce, 0) + currentChariotVelocity, ForceMode.Impulse);

        // destroy after time
        Destroy(_projectile, 5);

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

    Vector3 GetMousePositionInWord(Vector2 screenPos)
    {
        //screenPos = Input.mousePosition; // remove this later
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
        mousePointsInCurrentLine.Clear();

        mousePointsInCurrentLine.Add(Input.mousePosition);
        mousePointsInCurrentLine.Add(Input.mousePosition);

        //linePointPositions.Add(Camera.main.ScreenToWorldPoint(Input.mousePosition));
        //linePointPositions.Add(Camera.main.ScreenToWorldPoint(Input.mousePosition));

        //lineRenderer.SetPosition(0, linePointPositions[0]);
        //lineRenderer.SetPosition(0, linePointPositions[1]);



    } // wip

    void UpdateLineFunction(Vector3 newPointPosition)
    {
        linePointPositions.Add(newPointPosition);
        lineRenderer.positionCount++;
        lineRenderer.SetPosition(lineRenderer.positionCount - 1, newPointPosition);
    } // wip

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
