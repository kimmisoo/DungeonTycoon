using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatBaseDiscrete
{

	private readonly int statMax = 100;
	private readonly int statMin = 0;
	private int recentCalculatedValue = 0;
	private List<StatModDiscrete> modList = new List<StatModDiscrete>();

	public virtual int BaseValue
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

			if (mod.Type == ModType.Fixed)
			{
				valueFixed += mod.ModValue;
			} // Fixed 합
			else
			{
				valueMult += mod.ModValue;
			} // Mult 합
			
		}

		return (BaseValue * valueMult) + valueFixed;
	}

    public virtual void AddStatMod(StatModDiscrete mod)
    {
        modList.Add(mod);
    }
    public virtual void RemoveStatMod(StatModDiscrete mod)
    {
        modList.Remove(mod);
    }
    public void ClearStatModList()
    {
        modList.Clear();
    }
}
