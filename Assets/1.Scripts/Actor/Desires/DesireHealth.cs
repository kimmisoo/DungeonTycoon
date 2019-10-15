using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DesireHealth : DesireBase {


	public override IEnumerator Tick()
	{
		if (!(owner is Adventurer))
			yield break;
		while (true)
		{
			yield return tickBetweenWait;
			desireValue = owner.stat.GetCurrentHealth() / owner.stat.GetHealthMax() * 100.0f;
		}

	}
}
