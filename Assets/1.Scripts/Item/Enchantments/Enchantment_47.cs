using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enchantment_47 : Enchantment {

	//용암
	//매 공격시 26 추가데미지

	public override void OnDamage(Actor user, Actor target, Actor[] targets, float damage, bool isCritical)
	{
		target.TakeDamageFromEnchantment(26.0f, user, this, false, out isHit, out isDead);
	}

}
