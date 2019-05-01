using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enchantment_21 : Enchantment {

	//가시갑옷
	//데미지 15% 반사

	
	public override void OnDamaged(Character user, Monster target, Monster[] targets, float damage, bool isCritical)
	{
		target.TakeDamageFromEnchantment(damage * 0.15f, user, this, false, out isHit, out isDead);
	}

}
