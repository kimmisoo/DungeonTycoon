using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatDamageTakenSum : StatBaseContinuous
{
	private readonly float statMax;
	private readonly float statMin;
	public const StatType type = StatType.DamageTakenSum;
	
	public StatDamageTakenSum(float max, float min)
	{
		statMax = max;
		statMin = min;
	}
}
