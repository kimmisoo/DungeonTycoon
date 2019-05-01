using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enchantment_50 : Enchantment {

	//파이어볼
	//공격시 7% 확률로 Range 1 범위에 80 데미지.
	int rand = 0;

	public override void OnAttack(Actor user, Actor target, Actor[] targets, bool isCritical)
	{
		rand = UnityEngine.Random.Range(0, 100);
		if(rand < 7)
		{
			foreach(Actor a in target.GetAdjacentActor(1))
			{
				a.TakeDamageFromEnchantment(80.0f, user, this, false, out isHit, out isDead);
			}
		}
	}
}
