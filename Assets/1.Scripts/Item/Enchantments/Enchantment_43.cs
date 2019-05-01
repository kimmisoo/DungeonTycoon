using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enchantment_43 : Enchantment {
	//신의가호
	//모험가 1칸이내 있는 몬스터 초당 12데미지. 방어력 - 15(1회)


	Coroutine nearMonsterDamage;
	Coroutine nearMonsterDebuff;
	WaitForSeconds intervalDamage = new WaitForSeconds(1.0f);
	WaitForSeconds intervalDebuff = new WaitForSeconds(3.0f);
	List<Actor> nearDebuff = new List<Actor>();
	List<Actor> nearDamage = new List<Actor>();
	const float amount = 0.1f;
	EquipmentEffect tempEffect;

	public override void OnEquip(Actor user)
	{
		nearMonsterDamage = StartCoroutine(NearMonsterDamage(user));
		nearMonsterDebuff = StartCoroutine(NearMonsterDebuff(user));
	}

	public override void OnUnequip(Actor user)
	{
		StopCoroutine(nearMonsterDamage);
		StopCoroutine(nearMonsterDebuff);
		foreach (Actor a in nearDebuff)
		{
			a.RemoveAllEquipmentEffectByParent(this);
		}
	}
	IEnumerator NearMonsterDamage(Actor user)
	{
		while(true)
		{
			yield return intervalDamage;
			nearDamage = user.GetAdjacentActor(1);
			foreach(Actor a in nearDamage)
			{
				if(a is Monster)
				{
					a.TakeDamageFromEnchantment(12.0f, user, this, false, out isHit, out isDead);
				}
			}
			nearDamage.Clear();
		}
	}
	IEnumerator NearMonsterDebuff(Actor user)
	{

		
		//near에 인근 몬스터 추가
		while (true)
		{
			nearDebuff = user.GetAdjacentActor(1);
			foreach (Actor a in nearDebuff)
			{
				if (a is Monster)
				{
					tempEffect = new EquipmentEffect(this, a);
					tempEffect.defence -= 15.0f;
					a.AddEquipmentEffect(tempEffect);
				}
			}
			yield return intervalDebuff;

			foreach (Actor a in nearDebuff)
			{
				if (a is Monster)
				{
					a.RemoveAllEquipmentEffectByParent(this);
				}
			}
			nearDebuff.Clear();
			

			///버프, 디버프 해제
		}
	}
}
