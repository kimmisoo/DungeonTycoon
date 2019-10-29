using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatMod {

	

	public float modValue
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
	private float _modValue = 0.0f;
}
