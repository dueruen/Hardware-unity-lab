using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;

public class ArduinoMain : MonoBehaviour
{
    public Breadboard breadboard;
    //On included/premade Arduino functions:
    //delay(timeInMilliseconds) : use "yield return delay(timeInMilliseconds)", to get similar functionality as delay() in arduino would give you.

    //map() : works exactly as on Arduino, maps a long from one range to another. 
    //If you want to get an int or a float from the map()-function, you can cast the output like this: (int)map(s, a1, a2, b1, b2) or (float)map(s, a1, a2, b1, b2) 

    //millis() : returns the time as a ulong since the start of the scene (and therefore also the time since setup() was run) in milliseconds.

    //If you want to do something similar to serial.println(), use Debug.Log(). 

    //analogWrite() and analogRead() works as they do in arduino - remember to give them correct input-values.
    //digitalRead() and digitalWrite() writes and returns bools. (High = true). 
    //LineSensors have both write-functions implemented, motors/hbridge have both read-functions implemented.

    //The console will display a "NotImplementedException" if you attempt to write to sensors or read from motors. 

    IEnumerator setup()
    {
        //Your code goes here:

        //Example of delay:
        Debug.Log("pre-delay log");
        yield return delay(2000); //2 second delay
        Debug.Log("After delay");

        //Your code ends here -----

        //following region ensures delay-functionality for setup() & loop(). Do not delete, must always be last thing in setup.
        #region PremadeSetup
        yield return StartCoroutine(loop()); ;
        #endregion PremadeSetup
    }

    IEnumerator loop()
    {
        //Your code goes here:
        digitalWrite(1, false);
        digitalWrite(3, false);
        digitalWrite(0, false);
        digitalWrite(2, false);

        //Example analogRead:
        int ldrLeft = analogRead(4);
        int ldrRight = analogRead(5);
       // Debug.Log(value + " value at pin 5");

        if (ldrLeft > 600 && ldrRight > 600)
        {
            analogWrite(1, 70);
            analogWrite(3, 70);
        }
        else if (ldrLeft < 600)
        {
            analogWrite(1, 40);
            analogWrite(2, 40);
        }
        else if (ldrRight < 600)
        {
            analogWrite(0, 40);
            analogWrite(3, 40);
        }
        else
        {
            digitalWrite(1, false);
            digitalWrite(3, false);
            digitalWrite(0, false);
            digitalWrite(2, false);
        }

        //Your code ends here -----

        //Following region is implemented as to allow "yield return delay()" to function the same way as one would expect it to on Arduino.
        //It should always be at the end of the loop()-function, and shouldn't be edited.
        #region DoNotDelete
        //Wait for one frame
        yield return new WaitForSeconds(0);
        //New loop():
        yield return loop();
        #endregion DoNotDelete 
    }



    #region PremadeDefinitions
    void Start()
    {
        Time.fixedDeltaTime = 0.005f; //4x physics steps of what unity normally does - to improve sensor-performance.
        StartCoroutine(setup());

    }

    IEnumerator delay(int _durationInMilliseconds)
    {
        float durationInSeconds = ((float)_durationInMilliseconds * 0.001f);
        yield return new WaitForSeconds(durationInSeconds);
    }

    public long map(long s, long a1, long a2, long b1, long b2)
    {
        return b1 + (s - a1) * (b2 - b1) / (a2 - a1);
    }

    public ulong millis()
    {
        return (ulong)(Time.timeSinceLevelLoad * 1000f);
    }

    #endregion PremadeDefinitions

    #region InterfacingWithBreadboard
    public int analogRead(int pin)
    {
        return breadboard.analogRead(pin);
    }
    public void analogWrite(int pin, int value)
    {
        breadboard.analogWrite(pin, value);
    }
    public bool digitalRead(int pin)
    {
        return breadboard.digitalRead(pin);
    }
    public void digitalWrite(int pin, bool isHigh)
    {
        breadboard.digitalWrite(pin, isHigh);
    }
    #endregion InterfacingWithBreadboard
}