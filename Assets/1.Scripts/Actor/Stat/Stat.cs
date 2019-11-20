using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public class Stat : MonoBehaviour{
	
	#region CommonStat
	public int id
	{
		get; set;
	}
	public RaceType race
	{
		get; set;
	}
	public WealthType wealth
	{
		get; set;
	}
	public JobType job
	{
		get; set;
	}
	public string name
	{
		get; set;
	}
	public string explanation
	{
		get; set;
	}
	public int gender
	{
		get; set;
	}// 0 - male, 1 - female;
	public int gold
	{
		get; set;
	}
	#endregion

	Dictionary<DesireType, DesireBase> desireDict;
	Traveler owner;

	public void Init(int _id, RaceType _race, WealthType _wealth, string _name, string _explanation, int _gender, int _gold, Dictionary<DesireType, DesireBase> _desireDict, Traveler _owner)
	{
		id = _id;
		race = _race;
		wealth = _wealth;
		name = _name;
		explanation = _explanation;
		gender = _gender;
		gold = _gold;
		desireDict = _desireDict;
		owner = _owner;
		StartDesireTick();
	}
	#region Desire Tick메소드
	public void StartDesireTick()
	{
		foreach(KeyValuePair<DesireType, DesireBase> kvp in desireDict)
		{
			StartCoroutine(kvp.Value.Tick());
		}
	}
	public void StopDesireTick()
	{
		StopAllCoroutines();
	}

	/*
	IEnumerator Ticking()
	{
		float tickTimeOrigin = tickTime;
		float eps = 0.0001f;
		WaitForSeconds wait = new WaitForSeconds(tickTime);

		while (owner.GetState() != State.Dead)
		{
			yield return wait;
			TickDesire();
			if (Mathf.Abs(tickTimeOrigin - tickTime) > eps)
			{
				tickTimeOrigin = tickTime;
				wait = new WaitForSeconds(tickTimeOrigin);
			}
		}
	}

	public void TickDesire()
	{

		thirsty += (thirstyTick * tickAllMult);
		hungry += (hungryTick * tickAllMult);
		sleep += (sleepTick * tickAllMult);
		tour += (tourTick * tickAllMult);
		fun += (funTick * tickAllMult);
		convenience += (convenienceTick * tickAllMult);
		equipment += (equipmentTick * tickAllMult);
		health = (1.0f - (owner.stat.GetCurrentHealth() / owner.stat.GetCalculatedHealthMax())) * 100.0f;
	}*/

	#endregion

	public DesireType GetHighestDesire()
	{
		//thirsty, hungry, sleep, tour, fun, convenience, equipment, health....
		float maxVal = 0.0f;
		DesireType max = DesireType.Base;
		foreach (KeyValuePair<DesireType, DesireBase> kvp in desireDict)
		{
			if (maxVal <= kvp.Value.desireValue)
			{
				maxVal = kvp.Value.desireValue;
				max = kvp.Key;
			}
		}
		return max;
	}
	public DesireBase GetSpecificDesire(DesireType desireType)
	{
		return desireDict[desireType];
	}

	
	

}
