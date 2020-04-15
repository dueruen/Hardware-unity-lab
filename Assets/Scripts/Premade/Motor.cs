using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ArduinoFunctions;


public class Motor : MonoBehaviour
{
    public Rigidbody wheel;

    [SerializeField]
    private float maxMotorTorque = 14;

    [SerializeField]
    private float motorTorque;

    [SerializeField]
    private int motorSign = 1; //-1 or 1;

    void Start()
    {
        maxMotorTorque = 25; 
        wheel.maxAngularVelocity = maxMotorTorque;
    }

    void FixedUpdate()
    {
        Vector3 localAngularVelocity = transform.InverseTransformDirection(wheel.angularVelocity);

        //Updates direction and speed
        //wheel.AddRelativeTorque(Vector3.right * motorTorque * motorSign, ForceMode.Force);
        wheel.angularVelocity = transform.TransformDirection(Vector3.right) * motorTorque * motorSign;
        //Debug.Log(localAngularVelocity);
    }

    #region ControlMethods
    //SetDirection is what the H-bridge will interface with the motor with.
    public void SetDirection(bool isHigh)
    {
        //Default is driving forward, if "HIGH" is sent through this function, motor will reverse
        motorSign = isHigh ? -1 : 1;
    }

    public void SetSpeed(int analogValue)
    {
        Debug.Assert(analogValue <= 255 && analogValue >= 0);

        //Mapping of analog value to force in motor/wheel:

        motorTorque = Functions.map(analogValue, 0, 255, 0, maxMotorTorque);
    }
    #endregion ControlMethods
}