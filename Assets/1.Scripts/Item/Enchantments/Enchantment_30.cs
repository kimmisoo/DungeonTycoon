using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enchantment_30 : Enchantment {

	//충격파 생성
	//매 공격시 주위 1칸범위 데미지 10


	public override void OnDamage(Actor user, Actor target, Actor[] targets, float damage, bool isCritical)
	{
		foreach(Actor a in target.GetAdjacentActor(1))
		{
			if(a is Monster)
			{
				a.TakeDamageFromEnchantment(10.0f, user, this, false, out isHit, out isDead);
			}
		}
	}
}
