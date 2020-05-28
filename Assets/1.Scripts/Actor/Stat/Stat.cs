using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public enum Gender { Male, Female }

public class Stat{
	
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
	public Gender gender
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
	WaitForSeconds tickBetween;
    public Stat()
    {
        desireDict = new Dictionary<DesireType, DesireBase>();
    }

    public Stat(Stat inputStat, Traveler owner)
    {
        id = inputStat.id;
        race = inputStat.race;
        wealth = inputStat.wealth;
        name = inputStat.name;
        explanation = inputStat.explanation;
        gender = inputStat.gender;
        gold = inputStat.gold;
        desireDict = new Dictionary<DesireType, DesireBase>(inputStat.GetDesireDict());
        
        this.owner = owner;
        for (int i = 0; i < desireDict.Count; i++)
        {
            desireDict[desireDict.Keys.ToArray()[i]].SetOwner(owner);
        }
        //StartDesireTick();
    }

	#region Desire Tick메소드
	//Tick은 DesireBase에 Tick으로 Traveler에서 호출.
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

    public Dictionary<DesireType, DesireBase> GetDesireDict()
    {
        return desireDict;
    }

    public void AddDesire(DesireBase input)
    {
        desireDict.Add(input.desireName, new DesireBase(input));
    }
}
