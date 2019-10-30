using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatModDiscrete
{



	public int modValue
	{
		get
		{
			return _modValue;
		}
		set
		{

		}
	}
	public ModType type
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
