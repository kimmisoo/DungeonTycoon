using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enchantment_25 : Enchantment {

	//재생
	//매 초마다 잃은체력의 1.5% 회복

	Coroutine regenerate;
	WaitForSeconds interval = new WaitForSeconds(1.0f);

	public override void OnEquip(Character user)
	{
		regenerate = StartCoroutine(Regenerate(user));
	}

	public override void OnUnequip(Character user)
	{
		StopCoroutine(regenerate);
	}

	IEnumerator Regenerate(Character user)
	{
		while(true)
		{
			yield return interval;
			user.TakeHealFromEnchantment((user.GetCalculatedHealthMax() - user.health) * 0.015f, user, this);
		}
	}
}
