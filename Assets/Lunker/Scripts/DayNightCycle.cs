using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DayNightCycle : MonoBehaviour
{
    public float timeRate = 1f; // Speed at which time elapses
    public int day = 1; // Current in-game day
    private float currentTime = 0f; // Current time of day in Unity units (0 to 24)

    private void Update()
    {
        // Increment the current time based on the time rate
        currentTime += Time.deltaTime * timeRate;

        // Rotate the directional light to simulate the sun's movement
        transform.rotation = Quaternion.Euler((currentTime / 24f) * 360f, 0f, 0f);

        // If the current time exceeds 24 Unity units, increment the day
        if (currentTime >= 24f)
        {
            day++;
            currentTime -= 24f;

            // Check for major holidays and trigger events if necessary
            DateTime currentDate = DateTime.Now;
            if (currentDate.Month == 12 && currentDate.Day == 25)
            {
                // Christmas event
            }
            else if (currentDate.Month == 11 && currentDate.Day == 26)
            {
                // Thanksgiving event
            }
            // Add more holiday checks here
        }
    }
}
