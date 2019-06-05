using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enchantment_1005 : Enchantment {
	//같은 대상을 공격하는 몬스터(최대 1개체)에게 60보호막. 2초 지속, 6초 쿨타임


	
	Coroutine cooldown;
	Coroutine duration;
	bool isCooldown = false;
	WaitForSeconds interval = new WaitForSeconds(2.0f);
	WaitForSeconds cooldownInterval = new WaitForSeconds(6.0f);
	Actor buffActor;
	EquipmentEffect tempEffect;
	public override void OnAttack(Actor user, Actor target, Actor[] targets, bool isCritical)
	{
		if (isCooldown == true)
			return;
		foreach (Actor a in user.GetAdjacentActor(3))
		{
			if (a is Monster && (a as Monster).GetCurrentTarget().Equals(target))
			{
				tempEffect = new EquipmentEffect(this, a);
				tempEffect.shield += 60.0f;
				a.AddEquipmentEffect(tempEffect);
				buffActor = a;
				isCooldown = true;
				duration = StartCoroutine(Duration(a));
				cooldown = StartCoroutine(Cooldown());
				break;
			}
		}
	}
	IEnumerator Cooldown()
	{
		yield return cooldownInterval;
		isCooldown = false;
	}
	IEnumerator Duration(Actor target)
	{
		yield return interval;
		RemoveEquipmentEffect(target);
	}
	public override void OnDead(Actor user, Actor target, Actor[] targets)
	{
		StopCoroutine(duration);
		StopCoroutine(cooldown);
		if (buffActor == null)
			return;
		else
		{
			buffActor.RemoveAllEquipmentEffectByParent(this);
		}
	}
	public override void OnEndBattle(Actor user, Actor target, Actor[] targets)
	{
		StopCoroutine(duration);
		StopCoroutine(cooldown);
		if (buffActor == null)
			return;
		else
		{
			buffActor.RemoveAllEquipmentEffectByParent(this);
		}
	}
	void RemoveEquipmentEffect(Actor target)
	{
		if (target == null)
		{
			tempEffect = null;
		}
		else
		{
			target.RemoveAllEquipmentEffectByParent(this);
		}
	}


}
