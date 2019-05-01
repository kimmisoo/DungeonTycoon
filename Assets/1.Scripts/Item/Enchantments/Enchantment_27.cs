using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enchantment_27 : Enchantment {

	//자기방어
	//방어력의 20%만큼 추가공격력

	EquipmentEffect tempEffect;
	Coroutine statusCheck;
	float defenceOrigin;
	float eps = 0.01f;
	WaitForSeconds interval = new WaitForSeconds(3.0f);

	public override void OnEquip(Character user)
	{
		tempEffect = new EquipmentEffect(this, user);
		defenceOrigin = user.GetCalculatedDefence();
		tempEffect.attack += defenceOrigin * 0.2f;
		user.AddEquipmentEffect(tempEffect);
		StartCoroutine(StatusCheck(user));
		
	}
	public override void OnUnequip(Character user)
	{
		StopCoroutine(statusCheck);
		user.RemoveAllEquipmentEffectByParent(this);
	}
	IEnumerator StatusCheck(Character user)
	{
		while (true)
		{
			if (Mathf.Abs(defenceOrigin - user.GetCalculatedDefence()) > eps)
			{
				defenceOrigin = user.GetCalculatedDefence();
				user.RemoveAllEquipmentEffectByParent(this);
				tempEffect = new EquipmentEffect(this, user);
				tempEffect.attack += defenceOrigin * 0.2f;
				user.AddEquipmentEffect(tempEffect);
			}
			yield return interval;
		}
	}

}
