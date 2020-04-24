using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class DistanceSensor : ArduinoObject
{
    private float distance;

    public LayerMask raycastMask;
    private float raycastRange;

    private void Start()
    {
        raycastRange = 200f;
    }

    private void Update()
    {

    }



    override public ulong pulseIn()
    {
        Vector3 direction = transform.right * -1;
        RaycastHit raycastHit;
        Debug.DrawRay(transform.position, direction * raycastRange, Color.blue);

        if (Physics.Raycast(transform.position, direction * raycastRange, out raycastHit, raycastMask))
        {
            distance = raycastHit.distance;
        }
        else //Raycast hits nothing:
        {
            return 0;
        }
        
        float time = (distance/0.034f) * 2;

        
        return (ulong)time;
    }

    override public int analogRead()
    {
        throw new NotImplementedException();
    }
    override public void analogWrite(int value)
    {
        throw new NotImplementedException();
    }
    override public bool digitalRead()
    {
        throw new NotImplementedException();
    }
    override public void digitalWrite(bool isHigh)
    {
        throw new NotImplementedException();
    }
}
