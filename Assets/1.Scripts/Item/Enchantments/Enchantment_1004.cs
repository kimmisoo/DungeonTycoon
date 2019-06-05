using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enchantment_1004 : Enchantment {
	//같은 대상을 공격중인 스켈레톤 대형도끼병에게 공격속도 +15% 버프.본인이 죽으면 대형도끼병에게 250데미지(최대 2대상)

	List<Actor> buffActor = new List<Actor>();
	EquipmentEffect buff;

	//같은 대상을 공격중인 스켈레톤 대형도끼병에게 공격속도 +15% 버프. 본인이 죽으면 대형도끼병에게 200데미지 (최대 2대상)
	public override void OnStartBattle(Actor user, Actor target, Actor[] targets)
	{
		buffActor.Clear();
	}

	public override void OnAttack(Actor user, Actor target, Actor[] targets, bool isCritical)
	{
		if (buffActor.Count > 2)
			return;
		foreach (Actor a in user.GetAdjacentActor(3))
		{
			if (a is Monster && (a as Monster).GetMonsterCode() == 1111)//스켈레톤 대형 도끼병 코드
			{
				if (buffActor.Contains(a))
					continue;
				else
				{
					buffActor.Add(a);
					buff = new EquipmentEffect(this, a);
					buff.attackspeedMult += 0.15f;
					a.AddEquipmentEffect(buff);
				}
			}
		}
	}
	public override void OnDead(Actor user, Actor target, Actor[] targets)
	{
		bool isDead = false;
		foreach (Actor a in buffActor)
		{
			a.TakeDamageFromEnchantment(250.0f, user, this, false, out isDead);
			if (isDead == true)
			{
				a.Die(target);
				isDead = false;
			}
			a.RemoveAllEquipmentEffectByParent(this);
		}
	}
	public override void OnEndBattle(Actor user, Actor target, Actor[] targets)
	{
		foreach (Actor a in buffActor)
		{
			a.RemoveAllEquipmentEffectByParent(this);
		}
	}
}
