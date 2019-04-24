using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enchantment_2 : Enchantment {
	//쌍수무기
	//2번 공격하지만 데미지 절반
	public override void OnEquip(Character user)
	{
		user.attackMultFinal -= 0.5f;
		user.attackspeedMultFinal += 1.0f;
	}
	public override void OnUnequip(Character user)
	{
		user.attackMultFinal += 0.5f;
		user.attackspeedMultFinal -= 1.0f;
	}
	
	

}
