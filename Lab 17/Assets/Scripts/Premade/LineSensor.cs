using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class LineSensor : ArduinoObject
{
    public LayerMask raycastMask;

    [SerializeField]
    private Transform raycastTransform; //Assign in inspector - it's there for a functional minimum and maximum range to work.
    private const int pixelsToAverage = 2; //(pixelsToAverage+1)^2 = amount of pixels that will be averaged. A bit of a misleading name.
    private const float raycastRange = 1.5f;

    void Start()
    {
        Debug.Assert(raycastTransform != null, "rayCastTransform missing from LineSensor.cs component - revert to prefab settings or assign manually");
    }

    /*
    void Update()
    {
        //Just for testing.
        Debug.Log(analogRead() + " = greyscale value");
    }
    */
    override public ulong pulseIn()
    {
        throw new NotImplementedException();
    }
    override public void analogWrite(int value)
    {
        //Not quite sure what this would/should do. It shouldn't be done.
        throw new NotImplementedException();
    }
    override public bool digitalRead()
    {
        //Simulated result of digitalReading from line-sensor.
        bool isAbove3V = (ArduinoFunctions.Functions.map(analogRead(), 0, 1023, 0, 5) > 3);
        return isAbove3V ? true : false;
    }
    override public void digitalWrite(bool isHigh)
    {
        //Not quite sure what this would/should do. It shouldn't be done.
        throw new NotImplementedException();
    }

    //Raycasts to ground, finds point-coordinate on texture, averages out the greyscale of said point and an amount of its neighbors:
    override public int analogRead()
    {
        float greyScale = 0;
        Vector3 direction = transform.forward;
        RaycastHit raycastHit;
        Debug.DrawRay(raycastTransform.position, direction * raycastRange, Color.red);

        if (Physics.Raycast(raycastTransform.position, direction * raycastRange, out raycastHit, raycastMask))
        {
            Renderer renderer = raycastHit.collider.GetComponent<MeshRenderer>();
            Texture2D texture2D = renderer.material.mainTexture as Texture2D;
            Vector2 pCoord = raycastHit.textureCoord;
            pCoord.x *= texture2D.width;
            pCoord.y *= texture2D.height;
            Vector2 tiling = renderer.material.mainTextureScale;

            //Average all rgb-values 
            int counter = 0;
            for (int x = -pixelsToAverage; Mathf.Abs(x) < pixelsToAverage + 1; x++)
            {
                for (int y = -pixelsToAverage; Mathf.Abs(y) < pixelsToAverage + 1; y++)
                {
                    Color color = texture2D.GetPixel(Mathf.FloorToInt(pCoord.x * tiling.x) + x, Mathf.FloorToInt(pCoord.y * tiling.y) + y);
                    counter++;
                    greyScale += CustomGreyscaleOfColor(color);
                }
            }
            //Right now all pixels are equally weighted - they could be weighted based on distance from center-pixel. 
            //As to simulate light-dropoff with range.
            greyScale /= counter;
            //Debug.Log(greyScale + " = greyscale value" + " over " + counter + " pixels");
        }
        else
        {
            return 0;
        }

        return (int)ArduinoFunctions.Functions.map(greyScale, 0, 1, 0, 1023); ;
    }

    private float CustomGreyscaleOfColor(Color color)
    {
        return (color.r + color.g + color.b) / 3;
    }


}
