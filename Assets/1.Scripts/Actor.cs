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
	public float health { get; set; }
	public float itemHealthMax { get; set; }
	public float enchantmentHealthMax { get; set; }
	public float healthMaxMult { get; set; }
	public float itemHealthMaxMult { get; set; }
	public float enchantmentHealthMaxMult { get; set; }
	public float healthMaxMultFinal { get; set; } = 1.0f;

	public float shieldMax { get; set; }
	public float shield { get; set; }
	public float itemShieldMax { get; set; }
	public float enchantmentShieldMax { get; set; }
	public float shieldMaxMult { get; set; }
	public float itemshieldMaxMult { get; set; }
	public float enchantmentShieldMaxMult { get; set; }
	public float shieldMaxMultFinal { get; set; } = 1.0f;

	public float attack { get; set; }
	public float itemAttack { get; set; }
	public float enchantmentAttack { get; set; }
	public float attackMult { get; set; }
	public float itemAttackMult { get; set; }
	public float enchantmentAttackMult { get; set; }
	public float attackMultFinal { get; set; } = 1.0f;

	public float defence { get; set; }
	public float itemDefence { get; set; }
	public float enchantmentDefence { get; set; }
	public float defenceMult { get; set; }
	public float itemDefenceMult { get; set; }
	public float enchantmentDefenceMult { get; set; }
	public float defenceMultFinal { get; set; } = 1.0f;

	public float penetration { get; set; }
	public float itemPenetration { get; set; }
	public float enchantmentPenetration { get; set; }
	public float penetrationMult { get; set; }
	public float itemPenetrationMult { get; set; }
	public float enchantmentPenetrationMult { get; set; }
	public float penetrationMultFinal { get; set; } = 1.0f;

	/*public float avoid { get; set; }
	public float itemAvoid { get; set; }
	public float enchantmentAvoid { get; set; }*/
	public float avoidMult { get; set; }
	public float itemAvoidMult { get; set; }
	public float enchantmentAvoidMult { get; set; }
	public float avoidMultFinal { get; set; } = 1.0f;

	/*public float criticalChance { get; set; }
	public float itemCriticalChance { get; set; }
	public float enchantmentCriticalChance { get; set; }*/
	public float criticalChanceMult { get; set; }
	public float itemCriticalChanceMult { get; set; }
	public float enchantmentCriticalChanceMult { get; set; }
	public float criticalChanceMultFinal { get; set; } = 1.0f;

	/*public float criticalDamage { get; set; }
	public float itemCriticalDamage { get; set; }
	public float enchantmentCriticalDamage { get; set; }*/
	public float criticalDamageMult { get; set; }
	public float itemCriticalDamageMult { get; set; }
	public float enchantmentCriticalDamageMult { get; set; }
	public float criticalDamageMultFinal { get; set; } = 1.0f;

	public bool isStunned { get; set; }
	public bool isDebuffed  { get; set; }
	public bool isDead { get; set; }
	public float damageTakedSum { get; set; }

	public float movespeedMult { get; set; }
	public float itemMovespeedMult { get; set; }
	public float enchantmentMovespeedMult { get; set; }
	public float movespeedMultFinal { get; set; } = 1.0f;

	public float attackspeedMult { get; set; }
	public float itemAttackspeedMult { get; set; }
	public float enchantmentAttackspeedMult { get; set; }
	public float attackspeedMultFinal { get; set;} = 1.0f;

	public string name { get; set;}
	public string explanation { get; set;}
	
	public int attackRange { get; set; }
	public int itemAttackRange { get; set; }
	public int enchantmentAttackRange { get; set; } 
	
	
	private Moveto moveto;
	private List<Enchantment> enchantmentList;
	private List<EquipmentEffect> equipmentEffectList;
	private List<Item> itemList;
	
	
	//실제 데미지 계산 =
	//데미지 = 공격력 * 1/(1+( (방어 - 고정방관) * (1- %방관) * 1/100))
	//데미지 입었을때 target에서 user OnDamage 호출.
	
	public void Start()
	{
		moveto = GetComponent<Moveto>();
		enchantmentList = new List<Enchantment>();
		
	}



	public abstract void TakeDamage(float damage, float penetration, Actor from);
	public abstract void TakeDamageFromEnchantment(float damage, float penetration, Actor from, Enchantment enchantment);
	public abstract void Die(Actor Opponent);
	public abstract void TakeHeal(float heal, Actor from);
	public abstract void TakeHealFromEnchantment(float heal, Actor from, Enchantment enchantment);
	public abstract void AddEnchantment(Enchantment enchantment);
	public abstract void RemoveEnchantment(Enchantment enchantment);
	public abstract void TakeStunned(Actor from, Enchantment enchantment, float during);
	
	

	public int GetGold()
	{
		return gold;
	}
	public void SetGold(int _gold)
	{
		gold = _gold;
	}
	public float GetHealthMax()
	{
		return healthMax;
	}
	public float GetCalculatedHealthMax()
	{
		return (healthMax * (1.0f + healthMaxMult)) + (itemHealthMax * (1.0f + itemHealthMaxMult)) + (enchantmentHealthMax * (1.0f + enchantmentHealthMaxMult));
	}
	public void SetHealthMax(float _healthMax)
	{
		healthMax = _healthMax;
	}
	
	public float GetCalculatedAttack()
	{
		//return 0.0f;
		//실제 공식에따라 ~~~ 공격력 리턴
		return ((attack + itemAttack + enchantmentAttack) * (1.0f + attackMult + itemAttackMult + enchantmentAttackMult) * attackMultFinal);

	}
	public float GetCalculatedDefence()
	{
		return ((defence + itemDefence + enchantmentDefence) * (1.0f + defenceMult + itemDefenceMult + enchantmentDefenceMult) * defenceMultFinal);
	}
	public float GetCalculatedPenetration()
	{
		return (1.0f + penetrationMult + itemPenetrationMult + enchantmentPenetrationMult) * penetrationMultFinal;
	}
	public float GetCalculatedAvoid()
	{
		return (1.0f + avoidMult + itemAvoidMult + enchantmentAvoidMult) * avoidMultFinal;
	}
	public float GetCalculatedCriticalChance()
	{
		return (1.0f + criticalChanceMult + itemCriticalChanceMult + enchantmentCriticalChanceMult) * criticalChanceMultFinal;
	}
	public float GetCalculatedCriticalDamage()
	{
		return (1.0f + criticalDamageMult + itemCriticalDamageMult + enchantmentCriticalDamageMult) * criticalDamageMultFinal;
	}
	public float GetCalculatedMovespeed()
	{
		return (1.0f + movespeedMult + itemMovespeedMult + enchantmentMovespeedMult) * movespeedMultFinal;
	}
	public float GetCalculatedAttackspeed()
	{
		return (1.0f + attackspeedMult + itemAttackspeedMult + enchantmentAttackspeedMult) * attackspeedMultFinal;
	}
	public int GetCalculatedAttackRange()
	{
		return attackRange + itemAttackRange + enchantmentAttackRange;
	}
	public Tile GetCurTile()
	{
		//return moveto.cur
		return null; 
	}
	public TileForMove GetCurTileForMove()
	{
		return null;
	}
	
	public List<Actor> GetAdjacentActor(int range)
	{
		List<Actor> adjacentActors = new List<Actor>();
		TileLayer layer = moveto.tileLayer;
		int x = moveto.GetCurPosForMove().GetX();
		int y = moveto.GetCurPosForMove().GetY();
		TileForMove tileForMoveTemp;
		for (int i = -range; i <= range; i++)
		{
			for (int j = -range; j <= range; j++)
			{
				if (Mathf.Abs(i) + Mathf.Abs(j) > range) // + 실제 actor가 타일 위에 있는지
					continue;
				tileForMoveTemp = layer.GetTileForMove(x + i, y + j);
				if(tileForMoveTemp.Equals(tileForMoveTemp.GetRecentActor().moveto.GetCurPosForMove())) // tileForMoveTemp에 기록된 recentActor의 현재위치가 tileForMoveTemp와 일치하는지
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
	
	public float GetCalculatedDamage(float damage, float penetration, Actor from)
	{
		return 0.0f;
		//damage 연산 필요
		
	}
}
