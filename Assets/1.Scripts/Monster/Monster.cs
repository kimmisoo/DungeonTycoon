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
	int chaseMoveCount = 0;
	ActingResult actingResult = new ActingResult();
	TileLayer tileLayer;
	//Coroutine currentCoroutine;
	int turn = 0;
	Direction curDirection = Direction.DownRight;
	//이동시 Monster Tile만 체크

	public override void Start () {
		base.Start();
		spriteRenderer = GetComponent<SpriteRenderer>();
		animator = GetComponent<Animator>();		
	}
	private void OnEnable()
	{
		tileLayer = GetCurTile().GetLayer();
		StartCoroutine(Moving());
		StartCoroutine(Attacking());
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
			while (actingResult.isFoundEnemy == false)
			{
				yield return StartCoroutine(EnemySearch(actingResult));
				if (actingResult.isFoundEnemy == false)
					yield return StartCoroutine(Wander());
				else
					break;
			}
			while(target != null && (target.state == State.Dead || state == State.Dead))
			{
				yield return null;
				SetAnimFlagFromDirection(GetDirectionFromOtherTileForMove(target.GetCurTileForMove()));

				if (GetCurTile().Equals(target.GetCurTile()))
				{
					//1칸 Tileformove 이동
					yield return StartCoroutine(Wander());
				}
				else
				{
					if(GetCalculatedAttackRange() >= GetDistanceFromOtherActorForMove(target))
					{
						//Nothing...
						yield return null;
					}
					else
					{
						TileForMove nextTileForMove = GetNextTileForMoveFromDirection(GetDirectionFromOtherTileForMove(target.GetCurTileForMove()));
						if(nextTileForMove.GetRecentActor().GetCurTileForMove().Equals(nextTileForMove))
						{
							yield return StartCoroutine(Wander());
						}
						else
						{
							SetCurTile(nextTileForMove.GetParent());
							SetCurTileForMove(nextTileForMove);
							
						}
						//타일 차이나면 moveto 이동. 아니면 direction 계산해서 이동
					}
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
		actingResult.Clear();
		while(actingResult.isFoundEnemy == false)
		{
			yield return StartCoroutine(EnemySearch(actingResult));
			if (actingResult.isFoundEnemy == false)
				yield return StartCoroutine(Wander());
		}
		//target에 발각 메소드 호출하기(전투중인지 아닌지 체크)
		//////enemy found
		while(actingResult.isReachEnemy == false && target)
		{
			//yield return StartCoroutine(Chase(actingResult));
		}
		while(actingResult.isDeadEnemy == false && actingResult.isDead == false)
		{
			yield return StartCoroutine(Battle(actingResult));
		}
		if (actingResult.isDead == true)
			yield return StartCoroutine(Respawn(actingResult)); //or monstermanager.AddDeadList
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
	/*
	IEnumerator Chase(ActingResult result)
	{
		yield return null;
		
		for(int i=0; i<7; i++)
		{
			if (target == null || target.isDead || !target.GetCurTileForMove().GetParent().GetMonsterArea() || GetDistanceFromOtherActorForMove(target) >=3)
			{
				result.isReachEnemy = true;
				result.isDeadEnemy = true;
				break; // 상대가 쓰러졌거나 Monster Area를 벗어났을때
			}
			else
			{
				//방향계산 후 한칸씩 이동하면서 추격
				curDirection = GetDirectionFromOtherTileForMove(target.GetCurTileForMove());
				SetAnimFlagFromDirection(curDirection);
				GetNextTileForMoveFromDirection(curDirection);
				
			}
		}
		
	}*/
	IEnumerator Battle(ActingResult result)
	{
		WaitForSeconds interval = new WaitForSeconds(1.0f / (1.0f+GetCalculatedAttackspeed()));
		Attack();
		yield return null;
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

	}
	public override void TakeHealFromEnchantment(float heal, Actor from, Enchantment enchantment)
	{

	}
	public override void AddEnchantment(Enchantment enchantment)
	{

	}
	public override void RemoveEnchantment(Enchantment enchantment)
	{

	}
	public override void TakeStunned(Actor from, Enchantment enchantment, float during)
	{
		
	}
	public  void Attack()
	{
		if(target != null && target.state != State.Dead)
		{
			float dealtDamage = 0.0f;
			bool isCritical = false;
			isCritical = GetIsCriticalAttack();
			foreach(Enchantment e in enchantmentList)
			{
				e.OnAttack(this, target, GetAdjacentActor(GetCalculatedAttackRange()).ToArray(), isCritical);
			}
			

			
			//크리티컬 계산
			//onattack
			//데미지 계산
			//ondamage

			
			//target.TakeDamage(this, )
		}
	}
	public  void Attacked()
	{

	}
	public override float TakeDamage(Actor from, bool isCritical, out bool isHit, out bool isDead)
	{
		isHit = false;
		isDead = false;
		return 0.0f;
	}
	public override float TakeDamageFromEnchantment(float damage, Actor from, Enchantment enchantment, bool isCritical, out bool isHit, out bool isDead)
	{
		isHit = false;
		isDead = false;
		return 0.0f;
	}
	IEnumerator Wander() // 1칸이동
	{
		TileForMove tempTileForMove;
		TileForMove origin = GetCurTileForMove();
		
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
		if (tempTileForMove.GetParent().GetMonsterArea() == true && !tempTileForMove.GetRecentActor().GetCurTileForMove().Equals(tempTileForMove))
		{
			SetCurTileForMove(tempTileForMove);
			SetCurTile(tempTileForMove.GetParent());
			//방향 파악 및 Animation Trigger 설정해야함.

			curDirection = GetDirectionFromOtherTileForMove(tempTileForMove);
			SetAnimFlagFromDirection(curDirection);
			animator.SetBool("MoveFlg", true);
			yield return StartCoroutine(MoveAnim(origin.GetPosition(), tempTileForMove.GetPosition()));
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
