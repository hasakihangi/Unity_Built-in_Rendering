using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class WeatherAndTimeView : MonoBehaviour
{
	WeatherAndTimeManager weatherAndTimeManager;
	public Toggle sunnyToggle;
	public Toggle rainyToggle;
	public Toggle cloudyToggle;
	public Slider timeSlider;

	public Button timeButton;
	public Button skyBoxButton;

	public Texture cubeMap;
	public float toTime;
	
    void Start()
    {
		weatherAndTimeManager = GameObject.FindWithTag("WeatherAndTime").GetComponent<WeatherAndTimeManager>();
		weatherAndTimeManager.Initialize(15f, Weather.Sunny, 20f);
		timeSlider.value = (15f - 6f) / (20f - 6f);

		sunnyToggle.onValueChanged.AddListener( t =>
		{
			if (t)
			{
				weatherAndTimeManager.weatherStateMachine.ChangeState(Weather.Sunny);
			}
		});

		rainyToggle.onValueChanged.AddListener(t =>
		{
			if (t)
			{
				weatherAndTimeManager.weatherStateMachine.ChangeState(Weather.Rainy);
			}
		});

		cloudyToggle.onValueChanged.AddListener(t =>
		{
			if (t)
			{
				weatherAndTimeManager.weatherStateMachine.ChangeState(Weather.Cloudy);
			}
		});

		timeSlider.onValueChanged.AddListener(t =>
		{
			float time = 6f + (20f - 6f) * t;
			weatherAndTimeManager.UpdateTime(time);
		});

		
		timeButton.onClick.AddListener(() =>
		{
			weatherAndTimeManager.SetTimeTarget(toTime);
		});

		
		skyBoxButton.onClick.AddListener(() =>
		{
			weatherAndTimeManager.ChangeSunnyTexture(cubeMap);
		});
	}
    
    void Update()
    {
        
    }
}
