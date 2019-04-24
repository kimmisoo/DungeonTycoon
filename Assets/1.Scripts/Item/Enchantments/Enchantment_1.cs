using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enchantment_1 : Enchantment {
	//정전기발생
	//매 공격 시 +4 추가공격
	//치명타 적용안됨
	public override void OnDamage(Character user, Monster target, Monster[] targets, bool isCritical)
	{
		//target.Take
		target.TakeDamageFromEnchantment(4, 0, user, this);
	}

}
