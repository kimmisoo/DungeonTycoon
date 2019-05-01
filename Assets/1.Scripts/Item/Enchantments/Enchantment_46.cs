using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enchantment_46 : Enchantment {

	//불꽃
	//매 공격시 12 추가데미지

	public override void OnDamage(Character user, Monster target, Monster[] targets, float damage, bool isCritical)
	{
		target.TakeDamageFromEnchantment(12.0f, user, this, false, out isHit, out isDead);
	}
}
