using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
public enum Race
{
	Human, Elf, Dwarf, Orc, Dog, Cat
}
public enum Wealth
{
	Upper, Middle, Lower
}
public enum Desire
{
	Thirsty, Hungry, Sleep, Tour, Fun, Convenience, Equipment, Health
}*/

public class CommonStat {

	public CommonStat(int _id, Race _race, Wealth _wealth, string _name, string _explanation, int _gender, int _gold,
		float _thirstyTick, float _hungryTick, float _sleepTick, float _tourTick, float _funTick, float _convenienceTick, float _equipmentTick,
		float _tickAllMult, float _tickTime, Traveler _owner)
	{
		id = _id;
		race = _race;
		wealth = _wealth;
		name = _name;
		explanation = _explanation;
		gender = _gender;
		gold = _gold;

		thirsty = 0.0f;
		hungry = 0.0f;
		sleep = 0.0f;
		tour = 0.0f;
		fun = 0.0f;
		convenience = 0.0f;
		equipment = 0.0f;
		health = 0.0f;
		///desire

		thirstyTick = _thirstyTick;
		hungryTick = _hungryTick;
		sleepTick = _sleepTick;
		tourTick = _tourTick;
		funTick = _funTick;
		convenienceTick = _convenienceTick;
		equipmentTick = _equipmentTick;
		///desireTick

		tickAllMult = _tickAllMult;
		tickTime = _tickTime;
		owner = _owner;
	}

	Traveler owner;

	public int id
	{
		get;set;
	}
	Race race
	{
		get;set;
	}
	Wealth wealth
	{
		get;set;
	}
	string name
	{
		get;set;
	}
	string explanation
	{
		get;set;
	}
	int gender
	{
		get;set;
	}// 0 - male, 1 - female;
	int gold
	{
		get;set;
	}


	private const float maxDesire = 100.0f;
	private const float minDesire = 0.0f;

	#region desires
	public float thirsty
	{
		get
		{
			return thirsty;
		}
		set
		{
			thirsty = Mathf.Clamp(value, minDesire, maxDesire);
		}
	}
	public float hungry
	{
		get
		{
			return hungry;
		}
		set
		{
			thirsty = Mathf.Clamp(value, minDesire, maxDesire);
		}
	}
	public float sleep
	{
		get
		{
			return sleep;
		}
		set
		{
			sleep = Mathf.Clamp(value, minDesire, maxDesire);
		}
	}
	public float equipment
	{
		get
		{
			return equipment;
		}
		set
		{
			equipment = Mathf.Clamp(value, minDesire, maxDesire);
		}
	}
	public float tour
	{
		get
		{
			return tour;
		}
		set
		{
			tour = Mathf.Clamp(value, minDesire, maxDesire);
		}
	}
	public float fun
	{
		get
		{
			return fun;
		}
		set
		{
			fun = Mathf.Clamp(value, minDesire, maxDesire);
		}
	}
	public float convenience
	{
		get
		{
			return convenience;
		}
		set
		{
			convenience = Mathf.Clamp(value, minDesire, maxDesire);
		}
	}
	public float health
	{
		get
		{
			return health;
		}
		set
		{
			health = Mathf.Clamp(value, minDesire, maxDesire);
		}
	}

	public float thirstyTick
	{
		get
		{
			return thirstyTick;
		}
		set
		{
			thirstyTick = Mathf.Clamp(value, minDesire, maxDesire);
		}
	}
	public float hungryTick
	{
		get
		{
			return hungryTick;
		}
		set
		{
			hungryTick = Mathf.Clamp(value, minDesire, maxDesire);
		}
	}
	public float sleepTick
	{
		get
		{
			return sleepTick;
		}
		set
		{
			sleepTick = Mathf.Clamp(value, minDesire, maxDesire);
		}
	}
	//private float equipmentTick;
	public float tourTick
	{
		get
		{
			return tourTick;
		}
		set
		{
			tourTick = Mathf.Clamp(value, minDesire, maxDesire);
		}
	}
	public float funTick
	{
		get
		{
			return funTick;
		}
		set
		{
			funTick = Mathf.Clamp(value, minDesire, maxDesire);
		}
	}
	public float convenienceTick
	{
		get
		{
			return convenienceTick;
		}
		set
		{
			convenienceTick = Mathf.Clamp(value, minDesire, maxDesire);
		}
	}
	public float equipmentTick
	{
		get
		{
			return equipmentTick;
		}
		set
		{
			equipmentTick = Mathf.Clamp(value, minDesire, maxDesire);
		}
	}
	

	public float tickAllMult
	{
		get
		{
			return tickAllMult;
		}
		set
		{
			tickAllMult = Mathf.Clamp(value, minDesire, maxDesire);
		}

	}
	public float tickTime
	{
		get; set;
	}
	#endregion

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
		//health = (1.0f - (owner.battleStatus.GetCurrentHealth() / owner.battleStatus.GetCalculatedHealthMax())) * 100.0f;
	}


}
