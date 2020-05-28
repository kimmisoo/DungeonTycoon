using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DesireEquipment : DesireBase {

	public override IEnumerator Tick()
	{
		if (!(owner is Adventurer))
			yield break;
		else // 레벨 x 시간 지나면 차도록...
			yield return base.Tick();
		
	}
	public DesireEquipment(DesireType name, float initDesireValue, float initTickAmount, float initTickMult, float initTickBetween, Traveler _owner)
		: base(name, initDesireValue, initTickAmount, initTickMult, initTickBetween, _owner)
	{

	}
}
