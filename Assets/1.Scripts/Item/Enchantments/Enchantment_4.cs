using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enchantment_4 : Enchantment {
	//힘 흡수
	//주변 1칸 적의 공격력 - 10%씩, 주변 적의 수 * 10% 만큼 자신의 공격력 증가.
	Coroutine nearMonsterCheck;
	WaitForSeconds interval = new WaitForSeconds(5.0f);
	List<Actor> near = new List<Actor>();
	const float amount = 0.1f;
	EquipmentEffect tempEffect;

	public override void OnEquip(Actor user)
	{
		nearMonsterCheck = StartCoroutine(NearMonsterCheck(user));
	}
	
	public override void OnUnequip(Actor user)
	{
		StopCoroutine(nearMonsterCheck);
	}
	IEnumerator NearMonsterCheck(Actor user)
	{

		int nearCount = 0;
		//near에 인근 몬스터 추가
		while (true)
		{
			near = user.GetAdjacentActor(1);
			nearCount = 0;
			foreach (Actor a in near)
			{
				if (a is Monster)
				{
					tempEffect = new EquipmentEffect(this, a);
					tempEffect.attackMult = -0.1f;
					a.AddEquipmentEffect(tempEffect);
					nearCount++;
				}
			}

			tempEffect = new EquipmentEffect(this, user);
			tempEffect.attackMult += amount * nearCount;
			user.AddEquipmentEffect(tempEffect);
			
			///버프, 디버프 추가

			yield return interval;

			foreach(Actor a in near)
			{
				if (a is Monster)
				{
					a.RemoveAllEquipmentEffectByParent(this); 
				}
			}
			user.RemoveAllEquipmentEffectByParent(this);
			
			///버프, 디버프 해제
		}
	}
}
