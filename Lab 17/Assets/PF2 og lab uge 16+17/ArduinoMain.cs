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


    //Additions from 21-04-2020:

    //Distance sensor:
    //The Distance (ultrasonic) sensor is added, if you use "pulseIn()" on the pin it is assigned to, 
    //it will return the time it took sound to travel double the distance to the point of impact in microseconds (type: ulong).
    //This mimics roughly how the HC-SR04 sensor works. 
    //There is no max-range of the distance-sensor. If it doesn't hit anything, it returns a 0.

    //Servo:
    //if you add the servo-prefab to the scene, ArduinoMain will automatically find the servo object, essentially handling "servo.attach()" automatically. 
    //There can be only one servo controlled by this script.
    //servo.write() and servo.read() implemented, they function similar to a servomotor. 
    //The angles that servo.write() can handle are [0:179]. All inputs outside of this range, are clamped within the range.
    //servo.read() will return the last angle written to the servo-arm. 
    //In order to attach something to the servo, so that it rotates with the servo-arm, simply make the object you wish to rotate, a child of either: Servo-rotationCenter or Servo-arm. 
    //Make sure to take into account the position of the object relative to Servo-rotationCenter. The rotated object will rotate positively around the Y-axis (up) of the Servo-rotationCenter gameobject.




    IEnumerator setup()
    {
        //Your code goes here:

        //Example of delay:
        Debug.Log("pre-delay log");
        yield return delay(2000); //2 second delay
        //Your code ends here -----

        //following region ensures delay-functionality for setup() & loop(). Do not delete, must always be last thing in setup.
        #region PremadeSetup
        yield return StartCoroutine(loop()); ;
        #endregion PremadeSetup
    }

    //IEnumerator direction() 
    //{
    //    servo.write(0);
    //    yield return delay(3000);
    //    bool wallLeft = true;
    //    bool wallRight = true;
    //    bool wallFront = true;
    //    if (pulseIn(6) == 0)
    //    {
    //        Debug.Log("1");
    //        wallLeft = false;
    //    }
    //    servo.write(90);
    //    yield return delay(3000);
    //    if (pulseIn(6) == 0)
    //    {
    //        Debug.Log("2");
    //        wallFront = false;
    //    }
    //    servo.write(180);
    //    delay(2000);
    //    if (pulseIn(6) == 0)
    //    {
    //        Debug.Log("3");
    //        wallFront = false;
    //    }
    //    if (wallFront && wallLeft && wallRight)
    //    {
    //        servo.write(90);
    //        delay(2000);
    //        wallFront = false;
    //        wallRight = true;
    //        wallLeft = true;
    //    } else if (!wallFront)
    //    {
    //        servo.write(90);
    //        delay(2000);
    //    } else if (!wallLeft)
    //    {
    //        servo.write(0);
    //        delay(2000);
    //    }

    //    bool goLeft = false;
    //    bool goRight = false;
    //    bool goFront = false;

    //    if (wallLeft && wallRight)
    //    {
    //        Debug.Log("4");
    //        goFront = true;
    //    } else if (wallLeft && wallFront)
    //    {
    //        Debug.Log("5");
    //        goRight = true;
    //    } else if (wallRight && wallFront)
    //    {
    //        Debug.Log("6");
    //        goLeft = true;
    //    } else
    //    {
    //        Debug.Log("7");
    //        goFront = true;
    //    }


    //    return (goLeft, goFront, goRight);
    //}

    //void useDistance()
    //{    
    //    if (!distanceOk)
    //    {
    //        Debug.Log("NEW!!!!!!!!!!!!");
    //        stop();
    //        goTo = direction();

    //        if (goTo.Item1)
    //        {
    //            left();
    //        }
    //        else if (goTo.Item2)
    //        {
    //            front();
    //        }
    //        else if (goTo.Item3)
    //        {
    //            right();
    //        }
    //    }

    //    //Debug.Log("pre-delay log" + pulseIn(6));
    //    if (pulseIn(6) < 1000)
    //    {
    //        Debug.Log("8!!!!!!!!!!!!");
    //        distanceOk = false;
    //        stop();
    //    }
    //}

    void left()
    {
        analogWrite(1, 40);
        analogWrite(2, 40);
    }

    void right()
    {
        analogWrite(0, 40);
        analogWrite(3, 40);
    }

    void front()
    {
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

    bool distanceOk = false;
    (bool, bool, bool) goTo;
    bool allWall = false;
    bool forceFront = false;

    IEnumerator loop()
    {
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
                //distanceOk = true;
            }
            else if (wallRight && wallFront)
            {
                Debug.Log("6");
                left();
                yield return delay(1300);
                stop();
                //distanceOk = true;
            }
            else
            {
                Debug.Log("7");
                front();
                distanceOk = true;
            }
            //if (goLeft)
            //{
            //    left();
            //}
            //else if (goTo.Item2)
            //{
            //    front();
            //}
            //else if (goTo.Item3)
            //{
            //    right();
            //}
        }
        if (allWall || forceFront)
        {
            Debug.Log("All wall!!!!!!!!!!!!");

        } else if (pulseIn(6) < 600)
        {
            Debug.Log("8!!!!!!!!!!!!");
            distanceOk = false;
            stop();
        }


        //Your code goes here:
        // digitalWrite(1, false);
        // digitalWrite(3, false);
        // digitalWrite(0, false);
        // digitalWrite(2, false);

        // //Example analogRead:
        // int ldrLeft = analogRead(4);
        // int ldrRight = analogRead(5);
        //// Debug.Log(value + " value at pin 5");

        // if (ldrLeft > 600 && ldrRight > 600)
        // {
        //     analogWrite(1, 70);
        //     analogWrite(3, 70);
        // }
        // else if (ldrLeft < 600)
        // {
        //     analogWrite(1, 40);
        //     analogWrite(2, 40);
        // }
        // else if (ldrRight < 600)
        // {
        //     analogWrite(0, 40);
        //     analogWrite(3, 40);
        // }
        // else
        // {
        //     digitalWrite(1, false);
        //     digitalWrite(3, false);
        //     digitalWrite(0, false);
        //     digitalWrite(2, false);
        // }

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