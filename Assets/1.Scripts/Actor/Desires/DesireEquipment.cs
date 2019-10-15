using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DesireEquipment : DesireBase {

	public override IEnumerator Tick()
	{
		if (!(owner is Adventurer))
			yield break;
		else
			yield return base.Tick();
			
		
	}
}
