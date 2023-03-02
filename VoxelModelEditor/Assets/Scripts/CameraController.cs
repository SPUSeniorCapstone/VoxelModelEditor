using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float speed;
    public float rotateSpeed;
    public bool lockMouse = true;

    private void Start()
    {
        if (lockMouse)
        {
            Cursor.lockState = CursorLockMode.Locked;
        }
    }


    private void Update()
    {
        Vector3 move = Vector3.zero;
        move += Input.GetAxis("Vertical") * transform.forward;
        move += Input.GetAxis("Horizontal") * transform.right;

        transform.position += move * speed * Time.deltaTime;

        if(lockMouse || Input.GetMouseButton(2))
        {
            if (Input.GetMouseButtonDown(2))
            {
                Cursor.lockState = CursorLockMode.Locked;
            }


            var X = Input.GetAxis("Mouse X") * rotateSpeed;
            var Y = Input.GetAxis("Mouse Y") * rotateSpeed;

            transform.Rotate(Vector3.up, X, Space.World);
            transform.Rotate(transform.right, -Y, Space.World);



            //transform.eulerAngles += rotateSpeed * new Vector3(-Input.GetAxis("Mouse Y"), Input.GetAxis("Mouse X"), 0) * Time.deltaTime;
        }
        if (Input.GetMouseButtonUp(2))
        {
            Cursor.lockState = CursorLockMode.None;
        }
    }
}
