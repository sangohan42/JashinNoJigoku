using UnityEngine;

[RequireComponent(typeof(Light))]
public class AlarmLightIntensitySwitcher : MonoBehaviour
{
    [SerializeField]
    private float _fadeSpeed = 2f;			// How fast the light fades between intensities.
    [SerializeField]
    private float _maxIntensity = 2f;		// The maximum intensity of the light whilst the alarm is on.
    [SerializeField]
    private float _minIntensity = 0.5f;		// The minimum intensity of the light whilst the alarm is on.
    [SerializeField]
    private float _changeMargin = 0.2f;		// The margin within which the target intensity is changed.
    [SerializeField]
    private bool _alarmOn;					// Whether or not the alarm is on.

    private Light _light;
    private float _targetIntensity;          // The intensity that the light is aiming for currently.

    void Awake ()
    {
        _light = GetComponent<Light>();

        // When the level starts we want the light to be "off".
        _light.intensity = 0f;

        // When the alarm starts for the first time, the light should aim to have the maximum intensity.
        _targetIntensity = _maxIntensity;
	}
	
	
	void Update ()
	{
		// If the light is on...
		if(_alarmOn)
		{
            // ... Lerp the light's intensity towards the current target.
            _light.intensity = Mathf.Lerp(_light.intensity, _targetIntensity, _fadeSpeed * Time.deltaTime);
			
			// Check whether the target intensity needs changing and change it if so.
			CheckTargetIntensity();
		}
		else
            // Otherwise fade the light's intensity to zero.
            _light.intensity = Mathf.Lerp(_light.intensity, 0f, _fadeSpeed * Time.deltaTime);
	}
	
	
	void CheckTargetIntensity ()
	{
		// If the difference between the target and current intensities is less than the change margin...
		if(Mathf.Abs(_targetIntensity - _light.intensity) < _changeMargin)
		{
			// ... if the target intensity is high...
			if(_targetIntensity == _maxIntensity)
                // ... then set the target to low.
                _targetIntensity = _minIntensity;
			else
                // Otherwise set the targer to high.
                _targetIntensity = _maxIntensity;
		}
	}
}
