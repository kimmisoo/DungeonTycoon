using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enchantment_32 : Enchantment {

	//데미지흡수
	//데미지 받을때마다 체력 15 회복

	public override void OnDamaged(Character user, Monster target, Monster[] targets, float damage)
	{
		if(user.GetCalculatedHealthMax() > user.health - 15.0f)
		{
			user.TakeHealFromEnchantment(15.0f, user, this);
		}
	}

}
