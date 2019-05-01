using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enchantment_26 : Enchantment {

	//위기대처
	//체력이 10% 이하일때 받는피해 60% 감소

	//60% 감소 계산해야함
	EquipmentEffect tempEffect;
	Coroutine healthCheck;
	WaitForSeconds interval = new WaitForSeconds(3.0f);

	public override void OnEquip(Actor user)
	{
		tempEffect = new EquipmentEffect(this, user);
		tempEffect.reduceDamageMult = 0.6f;
		healthCheck = StartCoroutine(HealthCheck(user));
	}
	public override void OnUnequip(Actor user)
	{
		StopCoroutine(healthCheck);
	}
	IEnumerator HealthCheck(Actor user)
	{
		while (true)
		{
			if (user.GetCurrentHealth() / user.GetHealthMax() > 0.1f)
			{
				user.RemoveAllEquipmentEffectByParent(this);
				user.AddEquipmentEffect(tempEffect);
			}
			else
			{
				user.RemoveAllEquipmentEffectByParent(this);
			}
			yield return interval;
		}
	}
	
}
