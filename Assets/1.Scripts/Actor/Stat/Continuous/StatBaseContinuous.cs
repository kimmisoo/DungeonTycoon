using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatBaseContinuous {
	

	protected List<StatModContinuous> modList = new List<StatModContinuous>();

	public virtual float baseValue
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
			if(mod.type == ModType.Fixed)
			{
				valueFixed += mod.modValue;
			} // Fixed 합
			else
			{
				valueMult += mod.modValue;
			} // Mult 합
		}
		return (baseValue + valueFixed) * valueMult;
	}
	
	public virtual void AddStatMod(StatModContinuous mod)
	{
		modList.Add(mod);
	}
	public virtual void RemoveStatMod(StatModContinuous mod)
	{
		modList.Remove(mod);
	}
	
	

}
