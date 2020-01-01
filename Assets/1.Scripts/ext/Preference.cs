using UnityEngine;
using System.Collections;

public class Preference {

	//직업
	private float prefAdventurer;
	private float prefTourist;
	
	//종족
	private float prefHuman;
	private float prefElf;
	private float prefDwarf;
	private float prefOrc;
	private float prefDog;
	private float prefCat;

	//계층
	private float prefUpperclass;
	private float prefMiddleclass;
	private float prefLowerclass;

	public void SetPrefAdventurer(float _prefAdventurer)
	{
		prefAdventurer = _prefAdventurer;
	}
	public void SetPrefTourist(float _prefTourist)
	{
		prefTourist = _prefTourist;
	}
	public void SetPrefHuman(float _prefHuman)
	{
		prefHuman = _prefHuman;
	}
	public void SetPrefElf(float _prefElf)
	{
		prefElf = _prefElf;
	}
	public void SetPrefDwarf(float _prefDwarf)
	{
		prefDwarf = _prefDwarf;
	}
	public void SetPrefOrc(float _prefOrc)
	{
		prefOrc = _prefOrc;
	}
	public void SetPrefDog(float _prefDog)
	{
		prefDog = _prefDog;
	}
	public void SetPrefCat(float _prefCat)
	{
		prefCat = _prefCat;
	}
	public void SetPrefUpperclass(float _prefUpperclass)
	{
		prefUpperclass = _prefUpperclass;
	}
	public void SetPrefMiddleclass(float _prefMiddleclass)
	{
		prefMiddleclass = _prefMiddleclass;
	}
	public void SetPrefLowerclass(float _prefLowerclass)
	{
		prefLowerclass = _prefLowerclass;
	}
	public float GetPrefAdventurer()
	{
		return prefAdventurer;
	}
	public float GetPrefTourist()
	{
		return prefTourist;
	}
	public float GetPrefHuman()
	{
		return prefHuman;
	}
	public float GetPrefElf()
	{
		return prefElf;
	}
	public float GetPrefDwarf()
	{
		return prefDwarf;
	}
	public float GetPrefOrc()
	{
		return prefOrc;
	}
	public float GetPrefDog()
	{
		return prefDog;
	}
	public float GetPrefCat()
	{
		return prefCat;
	}
	public float GetPrefUpperclass()
	{
		return prefUpperclass;
	}
	public float GetPrefMiddleclass()
	{
		return prefMiddleclass;
	}
	public float GetPrefLowerclass()
	{
		return prefLowerclass;
	}
	public float GetPrefSum(RaceType race, WealthType wealth, JobType job)
	{
		float sum = 0.0f;
		switch (race)
		{
			case RaceType.Cat:
				sum += GetPrefCat();
				break;
			case RaceType.Dog:
				sum += GetPrefDog();
				break;
			case RaceType.Dwarf:
				sum += GetPrefDwarf();
				break;
			case RaceType.Elf:
				sum += GetPrefElf();
				break;
			case RaceType.Human:
				sum += GetPrefHuman();
				break;
			case RaceType.Orc:
				sum += GetPrefOrc();
				break;
		}

		switch (wealth)
		{
			case WealthType.Lower:
				sum += GetPrefLowerclass();
				break;
			case WealthType.Middle:
				sum += GetPrefMiddleclass();
				break;
			case WealthType.Upper:
				sum += GetPrefUpperclass();
				break;
		}

		switch (job)
		{
			case JobType.Traveler:
				sum += GetPrefTourist();
				break;
			case JobType.Adventurer:
				sum += GetPrefAdventurer();
				break;
			case JobType.SpecialAdventurer:
				sum += GetPrefAdventurer();
				break;
		}
		return sum;
	}
} 
