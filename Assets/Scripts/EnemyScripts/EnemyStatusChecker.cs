using UnityEngine;

public class EnemyStatusChecker : MonoBehaviour
{
	private GameObject[] _enemies;
	private Animator[] _enemiesAnimator;
	private AnimatorHashIds _animatorHashIds;

	// Use this for initialization
	void Start ()
    {
        _enemies = GameObject.FindGameObjectsWithTag(Tags.enemy);
		_enemiesAnimator = new Animator[_enemies.Length];
		for(int i = 0; i< _enemies.Length; i++)
		{
			_enemiesAnimator[i] = _enemies[i].GetComponent<Animator>();
		}
		_animatorHashIds = GameObject.FindGameObjectWithTag(Tags.animatorHashController).GetComponent<AnimatorHashIds>();
	}

	public bool isNotSeen()
    {
        foreach (var animator in _enemiesAnimator)
        {
            if(animator.GetBool(_animatorHashIds.PlayerInSightBool) || animator.GetBool(_animatorHashIds.InPursuitBool))
                return false;
        }

        return true;
    }
}
