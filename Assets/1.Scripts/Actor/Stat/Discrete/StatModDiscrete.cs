using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatModDiscrete
{
    public StatModDiscrete(ModType typeIn, int value)
    {
        Type = typeIn;
        ModValue = value;
    }

    public int ModValue
	{
		get
		{
			return _modValue;
		}
		set
		{

		}
	}
	public ModType Type
	{
		get
		{
			return _type;
		}
		set
		{

		}
	}

	ModType _type = ModType.Fixed;
	private int _modValue = 0;
}
