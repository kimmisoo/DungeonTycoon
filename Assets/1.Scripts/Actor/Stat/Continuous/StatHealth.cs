using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatHealth : StatBaseContinuous
{
	public delegate void NotifyDie();
	NotifyDie notifyDie;
	private float statMax;
	private float statMin;
	
	public const StatType type = StatType.Health;
	
	public override float baseValue
	{
		get
		{
			return Mathf.Clamp(_baseValue, statMin, statMax);
		}
		set
		{
			if (value < statMin)
				notifyDie();
			else
				_baseValue = Mathf.Clamp(value, statMin, statMax);
			
		}
	}
	public StatHealth(float max, float min, NotifyDie die)
	{
		statMax = max; // healthmax 초기값
		statMin = min; // healthmin 초기값 ==...0.0f;;
		notifyDie = die;
	}
	private void ClampHealth(float healthMax)
	{
		statMax = healthMax;
		if (baseValue >= healthMax)
			baseValue = healthMax;
	}
	public StatHealthMax.NotifyHealthMaxChanged GetClampFunc()
	{
		return ClampHealth;
	}
	
}
