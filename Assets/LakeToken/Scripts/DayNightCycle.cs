using System;
using UnityEngine;

public class DayNightCycle : MonoBehaviour
{
    public float timeRate = 1f; // Speed at which time elapses
    public int day = 1; // Current in-game day
    public float startTime = 6f; // Starting time of day in Unity units (0 to 24)
    public Light directionalLight;
    public Gradient fogColorGradient; // Gradient representing fog colors based on the angle of the light
    public float transitionDuration = 2f; // Duration of color transition in seconds

    private float currentTime; // Current time of day in Unity units (0 to 24)
    private Color targetFogColor;
    private float transitionTimer;

    private void Start()
    {
        currentTime = startTime;
        RenderSettings.fogColor = GetCurrentFogColor(currentTime);
        targetFogColor = RenderSettings.fogColor;
        transitionTimer = transitionDuration;
    }

    private void Update()
    {
        UpdateTime();
        RotateDirectionalLight();
        UpdateFogColor();
    }

    private void UpdateTime()
    {
        currentTime += Time.deltaTime * timeRate;
        if (currentTime >= 24f)
        {
            day++;
            currentTime -= 24f;
        }
    }

    private void RotateDirectionalLight()
    {
        directionalLight.transform.rotation = Quaternion.Euler((currentTime / 24f) * 360f, 0f, 0f);
    }

    private void UpdateFogColor()
    {
        targetFogColor = GetCurrentFogColor(currentTime);
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

    private Color GetCurrentFogColor(float time)
    {
        float currentAngle = (time / 24f) * 360f;
        float gradientTime = Mathf.InverseLerp(0f, 360f, currentAngle);
        return fogColorGradient.Evaluate(gradientTime);
    }
}

