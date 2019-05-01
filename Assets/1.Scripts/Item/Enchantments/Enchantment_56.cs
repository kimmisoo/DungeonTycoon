using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enchantment_56 : Enchantment {

	//생명력 전환
	//체력이 80%까지만 회복되며 
	//원래 체력의 20%의 15% 만큼 추가 공격력

	EquipmentEffect tempEffect;
	Coroutine healthCheck;
	WaitForSeconds interval = new WaitForSeconds(3.0f);
	public override void OnEquip(Actor user)
	{
		tempEffect = new EquipmentEffect(this, user);
		tempEffect.healthMaxMult -= 0.2f;
		tempEffect.attack = user.GetCalculatedHealthMax() * 0.2f * 0.15f;
		user.AddEquipmentEffect(tempEffect);
		healthCheck = StartCoroutine(HealthCheck(user));
	}
	public override void OnUnequip(Actor user)
	{
		StopCoroutine(healthCheck);
		user.RemoveAllEquipmentEffectByParent(this);

	}
	IEnumerator HealthCheck(Actor user)
	{
		while(true)
		{
			yield return interval;
			tempEffect.attack = user.GetCalculatedHealthMax() * 0.2f * 0.15f;
		}
	}
}
