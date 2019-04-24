using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enchantment_16 : Enchantment {

	//멋짐
	//전투시작시 적 4초간 스턴

	public override void OnStartBattle(Character user, Monster target, Monster[] targets)
	{
		target.TakeStunned(user, this, 4.0f);
	}
}
