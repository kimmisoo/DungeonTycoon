﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enchantment_38 : Enchantment {

	//증폭
	//공격시 주위 1칸 범위에 15% 데미지 추가공격

	public override void OnDamage(Character user, Monster target, Monster[] targets, bool isCritical)
	{
		foreach(Actor a in target.GetAdjacentActor(1))
		{
			if(a is Monster)
			{
				a.TakeDamageFromEnchantment(user.GetCalculatedAttack() * 0.15f, user.GetCalculatedPenetration(), user, this);
			}
		}
	}
}
