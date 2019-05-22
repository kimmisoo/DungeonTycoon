using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Adventurer : Character
{

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
	enum State
	{
		Searching, Chasing, Battle, Dead
	}
	Direction curDirection = Direction.DownRight;
	//이동시 Monster Tile만 체크

	public override void Start()
	{
		base.Start();
		spriteRenderer = GetComponent<SpriteRenderer>();
		animator = GetComponent<Animator>();
	}
	private void OnEnable()
	{
		tileLayer = GetCurTile().GetLayer();
		StartCoroutine(Acting());
	}
	private void OnDisable()
	{

	}
	IEnumerator Acting()
	{
		//대기중(3초 1칸)
		//주변 모험가 체크 (2칸)
		//모험가로 이동 or 추격 -> 발각되는순간 target 이동 StopCoroutine. 그자리에 멈춰서게 하고 몬스터가 접근? 모험가가 접근? 중간지점? -> 모험가가 전투중이면 접근, 아니라면 모험가 StartBattle 호출하기
		//전투 // 사정거리 2칸이상일때 도망가야하나?
		//사망 or 승리
		actingResult.Clear();
		while (actingResult.isFoundEnemy == false)
		{
			//yield return StartCoroutine(EnemySearch(actingResult));
			if (actingResult.isFoundEnemy == false)
				yield return StartCoroutine(Wander());
		}
		//target에 발각 메소드 호출하기(전투중인지 아닌지 체크)
		//////enemy found
		while (actingResult.isReachEnemy == false)
		{
			//yield return StartCoroutine(Chase(actingResult));
		}
		while (actingResult.isDeadEnemy == false)
		{
			yield return StartCoroutine(Battle(actingResult));
		}


		yield return null;
	}
	
	IEnumerator Battle(ActingResult result)
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
		switch (curDirection)
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
