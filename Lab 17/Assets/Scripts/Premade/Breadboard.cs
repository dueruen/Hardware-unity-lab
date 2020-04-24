using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Breadboard : MonoBehaviour
{
    public ArduinoObject[] arduinoObjectPins;
    


    public int analogRead(int pin)
    {
        if (!isPinAssigned(pin))
        {
            Debug.LogError("Attempting to write or read to unnassigned pin (Null or out of bounds of array) of index: " + pin + "\n");
            throw new ArgumentOutOfRangeException();
        }

        return arduinoObjectPins[pin].analogRead();
    }
    public void analogWrite(int pin, int value)
    {
        if (!isPinAssigned(pin))
        {
            Debug.LogError("Attempting to write or read to unnassigned pin (Null or out of bounds of array) of index: " + pin + "\n");
            throw new ArgumentOutOfRangeException();
        }
        arduinoObjectPins[pin].analogWrite(value);
    }
    public bool digitalRead(int pin)
    {
        if (!isPinAssigned(pin))
        {
            Debug.LogError("Attempting to write or read to unnassigned pin (Null or out of bounds of array) of index: " + pin + "\n");
            throw new ArgumentOutOfRangeException();
        }

        return arduinoObjectPins[pin].digitalRead();
    }
    public void digitalWrite(int pin, bool isHigh)
    {
        arduinoObjectPins[pin].digitalWrite(isHigh);
    }

    public ulong pulseIn(int pin)
    {
        if (!isPinAssigned(pin))
        {
            Debug.LogError("Attempting to write or read to unnassigned pin (Null or out of bounds of array) of index: " + pin + "\n");
            throw new ArgumentOutOfRangeException();
        }

        return arduinoObjectPins[pin].pulseIn();
    }


    private bool isPinAssigned(int pin)
    {

        if (pin < arduinoObjectPins.Length)
        {
            return arduinoObjectPins[pin] != null;
        }

        return false;
    }
}
