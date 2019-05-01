using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enchantment_32 : Enchantment {

	//데미지흡수
	//데미지 받을때마다 체력 15 회복

	public override void OnDamaged(Actor user, Actor target, Actor[] targets, float damage, bool isCritical)
	{
		if(user.GetCalculatedHealthMax() - user.GetCurrentHealth() > 15.0f)
		{
			user.TakeHealFromEnchantment(15.0f, user, this);
		}
	}
}
