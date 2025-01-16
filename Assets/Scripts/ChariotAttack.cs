using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Cinemachine;

public class ChariotAttack : MonoBehaviour
{

    Rigidbody rb;

    [Header("Projectile")]
    [SerializeField] GameObject projectile;
    [SerializeField] float projectileVerticalForce;
    [SerializeField] float projectileHorizontalForceMultiplier;

    [SerializeField] float slingWindUpSpeed;
    [SerializeField] float slingSlowDownSpeed;
    float slingSpeed;

    Vector3 projectileTarget;

    Vector3 lastMousePos;

    


    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {


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
}
