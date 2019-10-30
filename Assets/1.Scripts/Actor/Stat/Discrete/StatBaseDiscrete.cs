using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatBaseDiscrete
{

	private readonly int statMax;
	private readonly int statMin;
	private int recentCalculatedValue = 0;
	private List<StatModDiscrete> modList = new List<StatModDiscrete>();

	public virtual int baseValue
	{
		get
		{

			return (int)Mathf.Clamp(_baseValue, statMin, statMax);
		}
		set
		{
			_baseValue = (int)Mathf.Clamp(value, statMin, statMax);
		}
	}
	protected int _baseValue = 0;

	public int GetCalculatedValue()
	{
		int valueFixed = 0;
		int valueMult = 1;
		foreach (StatModDiscrete mod in modList)
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

		return (baseValue * valueMult) + valueFixed;
	}
	


}
