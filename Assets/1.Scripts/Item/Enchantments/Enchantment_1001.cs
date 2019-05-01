using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enchantment_1001 : Enchantment {

	//받은 데미지 10%를 반사
	public override void OnDamaged(Actor user, Actor target, Actor[] targets, float damage, bool isCritical)
	{
		user.TakeDamageFromEnchantment(damage * 0.1f, user, this, false, out isHit, out isDead);
	}
}
