using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatAttackSpeed : StatBaseContinuous
{
	private readonly float statMax;
	private readonly float statMin;
	public const StatType type = StatType.AttackSpeed;

	public StatAttackSpeed(float max, float min)
	{
		statMax = max;
		statMin = min;
	}
}