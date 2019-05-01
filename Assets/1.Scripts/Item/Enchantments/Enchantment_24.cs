using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enchantment_24 : Enchantment {

	//분신
	//치명타시 50% 공격력의 추가타

	public override void OnDamage(Character user, Monster target, Monster[] targets, float damage, bool isCritical)
	{
		if(isCritical == true)
		{
			target.TakeDamageFromEnchantment(damage * 0.5f, user, this, false, out isHit, out isDead);
		}
	}
}
