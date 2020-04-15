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
        analogRead();
    }

    override public int analogRead()
    {
        Vector3 direction = transform.right * -1;
        RaycastHit raycastHit;
        Debug.DrawRay(transform.position, direction * raycastRange, Color.blue);


        if (Physics.Raycast(transform.position, direction * raycastRange, out raycastHit, raycastMask))
        {
            distance = raycastHit.distance;
        }



        return (int)distance;
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
