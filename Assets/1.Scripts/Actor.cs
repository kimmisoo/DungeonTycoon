using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class Actor : MonoBehaviour {

	//-----기능-----
	//이동
	//인챈트
	//전투
	//-----스탯-----
	//소지골드
	//체력
	//공격력
	//방어력
	//방어구관통
	//회피율
	//이동속도
	//공격속도
	//이름
	//설명
	//받은데미지 총합
	//is디버프
	//is스턴
	//사망여부
	//
	public int gold { get; set; }

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
	public bool isDebuffed { get; set; }
	public bool isDead { get; set; }
	public float damageTakedSum { get; set; }

	public float movespeedMult { get; set; }
	public float movespeedMultFinal { get; set; } = 1.0f;

	public float attackspeedMult { get; set; }
	public float attackspeedMultFinal { get; set; } = 1.0f;

	public string actorName { get; set; }
	public string explanation { get; set; }

	public int attackRange { get; set; } = 1;
	public bool isHitRecent { get; set; } = false;
	public bool isCriticalRecent { get; set; } = false;
	public int invincibleCount { get; set; } = 0;

	public Moveto moveto;
	public List<Enchantment> enchantmentList = new List<Enchantment>();
	public List<EquipmentEffect> equipmentEffectList = new List<EquipmentEffect>();
	public List<Item> itemList = new List<Item>();
	public Structure destStructure = null;
	public List<Moveto.PathVertex> wayList = new List<Moveto.PathVertex>();
	public List<TileForMove> wayForMove = new List<TileForMove>();
	public enum State
	{
		Idle, Move, Battle, Dead, Indoor
	}
	public State state = new State();
	//실제 데미지 계산 =
	//데미지 = 공격력 * 1/(1+( (방어 - 고정방관) * (1- %방관) * 1/100))
	//데미지 입었을때 target에서 user OnDamage 호출.
	
	public virtual void Start()
	{
		moveto = GetComponent<Moveto>();
	}


	public abstract void Attack();
	public abstract void Attacked(Actor from, bool isCritical, out bool isHit);
	public abstract void AttackedFromEnchantment(Actor from, Enchantment enchantment, bool isCritical, out bool isHit);
	public abstract void TakeDamage(Actor from, bool isCritical, out bool isDead, out float damage);
	public abstract void TakeDamageFromEnchantment(float damage, Actor from, Enchantment enchantment, bool isCritical, out bool isDead);
	public abstract void Die(Actor Opponent);
	public abstract void TakeHeal(float heal, Actor from);
	public abstract void TakeHealFromEnchantment(float heal, Actor from, Enchantment enchantment);
	public abstract void AddEnchantment(Enchantment enchantment);
	public abstract void RemoveEnchantment(Enchantment enchantment);
	public void AddEquipmentEffect(EquipmentEffect equipmentEffect)
	{
		equipmentEffectList.Add(equipmentEffect);
	}
	public void RemoveEquipmentEffect(EquipmentEffect equipmentEffect)
	{
		equipmentEffectList.Remove(equipmentEffect);
	}
	public void RemoveAllEquipmentEffectByParent(IHasEquipmentEffect parent)
	{
		int i = 0;
		while(i<equipmentEffectList.Count)
		{
			if (equipmentEffectList[i].GetParent().Equals(parent))
			{
				equipmentEffectList.RemoveAt(i--);
			}
			i++;
		}
		
	}
	public abstract void TakeStunned(Actor from, Enchantment enchantment, float during);

	public int GetGold()
	{
		return gold;
	}
	public void SetGold(int _gold)
	{
		gold = _gold;
	}
	public float GetCurrentHealth()
	{
		return currentHealth;
	}
	public float GetCurrentShield()
	{
		return currentShield;
	}
	public float GetHealthMax()
	{
		return healthMax;
	}
	public float GetCalculatedHealthMax()
	{
		
		return ((healthMax + GetHealthMaxFromEquipmentEffect()) * (1.0f + GetHealthMaxMultFromEquipmentEffect()) * (1.0f + GetHealthMaxMultFinalFromEquipmentEffect()));
	}
	public void SetHealthMax(float _healthMax)
	{
		healthMax = _healthMax;
	}
	
	public float GetCalculatedAttack()
	{
		//return 0.0f;
		//실제 공식에따라 ~~~ 공격력 리턴
		return ((attack + GetAttackFromEquipmentEffect()) * (1.0f + GetAttackMultFromEquipmentEffect()) * (1.0f + GetAttackMultFinalFromEquipmentEffect()));

	}
	public float GetCalculatedDefence()
	{
		return ((defence + GetDefenceFromEquipmentEffect()) * (1.0f + GetDefenceMultFromEquipmentEffect()) * (1.0f + GetDefenceMultFinalFromEquipmentEffect()));
	}
	public float GetCalculatedPenetration()
	{
		return ((penetration + GetPenetrationFromEquipmentEffect()) * (1.0f + GetPenetrationMultFromEquipmentEffect()) * (1.0f + GetPenetrationMultFinalFromEquipmentEffect()));
	}
	public float GetCalculatedAvoidMult()
	{
		return (avoidMult * (1.0f + GetAvoidMultFromEquipmentEffect()) * (1.0f + GetAvoidMultFinalFromEquipmentEffect()));
	}
	public float GetCalculatedCriticalChance()
	{
		return (criticalChanceMult * (1.0f + GetCriticalChanceMultFromEquipmentEffect()) * (1.0f + GetCriticalChanceMultFinalFromEquipmentEffect()));
	}
	public float GetCalculatedCriticalDamage()
	{
		return (criticalDamageMult * (1.0f + GetCriticalDamageMultFromEquipmentEffect()) * (1.0f + GetCriticalDamageMultFinalFromEquipmentEffect()));
	}
	public float GetCalculatedMovespeed()
	{
		return (movespeedMult * (1.0f + GetMovespeedMultFromEquipmentEffect()) * (1.0f + GetMovespeedMultFinalFromEquipmentEffect()));
	}
	public float GetCalculatedAttackspeed()
	{
		return (attackspeedMult * (1.0f + GetAttackspeedMultFromEquipmentEffect()) * (1.0f + GetAttackspeedMultFinalFromEquipmentEffect()));
	}
	public int GetCalculatedAttackRange()
	{
		return attackRange + GetAttackRangeFromEquipmentEffect();
	}
	public int GetCalculatedInvincibleCount()
	{
		return invincibleCount + GetInvincibleCountFromEquipmentEffect();
	}
	
	
	public List<Actor> GetAdjacentActor(int range)
	{
		List<Actor> adjacentActors = new List<Actor>();
		TileLayer layer = moveto.tileLayer;
		int x = GetCurTileForMove().GetX();
		int y = GetCurTileForMove().GetY();
		TileForMove tileForMoveTemp;
		for (int i = -range; i <= range; i++)
		{
			for (int j = -range; j <= range; j++)
			{
				if (Mathf.Abs(i) + Mathf.Abs(j) > range) // + 실제 actor가 타일 위에 있는지
					continue;
				tileForMoveTemp = layer.GetTileForMove(x + i, y + j);
				if(tileForMoveTemp.Equals(tileForMoveTemp.GetRecentActor().moveto.GetCurTileForMove())) // tileForMoveTemp에 기록된 recentActor의 현재위치가 tileForMoveTemp와 일치하는지
				{
					adjacentActors.Add(layer.GetTileForMove(x + i, y + j).GetRecentActor());
				}
				else
				{
					tileForMoveTemp.SetRecentActor(null);
				}
			}
		}
		return adjacentActors;
	}
	public List<EquipmentEffect> GetSameCategoryEffects(int category)
	{
		List<EquipmentEffect> tempList = new List<EquipmentEffect>();
		foreach(EquipmentEffect e in equipmentEffectList)
		{
			if (e.category != -1)
				tempList.Add(e);
		}
		return tempList;
	}
	public float GetCalculatedDamage(float damage, float penetration, Actor from)
	{
		return 0.0f;
		//damage 연산 필요		
	}

	public float GetHealthMaxFromEquipmentEffect()
	{
		float sum = 0.0f;
		foreach(EquipmentEffect e in equipmentEffectList)
		{
			sum += e.healthMax;
		}
		return sum;
	}
	public float GetHealthMaxMultFromEquipmentEffect()
	{
		float sum = 0.0f;
		foreach (EquipmentEffect e in equipmentEffectList)
		{
			sum += e.healthMaxMult;
		}
		return sum;
	}
	public float GetHealthMaxMultFinalFromEquipmentEffect()
	{
		float sum = 0.0f;
		foreach (EquipmentEffect e in equipmentEffectList)
		{
			sum += e.healthMaxMultFinal;
		}
		return sum;
	}
	public float GetShieldFromEquipmentEffect()
	{
		float sum = 0.0f;
		foreach (EquipmentEffect e in equipmentEffectList)
		{
			sum += e.shield;
		}
		return sum;
	}
	public float GetShieldMultFromEquipmentEffect()
	{
		float sum = 0.0f;
		foreach (EquipmentEffect e in equipmentEffectList)
		{
			sum += e.shieldMult;
		}
		return sum;
	}
	public float GetShieldMultFinalFromEquipmentEffect()
	{
		float sum = 0.0f;
		foreach (EquipmentEffect e in equipmentEffectList)
		{
			sum += e.shieldMultFinal;
		}
		return sum;
	}
	public float GetAttackFromEquipmentEffect()
	{
		float sum = 0.0f;
		foreach (EquipmentEffect e in equipmentEffectList)
		{
			sum += e.attack;
		}
		return sum;
	}
	public float GetAttackMultFromEquipmentEffect()
	{
		float sum = 0.0f;
		foreach (EquipmentEffect e in equipmentEffectList)
		{
			sum += e.attackMult;
		}
		return sum;
	}
	public float GetAttackMultFinalFromEquipmentEffect()
	{
		float sum = 0.0f;
		foreach (EquipmentEffect e in equipmentEffectList)
		{
			sum += e.attackMultFinal;
		}
		return sum;
	}
	public float GetDefenceFromEquipmentEffect()
	{
		float sum = 0.0f;
		foreach (EquipmentEffect e in equipmentEffectList)
		{
			sum += e.defence;
		}
		return sum;
	}
	public float GetDefenceMultFromEquipmentEffect()
	{
		float sum = 0.0f;
		foreach (EquipmentEffect e in equipmentEffectList)
		{
			sum += e.defenceMult;
		}
		return sum;
	}
	public float GetDefenceMultFinalFromEquipmentEffect()
	{
		float sum = 0.0f;
		foreach (EquipmentEffect e in equipmentEffectList)
		{
			sum += e.defenceMultFinal;
		}
		return sum;
	}
	public float GetPenetrationFromEquipmentEffect()
	{
		float sum = 0.0f;
		foreach (EquipmentEffect e in equipmentEffectList)
		{
			sum += e.penetration;
		}
		return sum;
	}
	public float GetPenetrationMultFromEquipmentEffect()
	{
		float sum = 0.0f;
		foreach (EquipmentEffect e in equipmentEffectList)
		{
			sum += e.penetrationMult;
		}
		return sum;
	}
	public float GetPenetrationMultFinalFromEquipmentEffect()
	{
		float sum = 0.0f;
		foreach (EquipmentEffect e in equipmentEffectList)
		{
			sum += e.penetrationMultFinal;
		}
		return sum;
	}
	public float GetAvoidMultFromEquipmentEffect()
	{
		float sum = 0.0f;
		foreach (EquipmentEffect e in equipmentEffectList)
		{
			sum += e.avoidMult;
		}
		return sum;
	}
	public float GetAvoidMultFinalFromEquipmentEffect()
	{
		float sum = 0.0f;
		foreach (EquipmentEffect e in equipmentEffectList)
		{
			sum += e.avoidMultFinal;
		}
		return sum;
	}
	public float GetCriticalChanceMultFromEquipmentEffect()
	{
		float sum = 0.0f;
		foreach (EquipmentEffect e in equipmentEffectList)
		{
			sum += e.criticalChanceMult;
		}
		return sum;
	}
	public float GetCriticalChanceMultFinalFromEquipmentEffect()
	{
		float sum = 0.0f;
		foreach (EquipmentEffect e in equipmentEffectList)
		{
			sum += e.criticalChanceMultFinal;
		}
		return sum;
	}
	public float GetCriticalDamageMultFromEquipmentEffect()
	{
		float sum = 0.0f;
		foreach (EquipmentEffect e in equipmentEffectList)
		{
			sum += e.criticalDamageMult;
		}
		return sum;
	}
	public float GetCriticalDamageMultFinalFromEquipmentEffect()
	{
		float sum = 0.0f;
		foreach (EquipmentEffect e in equipmentEffectList)
		{
			sum += e.criticalDamageMultFinal;
		}
		return sum;
	}
	public float GetMovespeedMultFromEquipmentEffect()
	{
		float sum = 0.0f;
		foreach (EquipmentEffect e in equipmentEffectList)
		{
			sum += e.movespeedMult;
		}
		return sum;
	}
	public float GetMovespeedMultFinalFromEquipmentEffect()
	{
		float sum = 0.0f;
		foreach (EquipmentEffect e in equipmentEffectList)
		{
			sum += e.movespeedMultFinal;
		}
		return sum;
	}
	public float GetAttackspeedMultFromEquipmentEffect()
	{
		float sum = 0.0f;
		foreach (EquipmentEffect e in equipmentEffectList)
		{
			sum += e.attackspeedMult;
		}
		return sum;
	}
	public float GetAttackspeedMultFinalFromEquipmentEffect()
	{
		float sum = 0.0f;
		foreach (EquipmentEffect e in equipmentEffectList)
		{
			sum += e.attackspeedMultFinal;
		}
		return sum;
	}
	public int GetAttackRangeFromEquipmentEffect()
	{
		int sum = 0;
		foreach (EquipmentEffect e in equipmentEffectList)
		{
			sum += e.attackRange;
		}
		return sum;
	}
	public int GetInvincibleCountFromEquipmentEffect()
	{
		int sum = 0;
		foreach(EquipmentEffect e in equipmentEffectList)
		{
			sum += e.invincibleCount;
		}
		return sum;
	}
	
	public void SetCurTile(Tile _tile)
	{
		moveto.SetCurTile(_tile);
	}
	public Tile GetCurTile()
	{
		return moveto.GetCurTile();
	}
	public void SetCurTileForMove(TileForMove _tileForMove)
	{ 
		moveto.SetCurTileForMove(_tileForMove);
	}
	public TileForMove GetCurTileForMove()
	{
		return moveto.GetCurTileForMove();
	}
	public int GetDistanceFromOtherActorForMove(Actor actor)
	{
		if(actor != null)
		{
			return Mathf.Abs(actor.GetCurTileForMove().GetX() - GetCurTileForMove().GetX()) + Mathf.Abs(actor.GetCurTileForMove().GetY() - GetCurTileForMove().GetY()); 
		}
		else
		{
			return int.MaxValue;
		}
	}
	
	public Direction GetDirectionFromOtherTileForMove(TileForMove tileForMove)
	{
		Direction direction = Direction.DownLeft;
		int distanceX = GetCurTileForMove().GetX() - tileForMove.GetX();
		int distanceY = GetCurTileForMove().GetY() - tileForMove.GetY();
		int absX = Mathf.Abs(distanceX);
		int absY = Mathf.Abs(distanceY);

		if(absX == 0 && absY == 0)
		{
			direction = Direction.None;
			return direction;
		}
		if (distanceX >= 0 && distanceY >= 0)
		{
			if (absX > absY)
				direction = Direction.DownRight;
			else
				direction = Direction.DownLeft;
			
		}
		else if (distanceX >=0 && distanceY < 0) //Right
		{
			if (absX > absY)
				direction = Direction.DownRight;
			else
				direction = Direction.UpRight;
		}
		else if(distanceX < 0 && distanceY >= 0) //Left
		{
			if (absX > absY)
				direction = Direction.UpLeft;
			else
				direction = Direction.DownLeft;
		}
		else
		{
			if (absX > absY)
				direction = Direction.UpLeft;
			else
				direction = Direction.UpRight;
		}

		return direction;

	}
	
	public bool GetIsCriticalAttack()
	{
		if (GetCalculatedCriticalChance() < 0.001f)
			return false;
		if (Random.Range(0.0f, 1.0f) < GetCalculatedCriticalChance())
			return true;
		return false;
	}
}


