using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ArduinoObject : MonoBehaviour
{
    abstract public ulong pulseIn();
    abstract public int analogRead();
    abstract public void analogWrite(int value); //from 0 to 1023
    abstract public bool digitalRead(); //true or false / high or low
    abstract public void digitalWrite(bool isHigh);
}
