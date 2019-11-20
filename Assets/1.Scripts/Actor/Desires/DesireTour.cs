using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DesireTour : DesireBase {

	public override IEnumerator Tick()
	{
		yield return base.Tick();
	}
	public DesireTour(DesireType name, float initDesireValue, float initTickAmount, float initTickMult, float initTickBetween, Traveler _owner)
		: base(name, initDesireValue, initTickAmount, initTickMult, initTickBetween, _owner)
	{

	}
}
