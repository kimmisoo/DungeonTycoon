using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatCriticalDamage : StatBaseContinuous
{
	private readonly float statMax;
	private readonly float statMin;
	public const StatType type = StatType.CriticalDamage;
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
	public StatCriticalDamage(float max, float min)
	{
		statMax = max;
		statMin = min;
	}
}
