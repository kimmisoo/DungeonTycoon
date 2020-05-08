using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatAttackRange : StatBaseDiscrete
{
	private readonly int statMax;
	private readonly int statMin;
	public const StatType type = StatType.AttackRange;
	public override int BaseValue
	{
		get
		{
			return Mathf.Clamp(_baseValue, statMin, statMax);
		}

		set
		{
			_baseValue = Mathf.Clamp(value, statMin, statMax);
		}
	}
	public StatAttackRange(int max, int min)
	{
		statMax = max;
		statMin = min;
	}
}
