using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatAttack : StatBaseContinuous
{
	private readonly float statMax;
	private readonly float statMin;
	public const StatType type = StatType.Attack;
	
	public StatAttack(float max, float min)
	{
		statMax = max;
		statMin = min;
	}
	
}
