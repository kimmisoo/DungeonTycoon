using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DesireConvenience : DesireBase {

	public DesireConvenience(DesireType name, float initDesireValue, float initTickAmount, float initTickMult, float initTickBetween, Traveler _owner) 
		: base(name, initDesireValue, initTickAmount, initTickMult, initTickBetween, _owner)
	{

	}
}
