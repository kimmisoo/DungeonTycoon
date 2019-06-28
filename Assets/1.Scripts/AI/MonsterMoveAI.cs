using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterMoveAI : MonoBehaviour, IMoveAI {
	//타겟에 따라 움직이기 or 배회하기
	Monster monster;
	SpriteRenderer spriteRenderer;
	Animator animator;
	WaitForSeconds moveInterval = new WaitForSeconds(1.0f);
	private int chaseMoveCount = 0;
	public void Start()
	{
		spriteRenderer = GetComponent<SpriteRenderer>();
		animator = GetComponent<Animator>();
		monster = GetComponent<Monster>();
	}
	public IEnumerator Moving()
	{
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
			while (target != null && !(target.state == State.Dead || state == State.Dead) || target.GetCurTile().GetMonsterArea() || chaseMoveCount <= 10)
			{
				yield return moveInterval;
				SetAnimFlagFromDirection(GetDirectionFromOtherTileForMove(target.GetCurTileForMove()));

				if (GetCurTile().Equals(target.GetCurTile())) // 겹칠경우
				{
					//1칸 Tileformove 이동
					yield return StartCoroutine(Wander());
				}
				else if (GetCurTileForMove().GetDistance(target.GetCurTileForMove()) <= GetCalculatedAttackRange()) // 공격범위내
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

	IEnumerator Wander() // 1칸이동
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
