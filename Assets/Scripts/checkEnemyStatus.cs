using UnityEngine;
using System.Collections;

public class checkEnemyStatus : MonoBehaviour {
	private GameObject[] enemies;
	private Animator[] enemiesAnimator;
	private HashIds hash;
	private int enemyNumber;

	// Use this for initialization
	void Start () {
		enemies = GameObject.FindGameObjectsWithTag (DoneTags.enemy);
		enemyNumber = enemies.Length;
		enemiesAnimator = new Animator[enemyNumber];
		for(int i = 0; i< enemyNumber; i++)
		{
			enemiesAnimator[i] = enemies[i].GetComponent<Animator>();
		}
		hash = GameObject.FindGameObjectWithTag(DoneTags.gameController).GetComponent<HashIds>();
	}

	public bool isNotSeen()
	{
		for(int i = 0; i< enemyNumber; i++)
		{
			if(enemiesAnimator[i].GetBool(hash.playerInSightBool) || enemiesAnimator[i].GetBool(hash.inPursuitBool))return false;
		}
		return true;
	}
}
