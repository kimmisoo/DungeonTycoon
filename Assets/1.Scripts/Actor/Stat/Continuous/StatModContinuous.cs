using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatModContinuous
{
    public StatModContinuous(ModType typeIn, float value)
    {
        Type = typeIn;
        ModValue = value;
    }
	public float ModValue
	{
		get
		{
			return _modValue;
		}
		set
		{
			_modValue = value; 
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
			_type = value;
		}
	}

	ModType _type = ModType.Fixed;
	private float _modValue = 0.0f;
}
