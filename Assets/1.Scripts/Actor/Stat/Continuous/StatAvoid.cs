using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatAvoid : StatBaseContinuous
{
	private readonly float statMax;
	private readonly float statMin;
	public const StatType type = StatType.Avoid;
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
	public StatAvoid(float max, float min)
	{
		statMax = max;
		statMin = min;
	}
}
