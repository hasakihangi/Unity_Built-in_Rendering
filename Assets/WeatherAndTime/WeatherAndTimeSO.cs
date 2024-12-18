using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="WeatherAndTimeParameters", menuName = "ScriptableObject/WeatherAndTimeParameters")]
public class WeatherAndTimeSO : ScriptableObject
{
    [Header("Enviroment Lighting Gradient")]
    public Gradient skyGradient;
    public Gradient equatorGradient;
    public Gradient groundGradient;

    [Header("ChangeRate")] public float weatherBlendSpeed = 0.6f;
    public float timeChangeSpeed = 3f;

    [Header("Weather")] public bool isLightIntensityToChange = true;
    public bool isFogIntensityToChange = true;
    
    [Header("Sunny")] public float sunnyLightIntensity = 0.5f;
    public float sunnyGradientMultiplier = 0.9f;
    public float cloudTextureBlendInSunny = 0.1f;
    public float cloudRotationSpeedInSunny = 0.5f;
    public float fogMuliplierInSunny = 1f;

    [Header("Cloudy")] public float cloudyLightIntensity = 0.3f;
    public float cloudyGradientMultiplier = 0.8f;
    public float cloudTextureBlendInCloudy = 0.9f;
    public float cloudRotationSpeedInCloudy = 1f;
    public float fogMuliplierInCloudy = 1.5f;
    
    [Header("Rainy")] public float rainyLightIntensity = 0.3f;
    public float rainyGradientMultiplier = 0.75f;
    public float cloudTextureBlendInRainy = 0.9f;
    public float cloudRotationSpeedInRainy = 1.5f;
    public float fogMuliplierInRainy = 2f;
}
