using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

public class SpecialAdventurer : Character {

	enum Type
	{ Human, Elf, Dwarf, Orc, Dog, Cat }
	Type type;
	public Tile dest;
	const int _typeSize = 6;
	public int typeSize { get { return _typeSize; } }
	Structure destStructure;
	Structure blocked; // 길 막혔을때 -- 처음 막힌건물
	List<Moveto.PathVertex> way;
	List<TileForMove> wayForMove;
	int curChildIndex = -1;

	Monster monster;
	WaitForSeconds wait;
	Transform mTransform;
	
	protected override void Start()
	{
		base.Start();//
		mTransform = this.transform;
		wait = new WaitForSeconds(1.0f);
		wayForMove = new List<TileForMove>();
	}
	private void Update()
	{
		//Debug.Log(animator.GetCurrentAnimatorStateInfo(0).fullPathHash + " / " +animator.GetCurrentAnimatorStateInfo(0).normalizedTime);
	}
	protected override void Init()
	{
		base.Init();
		moveSpeed = 1.0f;///////
		type = (Type)Random.Range(0, typeSize);
	}
	protected override void Activate()
	{
		Actor a;
		
	}
	void OnEnable()
	{
		Init();
		Activate();
		//damage = 5;
		//health = 100;
		StartCoroutine("Acting");
	}
	void OnDisable()
	{
		Deactivate();
		
	}
	protected override void Deactivate()
	{
		//transform.position = new Vector3(5000.0f, 5000.0f, 5000.0f);
		//저장해놔야 할것들이라던지 ~~
		//

		gameObject.transform.position = new Vector3(5000.0f, 5000.0f, 5000.0f);
		curChildIndex = -1;
	}
	
	protected override IEnumerator Acting()
	{
		int index = 0;
		List<Structure> strs = StructureManager.Instance.structures;
		TileForMove dest;
		moveto.isAdventurer = true;
		moveto.SetCurPos(GameManager.Instance.GetRandomEntrance());
		int wanderCount = 0;
		while (true)
		{
			yield return null;
			do
			{
				moveto.destination = map.GetLayer(0).GetComponent<TileLayer>().GetTileAsComponent(Random.Range((int)0, (int)50), Random.Range((int)0, (int)50)); // 범위 수정해줘야ㅏ함.
				yield return null;
			} while (moveto.destination == null || !moveto.destination.GetPassable());
			//Debug.Log(moveto.destination.GetX() + " , " + moveto.destination.GetY() + " !!!!!");
			yield return StartCoroutine("MovetoTarget");

			if (destStructure != null)
			{
				GameManager.Instance.AddGold(destStructure.charge);
				destStructure = null;
			}

			wanderCount++;
			//Debug.Log("WnaderCount - " + wanderCount);
			if (wanderCount >= 10)
				break;
		}

		/////////Exit Map;;
		//Debug.Log("EXIT!!!!!");
		moveto.destination = GameManager.Instance.GetRandomEntrance();
		yield return StartCoroutine("MovetoTarget");

		gameObject.SetActive(false);


	}
	
	IEnumerator Fight()
	{
		yield return null;
			
	}

	IEnumerator MovetoTarget()
	{


		// init 할때 설정

		//acting 영역


		

		yield return StartCoroutine(moveto.Moves());
		////////////////////////////////////////////
		way = moveto.GetPath();
		if (way == null || moveto.isNoPath == true)
		{
			yield return StartCoroutine(moveto.MoveinNoPath());
		}
		//Debug.Log("haaa");

		//길만들기
		if(curChildIndex == -1)
			curChildIndex = Random.Range(0, 4);

		wayForMove.Clear();
		wayForMove.Add(way[0].curTile.GetChild(curChildIndex));
		
		//직선이동시 한번에 벡터계산 x //이동시 공격받을때도 생각해야함!
		for(int i=0; i<way.Count - 1; i++)
		{
			switch(wayForMove[wayForMove.Count - 1].GetChildNum())
			{
				case 0:
					if (way[i + 1].myTilePos.GetX() - way[i].myTilePos.GetX() == 1) // move down right
					{
						wayForMove.Add(way[i].curTile.GetChild(1));
						wayForMove.Add(way[i + 1].curTile.GetChild(0));
						if (Random.Range(0, 2) == 0)
						{
							//3칸가기
							wayForMove.Add(way[i + 1].curTile.GetChild(1));
						}
					}
					else if (way[i + 1].myTilePos.GetX() - way[i].myTilePos.GetX() == -1) // move up left
					{
						wayForMove.Add(way[i + 1].curTile.GetChild(1));
						if (Random.Range(0, 2) == 0)
						{
							//2칸가기
							wayForMove.Add(way[i + 1].curTile.GetChild(0));
						}
					}
					else if (way[i + 1].myTilePos.GetY() - way[i].myTilePos.GetY() == 1) // move down left
					{
						wayForMove.Add(way[i].curTile.GetChild(2));
						wayForMove.Add(way[i + 1].curTile.GetChild(0));
						if (Random.Range(0, 2) == 0)
						{
							wayForMove.Add(way[i + 1].curTile.GetChild(2));
							//3칸가기
						}
					}
					else // move up right
					{
						wayForMove.Add(way[i + 1].curTile.GetChild(2));
						if (Random.Range(0, 2) == 0)
						{
							wayForMove.Add(way[i + 1].curTile.GetChild(0));
							//2칸가기
						}
					}
					break;
				case 1:
					if (way[i + 1].myTilePos.GetX() - way[i].myTilePos.GetX() == 1) // move down right
					{
						wayForMove.Add(way[i + 1].curTile.GetChild(0));
						if (Random.Range(0, 2) == 0)
						{
							wayForMove.Add(way[i + 1].curTile.GetChild(1));
							//2칸가기
						}
					
					}
					else if (way[i + 1].myTilePos.GetX() - way[i].myTilePos.GetX() == -1) // move up left
					{
						wayForMove.Add(way[i].curTile.GetChild(0));
						wayForMove.Add(way[i + 1].curTile.GetChild(1));
						if (Random.Range(0, 2) == 0)
						{
							wayForMove.Add(way[i + 1].curTile.GetChild(0));
							//2칸가기
						}
					}
					else if (way[i + 1].myTilePos.GetY() - way[i].myTilePos.GetY() == 1) // move down left
					{
						wayForMove.Add(way[i].curTile.GetChild(3));
						wayForMove.Add(way[i + 1].curTile.GetChild(1));
						if (Random.Range(0, 2) == 0)
						{
							wayForMove.Add(way[i + 1].curTile.GetChild(3));
							//2칸가기
						}
						
					}
					else // move up right
					{
						wayForMove.Add(way[i + 1].curTile.GetChild(3));
						if (Random.Range(0, 2) == 0)
						{
							wayForMove.Add(way[i + 1].curTile.GetChild(1));
							//2칸가기
						}
						

					}
					break;
				case 2:
					if (way[i + 1].myTilePos.GetX() - way[i].myTilePos.GetX() == 1) // move down right
					{
						wayForMove.Add(way[i].curTile.GetChild(3));
						wayForMove.Add(way[i + 1].curTile.GetChild(2));
						if (Random.Range(0, 2) == 0)
						{
							wayForMove.Add(way[i + 1].curTile.GetChild(3));
							//3칸가기
						}
						
					}
					else if (way[i + 1].myTilePos.GetX() - way[i].myTilePos.GetX() == -1) // move up left
					{
						wayForMove.Add(way[i + 1].curTile.GetChild(3));
						if (Random.Range(0, 2) == 0)
						{
							wayForMove.Add(way[i + 1].curTile.GetChild(2));
							//2칸가기
						}
						

					}
					else if (way[i + 1].myTilePos.GetY() - way[i].myTilePos.GetY() == 1) // move down left
					{
						wayForMove.Add(way[i + 1].curTile.GetChild(0));
						if (Random.Range(0, 2) == 0)
						{
							wayForMove.Add(way[i + 1].curTile.GetChild(2));
							//2칸가기
						}
						
					}
					else // move up right
					{
						wayForMove.Add(way[i].curTile.GetChild(0));
						wayForMove.Add(way[i + 1].curTile.GetChild(2));
						if (Random.Range(0, 2) == 0)
						{
							wayForMove.Add(way[i + 1].curTile.GetChild(0));
							//3칸가기
						}
						

					}
					break;
				case 3:
					if (way[i + 1].myTilePos.GetX() - way[i].myTilePos.GetX() == 1) // move down right
					{
						wayForMove.Add(way[i + 1].curTile.GetChild(2));
						if (Random.Range(0, 2) == 0)
						{
							wayForMove.Add(way[i + 1].curTile.GetChild(3));
							//2칸가기
						}
						
					}
					else if (way[i + 1].myTilePos.GetX() - way[i].myTilePos.GetX() == -1) // move up left
					{
						wayForMove.Add(way[i].curTile.GetChild(2));
						wayForMove.Add(way[i + 1].curTile.GetChild(3));
						if (Random.Range(0, 2) == 0)
						{
							wayForMove.Add(way[i + 1].curTile.GetChild(2));
							//3칸가기
						}
					}
					else if (way[i + 1].myTilePos.GetY() - way[i].myTilePos.GetY() == 1) // move down left
					{
						wayForMove.Add(way[i + 1].curTile.GetChild(1));
						if (Random.Range(0, 2) == 0)
						{
							wayForMove.Add(way[i + 1].curTile.GetChild(3));
							//2칸가기
						}
						
					}
					else // move up right
					{
						wayForMove.Add(way[i].curTile.GetChild(1));
						wayForMove.Add(way[i + 1].curTile.GetChild(3));
						if (Random.Range(0, 2) == 0)
						{
							wayForMove.Add(way[i + 1].curTile.GetChild(1));
							//2칸가기
						}
						

					}
					break;
				default:
					break;
			}
		}
		//


		mTransform.position = wayForMove[0].GetPosition() + Vector3.up * 0.166f;//moveto.myCurPos.GetPosition() + Vector3.up * 0.166f;

		int pre = 0;
		
		curState = State.Move;
		//Move 시작
		//Debug.Log("Count - " + moveto.GetPath().Count);
		for (int i = 0; i < wayForMove.Count - 2; i++)
		{

			//yield return new WaitForSeconds(0.1f);
			//Debug.Log("haaa2");

			if (wayForMove[i + 1].GetX() - wayForMove[i].GetX() == 1) // move down right
			{

				//spriteRenderer.flipX = true;
				foreach (SpriteRenderer sr in spriteRenderers)
				{
					sr.flipX = true;
				}
				animator.SetTrigger("UpToDownFlg");
				animator.SetBool("MoveFlg", true);
				/*if (pre != 1 && pre != 3)
					animator.SetTrigger("MDL");*/
				pre = 1;


			}
			else if (wayForMove[i + 1].GetX() - wayForMove[i].GetX() == -1) // move up left
			{

				//spriteRenderer.flipX = false;
				foreach (SpriteRenderer sr in spriteRenderers)
				{
					sr.flipX = false;
				}
				animator.SetTrigger("DownToUpFlg");
				animator.SetBool("MoveFlg", true);
				/*if (pre != 2 && pre != 4)
					animator.SetTrigger("MUL");*/
				pre = 2;
			}
			else if (wayForMove[i + 1].GetY() - wayForMove[i].GetY() == 1) // move down left
			{
				//spriteRenderer.flipX = false;
				foreach (SpriteRenderer sr in spriteRenderers)
				{
					sr.flipX = false;
				}
				animator.SetTrigger("UpToDownFlg");
				animator.SetBool("MoveFlg", true);
				/*if (pre != 3 && pre != 1)
					animator.SetTrigger("MDL");*/
				pre = 3;
			}
			else // move up right
			{

				//spriteRenderer.flipX = true;
				foreach (SpriteRenderer sr in spriteRenderers)
				{
					sr.flipX = true;
				}
				animator.SetTrigger("DownToUpFlg");
				animator.SetBool("MoveFlg", true);
				/*if (pre != 4 && pre != 2)
					animator.SetTrigger("MUL");*/

				pre = 4;
			}

			if (moveto.isNoPath == true && moveto.tileLayer.GetTileAsComponent(way[i + 1].myTilePos.GetX(), way[i + 1].myTilePos.GetY()).GetPassable() == false)
			{
				blocked = moveto.tileLayer.GetTileAsComponent(way[i + 1].myTilePos.GetX(), way[i + 1].myTilePos.GetY()).GetStructure();
				break;
			}

			yield return StartCoroutine(anim(wayForMove[i].GetPosition(), wayForMove[i + 1].GetPosition()));//
			
			//yield return wait;
			moveto.SetCurPos(wayForMove[i + 1].GetParent());
			moveto.SetCurPosForMove(wayForMove[i + 1]);
			wayForMove[i + 1].SetRecentActor(this);
			curChildIndex = wayForMove[i + 1].GetChildNum();
			mTransform.position = wayForMove[i + 1].GetPosition() + Vector3.up * 0.166f;//moveto.GetCurPos().GetPosition() + Vector3.up * 0.166f;

		}

		/*
		// idle 상태.. 
		if (pre == 1) // down right
		{
			spriteRenderer.flipX = true;
			animator.SetTrigger("IDL");
		}
		else if (pre == 2) // up left
		{
			spriteRenderer.flipX = false;
			animator.SetTrigger("IUL");
		}
		else if (pre == 3) // down left
		{
			spriteRenderer.flipX = false;
			animator.SetTrigger("IDL");
		}
		else // up right
		{
			spriteRenderer.flipX = true;
			animator.SetTrigger("IUL");
		}
		*/
		animator.SetBool("MoveFlg", false);
		
		if (moveto.isNoPath)
		{
			yield return StartCoroutine(SitIn(blocked));
			moveto.isNoPath = false;
		}
		//Debug.Log("haaa3");
		
		////////////////////////////////////
	}
	IEnumerator SitIn(Structure s)
	{
		if (s != null)
			s.sitInCount++;
		while (s != null && s.sitInCount < 3)
		{
			yield return null;
		}
		if (s != null)
			StructureManager.Instance.DestroyStructure(s);

	}
	IEnumerator anim(Vector3 start, Vector3 end)
	{
		
		Vector3 d = end - start;
		float mag = 0;
		
		while(mag <= Vector3.Magnitude(d))
		{
			yield return null;
			mag += Vector3.Magnitude(d * moveSpeed * Time.deltaTime);
			if(mag >= Vector3.Magnitude(d))
			{
				break;
			}
			transform.Translate(d * moveSpeed * Time.deltaTime);
		}
		
	}

	public void GetDamage()
	{

	}
	public void GetEnchantmentEffect()
	{

	}
}
