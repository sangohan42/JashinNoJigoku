using UnityEngine;

[RequireComponent(typeof(Renderer))]
[RequireComponent(typeof(Light))]
public class LaserBlinking : MonoBehaviour
{
	public float onTime;			// Amount of time in seconds the laser is on for.
	public float offTime;			// Amount of time in seconds the laser is off for.
    
	private float _timer;			// Timer to time the laser blinking.
    private Renderer _renderer;
    private Light _light;

    void Start()
    {
        _timer = 0f;
        _renderer = GetComponent<Renderer>();
        _light = GetComponent<Light>();
    }

    void Update ()
	{
		// Increment the timer by the amount of time since the last frame.
        _timer += Time.deltaTime;
		
		// If the beam is on and the onTime has been reached...
        if(_renderer.enabled && _timer >= onTime)
			// Switch the beam.
            SwitchBeam();
		
		// If the beam is off and the offTime has been reached...
        if(!_renderer.enabled && _timer >= offTime)
			// Switch the beam.
            SwitchBeam();
	}
	
	
	void SwitchBeam ()
	{
		// Reset the timer.
		_timer = 0f;

        // Switch whether the beam and light are on or off.
        _renderer.enabled = !_renderer.enabled;
        _light.enabled = !_light.enabled;
	}
}
