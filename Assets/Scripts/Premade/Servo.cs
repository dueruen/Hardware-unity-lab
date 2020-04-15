using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Servo : MonoBehaviour
{
    [SerializeField]
    private float currentAngle;
    [SerializeField]
    private int targetAngle;

    private float rotationSpeed;

    private void Start()
    {
        currentAngle = transform.rotation.eulerAngles.y;
        rotationSpeed = 70;
    }

    void Update()
    {
        //Rotate from current-angle towards target angle at deg/sec rate equal to rotationSpeed.
        if (currentAngle != targetAngle /*should be within certain angle*/)
        {
            Debug.Log("rotating");
            transform.localRotation = Quaternion.RotateTowards(transform.localRotation, Quaternion.Euler(new Vector3(0,targetAngle,0)), rotationSpeed*Time.deltaTime);
        }
    }

    public void write(int angle)
    {
        targetAngle = Mathf.Clamp(angle, 1, 179);
    }

    public int read()
    {
        return targetAngle;
    }

}