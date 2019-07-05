using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamagable {

	void Attacked(IDamagable from, bool isCritical, out bool isHit);
	void AttackedFromEnchantment(IDamagable from, Enchantment enchantment, bool isCritical, out bool isHit);
	void TakeDamage(IDamagable from, bool isCritical, out bool isDead, out float damage);
	void TakeDamageFromEnchantment(float damage, IDamagable from, Enchantment enchantment, bool isCritical, out bool isDead);
	void TakeHeal(float heal, IDamagable from);
	void TakeHealFromEnchantment(float heal, IDamagable from, Enchantment enchantment);
	bool GetIsCritical(float criticalChance);

	float GetCalculatedDamage(IDamagable from, bool isCritical);
	float GetCurrentHealth();
	float GetCurrentShield();
	float GetHealthMax();
	float GetCalculatedHealthMax();
	void SetHealthMax(float _healthMax);
	float GetCalculatedAttack();
	float GetCalculatedDefence();
	float GetCalculatedFixedPenetration();
	float GetCalculatedRatioPenetration();
	float GetCalculatedAvoidMult();
	float GetCalculatedCriticalChance();
	float GetCalculatedCriticalDamage();
	float GetCalculatedMovespeed();
	float GetCalculatedAttackspeed();
	int GetCalculatedAttackRange();
	int GetCalculatedInvincibleCount();
	bool GetCalculatedImmunedStun();
	float GetHealthMaxFromEquipmentEffect();
	float GetHealthMaxMultFromEquipmentEffect();
	float GetHealthMaxMultFinalFromEquipmentEffect();
	float GetShieldFromEquipmentEffect();
	float GetShieldMultFromEquipmentEffect();
	float GetShieldMultFinalFromEquipmentEffect();
	float GetAttackFromEquipmentEffect();
	float GetAttackMultFromEquipmentEffect();
	float GetAttackMultFinalFromEquipmentEffect();
	float GetDefenceFromEquipmentEffect();
	float GetDefenceMultFromEquipmentEffect();
	float GetDefenceMultFinalFromEquipmentEffect();
	float GetPenetrationFromEquipmentEffect();
	float GetPenetrationMultFromEquipmentEffect();
	float GetPenetrationMultFinalFromEquipmentEffect();
	float GetAvoidMultFromEquipmentEffect();
	float GetAvoidMultFinalFromEquipmentEffect();
	float GetCriticalChanceMultFromEquipmentEffect();
	float GetCriticalChanceMultFinalFromEquipmentEffect();
	float GetCriticalDamageMultFromEquipmentEffect();
	float GetCriticalDamageMultFinalFromEquipmentEffect();
	float GetMovespeedMultFromEquipmentEffect();
	float GetMovespeedMultFinalFromEquipmentEffect();
	float GetAttackspeedMultFromEquipmentEffect();
	float GetAttackspeedMultFinalFromEquipmentEffect();
	int GetAttackRangeFromEquipmentEffect();
	int GetInvincibleCountFromEquipmentEffect();
	bool GetImmunedStunFromEquipmentEffect();

}
