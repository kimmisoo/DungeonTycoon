using UnityEngine;
using System.Collections;

public class Preference {

	/*
    public float adventurer
    { get; set; }
    public float tourist
    { get; set; }
    public float human
    { get; set; }
    public float elf
    { get; set; }
    public float dwarf
    { get; set; }
    public float orc
    { get; set; }
    public float dog
    { get; set; }
    public float cat
    { get; set; }
    public float upperclass
    { get; set; }
    public float middleclass
    { get; set; }
    public float lowerclass
    { get; set; }
	*/

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

}
