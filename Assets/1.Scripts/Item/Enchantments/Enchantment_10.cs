using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enchantment_10 : Enchantment {
	//축복
	//입힌데미지의 15%만큼 보호막 생성
	//쿨타임은?

	public override void OnDamage(Character user, Monster target, Monster[] targets, bool isCritical)
	{
		user.enchantmentShieldMax += target.GetCalculatedDamage(user.GetCalculatedAttack(), user.GetCalculatedPenetration(), user);
	}

}
