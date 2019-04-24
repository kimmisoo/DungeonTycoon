using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enchantment_3 : Enchantment {
	//휩쓸기
	//공격범위 내 모든 적에게 25%데미지로 공격
	// Use this for initialization
	
	public override void OnAttack(Character user, Monster target, Monster[] targets, bool isCritical)
	{
		foreach (Actor a in targets)
		{
			//if 공격범위 내 OnAttack시 계산.
			a.TakeDamageFromEnchantment(user.GetCalculatedAttack() * 0.25f, user, this, false, out isHit, out isDead);
		}
	}
}
