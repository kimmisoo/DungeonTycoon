using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterActAI : MonoBehaviour {
	//사망 이후 처리, move , attack 관리

	

	public void Start()
	{
		StartCoroutine(Acting());
		target = null;
		actingResult.Clear();		
	}
	//battle;
	//move;
	//act?
	public IEnumerator Acting()
	{
		yield return StartCoroutine(EnemySearch(actingResult));
	}



	
}
