using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DesireSleep : DesireBase {

	public override IEnumerator Tick()
	{
		yield return base.Tick();
	}
}
