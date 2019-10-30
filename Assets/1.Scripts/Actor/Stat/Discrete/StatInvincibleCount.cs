using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatInvincibleCount : StatBaseDiscrete
{
	private readonly int statMax;
	private readonly int statMin;
	public const StatType type = StatType.InvincibleCount;
	public override int baseValue
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
	public StatInvincibleCount(int max, int min)
	{
		statMax = max;
		statMin = min;
	}
}