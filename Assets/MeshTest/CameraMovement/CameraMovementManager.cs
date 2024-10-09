using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovementManager : MonoBehaviour
{
    public float verticalSpeed = 100;
    public float horizontalSpeed = 100;
    public float rotationSpeed = 100;
    
    PlayerInput inputs = PlayerInput.instance;
    private void Update()
    {
        Vector3 direction = Vector3.zero;
        Vector2 WASD = inputs.GetMoveInputs();
        
        Vector3 rotation = Vector3.zero;
        
        if (!WASD.Equals(Vector2.zero))
        {
            direction +=  WASD.x * horizontalSpeed * Time.deltaTime * transform.forward;
            direction += -WASD.y * horizontalSpeed * Time.deltaTime * transform.right;
        }

        if (inputs.GetUp())
            direction += (verticalSpeed * Time.deltaTime * Vector3.up);
        
        if (inputs.GetDown())
            direction -= (verticalSpeed * Time.deltaTime * Vector3.up);

        if (inputs.GetTurnLeft())
            rotation.y += rotationSpeed * Time.deltaTime;
        
        if (inputs.GetTurnRight())
            rotation.y -= rotationSpeed * Time.deltaTime;
        
        transform.position += direction;
        transform.eulerAngles += rotation;
    }
}
