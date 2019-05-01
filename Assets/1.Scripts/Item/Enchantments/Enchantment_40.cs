using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enchantment_40 : Enchantment{

	//처형
	//대상의 체력이 20% 이하일때 공격력의 200% 데미지 공격 가함.

	WaitForSeconds interval = new WaitForSeconds(3.0f);
	Coroutine targetHealthCheck;
	Monster currentTarget = null;
	EquipmentEffect tempEffect;
	public override void OnEquip(Character user)
	{
		tempEffect = new EquipmentEffect(this, user);
		tempEffect.attackMult += 1.0f;
		targetHealthCheck = StartCoroutine(TargetHealthCheck(user));
	}
	public override void OnUnequip(Character user)
	{
		StopCoroutine(targetHealthCheck);
		user.RemoveAllEquipmentEffectByParent(this);
		tempEffect = null;
	}
	public override void OnAttack(Character user, Monster target, Monster[] targets, bool isCritical)
	{
		currentTarget = target;
	}
	IEnumerator TargetHealthCheck(Character user)
	{
		while (true)
		{
			if (currentTarget != null && currentTarget.GetCurrentHealth() / currentTarget.GetCalculatedHealthMax() <= 0.2f)
			{
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
	
