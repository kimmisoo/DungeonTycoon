using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentEffect {

	IHasEquipmentEffect parent;
	Actor user;
	public EquipmentEffect(IHasEquipmentEffect _parent, Actor _user)
	{
		SetParent(_parent);
		SetUser(_user);
	}

	public float healthMax { get; set; } = 0.0f; // 고정수치 
	public float healthMaxMult { get; set; } = 0.0f; // 퍼센트 0.0f 이 기본
	public float healthMaxMultFinal { get; set; } = 0.0f;
	public float shield { get; set; } = 0.0f;
	public float shieldMult { get; set; } = 0.0f;
	public float shieldMultFinal { get; set; } = 0.0f;
	public float attack { get; set; } = 0.0f;
	public float attackMult { get; set; } = 0.0f;
	public float attackMultFinal { get; set; } = 0.0f;
	public float defence { get; set; } = 0.0f;
	public float defenceMult { get; set; } = 0.0f;
	public float defenceMultFinal { get; set; } = 0.0f;
	public float reduceDamageMult { get; set; } = 0.0f;
	public float penetration { get; set; } = 0.0f;
	public float penetrationMult { get; set; } = 0.0f;
	public float penetrationMultFinal { get; set; } = 0.0f;
	public float avoidMult { get; set; } = 0.0f;
	public float avoidMultFinal { get; set; } = 0.0f;
	public float criticalChanceMult { get; set; } = 0.0f;
	public float criticalChanceMultFinal { get; set; } = 0.0f;
	public float criticalDamageMult { get; set; } = 0.0f;
	public float criticalDamageMultFinal { get; set; } = 0.0f;
	public float movespeedMult { get; set; } = 0.0f;
	public float movespeedMultFinal { get; set; } = 0.0f;
	public float attackspeedMult { get; set; } = 0.0f;
	public float attackspeedMultFinal { get; set; } = 0.0f;
	public int attackRange { get; set; } = 0;
	public int category { get; set; } = -1;
	public int invincibleCount { get; set; } = 0;

	public void SetParent(IHasEquipmentEffect _parent)
	{
		parent = _parent;
	}
	public void SetUser(Actor _user)
	{
		user = _user;
	}
	public IHasEquipmentEffect GetParent()
	{
		return parent;
	}
	public Actor GetUser()
	{
		return user;
	}
	


}
