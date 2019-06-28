using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterAttackAI : MonoBehaviour, IAttackAI {
	Monster monster;
	//범위 내에 들어오면 공격 처리
	public IEnumerator Attacking()
	{
		while (true)
		{
			if (target != null && actingResult.isReachEnemy == true && isStunned == false)
			{
				//공격루틴
				yield return new WaitForSeconds(1.0f / (1.0f + GetCalculatedAttackspeed()));
				Attack();
			}
			yield return null;
		}
	}
	public override void Attack()
	{
		if (target != null && target.state != State.Dead)
		{
			float dealtDamage = 0.0f;
			bool isCritical = false;
			bool isHit = false;
			bool isDead = false;
			isCritical = GetIsCriticalAttack();

			target.Attacked(this, isCritical, out isHit);
			foreach (Enchantment e in enchantmentList)
			{
				e.OnAttack(this, target, GetAdjacentActor(GetCalculatedAttackRange()).ToArray(), isCritical);
			}
			if (isHit == true)
			{
				target.TakeDamage(this, isCritical, out isDead, out dealtDamage);
				foreach (Enchantment e in enchantmentList)
				{
					e.OnDamage(this, target, GetAdjacentActor(2).ToArray(), dealtDamage, isCritical);
				}
			}
		}
	}
}
