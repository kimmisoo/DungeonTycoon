using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatPenetrationFixed : StatBaseContinuous
{
	private readonly float statMax;
	private readonly float statMin;
	public const StatType type = StatType.PenetrationFixed;
	
	public StatPenetrationFixed(float max, float min)
	{
		statMax = max;
		statMin = min;
	}
}
