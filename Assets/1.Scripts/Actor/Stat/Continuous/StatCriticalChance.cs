using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatCriticalChance : StatBaseContinuous
{
	private readonly float statMax;
	private readonly float statMin;
	public const StatType type = StatType.CriticalChance;
	public override float baseValue
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
	public StatCriticalChance(float max, float min)
	{
		statMax = max;
		statMin = min;
	}
}
