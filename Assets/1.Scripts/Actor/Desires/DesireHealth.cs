using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DesireHealth : DesireBase {


	public override IEnumerator Tick()
	{
		if (!(owner is Adventurer) || !(owner is SpecialAdventurer))
			yield break;
		while (true)
		{
			yield return tickBetweenWait;
			//desireValue = owner.stat.GetCurrentHealth() / owner.stat.GetHealthMax() * 100.0f;
			//desireValue = (owner as Adventurer).battleStat.GetCurrentHealth / 
			desireValue = ((owner as Adventurer).GetBattleStat().MissingHealth / (owner as Adventurer).GetBattleStat().HealthMax) * 100.0f;
		}

	}
	public DesireHealth(DesireType name, float initDesireValue, float initTickAmount, float initTickMult, float initTickBetween, Traveler _owner)
		: base(name, initDesireValue, initTickAmount, initTickMult, initTickBetween, _owner)
	{

	}
}
