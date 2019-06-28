using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleStatus {

	public float healthMax { get; set; }
	public float currentHealth { get; set; }
	public float currentShield { get; set; }
	public float attack { get; set; }
	public float defence { get; set; }
	public float reduceDamageMult { get; set; }
	public float penetration { get; set; }
	public float avoidMult { get; set; }
	public float criticalChanceMult { get; set; }
	public float criticalDamageMult { get; set; }
	public bool isStunned { get; set; }
	public bool isImmunedStun { get; set; }
	public bool isDebuffed { get; set; }
	public bool isDead { get; set; }
	public float damageTakedSum { get; set; }
	public float movespeedMult { get; set; }
	public float movespeedMultFinal { get; set; } = 1.0f;
	public float attackspeedMult { get; set; }
	public float attackspeedMultFinal { get; set; } = 1.0f;
	public int attackRange { get; set; } = 1;
	public bool isHitRecent { get; set; } = false;
	public bool isCriticalRecent { get; set; } = false;
	public int invincibleCount { get; set; } = 0;

	
}
