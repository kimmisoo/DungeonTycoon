using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatShield : StatBaseContinuous
{
	private readonly float statMax;
	private readonly float statMin;
	public const StatType type = StatType.Shield;
	
	public StatShield(float max, float min)
	{
		statMax = max;
		statMin = min;
	}
}
