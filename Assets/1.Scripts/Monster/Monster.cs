using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Monster : Actor {
	
	//선공 인식 범위 십자 3칸
	//
	int monsterCode;
	SpriteRenderer spriteRenderer;
	Animator animator;
	Character target;
	WaitForSeconds searchInterval = new WaitForSeconds(2.0f);
	WaitForSeconds moveInterval = new WaitForSeconds(1.0f);
	int chaseMoveCount = 0;
	ActingResult actingResult = new ActingResult();
	TileLayer tileLayer;
	//Coroutine currentCoroutine;
	int turn = 0;
	Direction curDirection = Direction.DownRight;
	Coroutine moving;
	Coroutine attacking;
	//이동시 Monster Tile만 체크

	public override void Start () {
		base.Start();
		spriteRenderer = GetComponent<SpriteRenderer>();
		animator = GetComponent<Animator>();		
	}
	private void OnEnable()
	{
		tileLayer = GetCurTile().GetLayer();
		moving = StartCoroutine(Moving());
		attacking = StartCoroutine(Attacking());
	}
	private void OnDisable()
	{
		
	}
	IEnumerator Moving()
	{
		//enemy check
		//추격, actingResult 관리
		
		while (true)
		{
			actingResult.Clear();
			chaseMoveCount = 0;	
			while (actingResult.isFoundEnemy == false)
			{
				yield return StartCoroutine(EnemySearch(actingResult));
				if (actingResult.isFoundEnemy == false || target == null)
					yield return StartCoroutine(Wander());
				else
					break;
			}
			while(target != null && !(target.state == State.Dead || state == State.Dead) || target.GetCurTile().GetMonsterArea() || chaseMoveCount <= 10)
			{
				yield return moveInterval;
				SetAnimFlagFromDirection(GetDirectionFromOtherTileForMove(target.GetCurTileForMove()));
				
				if (GetCurTile().Equals(target.GetCurTile())) // 겹칠경우
				{
					//1칸 Tileformove 이동
					yield return StartCoroutine(Wander());
				}
				else if(GetCurTileForMove().GetDistance(target.GetCurTileForMove()) <= GetCalculatedAttackRange()) // 공격범위내
				{
					SetAnimFlagFromDirection(GetDirectionFromOtherTileForMove(target.GetCurTileForMove()));
					actingResult.isReachEnemy = true;
					//do nothing
				}
				else // 멀리있을경우
				{
					TileForMove nextTileForMove = GetNextTileForMoveFromDirection(GetDirectionFromOtherTileForMove(target.GetCurTileForMove()));
					if (nextTileForMove.GetRecentActor().GetCurTileForMove().Equals(nextTileForMove))
					{
						//이미 붙어있음
						SetAnimFlagFromDirection(GetDirectionFromOtherTileForMove(target.GetCurTileForMove()));
					}
					else
					{
						SetCurTile(nextTileForMove.GetParent());
						SetCurTileForMove(nextTileForMove);
						MoveOnce(nextTileForMove);//한칸 이동
						chaseMoveCount++;
					}
					//타일 차이나면 moveto 이동. 아니면 direction 계산해서 이동
				}


			}
			
			yield return null;
			
		}
	}
	IEnumerator Attacking()
	{
		//target 있는지 체크
		//있으면 attackrange 검사 후 공격여부
		//battle
		while(true)
		{
			if(target != null && actingResult.isReachEnemy == true)
			{
				//공격루틴
				yield return new WaitForSeconds(1.0f / (1.0f + GetCalculatedAttackspeed()));
				Attack();
			}
			yield return null;
		}
	}
	
	IEnumerator Acting()
	{
		//대기중(3초 1칸)
		//주변 모험가 체크 (2칸)
		//모험가로 이동 or 추격 -> 발각되는순간 target 이동 StopCoroutine. 그자리에 멈춰서게 하고 몬스터가 접근? 모험가가 접근? 중간지점? -> 모험가가 전투중이면 접근, 아니라면 모험가 StartBattle 호출하기
		//전투 // 사정거리 2칸이상일때 도망가야하나?
		//사망 or 승리
		yield return null;
	}
	public IEnumerator EnemySearch(ActingResult result)
	{ 
		target = null;
		int distance = int.MaxValue;
		yield return null;
		foreach (Actor a in GetAdjacentActor(2))
		{
			if ((a is Adventurer || a is SpecialAdventurer) && a.state != State.Dead)
			{
				if (target == null)
				{
					target = a as Character;
					distance = GetDistanceFromOtherActorForMove(target);
					actingResult.isFoundEnemy = true;
				}
				else
				{
					if (distance > GetDistanceFromOtherActorForMove(a))
					{
						target = a as Character;
					}
				}
			}
		}
		
		
	}
	
	IEnumerator Die(ActingResult result)
	{
		yield return null;
	}
	IEnumerator Respawn(ActingResult result)
	{
		yield return null;
	}

	public override void Die(Actor Opponent)
	{
		
	}
	public override void TakeHeal(float heal, Actor from)
	{
		currentHealth = Mathf.Min(GetCalculatedHealthMax(), currentHealth + heal);
	}
	public override void TakeHealFromEnchantment(float heal, Actor from, Enchantment enchantment)
	{
		currentHealth = Mathf.Min(GetCalculatedHealthMax(), currentHealth + heal);
	}
	public override void AddEnchantment(Enchantment enchantment)
	{
		enchantmentList.Add(enchantment);
	}
	public override void RemoveEnchantment(Enchantment enchantment)
	{
		enchantmentList.Remove(enchantment);
	}
	public override void TakeStunned(Actor from, Enchantment enchantment, float during)
	{
		
	}
	public override void Attack()
	{
		if(target != null && target.state != State.Dead)
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
				target.TakeDamage(this, isCritical, out isHit, out isDead, out dealtDamage);
				foreach (Enchantment e in enchantmentList)
				{
					e.OnDamage(this, target, GetAdjacentActor(2).ToArray(), dealtDamage, isCritical);
				}
			}
		}
	}
	public override void Attacked(Actor from, bool isCritical, out bool isHit)
	{
		float rand = Random.Range(0.0f, 100.0f);
		if(rand < GetCalculatedAvoidMult())
		{
			isHit = false;
			return;
		}
		isHit = true;
		foreach(Enchantment e in enchantmentList)
		{
			e.OnAttacked(this, target, GetAdjacentActor(GetCalculatedAttackRange()).ToArray(), isCritical);
		}
	}
	public override void AttackedFromEnchantment(Actor from, Enchantment enchantment, bool isCritical, out bool isHit)
	{
		float rand = Random.Range(0.0f, 100.0f);
		if (rand < GetCalculatedAvoidMult())
		{
			isHit = false;
			return;
		}
		isHit = true;
	} // 인챈트공격 회피 판정
	public override void TakeDamage(Actor from, bool isCritical, out bool isDead, out float damage)
	{
		//데미지 계산
		damage = GetCalculatedDamage(from);
		currentHealth = Mathf.Max(0.0f, currentHealth - damage);
		if(currentHealth <= 0.0f)
		{
			state = State.Dead;
			isDead = true;
			//Dead Process
		}
		else
		{
			isDead = false;
			foreach(Enchantment e in enchantmentList)
			{
				e.OnDamaged(this, from, GetAdjacentActor(GetCalculatedAttackRange()).ToArray(), damage, isCritical);
			}
		}
	}
	public override void TakeDamageFromEnchantment(float damage, Actor from, Enchantment enchantment, bool isCritical, out bool isDead)
	{
		isDead = false;
		currentHealth = Mathf.Max(0.0f, currentHealth - damage);
		if (currentHealth <= 0.0f)
		{
			state = State.Dead;
			isDead = true;
			//Dead Process
		}
		else
		{
			isDead = false;
		}
	}
	IEnumerator Wander() // 1칸이동
	{
		TileForMove tempTileForMove;
		
		int rand = 0;
		//랜덤 돌려서 4방향중 1방향으로.(Monster Area, 체크해야함)
		rand = Random.Range(0, 4);
		switch(rand)
		{
			case 0:
				tempTileForMove = GetCurTile().GetLayer().GetTileForMove(GetCurTileForMove().GetX() - 1, GetCurTileForMove().GetY());
				break;
			case 1:
				tempTileForMove = GetCurTile().GetLayer().GetTileForMove(GetCurTileForMove().GetX() + 1, GetCurTileForMove().GetY());
				break;
			case 2:
				tempTileForMove = GetCurTile().GetLayer().GetTileForMove(GetCurTileForMove().GetX() , GetCurTileForMove().GetY() - 1);
				break;
			case 3:
				tempTileForMove = GetCurTile().GetLayer().GetTileForMove(GetCurTileForMove().GetX() , GetCurTileForMove().GetY() + 1);
				break;
			default:
				tempTileForMove = GetCurTileForMove();
				break;
		}
		if (!tempTileForMove.GetRecentActor().GetCurTileForMove().Equals(tempTileForMove))
		{
			yield return StartCoroutine(MoveOnce(tempTileForMove));
		}
	}
	IEnumerator MoveOnce(TileForMove destination)
	{ 
		if(destination.GetParent().GetMonsterArea())
		{
			TileForMove origin = GetCurTileForMove();
			curDirection = GetDirectionFromOtherTileForMove(destination);
			SetCurTileForMove(destination);
			SetCurTile(destination.GetParent());
			SetAnimFlagFromDirection(curDirection);
			animator.SetBool("MoveFlg", true);
			yield return StartCoroutine(MoveAnim(origin.GetPosition(), destination.GetPosition()));
			animator.SetBool("MoveFlg", false);
		}
	}
	public IEnumerator MoveAnim(Vector3 start, Vector3 end)
	{
		Vector3 d = end - start;
		float mag = 0;

		while (mag <= Vector3.Magnitude(d))
		{
			yield return null;
			mag += Vector3.Magnitude(d * GetCalculatedMovespeed() * Time.deltaTime);
			if (mag >= Vector3.Magnitude(d))
			{
				break;
			}
			transform.Translate(d * GetCalculatedMovespeed() * Time.deltaTime);
		}
	}
	public void SetAnimFlagFromDirection(Direction direction)
	{
		switch (curDirection)
		{
			case Direction.DownRight:
				spriteRenderer.flipX = true;
				animator.SetTrigger("UpToDownFlg");
				break;
			case Direction.UpRight:
				spriteRenderer.flipX = true;
				animator.SetTrigger("DownToUpFlg");
				break;
			case Direction.DownLeft:
				spriteRenderer.flipX = false;
				animator.SetTrigger("UpToDownFlg");
				break;
			case Direction.UpLeft:
				spriteRenderer.flipX = false;
				animator.SetTrigger("DownToUpFlg");
				break;
			case Direction.None:
				break;
			default:
				break;
		}
	}
	public TileForMove GetNextTileForMoveFromDirection(Direction direction)
	{
		//TileForMove tempTileForMove;
		switch(curDirection)
		{
			case Direction.DownRight:
				return tileLayer.GetTileForMove(GetCurTile().GetX() + 1, GetCurTile().GetY());
				break;
			case Direction.UpRight:
				return tileLayer.GetTileForMove(GetCurTile().GetX(), GetCurTile().GetY() - 1);
				break;
			case Direction.DownLeft:
				return tileLayer.GetTileForMove(GetCurTile().GetX() + 1, GetCurTile().GetY());
				break;
			case Direction.UpLeft:
				return tileLayer.GetTileForMove(GetCurTile().GetX() - 1, GetCurTile().GetY());
				break;
			case Direction.None:
				return GetCurTileForMove();
				break;
			default:
				return GetCurTileForMove();
				break;
		}
	}

	


}
