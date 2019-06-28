using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterActAI : MonoBehaviour {
	//사망 이후 처리, move , attack 관리

	Monster monster;
	Character target;
	ActingResult actingResult = new ActingResult();
	Coroutine moving;
	Coroutine attacking;
	Coroutine stunning;

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



	public IEnumerator EnemySearch(ActingResult result)
	{
		target = null;
		int distance = int.MaxValue;
		yield return null;
		foreach (Actor a in GetAdjacentActor(2))
		{
			if ((a is Adventurer || a is SpecialAdventurer) && a.state != State.Dead)
			{
				if (target == null)
				{
					target = a as Character;
					distance = GetDistanceFromOtherActorForMove(target);
					actingResult.isFoundEnemy = true;
				}
				else
				{
					if (distance > GetDistanceFromOtherActorForMove(a))
					{
						target = a as Character;
					}
				}
			}
		}
	}
}
