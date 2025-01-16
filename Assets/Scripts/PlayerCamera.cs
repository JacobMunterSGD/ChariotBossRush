using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Cinemachine;

public class PlayerCamera : MonoBehaviour
{
    [SerializeField] CinemachineVirtualCamera virtualCamera;
    
    float fixedDeltaTime;

    [SerializeField] float decreaseFOVWhileSlowedSpeed;
    [SerializeField] float SlowmoMultiplier;
    float startingFOV;

    private void Start()
    {
        this.fixedDeltaTime = Time.fixedDeltaTime;
        startingFOV = virtualCamera.m_Lens.FieldOfView;


    }

    private void Update()
    {
        SlowTimeOnMouseDown();
    }

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
