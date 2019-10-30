using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatDefence : StatBaseContinuous
{
	private readonly float statMax;
	private readonly float statMin;
	public const StatType type = StatType.Defence;
	
	public StatDefence(float max, float min)
	{
		statMax = max;
		statMin = min; // => 0.0f... 방어력이 음수가 되면 0으로 나눌 수 있음..
	}
}
