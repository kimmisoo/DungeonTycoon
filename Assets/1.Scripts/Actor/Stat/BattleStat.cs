using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleStat {

	Dictionary <StatType, StatBaseContinuous> battleStatContinuous = new Dictionary<StatType, StatBaseContinuous> 
	
	public bool TakeDamageProcess(float damage) //계산된 데미지로 피해 처리
	{
		//returns true if Dead

		if (currentShield + currentHealth - damage <= 0.0f)
		{
			//Dead
			currentShield = 0.0f;
			currentHealth = 0.0f;
			damageTakenSum += damage;
			return true;
		}
		else
		{
			damageTakenSum += damage;
			if (currentShield - damage < 0.0f)
			{
				damage = damage - currentShield;
				currentShield = 0.0f;
				currentHealth = currentHealth - damage;
			}
			else
			{
				currentShield = currentShield - damage;
			}
			return false;
		}
	}
}
