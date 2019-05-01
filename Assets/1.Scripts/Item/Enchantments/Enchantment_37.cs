using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enchantment_37 : Enchantment {

	//방어막
	//1회 공격방어
	//쿨타임 25초
	//2분내 해제불가?

	//데미지 받기 이전.
	//데미지 받은 후 분리 해야함 -> Actor 데미지 처리 단계에서 invincible Count 체크하기.

	EquipmentEffect tempEffect;
	Coroutine cooldown;
	WaitForSeconds cooldownInterval = new WaitForSeconds(25.0f);
	WaitForSeconds interval = new WaitForSeconds(3.0f);
	bool isCooldown = false;

	public override void OnEquip(Actor user)
	{
		tempEffect = new EquipmentEffect(this, user);
		tempEffect.invincibleCount += 1;
		user.AddEquipmentEffect(tempEffect);
		cooldown = StartCoroutine(Cooldown(user));
	}
	
	public override void OnUnequip(Actor user)
	{
		StopCoroutine(cooldown);
		user.RemoveAllEquipmentEffectByParent(this);
	}
	IEnumerator Cooldown(Actor user)
	{
		while (true)
		{
			if (tempEffect.invincibleCount <= 0)
			{
				yield return cooldownInterval;
				tempEffect.invincibleCount += 1;
			}
			else
			{
				yield return interval;
			}
		}
		
	}
}
