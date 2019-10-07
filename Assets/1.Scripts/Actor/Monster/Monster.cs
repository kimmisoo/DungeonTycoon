using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Monster : Actor, IDamagable {
	
	//Stat 관련 처리. 데미지 처리. 등 ...
	//선공 인식 범위 십자 3칸
	
	
	int monsterCode;
	
	public BattleStatus battleStatus
	{
		get
		{
			return _battleStatus;
		}
	}

	private BattleStatus _battleStatus;
	
	Monster monster;
	Adventurer target;
	Coroutine currentCoroutine;
	WaitForSeconds idleInterval;

	//이동시 Monster Tile만 체크
	public void Awake()
	{
		base.Awake();
		SetBattleStatus(GetComponent<BattleStatus>());
	}
	public void Start()
	{
		idleInterval = new WaitForSeconds(1.0f);
		tileLayer = GetCurTile().GetLayer();
		direction = Direction.DownRight;
		state = State.Idle;
	}
	private void OnEnable()
	{
		//moving = StartCoroutine(Moving());
		//attacking = StartCoroutine(Attacking());
		state = new State();
		state = State.Idle;
		StartCoroutine(Act());
	}
	private void OnDisable()
	{	
		//reset...
	}
	
	public IEnumerator Act()
	{
		yield return null;
		while (true)
		{
			yield return null;
			switch (state)
			{
				case State.Idle:
					yield return currentCoroutine = StartCoroutine(Idle());
					break;
				case State.Chasing:
					yield return currentCoroutine = StartCoroutine(Chasing());
					break;
				case State.Battle:
					yield return currentCoroutine = StartCoroutine(Battle());
					break;
				case State.Dead:
					yield return currentCoroutine = StartCoroutine(Dead());
					break;
			}
		}
	}

	IEnumerator Idle()
	{
		while (state == State.Idle)
		{
			yield return StartCoroutine(EnemySearch());
			if (state != State.Chasing)
				yield return StartCoroutine(Wander());
		}
	}

	IEnumerator Chasing()
	{
		int chaseCount = 0;
		if (target == null)
		{
			state = State.Idle;
		}
		else if (target != null && !(target.GetState() == State.Dead || state == State.Dead) || !target.GetCurTile().GetMonsterArea() || chaseCount > 7)
		{
			//chase once coroutine.
			yield return StartCoroutine(MoveOnce(target.GetCurTileForMove()));
			chaseCount++;
			if (GetDistanceFromOtherActorForMove(target) <= battleStatus.GetCalculatedAttackRange())
			{
				state = State.Battle;
				//adventurer.Startbattle trigger!
			}
		}
		else
		{
			yield return new WaitForSeconds(1.0f);
			state = State.Idle;
		}

		yield return null;
	}
	IEnumerator Battle()
	{
		yield return null;
		StartBattle(target);
		float originAttackSpeed = battleStatus.GetCalculatedAttackspeed();
		float eps = 0.0001f;
		WaitForSeconds interval = new WaitForSeconds(1.0f / originAttackSpeed);
		while (target != null && !(state == State.Dead || target.GetState() == State.Dead))
		{
			if (Mathf.Abs(originAttackSpeed - battleStatus.GetCalculatedAttackspeed()) > eps)
			{
				originAttackSpeed = battleStatus.GetCalculatedAttackspeed();
				interval = new WaitForSeconds(1.0f / originAttackSpeed);
			}
			Attack(target);
			yield return interval;
			//공격속도에 따라 한번씩 공격
			//Callback 제대로 처리하기
			
		}
		//target.StartBattle!
		EndBattle(target);
		if(state == State.Dead)
		{
			Die(target as IDamagable);
		}
		else
		{
			state = State.Idle;
		}
		//둘 중 하나가 죽을때까지 싸움
	}
	IEnumerator Dead()
	{
		yield return null;
		yield return new WaitForSeconds(30.0f);
		
		state = State.Idle;
		//respawn;
		//N초 기다린 후 리스폰
	}

	
	
	
	
	public void SetMonsterCode(int code)
	{
		monsterCode = code;
	}
	public int GetMonsterCode()
	{
		return monsterCode;
	}
	public Adventurer GetCurrentTarget()
	{
		return target;
	}

	public IEnumerator EnemySearch() //trigger state
	{
		target = null;
		yield return null;
		foreach (Actor a in GetAdjacentActor(3))
		{
			if ((a is Adventurer || a is SpecialAdventurer) && a.GetState() != State.Dead)
			{
				if (target == null || GetDistanceFromOtherActorForMove(target) > GetDistanceFromOtherActorForMove(a))
				{
					target = a as Adventurer;
					state = State.Chasing;
				}
			}
		}
	}
	public IEnumerator Wander() // 1칸이동
	{
		TileForMove tempTileForMove;

		int rand = 0;
		//랜덤 돌려서 4방향중 1방향으로.(Monster Area, 체크해야함)
		rand = Random.Range(0, 4);
		switch (rand)
		{
			case 0:
				tempTileForMove = GetCurTile().GetLayer().GetTileForMove(GetCurTileForMove().GetX() - 1, GetCurTileForMove().GetY());
				break;
			case 1:
				tempTileForMove = GetCurTile().GetLayer().GetTileForMove(GetCurTileForMove().GetX() + 1, GetCurTileForMove().GetY());
				break;
			case 2:
				tempTileForMove = GetCurTile().GetLayer().GetTileForMove(GetCurTileForMove().GetX(), GetCurTileForMove().GetY() - 1);
				break;
			case 3:
				tempTileForMove = GetCurTile().GetLayer().GetTileForMove(GetCurTileForMove().GetX(), GetCurTileForMove().GetY() + 1);
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
		if (destination.GetParent().GetMonsterArea())
		{
			TileForMove origin = GetCurTileForMove();
			direction = GetDirectionFromOtherTileForMove(destination);
			SetCurTileForMove(destination);
			SetCurTile(destination.GetParent());
			SetAnimFlagFromDirection(direction);
			animator.SetBool("MoveFlg", true);
			yield return StartCoroutine(MoveAnim(origin.GetPosition(), destination.GetPosition()));
			animator.SetBool("MoveFlg", false);
		}
	}
	public override bool ValidateNextTile(Tile tile)
	{
		return tile.GetMonsterArea();
	}
	public void SetAnimFlagFromDirection(Direction direction)
	{
		switch (direction)
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

	public IEnumerator MoveAnim(Vector3 start, Vector3 end)
	{
		Vector3 d = end - start;
		float mag = 0;

		while (mag <= Vector3.Magnitude(d))
		{
			yield return null;
			mag += Vector3.Magnitude(d * battleStatus.GetCalculatedMovespeed() * Time.deltaTime);
			if (mag >= Vector3.Magnitude(d))
			{
				break;
			}
			transform.Translate(d * battleStatus.GetCalculatedMovespeed() * Time.deltaTime);
		}
	}


	public List<TileForMove> GetPathForMove(List<PathVertex> pathList)
	{
		List<TileForMove> wayForMove = new List<TileForMove>();
		wayForMove.Add(monster.GetCurTileForMove());
		for (int i = 0; i < pathList.Count - 1; i++)
		{
			switch (wayForMove[wayForMove.Count - 1].GetChildNum())
			{
				case 0:
					if (pathList[i + 1].myTilePos.GetX() - pathList[i].myTilePos.GetX() == 1) // move down right
					{
						wayForMove.Add(pathList[i].curTile.GetChild(1));
						wayForMove.Add(pathList[i + 1].curTile.GetChild(0));
						if (Random.Range(0, 2) == 0)
						{
							//3칸가기
							wayForMove.Add(pathList[i + 1].curTile.GetChild(1));
						}
					}
					else if (pathList[i + 1].myTilePos.GetX() - pathList[i].myTilePos.GetX() == -1) // move up left
					{
						wayForMove.Add(pathList[i + 1].curTile.GetChild(1));
						if (Random.Range(0, 2) == 0)
						{
							//2칸가기
							wayForMove.Add(pathList[i + 1].curTile.GetChild(0));
						}
					}
					else if (pathList[i + 1].myTilePos.GetY() - pathList[i].myTilePos.GetY() == 1) // move down left
					{
						wayForMove.Add(pathList[i].curTile.GetChild(2));
						wayForMove.Add(pathList[i + 1].curTile.GetChild(0));
						if (Random.Range(0, 2) == 0)
						{
							wayForMove.Add(pathList[i + 1].curTile.GetChild(2));
							//3칸가기
						}
					}
					else // move up right
					{
						wayForMove.Add(pathList[i + 1].curTile.GetChild(2));
						if (Random.Range(0, 2) == 0)
						{
							wayForMove.Add(pathList[i + 1].curTile.GetChild(0));
							//2칸가기
						}
					}
					break;
				case 1:
					if (pathList[i + 1].myTilePos.GetX() - pathList[i].myTilePos.GetX() == 1) // move down right
					{
						wayForMove.Add(pathList[i + 1].curTile.GetChild(0));
						if (Random.Range(0, 2) == 0)
						{
							wayForMove.Add(pathList[i + 1].curTile.GetChild(1));
							//2칸가기
						}

					}
					else if (pathList[i + 1].myTilePos.GetX() - pathList[i].myTilePos.GetX() == -1) // move up left
					{
						wayForMove.Add(pathList[i].curTile.GetChild(0));
						wayForMove.Add(pathList[i + 1].curTile.GetChild(1));
						if (Random.Range(0, 2) == 0)
						{
							wayForMove.Add(pathList[i + 1].curTile.GetChild(0));
							//2칸가기
						}
					}
					else if (pathList[i + 1].myTilePos.GetY() - pathList[i].myTilePos.GetY() == 1) // move down left
					{
						wayForMove.Add(pathList[i].curTile.GetChild(3));
						wayForMove.Add(pathList[i + 1].curTile.GetChild(1));
						if (Random.Range(0, 2) == 0)
						{
							wayForMove.Add(pathList[i + 1].curTile.GetChild(3));
							//2칸가기
						}

					}
					else // move up right
					{
						wayForMove.Add(pathList[i + 1].curTile.GetChild(3));
						if (Random.Range(0, 2) == 0)
						{
							wayForMove.Add(pathList[i + 1].curTile.GetChild(1));
							//2칸가기
						}


					}
					break;
				case 2:
					if (pathList[i + 1].myTilePos.GetX() - pathList[i].myTilePos.GetX() == 1) // move down right
					{
						wayForMove.Add(pathList[i].curTile.GetChild(3));
						wayForMove.Add(pathList[i + 1].curTile.GetChild(2));
						if (Random.Range(0, 2) == 0)
						{
							wayForMove.Add(pathList[i + 1].curTile.GetChild(3));
							//3칸가기
						}

					}
					else if (pathList[i + 1].myTilePos.GetX() - pathList[i].myTilePos.GetX() == -1) // move up left
					{
						wayForMove.Add(pathList[i + 1].curTile.GetChild(3));
						if (Random.Range(0, 2) == 0)
						{
							wayForMove.Add(pathList[i + 1].curTile.GetChild(2));
							//2칸가기
						}


					}
					else if (pathList[i + 1].myTilePos.GetY() - pathList[i].myTilePos.GetY() == 1) // move down left
					{
						wayForMove.Add(pathList[i + 1].curTile.GetChild(0));
						if (Random.Range(0, 2) == 0)
						{
							wayForMove.Add(pathList[i + 1].curTile.GetChild(2));
							//2칸가기
						}

					}
					else // move up right
					{
						wayForMove.Add(pathList[i].curTile.GetChild(0));
						wayForMove.Add(pathList[i + 1].curTile.GetChild(2));
						if (Random.Range(0, 2) == 0)
						{
							wayForMove.Add(pathList[i + 1].curTile.GetChild(0));
							//3칸가기
						}


					}
					break;
				case 3:
					if (pathList[i + 1].myTilePos.GetX() - pathList[i].myTilePos.GetX() == 1) // move down right
					{
						wayForMove.Add(pathList[i + 1].curTile.GetChild(2));
						if (Random.Range(0, 2) == 0)
						{
							wayForMove.Add(pathList[i + 1].curTile.GetChild(3));
							//2칸가기
						}

					}
					else if (pathList[i + 1].myTilePos.GetX() - pathList[i].myTilePos.GetX() == -1) // move up left
					{
						wayForMove.Add(pathList[i].curTile.GetChild(2));
						wayForMove.Add(pathList[i + 1].curTile.GetChild(3));
						if (Random.Range(0, 2) == 0)
						{
							wayForMove.Add(pathList[i + 1].curTile.GetChild(2));
							//3칸가기
						}
					}
					else if (pathList[i + 1].myTilePos.GetY() - pathList[i].myTilePos.GetY() == 1) // move down left
					{
						wayForMove.Add(pathList[i + 1].curTile.GetChild(1));
						if (Random.Range(0, 2) == 0)
						{
							wayForMove.Add(pathList[i + 1].curTile.GetChild(3));
							//2칸가기
						}

					}
					else // move up right
					{
						wayForMove.Add(pathList[i].curTile.GetChild(1));
						wayForMove.Add(pathList[i + 1].curTile.GetChild(3));
						if (Random.Range(0, 2) == 0)
						{
							wayForMove.Add(pathList[i + 1].curTile.GetChild(1));
							//2칸가기
						}


					}
					break;
				default:
					break;
			}
		}
		return wayForMove;
	}

	public void SetAnimFlgByDirection(Direction dir)
	{
		switch (dir)
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
		}
	}
	public IEnumerator MoveAnim(Vector3 dest)
	{
		yield return null;
		animator.SetBool("MoveFlg", true);
		Vector3 d = dest - monster.transform.position;
		float mag = 0;

		while (mag <= Vector3.Magnitude(d))
		{
			yield return null;
			mag += Vector3.Magnitude(d * battleStatus.GetCalculatedMovespeed() * Time.deltaTime);
			if (mag >= Vector3.Magnitude(d))
			{
				break;
			}
			transform.Translate(d * battleStatus.GetCalculatedMovespeed() * Time.deltaTime);
		}
	}
	
	

	
	void Die(IDamagable Opponent)
	{
		//object 비활성화 및 respawn 준비
		foreach(Enchantment e in enchantmentList)
		{
			e.OnDead(this, Opponent as Actor, GetAdjacentActor(2));
		}
		state = State.Dead;
	}
	#region IDamagable
	public void SetBattleStatus(BattleStatus bs)
	{
		_battleStatus = bs;
	}

	public bool Attack(IDamagable enemy)
	{
		if (enemy != null && (enemy as Adventurer).state != State.Dead)
		{
			float dealtDamage = 0.0f;
			bool isCritical = false;
			bool isHit = false;
			bool isDead = false;
			isCritical = battleStatus.GetIsCriticalAttack();

			enemy.Attacked(this, isCritical, out isHit);
			foreach (Enchantment e in enchantmentList)
			{
				e.OnAttack(this, enemy as Actor, GetAdjacentActor(battleStatus.GetCalculatedAttackRange()), isCritical);
			}
			if (isHit == true)
			{
				enemy.TakeDamage(this, isCritical, out isDead, out dealtDamage);
				foreach (Enchantment e in enchantmentList)
				{
					e.OnDamage(this, enemy as Actor, GetAdjacentActor(2), dealtDamage, isCritical);
				}
			}
			if (isDead)
			{
				EndBattle(enemy);
				state = State.Idle;
			}
			return isHit;
		}
		return false;
	}

	public void Attacked(IDamagable from, bool isCritical, out bool isHit)
	{
		if (Random.Range(0.0f, 1.0f) < battleStatus.GetCalculatedAvoidMult())
			isHit = false;
		else
			isHit = true;
		foreach (Enchantment e in enchantmentList)
		{
			e.OnAttacked(this, from as Actor, GetAdjacentActor(2), isCritical);
		}
		return;
	}

	public void AttackedFromEnchantment(IDamagable from, Enchantment enchantment, bool isCritical, out bool isHit)
	{
		if (Random.Range(0.0f, 1.0f) < battleStatus.GetCalculatedAvoidMult())
			isHit = false;
		else
			isHit = true;
		return;
	}

	public void TakeDamage(IDamagable from, bool isCritical, out bool isDead, out float damage)
	{
		float calculatedDamage = battleStatus.GetCalculatedDamage(from.battleStatus, isCritical);
		damage = calculatedDamage;
		isDead = battleStatus.TakeDamageProcess(calculatedDamage);
		foreach (Enchantment e in enchantmentList)
		{
			e.OnDamaged(this, from as Actor, GetAdjacentActor(2), damage, isCritical);
		}
		if (isDead)
		{
			EndBattle(from);
			Die(from);
			/*state = State.Dead;
			foreach (Enchantment e in enchantmentList)
			{
				e.OnEndBattle(this, from as Actor, GetAdjacentActor(2));
				e.OnDead(this, from as Actor, GetAdjacentActor(2));
			}*/
		}

	}

	public void TakeDamageFromEnchantment(float damage, IDamagable from, Enchantment enchantment, bool isCritical, out bool isDead)
	{
		isDead = battleStatus.TakeDamageProcess(damage);
		if (isDead)
		{
			EndBattle(from);

		}
	}

	public void TakeHeal(float heal, IDamagable from)
	{

	}
	public void TakeHealFromEnchantment(float heal, IDamagable from, Enchantment enchantment)
	{

	}

	public void StartBattle(IDamagable Opponent)
	{
		foreach (Enchantment e in enchantmentList)
		{
			e.OnStartBattle(this, Opponent as Actor, GetAdjacentActor(2));
		}

	}

	public void EndBattle(IDamagable Opponent)
	{
		foreach (Enchantment e in enchantmentList)
		{
			e.OnEndBattle(this, Opponent as Actor, GetAdjacentActor(2));
		}

	}

	public void AddEnchantment(Enchantment enchantment)
	{
		enchantmentList.Add(enchantment);
	}

	public void RemoveEnchantmnet(Enchantment enchantment)
	{
		enchantmentList.Remove(enchantment);
	}

	public void TakeStunned(IDamagable from, Enchantment enchantment, float during)
	{

	}

	public bool IsAttackable(IDamagable enemy)
	{
		if (!(enemy is Adventurer))
			return false;
		if (enemy != null && (enemy as Adventurer).GetCurTile().GetMonsterArea() && GetDistanceFromOtherActorForMove(enemy as Adventurer) <= battleStatus.GetCalculatedAttackRange())
			return true;
		return false;
	}

	public IEnumerator Stun(float duration)
	{
		yield return null;
	}

	#endregion
	
}
