﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatCriticalChance : StatBaseContinuous
{
	private readonly float statMax;
	private readonly float statMin;
    private readonly float OverallMax;
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
	public StatCriticalChance(float max, float min, float ovMax = 1.0f)
	{
		statMax = max;
		statMin = min;
        OverallMax = ovMax;
	}

    public override float GetCalculatedValue()
    {
        float valueFixed = 0.0f;
        float valueMult = 1.0f;
        foreach (StatModContinuous mod in modList)
        {
            if (mod.type == ModType.Fixed)
            {
                valueFixed += mod.modValue;
            } // Fixed 합
            else
            {
                valueMult += mod.modValue;
            } // Mult 합
        }
        return Mathf.Clamp((baseValue + valueFixed) * valueMult, 0, OverallMax);
    }
}
