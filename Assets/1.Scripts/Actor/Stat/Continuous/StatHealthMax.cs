using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatHealthMax : StatBaseContinuous
{
	public delegate void NotifyHealthMaxChanged(float healthMax);
	NotifyHealthMaxChanged notifyHealthMaxChanged;
	private readonly float statMax;
	private readonly float statMin;
	public const StatType type = StatType.HealthMax;
	public override float BaseValue
	{
		get
		{
			return Mathf.Clamp(_baseValue, statMin, statMax);
		}
		set
		{
			_baseValue = Mathf.Clamp(value, statMin, statMax);
			notifyHealthMaxChanged(GetCalculatedValue());
		}
	}
	public StatHealthMax(float max, float min, NotifyHealthMaxChanged notiMax) // StatHealth.ClampHealth 전달~~
	{
		statMax = max;
		statMin = min;
		notifyHealthMaxChanged = notiMax;
	}
	public override void AddStatMod(StatModContinuous mod)
	{
		base.AddStatMod(mod);
		notifyHealthMaxChanged(GetCalculatedValue());
	}
	public override void RemoveStatMod(StatModContinuous mod)
	{
		base.RemoveStatMod(mod);
		notifyHealthMaxChanged(GetCalculatedValue());
	}
}
