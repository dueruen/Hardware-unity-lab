using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class HBridge : MonoBehaviour
{
    [SerializeField]
    private Motor motor;
    private int speedForwards, speedBackwards; //Values from 0 to 255
    private int totalSpeed; //Values from 0 to 255, it's the absolute value of the difference between speedFowards and speedBackwards.
    private bool direction; //false = forwards, true = backwards.

    public void SetMotorSpeedAndDirection(HBridgePin.PinType pinType, int value)
    {
        //Use pin to figure out if the right direction is forwards or backwards:
        CalculateDirection(pinType, value);

        //Pass on analog value to motor:
        motor.SetSpeed(totalSpeed);
        motor.SetDirection(direction);
    }

    private void CalculateDirection(HBridgePin.PinType pinType, int value)
    {
        switch (pinType)
        {
            case HBridgePin.PinType.DriveBackward:
                speedBackwards = value;
                break;
            case HBridgePin.PinType.DriveForward:
                speedForwards = value;
                break;
        }

        int diff = speedForwards-speedBackwards;
        totalSpeed = Math.Abs(diff);
        //Drive forwards(false), when speedForwards>speedBackwards and vice-versa. In the case of 0, the totalSpeed will be 0 anyway.
        direction = Math.Sign(diff) == 1 ? false : true; 
    }

}
