using UnityEngine;

public class EnemyAnimatorSetup
{
    public float speedDampTime = 0.1f;				// Damping time for the Speed parameter.
	public float angularSpeedDampTime = 0.5f;		// Damping time for the AngularSpeed parameter
	public float angleResponseTime = 0.6f;			// Response time for turning an angle into angularSpeed.
    
	private Animator _animator;						// Reference to the animator component.
	private AnimatorHashIds _animatorHashIds;		// Reference to the HashIDs script.
    
	// Constructor
    public EnemyAnimatorSetup(Animator animator, AnimatorHashIds hashIDs)
    {
        _animator = animator;
		_animatorHashIds = hashIDs;
    }

    public void Setup(float speed, float angle)
    {
		// Angular speed is the number of degrees per second.
        float angularSpeed = angle / angleResponseTime;
        
		// Set the mecanim parameters and apply the appropriate damping to them.
        _animator.SetFloat(_animatorHashIds.SpeedFloatEnemy, speed, speedDampTime, Time.deltaTime);
		_animator.SetFloat(_animatorHashIds.AngularSpeedFloat, angularSpeed, angularSpeedDampTime, Time.deltaTime);
    }	
}
