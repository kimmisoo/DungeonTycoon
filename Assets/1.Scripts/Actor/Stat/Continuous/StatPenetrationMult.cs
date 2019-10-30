using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatPenetrationMult : StatBaseContinuous
{
	private readonly float statMax;
	private readonly float statMin;
	public const StatType type = StatType.PenetrationMult;
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
	public StatPenetrationMult(float max, float min)
	{
		statMax = max;
		statMin = min;
	}
}
