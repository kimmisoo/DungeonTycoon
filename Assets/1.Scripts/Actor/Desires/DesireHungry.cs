using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DesireHungry : DesireBase {

	public override IEnumerator Tick()
	{
		yield return base.Tick();
	}
	public DesireHungry(DesireType name, float initDesireValue, float initTickAmount, float initTickMult, float initTickBetween, Traveler _owner)
		: base(name, initDesireValue, initTickAmount, initTickMult, initTickBetween, _owner)
	{

	}
}
