using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enchantment_2 : Enchantment {
	//쌍수무기
	//2번 공격하지만 데미지 절반
	EquipmentEffect effect;
	public override void OnEquip(Character user)
	{
		if(effect != null)
		{
			// abnormal situation!
			OnUnequip(user);
		}
		effect = new EquipmentEffect(this, user);
		effect.attackMultFinal -= 0.5f;
		effect.attackspeedMultFinal += 1.0f;
		user.AddEquipmentEffect(effect);
	}
	public override void OnUnequip(Character user)
	{
		//user.attackMultFinal += 0.5f;
		//user.attackspeedMultFinal -= 1.0f;
		if(effect == null)
		{
			//abnormal situation	
			return;
		}
		user.RemoveEquipmentEffect(effect);
		effect = null;
	}
	public override void OnDead(Character user, Monster target, Monster[] targets)
	{
		if(effect != null)
		{
			OnUnequip(user);
		}
	}


}
