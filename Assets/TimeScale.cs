using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class TimeScale : MonoBehaviour
{
    // Time variable to track generations
    [HideInInspector]
    public int t;
    [HideInInspector]
    public int state;
    public float pauseCooldown;
    float timeOfLast;

    // Start is called before the first frame update
    void Start()
    {
        t = 0;
        state = 0;
    }

    // Update is called once per frame
    void Update()
    {
        timeOfLast = -1.0f;
        if ((Input.GetKeyDown(KeyCode.Space)) && ((timeOfLast == -1.0f) || (Time.time - timeOfLast >= pauseCooldown))) {
            if (state == 1) state = 0;
            else state = 1;
            timeOfLast = Time.time;
        } else if(Input.GetKeyDown(KeyCode.LeftArrow)) {
            // State 3 isnt gonna do anything until I figure out how to reverse time. Gonna be a while.
            state = 3 ;
        } else if(Input.GetKeyDown(KeyCode.RightArrow)) {
            state = 2;
        }
    }
}
