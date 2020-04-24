using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Servo : MonoBehaviour
{

    [Range(1, 179)]
    [SerializeField]
    private int targetAngle;

    private float rotationSpeed;

    private void Start()
    {
        rotationSpeed = 70;
    }

    void Update()
    {
        transform.localRotation = Quaternion.RotateTowards(transform.localRotation, Quaternion.Euler(new Vector3(0, targetAngle, 0)), rotationSpeed * Time.deltaTime);
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