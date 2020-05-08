using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatMoveSpeed : StatBaseContinuous
{
	private readonly float statMax;
	private readonly float statMin;
	public const StatType type = StatType.MoveSpeed;
	public override float BaseValue
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
	public StatMoveSpeed(float max, float min)
	{
		statMax = max;
		statMin = min;
	}
}
