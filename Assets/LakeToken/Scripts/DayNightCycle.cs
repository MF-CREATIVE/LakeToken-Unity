using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DayNightCycle : MonoBehaviour
{
    public float timeRate = 1f; // Speed at which time elapses
    public int day = 1; // Current in-game day
    public Color defaultFogColor;
    public Light directionalLight;
    public List<AngleColorPair> angleColorPairs = new List<AngleColorPair>();
    public float transitionDuration = 2f; // Duration of color transition in seconds
    private float currentTime = 0f; // Current time of day in Unity units (0 to 24)
    private Color targetFogColor;
    private float transitionTimer;

    [Serializable]
    public struct AngleColorPair
    {
        public float angle;
        public Color color;
    }

    private void Start()
    {
        RenderSettings.fogColor = defaultFogColor;
        targetFogColor = defaultFogColor;
        transitionTimer = transitionDuration;
    }

    private void Update()
    {
        // Increment the current time based on the time rate
        currentTime += Time.deltaTime * timeRate;

        // Rotate the directional light to simulate the sun's movement
        directionalLight.transform.rotation = Quaternion.Euler((currentTime / 24f) * 360f, 0f, 0f);

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

        // Get the current rotation angle of the directional light
        float currentAngle = directionalLight.transform.eulerAngles.x;

        // Find the closest angle-color pair
        float minAngleDiff = float.MaxValue;
        foreach (AngleColorPair angleColorPair in angleColorPairs)
        {
            float angleDiff = Mathf.Abs(angleColorPair.angle - currentAngle);
            if (angleDiff < minAngleDiff)
            {
                minAngleDiff = angleDiff;
                targetFogColor = angleColorPair.color;
            }
        }

        // Transition between current fog color and target fog color
        if (RenderSettings.fogColor != targetFogColor)
        {
            transitionTimer += Time.deltaTime;
            float colorProgress = Mathf.Clamp01(transitionTimer / transitionDuration);
            RenderSettings.fogColor = Color.Lerp(RenderSettings.fogColor, targetFogColor, colorProgress);
        }
        else
        {
            transitionTimer = 0f;
        }
    }
}
