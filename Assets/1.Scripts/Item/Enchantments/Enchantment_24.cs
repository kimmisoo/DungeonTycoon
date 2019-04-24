using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enchantment_24 : Enchantment {

	//분신
	//치명타시 50% 공격력의 추가타

	public override void OnDamage(Character user, Monster target, Monster[] targets, bool isCritical)
	{
		if(isCritical == true)
		{
			target.TakeDamageFromEnchantment(user.GetCalculatedAttack() * 0.5f * (1 + user.GetCalculatedCriticalDamage()), user.GetCalculatedPenetration(), user, this);
		}
	}

}
