using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using System;

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
    bool lastGoingLeft = false;
    bool lastGoingRight = false;
    bool rightAvailable = false;
    bool leftAvailable = false;
    bool passedObstacle = false;
    bool ostacleFound = false;
    int ostaclesPassed = 0;

    IEnumerator loop()
    {
        if (start)
        {

            servo.write(90);
            yield return delay(1500);
            start = false;
            fromLdr = true;
        }


        //Example analogRead:
        int ldrLeft = analogRead(4);
        int ldrRight = analogRead(5);
        ulong dist = pulseIn(6);

        Debug.Log("Ldr left: " + ldrLeft + " right: " + ldrRight + "  dist: " + dist);
        if (fromLdr && firstDist)
        {
            align();
            yield return delay(50);
            stop();
            firstDist = false;
        }
        else if (fromLdr)
        {
            align();
            followLine();
        }
        else if (modeDist)
        {
            dist = pulseIn(6);
            while (dist > 300 || dist == 0)
            {
                front();
                yield return delay(2);
                dist = pulseIn(6);
            }

            stop();
            //check right 
            servo.write(180);
            yield return delay(1500);
            dist = pulseIn(6);
            if (dist > 1000 || dist == 0)
            {
                rightAvailable = true;
                Debug.Log("right is available");
            }


            //check left
            servo.write(0);
            yield return delay(3000);
            dist = pulseIn(6);
            if (dist > 800 || dist == 0)
            {
                leftAvailable = true;
                Debug.Log("Left is available");
            }

            if (rightAvailable && ostaclesPassed < 1)
            {
                right();
                yield return delay(1400);
                stop();
                dist = pulseIn(6);
                while (dist < 350 && dist != 0)
                {
                    Debug.Log("i am in the loop");
                    front();
                    yield return delay(2);
                    dist = pulseIn(6);
                }

                yield return delay(900);
                stop();
                left();
                yield return delay(1600);

                while (!passedObstacle)
                {
                    front();
                    yield return delay(2);
                    dist = pulseIn(6);

                    if (dist < 300 && dist != 0)
                    {
                        ostacleFound = true;
                    }

                    if (ostacleFound && (dist > 300 || dist == 0))
                    {
                        passedObstacle = true;
                    }

                }
                yield return delay(900);
                stop();
                left();
                servo.write(90);
                yield return delay(1600);
                front();
                yield return delay(2);

                while (ldrLeft > 900 || ldrRight > 900)
                {
                    ldrLeft = analogRead(4);
                    ldrRight = analogRead(5);
                    Debug.Log("Last loop");
                    yield return delay(2);
                }

                yield return delay(200);
                stop();
                ldrLeft = analogRead(4);
                while (ldrLeft > 900)
                {
                    right();
                    yield return delay(2);
                    ldrLeft = analogRead(4);
                }

                modeDist = false;
                fromLdr = true;
                Debug.Log("I am done");
                rightAvailable = false;
                leftAvailable = false;
                ostaclesPassed++;


            }

            if (rightAvailable && ostaclesPassed >= 1)
            {
                right();
                servo.write(90);
                yield return delay(1500);
                rightAvailable = false;
                leftAvailable = false;

            }

            if (leftAvailable)
            {
                Debug.Log("left!");
                left();
                servo.write(90);
                yield return delay(1800);
                leftAvailable = false;
                rightAvailable = false;

            }  
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

    void align()
    {
        Debug.Log("ALIGN!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
        int ldrRight = analogRead(5);
        int ldrLeft = analogRead(4);
        if (lastGoingLeft)
        {
            left();
            if (ldrRight < 500)
            {
                fromLdr = false;
                firstDist = false;
                modeDist = true;
            }
        }
        else if (lastGoingRight)
        {
            right();
            if (ldrRight < 500)
            {
                fromLdr = false;
                firstDist = false;
                modeDist = true;
            }
        }
    }

    void followLine()
    {
        int ldrRight = analogRead(5);
        int ldrLeft = analogRead(4);
        int difLR = (ldrLeft - ldrRight);
        int difRL = (ldrRight - ldrLeft);
        ulong dist = pulseIn(6);

        int leftSpeed = (ldrLeft / 4) / 4;
        int rightSpeed = (ldrRight / 4) / 4;

        if (difLR < 200 && difRL < 200)
        {
            leftSpeed *= 3;
            rightSpeed *= 3;
        }
        else if (difRL > difLR)
        {
            lastGoingLeft = true;
            lastGoingRight = false;
        }
        else if (difRL < difLR)
        {
            lastGoingRight = true;
            lastGoingLeft = false;
        }

        analogWrite(3, leftSpeed);
        analogWrite(1, rightSpeed);
        fromLdr = true;

        if ((ldrLeft < 350 && ldrRight < 350) || (dist < 250 && dist != 0))
        {
            stop();
            fromLdr = false;
            modeDist = true;
            return;
        }
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