using UnityEngine;
using System.Collections;

public class Desire : MonoBehaviour{

	private const float maxDesire = 100.0f;
	private const float minDesire = 0.0f;
	private float thirsty;
	private float hungry;
	private float sleep;
	private float equipment;
	private float tour;
	private float fun;
	private float convenience;
	private float health;

	private float thirstyTick;
	private float hungryTick;
	private float sleepTick;
	//private float equipmentTick;
	private float tourTick;
	private float funTick;
	private float convenienceTick;
	//private float healthTick;

	private float tickMult = 1.0f;
	private int tickTime = 10;
	private Coroutine tickCoroutine;

	public float GetThirsty()
	{
		return thirsty;
	}
	public float GetHungry()
	{
		return hungry;
	}
	public float GetSleep()
	{
		return sleep;
	}
	public float GetEquipment()
	{
		return equipment;
	}
	public float GetTour()
	{
		return tour;
	}
	public float GetFun()
	{
		return fun;
	}
	public float GetConvenience()
	{
		return convenience;
	}
	public float GetHealth()
	{
		return health;
	}


	public void SetThirsty(float _thirsty)
	{		
		thirsty = _thirsty;
		if (thirsty > maxDesire)
			thirsty = maxDesire;
		if (thirsty < minDesire)
			thirsty = minDesire;
	}
	public void SetHungry(float _hungry)
	{
		hungry = _hungry;
		if (hungry > maxDesire)
			hungry = maxDesire;
		if (hungry < minDesire)
			hungry = minDesire;
	}
	public void SetSleep(float _sleep)
	{
		sleep = _sleep;
		if (sleep > maxDesire)
			sleep = maxDesire;
		if (sleep < minDesire)
			sleep = minDesire;
	}
	public void SetEquipment(float _equipment)
	{
		equipment = _equipment;
		if (equipment > maxDesire)
			equipment = maxDesire;
		if (equipment < minDesire)
			equipment = minDesire;
	}
	public void SetTour(float _tour)
	{
		tour = _tour;
		if (tour > maxDesire)
			tour = maxDesire;
		if (tour < minDesire)
			tour = minDesire;
	}
	public void SetFun(float _fun)
	{
		fun = _fun;
		if (fun > maxDesire)
			fun = maxDesire;
		if (fun < minDesire)
			fun = minDesire;
	}
	public void SetConvenience(float _convenience)
	{
		convenience = _convenience;
		if (convenience > maxDesire)
			convenience = maxDesire;
		if (convenience < minDesire)
			convenience = minDesire;
	}
	public void SetHealth(float _health)
	{
		health = _health;
		if (health > maxDesire)
			health = maxDesire;
		if (health < minDesire)
			health = minDesire;
	}



	public void AddThirsty(float _thirstyAmount)
	{
		thirsty += _thirstyAmount;
		if (thirsty > maxDesire)
			thirsty = maxDesire;
		if (thirsty < minDesire)
			thirsty = minDesire;

	}
	public void AddHungry(float _hungryAmount)
	{
		hungry += _hungryAmount;
		if (hungry > maxDesire)
			hungry = maxDesire;
		if (hungry < minDesire)
			hungry = minDesire;
	}
	public void AddSleep(float _sleepAmount)
	{
		sleep += _sleepAmount;
		if (sleep > maxDesire)
			sleep = maxDesire;
		if (sleep < minDesire)
			sleep = minDesire;
	}
	public void AddEquipment(float _equipmentAmount)
	{
		equipment += _equipmentAmount;
		if (equipment > maxDesire)
			equipment = maxDesire;
		if (equipment < minDesire)
			equipment = minDesire;
	}
	public void AddTour(float _tourAmount)
	{
		tour += _tourAmount;
		if (tour > maxDesire)
			tour = maxDesire;
		if (tour < minDesire)
			tour = minDesire;
	}
	public void AddFun(float _funAmount)
	{
		fun += _funAmount;
		if (fun > maxDesire)
			fun = maxDesire;
		if (fun < minDesire)
			fun = minDesire;
	}
	public void AddConvenience(float _convenienceAmount)
	{
		convenience += _convenienceAmount;
		if (convenience > maxDesire)
			convenience = maxDesire;
		if (convenience < minDesire)
			convenience = minDesire;
	}
	public void AddHealth(float _healthAmount)
	{
		health += _healthAmount;
		if (health > maxDesire)
			health = maxDesire;
		if (health < minDesire)
			health = minDesire;
	}

	public float GetThirstyTick()
	{
		return thirstyTick;
	}
	public float GetHungryTick()
	{
		return hungryTick;
	}
	public float GetSleepTick()
	{
		return sleepTick;
	}
	/*
	public float GetEquipmentTick()
	{
		return EquipmentTick;
	}*/
	public float GetTourTick()
	{
		return tourTick;
	}
	public float GetFunTick()
	{
		return funTick;
	}
	public float GetConvenienceTick()
	{
		return convenienceTick;
	}
	/*
	public float GetHealthTick()
	{
		return HealthTick;
	}*/

	public void SetThirstyTick(float _thirstyTick)
	{
		thirstyTick = _thirstyTick;
		if (thirstyTick > maxDesire)
			thirstyTick = maxDesire;
		if (thirstyTick < minDesire)
			thirstyTick = minDesire;
	}
	public void SetHungryTick(float _hungryTick)
	{
		hungryTick = _hungryTick;
		if (hungryTick > maxDesire)
			hungryTick = maxDesire;
		if (hungryTick < minDesire)
			hungryTick = minDesire;
	}
	public void SetSleepTick(float _sleepTick)
	{
		sleepTick = _sleepTick;
		if (sleepTick > maxDesire)
			sleepTick = maxDesire;
		if (sleepTick < minDesire)
			sleepTick = minDesire;
	}
	public void SetTourTick(float _tourTick)
	{
		tourTick = _tourTick;
		if (tourTick > maxDesire)
			tourTick = maxDesire;
		if (tourTick < minDesire)
			tourTick = minDesire;
	}
	public void SetFunTick(float _funTick)
	{
		funTick = _funTick;
		if (funTick > maxDesire)
			funTick = maxDesire;
		if (funTick < minDesire)
			funTick = minDesire;
	}
	public void SetConvenienceTick(float _convenienceTick)
	{
		convenienceTick = _convenienceTick;
		if (convenienceTick > maxDesire)
			convenienceTick = maxDesire;
		if (convenienceTick < minDesire)
			convenienceTick = minDesire;
	}

	public float GetTickMult()
	{
		return tickMult;
	}
	public int GetTickTime()
	{
		return tickTime;
	}
	public void SetTickMult(float _tickMult)
	{
		tickMult = _tickMult;
	}
	public void SetTickTime(int _tickTime)
	{
		tickTime = _tickTime;
	}
	

	public void StartDesireTick()
	{

		tickCoroutine = StartCoroutine(Ticking());
	}
	IEnumerator Ticking()
	{
		int tickTimeOrigin = tickTime;
		WaitForSeconds wait = new WaitForSeconds(tickTime);
		
		while (true)
		{
			yield return wait;
			TickDesire();
			if (tickTimeOrigin != tickTime)
			{
				tickTimeOrigin = tickTime;
				wait = new WaitForSeconds(tickTime);
			}
		}
	}
	public void StopDesireTick()
	{
		StopCoroutine(tickCoroutine);
	}
	public void TickDesire()
	{
		AddThirsty(thirstyTick * tickMult);
		AddHungry(hungryTick * tickMult);
		AddSleep(sleepTick * tickMult);
		AddTour(tourTick * tickMult);
		AddFun(funTick * tickMult);
		AddConvenience(convenienceTick * tickMult);
	}
}

