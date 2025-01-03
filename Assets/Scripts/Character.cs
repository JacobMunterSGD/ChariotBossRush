using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{

    CharacterController characterController;

    public float speed;

    void Start()
    {
        characterController = GetComponent<CharacterController>();   
    }

    void Update()
    {
        Vector3 move = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));

        characterController.Move(move * Time.deltaTime * speed);

    }
}
