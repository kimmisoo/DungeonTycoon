using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Traveler : Actor {
	//acting 구성
	//useStructure ~ 구현
	

	Tile destination = null;
	protected int pathFindCount = 0;
	Coroutine act;
    List<TileForMove> wayToNext;
	protected void Awake()
	{
		base.Awake();
	}
	// Use this for initialization
	void Start () {
		//_stat = GameManager.Instance.GetNewStats(Type Traveler);
		pathFinder.SetValidateTile(ValidateNextTile);
		SetPathFindEvent();
	}
	public void OnEnable()
	{
		act = StartCoroutine(Act());
        SetCurTile(GameManager.Instance.GetRandomEntrance());
        SetCurTileForMove(GetCurTile().GetChild(Random.Range(0, 3)));
	}
	public void OnDisable()
	{
		StopCoroutine(act);
		//골드, 능력치 초기화...  // current , origin 따로둬야할까?
	}
	public Stat stat
	{
		get
		{
			return _stat;
		}
	}

	private Stat _stat;

	IEnumerator Act()
	{
		Structure[] structureListByPref;
        List<TileForMove> wayForNextDest;
		while(true)
		{
			yield return null;
			switch(state)
			{
				case State.Idle:
					structureListByPref = StructureManager.Instance.FindStructureByDesire(stat.GetHighestDesire(), stat); // 1위 욕구에 따라 타입 결정하고 정렬된 건물 List 받아옴 // GC?
                    while (state == State.Idle)
                    {
                        if (pathFindCount > structureListByPref.Length)
                        {
                            destination = structureListByPref[pathFindCount].GetEntrance(); // 목적지 설정
                        }
                        else
                        {
                            state = State.Exit;
                        }
                        yield return StartCoroutine(pathFinder.Moves(curTile, destination));   
                    }
                    break;
                case State.Moving: // path 존재
                    
					//찾은 경로를 통해 1칸씩 이동? 혹은 한번에(코루틴 통해) 이동.
					break;
				case State.Indoor:
					//건물 들어가서 계산을 마치고 invisible로~
					break;
				case State.Exit:
                    destination = GameManager.Instance.GetRandomEntrance();

					break;
				default:
					break;
			}
		}
	}
	
	
	public override void SetPathFindEvent() // Pathfinder Delegate 설정
    {
		pathFinder.SetNotifyEvent(PathFindSuccess, PathFindFail);
	} 

	public void PathFindSuccess() // Pathfinder 길찾기 성공 Delegate
	{
		pathFindCount = 0;
		state = State.Moving;
	}
	public void PathFindFail() // PathFinder 길찾기 실패 Delegate
	{
		pathFindCount++;
	}

	public override bool ValidateNextTile(Tile tile) // Pathfinder delegate
	{
        return tile.GetPassableTraveler();
	}

	public List<TileForMove> GetWay(List<PathVertex> path) // Pathvertex -> TileForMove
    {
        List<TileForMove> tileForMoveWay = new List<TileForMove>();
        Tile t = path[0].myTilePos;

        for(int i= 1; i<path.Count; i++)
        {
            switch(t.GetDirectionFromOtherTile(path[i].myTilePos))
            {
                case Direction.UpRight:

                    break;
            }
        }
    }
    // dX = 1 : UR
    // dX = -1: DL
    // dY = 1 : DR
    // dY = -1: UL




}
