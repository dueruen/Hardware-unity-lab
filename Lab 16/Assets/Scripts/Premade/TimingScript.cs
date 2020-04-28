using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class TimingScript : MonoBehaviour
{
    private string timerString = "Timer: ";
    private float timer;
    public TextMeshProUGUI text;

    private bool timeUp;
    void Start()
    {
        timer = 0;

    }
    void Update()
    {
        if (timeUp)
        {
            return;
        }
        timer += Time.deltaTime;
        text.text = timerString + timer;
        if (Input.GetKeyDown(KeyCode.Space))
        {
            timeUp = true;
        }
    }
}
