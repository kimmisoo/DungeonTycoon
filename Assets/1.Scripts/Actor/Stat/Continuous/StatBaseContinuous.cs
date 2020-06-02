using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatBaseContinuous
{
	protected List<StatModContinuous> modList = new List<StatModContinuous>();
    const float MULT_MIN = 0.1f; // 최소값

	public virtual float BaseValue
	{
		get
		{
			return Mathf.Clamp(_baseValue, 0.0f, Mathf.Infinity);
		}
		set
		{
			_baseValue = Mathf.Clamp(value, 0.0f, Mathf.Infinity);
		}
	}
	protected float _baseValue = 0.0f;
	
	public virtual float GetCalculatedValue()
	{
		float valueFixed = 0.0f;
		float valueMult = 1.0f;
		foreach(StatModContinuous mod in modList)
		{
			if(mod.ModType == ModType.Fixed)
			{
				valueFixed += mod.ModValue;
			} // Fixed 합
			else
			{
				valueMult += mod.ModValue;
			} // Mult 합
		}
        if (valueMult < MULT_MIN)
            valueMult = MULT_MIN;
		return (BaseValue + valueFixed) * valueMult;
	}
	
	public virtual void AddStatMod(StatModContinuous mod)
	{
		modList.Add(mod);
	}
	public virtual void RemoveStatMod(StatModContinuous mod)
	{
		modList.Remove(mod);
	}
	public virtual void ClearStatModList()
    {
        modList.Clear();
    }
    public List<StatModContinuous> GetModList()
    {
        return modList;
    }
}
