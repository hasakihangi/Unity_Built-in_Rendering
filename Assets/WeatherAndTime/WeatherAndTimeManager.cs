using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class WeatherAndTimeManager : MonoBehaviour
{
	[HideInInspector]
	public WeatherStateMachine weatherStateMachine;

	[Header("Time")]
	public Light sun;
	[Range(0,24), SerializeField] private float time;
	public float Time
	{
		set 
		{
			time = Mathf.Clamp(value, 0f, 24f);
		}
		get
		{
			return time;
		}
	}
	[Range(0, 24)] public float sunriseTime = 6f;

	[Space] public WeatherAndTimeSO parameters;

	private float gradientMultiplier;

	[Header("Resources")]
	public Material skyMaterial;
	public LensFlare lensFlare;
	public ParticleSystem rainyParticleSystem;

	//
	public Camera mainCamera;

	//
	public Weather currentWeather = Weather.Sunny;
	
	//
	private bool isChangingTime = false;
	private float timeTarget;
	private bool isBlending = false;
	private float blendTarget;
	private float fogDensity;
	
	private void Awake()
	{
		if (mainCamera is null)
			mainCamera = Camera.main;
		if (rainyParticleSystem is null)
			rainyParticleSystem = GetComponentInChildren<ParticleSystem>(true);
		if (lensFlare is null)
			lensFlare = sun.GetComponent<LensFlare>();
		fogDensity = RenderSettings.fogDensity;
		RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Trilight;
	}
	
	public void Initialize(float time = 16f, Weather weather = Weather.Sunny, float angle = 20f)
	{
		weatherStateMachine = new WeatherStateMachine(this);
		weatherStateMachine.Initialize(weatherStateMachine.sunnyState);
		InitializShaderPropertyToId();
		
		UpdateTime(time);
		ChangeWeatherState(weather);
		SetSunYEulerAngle(angle);
	}

	public void ChangeWeatherState(Weather weather)
	{
		weatherStateMachine.ChangeState(weather);
	}

	int _ProceduralSkyboxExposure;
	int _SunnyTextureExposure;
	int _SunnyBlend;
	int _CloudyTextureExposure;
	int _CloudyBlend;
	int _SunSize;
	private int _SkyboxRotation;
	
	public void InitializShaderPropertyToId()
	{
		_ProceduralSkyboxExposure = Shader.PropertyToID("_Exposure");
		_CloudyTextureExposure = Shader.PropertyToID("_TextureExposure");
		_CloudyBlend = Shader.PropertyToID("_Blend");
		_SunnyTextureExposure = Shader.PropertyToID("_SunnyTextureExposure");
		_SunnyBlend = Shader.PropertyToID("_SunnyBlend");
		_SunSize = Shader.PropertyToID("_SunSize");
		_SkyboxRotation = Shader.PropertyToID("_TextureRotation");
	}
	

	void Update()
	{
		if (weatherStateMachine != null)
		{
			weatherStateMachine.Tick();
		}

		if (isBlending)
		{
			float difference = blendTarget - skyMaterial.GetFloat("_Blend");
			if (Mathf.Abs(difference) < 0.05)
			{
				isBlending = false;
			}
			else
			{
				if (difference > 0) 
				{
					skyMaterial.SetFloat(_CloudyBlend, skyMaterial.GetFloat
						(_CloudyBlend) + parameters.weatherBlendSpeed * UnityEngine.Time.deltaTime);
				}
				else
				{
					skyMaterial.SetFloat(_CloudyBlend, skyMaterial.GetFloat(_CloudyBlend) - parameters.weatherBlendSpeed * UnityEngine.Time.deltaTime);
				}
			}
		}

		if (isChangingTime)
		{
			float difference = timeTarget - time;
			if (Mathf.Abs(difference) < 0.05)
			{
				isChangingTime = false;
			}
			else
			{
				if (difference > 0)
				{
					time = time + parameters.timeChangeSpeed * UnityEngine.Time.deltaTime;
					UpdateTime(time);
				}
				else
				{
					time = time - parameters.timeChangeSpeed * UnityEngine.Time.deltaTime;
					UpdateTime(time);
				}
			}
		}
	}
	
	public void SetTimeTarget(float time)
	{
		isChangingTime = true;
		timeTarget = time;
	}

	private void OnValidate()
	{
		UpdateSunRotation();
	}

	public void ChangeSunnyTexture(Texture texture)
	{
		skyMaterial.SetTexture("_SunnyTexture", texture);
		string s = texture.name;
		SetUpParametersWithSunnyTexture(s);
	}

	// 如果还需要其他晴天贴图, 则在这里设置与贴图相关的参数
	public void SetUpParametersWithSunnyTexture(string skyTextureName = null)
	{
		if (string.IsNullOrEmpty(skyTextureName))
		{
			skyMaterial.SetFloat(_SunnyBlend, 0f);
			skyMaterial.SetFloat(_SunSize, 0.06f);
		}
		else
		{
			switch (skyTextureName)
			{
				case "sky-1":
					skyMaterial.SetFloat(_SunnyBlend, 0.4f);
					skyMaterial.SetFloat(_SunSize, 0.06f);
					break;
				case "sky-2":
					skyMaterial.SetFloat(_SunnyBlend, 0.35f);
					skyMaterial.SetFloat(_SunSize, 0.06f);
					break;
				case "sky-3":
					skyMaterial.SetFloat(_SunnyBlend, 0.3f);
					skyMaterial.SetFloat(_SunSize, 0.06f);
					break;
				case "sky-4":
					skyMaterial.SetFloat(_SunnyBlend, 0.3f);
					skyMaterial.SetFloat(_SunSize, 0.06f);
					break;
				case "sky-5":
					skyMaterial.SetFloat(_SunnyBlend, 0.5f);
					skyMaterial.SetFloat(_SunSize, 0f);
					break;
				case "sky-6":
					skyMaterial.SetFloat(_SunnyBlend, 0.5f);
					skyMaterial.SetFloat(_SunSize, 0f);
					break;
				case "sky-7":
					skyMaterial.SetFloat(_SunnyBlend, 0.3f);
					skyMaterial.SetFloat(_SunSize, 0.06f);
					break;
				case "sky-8":
					skyMaterial.SetFloat(_SunnyBlend, 0.3f);
					skyMaterial.SetFloat(_SunSize, 0.06f);
					break;
				case "sky-9":
					skyMaterial.SetFloat(_SunnyBlend, 0.3f);
					skyMaterial.SetFloat(_SunSize, 0.06f);
					break;
			}
		}
	}

	public void UpdateWeatherParameters(Weather weather)
	{
		switch (weather)
		{
			case Weather.Sunny:
				SetTextureBlendTarget(parameters.cloudTextureBlendInSunny);
				if (parameters.isLightIntensityToChange)
					sun.intensity = parameters.sunnyLightIntensity;
				gradientMultiplier = parameters.sunnyGradientMultiplier;
				if (parameters.isFogIntensityToChange)
					RenderSettings.fogDensity = parameters
						.fogMuliplierInSunny * fogDensity;
				if (lensFlare is not null)
					lensFlare.brightness = 0.04f;
				break;
			case Weather.Cloudy:
				SetTextureBlendTarget(parameters.cloudTextureBlendInCloudy);
				if (parameters.isLightIntensityToChange)
					sun.intensity = parameters.cloudyLightIntensity;
				gradientMultiplier = parameters.cloudyGradientMultiplier;
				if (parameters.isFogIntensityToChange)
					RenderSettings.fogDensity = parameters
						.fogMuliplierInCloudy * fogDensity;
				if (lensFlare != null)
					lensFlare.brightness = 0.01f;
				break;
			case Weather.Rainy:
				SetTextureBlendTarget(parameters.cloudTextureBlendInRainy);
				if (parameters.isLightIntensityToChange)
					sun.intensity = parameters.rainyLightIntensity;
				gradientMultiplier = parameters.rainyGradientMultiplier;
				if (parameters.isFogIntensityToChange)
					RenderSettings.fogDensity = parameters
						.fogMuliplierInRainy * fogDensity;
				if (lensFlare != null)
					lensFlare.brightness = 0f;
				break;
		}
	}

	public void UpdateEnvironmentGradient()
	{
		float t = time / 24f;
		if (parameters is not null)
		{
			RenderSettings.ambientSkyColor = parameters.skyGradient.Evaluate(t) *
				gradientMultiplier;
			RenderSettings.ambientEquatorColor = parameters.equatorGradient.Evaluate(t) *
				gradientMultiplier;
			RenderSettings.ambientGroundColor = parameters.groundGradient.Evaluate(t) *
				gradientMultiplier;
		}
	}

	public void UpdateRainParticlesPosition()
	{
		if (!(mainCamera is null) && !(rainyParticleSystem is null))
		{
			rainyParticleSystem.transform.position = mainCamera.transform.position + new Vector3(0, 10, 0);
		}
	}
	
	public void SetSunYEulerAngle(float angle)
	{
		Transform pivot = sun.transform.parent;
		if (!pivot)
			pivot.localEulerAngles = new Vector3(0, angle, 0);
	}

	/// <summary>
	/// Set up time of day.
	/// </summary>
	/// <param name="time"> Range: 0~24 </param>
	public void UpdateSunRotation()
	{
		float t = time / 24.0f;
		float tOffset = (sunriseTime - 6f) / 24.0f;
		t = Mathf.Repeat(t + tOffset, 1);

		float sunRotation = Mathf.Lerp(-90, 270, t);

		if (sun != null)
		{
			sun.transform.localRotation = Quaternion.Euler(sunRotation, sun.transform.localRotation.y, sun.transform.localRotation.z);
		}
	}


	public void UpdateTime(float time)
	{
		Time = time;
		UpdateSunRotation();
		UpdateEnvironmentGradient();
		UpdateCloudyTextureExposureWithTime(time);
	}
	
	public void UpdateCloudyTextureExposureWithTime(float time)
	{
		if (currentWeather == Weather.Cloudy || currentWeather == Weather.Rainy)
		{
			if (time > sunriseTime - 1f && time < sunriseTime)
			{
				float t = time - (sunriseTime - 1f);
				skyMaterial.SetFloat(_CloudyTextureExposure, Mathf.Lerp(0.25f, 0.8f, t));
			}
			else if (time > sunriseTime + 12f && time < sunriseTime + 13f)
			{
				float t = sunriseTime + 13f - time;
				skyMaterial.SetFloat(_CloudyTextureExposure, Mathf.Lerp(0.25f, 0.8f, t));
			}
		}
	}
	
	public void RotateClouds(float rotationSpeed)
	{
		skyMaterial.SetFloat(_SkyboxRotation, skyMaterial.GetFloat(_SkyboxRotation) + 
			UnityEngine.Time.deltaTime * rotationSpeed);
	}

	
	public void SetTextureBlendTarget(float t)
	{
		isBlending = true;
		blendTarget = t;
	}
}

public class WeatherStateMachine
{
	public Dictionary<Weather, WeatherState> weathers;

	public SunnyState sunnyState;
	public CloudyState cloudyState;
	public RainyState rainyState;

	public WeatherState currentState;
	public WeatherState lastState;

	public WeatherAndTimeManager weatherManager;

	public WeatherStateMachine(WeatherAndTimeManager weatherManager)
	{
		this.weatherManager = weatherManager;

		weathers = new Dictionary<Weather, WeatherState>();

		sunnyState = new SunnyState(weatherManager);
		weathers.Add(Weather.Sunny, sunnyState);
		cloudyState = new CloudyState(weatherManager);
		weathers.Add(Weather.Cloudy, cloudyState);
		rainyState = new RainyState(weatherManager);
		weathers.Add(Weather.Rainy, rainyState);
	}

	public void Initialize(WeatherState startState)
	{
		currentState = startState;
		startState.Enter();
	}

	public void ChangeState(WeatherState newState)
	{
		if (newState == currentState)
			return;
		currentState.Quit();
		lastState = currentState;
		currentState = newState;
		currentState.Enter();
	}

	public void ChangeState(Weather weather)
	{
		WeatherState weatherState = weathers[weather];
		ChangeState(weatherState);
	}

	public void Tick()
	{
		currentState.Tick();
	}
}

public class SunnyState : WeatherState
{
	public SunnyState(WeatherAndTimeManager weatherManager) : base(weatherManager)
	{

	}

	public override void Enter()
	{
		weatherManager.currentWeather = Weather.Sunny;
		weatherManager.UpdateWeatherParameters(Weather.Sunny);
	}

	public override void Quit()
	{
		
	}

	public override void Tick()
	{
		weatherManager.RotateClouds(weatherManager.parameters.cloudRotationSpeedInSunny);
	}
}

public class CloudyState : WeatherState
{
	public CloudyState(WeatherAndTimeManager weatherManager) : base(weatherManager)
	{
		
	}

	public override void Enter()
	{
		weatherManager.currentWeather = Weather.Cloudy;
		weatherManager.UpdateWeatherParameters(Weather.Cloudy);
	}

	public override void Quit()
	{
		
	}

	public override void Tick()
	{
		weatherManager.RotateClouds(weatherManager.parameters.cloudRotationSpeedInCloudy);
	}
}

public class RainyState : WeatherState
{
	ParticleSystem particleSystem;

	public RainyState(WeatherAndTimeManager weatherManager) : base(weatherManager)
	{
		particleSystem = weatherManager.rainyParticleSystem;
	}

	public override void Enter()
	{
		particleSystem.gameObject.SetActive(true);
		weatherManager.UpdateWeatherParameters(Weather.Rainy);
		weatherManager.currentWeather = Weather.Rainy;
	}

	public override void Quit()
	{
		particleSystem.gameObject.SetActive(false);
	}

	public override void Tick()
	{
		weatherManager.RotateClouds(weatherManager.parameters.cloudRotationSpeedInRainy);
		weatherManager.UpdateRainParticlesPosition();
	}
}

public enum Weather
{
	Sunny,
	Cloudy,
	Rainy
}

// 状态机构件
public abstract class WeatherState : IState
{
	protected WeatherAndTimeManager weatherManager;

	protected WeatherState(WeatherAndTimeManager weatherManager)
	{
		this.weatherManager = weatherManager;
	}
	public abstract void Enter();
	public abstract void Quit();
	public abstract void Tick();
}
public interface IState
{
	public void Enter();
	public void Tick();
	public void Quit();
}
