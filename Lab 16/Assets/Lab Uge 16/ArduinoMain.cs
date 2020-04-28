using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;

public class ArduinoMain : MonoBehaviour
{
    public Breadboard breadboard;
    public Servo servo;
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

    bool start = true;
    bool distanceOk = false;
    (bool, bool, bool) goTo;
    bool allWall = false;
    bool forceFront = false;
    bool fromLdr = false;
    bool firstDist = false;
    bool modeLeft = false;
    bool modeRight = false;
    bool modeFront = false;
    bool modeBack = false;
    bool modeDist = false;


    IEnumerator loop()
    {
        if (start)
        {
            servo.write(90);
            yield return delay(1500);
            start = false;
        }

        //Example analogRead:
        int ldrLeft = analogRead(4);
        int ldrRight = analogRead(5);
        ulong dist = pulseIn(6);

        Debug.Log("Ldr left: " + ldrLeft + " right: " + ldrRight + "  dist: " + dist);
        if (fromLdr && firstDist)
        {
            Debug.Log("Back");
            back();
            yield return delay(400);
            stop();
            if (ldrLeft > 1000 && ldrRight > 1000 && dist < 295)
            {
                fromLdr = false;
                firstDist = false;
                modeDist = true;
                Debug.Log("!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
                yield return delay(500);
            }
            firstDist = false;
        }
        else if (modeDist || dist != 0 && dist < 300)
        {
            if (!fromLdr)
            {
                Debug.Log("Dist ");
                bool wallLeft = true;
                bool wallRight = true;
                bool wallFront = true;
                if (!distanceOk)
                {
                    servo.write(0);
                    yield return delay(1500);

                    Debug.Log("left: " + pulseIn(6));
                    if (pulseIn(6) > 2000 || pulseIn(6) < 10)
                    {
                        Debug.Log("1");
                        wallLeft = false;
                    }
                    servo.write(90);
                    yield return delay(1500);
                    Debug.Log("front: " + pulseIn(6));
                    if (pulseIn(6) > 2000 || pulseIn(6) < 10)
                    {
                        Debug.Log("2");
                        wallFront = false;
                        if (pulseIn(6) == 0)
                        {
                            forceFront = true;
                        }
                    }
                    servo.write(180);
                    yield return delay(1500);
                    Debug.Log("right: " + pulseIn(6));
                    if (pulseIn(6) > 2000 || pulseIn(6) < 10)
                    {
                        Debug.Log("3");
                        wallRight = false;
                    }
                    if (wallFront && wallLeft && wallRight)
                    {
                        servo.write(90);
                        yield return delay(1500);
                        wallFront = false;
                        wallRight = true;
                        wallLeft = true;
                        allWall = true;
                    }
                    else if (!wallFront)
                    {
                        servo.write(90);
                        yield return delay(1500);
                    }
                    else if (!wallLeft)
                    {
                        servo.write(0);
                        yield return delay(3000);
                    }

                    if (wallLeft && wallRight)
                    {
                        Debug.Log("4");
                        front();
                        distanceOk = true;
                    }
                    else if (wallLeft && wallFront)
                    {
                        Debug.Log("5");
                        right();
                        yield return delay(1300);
                        stop();
                    }
                    else if (wallRight && wallFront)
                    {
                        Debug.Log("6");
                        left();
                        yield return delay(1300);
                        stop();
                    }
                    else
                    {
                        Debug.Log("7");
                        front();
                        distanceOk = true;
                    }
                }
                if (allWall || forceFront)
                {
                    Debug.Log("All wall!!!!!!!!!!!!");

                }
                else if (pulseIn(6) < 600)
                {
                    Debug.Log("8!!!!!!!!!!!!");
                    distanceOk = false;
                    stop();
                }
            }
            firstDist = true;
        } else
            {
                if (ldrLeft > 600 && ldrRight > 600)
                {
                    Debug.Log("Front");
                    front();
                }
                else if (ldrLeft < 600)
                {
                    left();
                }
                else if (ldrRight < 600)
                {
                    right();
                }
                else
                {
                    stop();
                }
                fromLdr = true;
            }

        // Debug.Log(value + " value at pin 5");



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


    void left()
    {
        if (!modeLeft)
        {
            stop();
            modeLeft = true;
            modeRight = false;
            modeFront = false;
            modeBack = false;
        }
        analogWrite(1, 30);
        analogWrite(2, 30);
    }

    void right()
    {
        if (!modeRight)
        {
            stop();
            modeLeft = false;
            modeRight = true;
            modeFront = false;
            modeBack = false;
        }
        analogWrite(0, 30);
        analogWrite(3, 30);
    }

    void front()
    {
        if (!modeFront)
        {
            stop();
            modeLeft = false;
            modeRight = false;
            modeFront = true;
            modeBack = false;
        }
        analogWrite(1, 70);
        analogWrite(3, 70);
    }

    void back()
    {
        if (!modeBack)
        {
            stop();
            modeLeft = false;
            modeRight = false;
            modeFront = false;
            modeBack = true;
        }
        analogWrite(0, 70);
        analogWrite(2, 70);
    }

    void stop()
    {
        digitalWrite(1, false);
        digitalWrite(3, false);
        digitalWrite(0, false);
        digitalWrite(2, false);
    }



    #region PremadeDefinitions
    void Start()
    {
        servo = FindObjectOfType<Servo>();
        if (servo == null)
        {
            Debug.Log("No servo found in the scene. Consider assigning it to 'ArduinoMain.cs' manually.");
        }
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

    public ulong abs(long x)
    {
        return (ulong)Mathf.Abs(x);
    }

    public long constrain(long x, long a, long b)
    {
        return (x < a ? a : (x > b ? b : x));
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

    public ulong pulseIn(int pin)
    {
        return breadboard.pulseIn(pin);
    }
    #endregion InterfacingWithBreadboard
}