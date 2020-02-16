using UnityEngine;

public class EnemyAnimatorSetup
{
    public float speedDampTime = 0.1f;				// Damping time for the Speed parameter.
	public float angularSpeedDampTime = 0.7f;		// Damping time for the AngularSpeed parameter
	public float angleResponseTime = 0.6f;			// Response time for turning an angle into angularSpeed.
    
	private Animator anim;							// Reference to the animator component.
	private AnimatorHashIds hash;					// Reference to the HashIDs script.
    
	// Constructor
    public EnemyAnimatorSetup(Animator animator, AnimatorHashIds hashIDs)
    {
        anim = animator;
		hash = hashIDs;
    }

    public void Setup(float speed, float angle)
    {
		// Angular speed is the number of degrees per second.
        float angularSpeed = angle / angleResponseTime;
        
		// Set the mecanim parameters and apply the appropriate damping to them.
        anim.SetFloat(hash.SpeedFloatEnemy, speed, speedDampTime, Time.deltaTime);
		anim.SetFloat(hash.AngularSpeedFloat, angularSpeed, angularSpeedDampTime, Time.deltaTime);
    }	
}
