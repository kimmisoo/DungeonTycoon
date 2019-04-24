using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentEffect {

	

	public float healthMax { get; set; } = 0.0f; // 고정수치 
	public float healthMaxMult { get; set; } = 0.0f; // 퍼센트 0.0f 이 기본
	public float shield { get; set; } = 0.0f;
	public float shieldMult { get; set; } = 0.0f;
	public float attack { get; set; } = 0.0f;
	public float attackMult { get; set; } = 0.0f;
	public float defence { get; set; } = 0.0f;
	public float defenceMult { get; set; } = 0.0f;
	public float penetration { get; set; } = 0.0f;
	public float penetrationMult { get; set; } = 0.0f;
	public float avoidMult { get; set; } = 0.0f;
	public float criticalChanceMult { get; set; } = 0.0f;
	public float criticalDamageMult { get; set; } = 0.0f;
	public float movespeedMult { get; set; } = 0.0f;
	public float attackspeedMult { get; set; } = 0.0f;
	public float attackrange { get; set; } = 0.0f;

}
