using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatBase {

	private const float statMax = Mathf.Infinity;
	private const float statMin = 0.0f;
	private float recentCalculatedValue = 0.0f;
	private List<StatMod> modList = new List<StatMod>();

	public float statValue
	{
		get
		{

			return Mathf.Clamp(_statValue, statMin, statMax);
		}
		set
		{
			_statValue = Mathf.Clamp(value, statMin, statMax);
		}
	}
	private float _statValue = 0.0f;
	
	public float GetCalculatedValue()
	{
		foreach(StatMod mod in modList)
		{

		}
	}

}
