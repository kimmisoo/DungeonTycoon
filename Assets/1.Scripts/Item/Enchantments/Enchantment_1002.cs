using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enchantment_1002 : Enchantment {

	//받은 데미지 5%를 반사

	public override void OnDamaged(Actor user, Actor target, Actor[] targets, float damage, bool isCritical)
	{
		user.TakeDamageFromEnchantment(damage * 0.05f, user, this, false, out isHit, out isDead);
	}
}
