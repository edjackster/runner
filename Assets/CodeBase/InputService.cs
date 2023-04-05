using System;
using UnityEngine;

public class InputService : IService
{
    private Vector3 _inputVector = Vector3.zero;
    private Vector3 _mouseMovement = Vector3.zero;

    public Vector3 MousePosition => Input.mousePosition;

    public Vector3 GetInputVector()
    {
        _inputVector.x = Input.GetAxis("Horizontal");
        _inputVector.z = Input.GetAxis("Vertical");

        return _inputVector.normalized;
    }

    public bool GetShootButton() 
        => Input.GetMouseButton(0);

    public bool GetJumpButtonDown()
        => Input.GetKeyDown(KeyCode.Space);

    public bool GetJumpButtonUp()
        => Input.GetKeyUp(KeyCode.Space) || Input.GetKeyUp(KeyCode.UpArrow) || Input.GetKeyUp(KeyCode.W);

    public Vector3 GetMouseMovement()
    {
        _mouseMovement.y = Input.GetAxis("Mouse X");
        _mouseMovement.x = Input.GetAxis("Mouse Y");

        return _mouseMovement;
    }
}
