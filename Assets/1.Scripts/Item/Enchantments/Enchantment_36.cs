using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enchantment_36 : Enchantment {

	//흡혈
	//12% 흡혈

	public override void OnDamage(Actor user, Actor target, Actor[] targets, float damage, bool isCritical)
	{
		user.TakeHealFromEnchantment(damage * 0.12f, user, this);
	}
}
