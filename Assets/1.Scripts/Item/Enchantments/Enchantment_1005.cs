using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enchantment_1005 : Enchantment {
	//같은 대상을 공격하는 몬스터(최대 1개체)에게 60보호막. 2초 지속, 6초 쿨타임



	Coroutine cooldown;
	WaitForSeconds interval = new WaitForSeconds(2.0f);
	WaitForSeconds cooldownInterval = new WaitForSeconds(6.0f);
	EquipmentEffect tempEffect;
	public override void OnAttack(Actor user, Actor target, Actor[] targets, bool isCritical)
	{
		tempEffect = new EquipmentEffect(this, user);
		tempEffect.shield = 60.0f;
		
	}
	IEnumerator Duration(Actor target)
	{
		yield return interval;
		if(target == null)
		{
			tempEffect = null;
		}
		else
		{
			target.RemoveAllEquipmentEffectByParent(this);
		}
	}
	
}
