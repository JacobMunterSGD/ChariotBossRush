using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{

    CharacterController characterController;

    public float speed;

    public float maxTurnAngle;

    float currentTurnAngle;

    void Start()
    {
        characterController = GetComponent<CharacterController>();   
    }

    void FixedUpdate()
    {
        //Vector3 PlayerInput = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));

        float ForwardDirection = Input.GetAxisRaw("Vertical");

        currentTurnAngle = maxTurnAngle * Input.GetAxis("Horizontal");

        //characterController.Move(move * Time.deltaTime * speed);

    }

}
