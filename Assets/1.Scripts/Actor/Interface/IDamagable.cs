using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamagable {
	BattleStatus battleStatus
	{
		get;
	}
	void SetBattleStatus(BattleStatus battleStatus);
	bool Attack(IDamagable enemy);
	void Attacked(IDamagable from, bool isCritical, out bool isHit);
	void AttackedFromEnchantment(IDamagable from, Enchantment enchantment, bool isCritical, out bool isHit);
	void TakeDamage(IDamagable from, bool isCritical, out bool isDead, out float damage);
	void TakeDamageFromEnchantment(float damage, IDamagable from, Enchantment enchantment, bool isCritical, out bool isDead);
	void TakeHeal(float heal, IDamagable from);
	void TakeHealFromEnchantment(float heal, IDamagable from, Enchantment enchantment);
	void StartBattle(IDamagable enemy);
	void EndBattle(IDamagable enemy);
	void AddEnchantment(Enchantment enchantment);
	void RemoveEnchantmnet(Enchantment enchantmet);
	void TakeStunned(IDamagable from, Enchantment enchantment, float during);
	bool IsAttackable(IDamagable enemy);
	IEnumerator Stun(float duration);


}
