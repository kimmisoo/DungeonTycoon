﻿using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

public delegate void NotifyToActor();
public delegate bool TileValidation(Tile tile);

public class PathFinder : MonoBehaviour
{
    // 길찾기 성공시 호출할 Delegate
    NotifyToActor pathFindSuccess;
    // 길찾기 실패시 호출할 Delegate
    NotifyToActor pathFindFail;
    // 타일이 passable인지 확인할 Delegate
    TileValidation validateTile;

    // A* 오픈 리스트용 구조체
    public struct openListVisited
    {
        public bool isVisited;
        public bool isClosed;
        public int F; // 아마 A* 계산값일듯
        public openListVisited(bool _isVisited, bool _isClosed, int _F)
        {
            isVisited = _isVisited;
            isClosed = _isClosed;
            F = _F;
        }
    }

    public bool isNoPath = false;

    // 방향 체크용
    enum Direction { left, right, up, down };
    static Vector2[] DirectionVectors = { new Vector2(-1.0f, 0.0f), new Vector2(1.0f, 0.0f), new Vector2(0.0f, 1.0f), new Vector2(0.0f, -1.0f) };
    
    // A* 클로즈 리스트
    Dictionary<int, PathVertex> closeList = new Dictionary<int, PathVertex>();
    // A* 오픈 리스트
    PriorityQueue<PathVertex> openList = new PriorityQueue<PathVertex>();
    // 지금까지의 path를 저장하는듯?
    List<PathVertex> path = new List<PathVertex>();
    // 뭔지 모르겠음. 전체 타일맵?
    public openListVisited[,] visited;

    int bestKey = 0;
    PathVertex latest = null;
    PathVertex best = null;
    PathVertex latest_simulate = null;

    // 현재 타일
    public Tile myCurPos;
    // 목적지 타일
    public Tile destination;
    // 다음 타일?
    public Tile nextMovePos;
    Tile next;

    public TileLayer tileLayer; //for Test
    bool isCal = false;
    public bool isEnd = false;
    System.Random rd = new System.Random();
    public int id;
    int index = 0;
    int parentIndex = 0;
    WaitForSeconds wait;
    ThreadStart ts;
    Thread t;
    PathVertex tempVertex;

    void Start()
    {
        // 게임 매니저에서 타일맵 받기
        TileMap tempTileMap = GameManager.Instance.GetMap();
        Debug.Log("타일맵? " + tempTileMap);
        // 타일레이어 받기
        tileLayer = tempTileMap.GetLayer(0).GetComponent<TileLayer>();
        Debug.Log("타일레이어? " + tileLayer);
        id = gameObject.GetInstanceID();

        wait = new WaitForSeconds(0.5f);

        // visited를 타일맵 x, y 만큼의 크기를 갖는 2차배열로 초기화
        visited = new openListVisited[GameManager.Instance.GetMap().GetLayer(0).GetComponent<TileLayer>().GetLayerHeight(), GameManager.Instance.GetMap().GetLayer(0).GetComponent<TileLayer>().GetLayerWidth()];

        // 전체 타일에 대해서 방문했나, 클로즈드인가 저장하는 듯
        for (int i = 0; i < GameManager.Instance.GetMap().GetLayer(0).GetComponent<TileLayer>().GetLayerHeight(); i++)
        {
            for (int j = 0; j < GameManager.Instance.GetMap().GetLayer(0).GetComponent<TileLayer>().GetLayerWidth(); j++)
            {
                visited[i, j] = new openListVisited(false, false, 0);
            }
        }
    }

    // 이 부분에서 에러가 남.
    public IEnumerator Moves(Tile pcurPos, Tile pdestination)
    {
        yield return null;
        myCurPos = pcurPos;
        destination = pdestination;

        // 리스트 및 큐들 초기화
        for (int i = 0; i < path.Count; i++)
        {
            path[i].ClearReference();
        }
        path.Clear();
        closeList.Clear();
        openList.HalfClear(); // Clear도 있는데 왜 HalfClear인지
        ClearVisited();
        isEnd = false;
        ThreadPool.UnsafeQueueUserWorkItem(this.Simulate, null); // 이거 볼 차례
        while (isEnd == false)
        {
            //Debug.Log("found == false While looop");
            yield return wait;
        }

        PathVertex trace = latest;
        while (trace != null)
        {
            path.Add(trace);
            trace = trace.Parent;
            yield return null;
        }
        path.Reverse();

        closeList.Clear();
        for (int i = 0; i < openList.Count; i++)
        {
			Debug.Log(i);
            openList[i].ClearReference(); // NullPointerException 이 나는 곳.
        }
        /*for (int i = 0; i < closeList.Count; i++)
        {
            closeList[i].ClearReference();
        }*/
        //delegate call
        if (isNoPath)
        {
            isNoPath = false;
            pathFindFail();
        }
        else
        {
            pathFindSuccess();
        }
    }

    public void Simulate(System.Object threadContext)
    {
        latest_simulate = new PathVertex(null, myCurPos, destination);
        bestKey = latest_simulate.X * 1000 + latest_simulate.Y;
        closeList.Add(latest_simulate.X * 1000 + latest_simulate.Y, latest_simulate);

        visited[latest_simulate.Y, latest_simulate.X].isClosed = true;
        Debug.Log("in");
        while (!latest_simulate.myTilePos.Equals(destination))
        {
            for (int i = 0; i < 4; i++)
            {
                if ((next = tileLayer.GetTileAsComponent((int)(latest_simulate.myTilePos.GetX() + DirectionVectors[i].x), (int)(latest_simulate.myTilePos.GetY() + DirectionVectors[i].y))) != null && next.GetPassable())
                {
                    if (visited[next.GetY(), next.GetX()].isClosed == false)
                    {
                        if (visited[next.GetY(), next.GetX()].isVisited == false)
                        {
                            if (openList.Count <= openList.useCount)
                            {
                                openList.Add(tempVertex = new PathVertex(latest_simulate, next, destination));
                                visited[next.GetY(), next.GetX()].isVisited = true;
                                visited[next.GetY(), next.GetX()].F = tempVertex.F;
                            }
                            else
                            {
                                openList[openList.useCount].ReUse(latest_simulate, next, destination);
                                tempVertex = openList[openList.useCount];
                                index = openList.useCount;
                                openList.useCount++;
                                bool keepGoing = true;

                                while (keepGoing)
                                {
                                    if (index == 0) break;
                                    parentIndex = (index - 1) / 2;
                                    keepGoing = openList.CompareAndSwap(index, parentIndex);
                                    if (keepGoing) index = parentIndex;
                                }
                                visited[next.GetY(), next.GetX()].isVisited = true;
                                visited[next.GetY(), next.GetX()].F = tempVertex.F;
                            }
                        }
                        else // visited 있는경우 - 최적의 값 비교.
                        {
                            // openList안에서 찾는것이 우선 . . .  F (F = G + H / 현재까지 온 거리 +남은 거리 최소값) 기준으로 정렬됨.

                            if (visited[next.GetY(), next.GetX()].F > (latest_simulate.G + 1 + (Mathf.Abs(destination.GetX() - next.GetX()) + Mathf.Abs(destination.GetY() - next.GetY()))))
                            {
                                bool find = false;
                                for (int u = 0; u < openList.useCount; u++)
                                {
                                    if (openList[u].X == next.GetX() && openList[u].Y == next.GetY())
                                    {
                                        find = true;
                                        index = u;
                                        break;
                                    }
                                }

                                if (find == true)
                                {
                                    openList.RemoveAt(index);

                                    if (openList.Count <= openList.useCount)
                                    {
                                        openList.Add(tempVertex = new PathVertex(latest_simulate, next, destination));
                                        visited[next.GetY(), next.GetX()].isVisited = true;
                                        visited[next.GetY(), next.GetX()].F = tempVertex.F;
                                    }
                                    else
                                    {
                                        //items.Add(item); 
                                        //reuse 할때도 priority queue ~~~
                                        openList[openList.useCount].ReUse(latest_simulate, next, destination);
                                        tempVertex = openList[openList.useCount];
                                        index = openList.useCount;
                                        openList.useCount++;
                                        bool keepGoing = true;

                                        while (keepGoing)
                                        {
                                            if (index == 0) break;
                                            parentIndex = (index - 1) / 2;
                                            keepGoing = openList.CompareAndSwap(index, parentIndex);
                                            if (keepGoing) index = parentIndex;
                                        }
                                        visited[next.GetY(), next.GetX()].isVisited = true;
                                        visited[next.GetY(), next.GetX()].F = tempVertex.F;
                                    }
                                }

                            }
                        }
                    }
                }
            }
            if (openList.useCount != 0)
            {
                best = openList.Pop();
                bestKey = best.X * 1000 + best.Y;
            }
            else
            {
                isNoPath = true;
                isEnd = true;
                //not found
                break;
            }
            if (!closeList.ContainsKey(bestKey))
            {
                closeList.Add(bestKey, best);
                best.myTilePos.AddedCloseList();
            }
            latest_simulate = best;
        }
        latest = latest_simulate;
        isEnd = true;
    }

    void AddOpenList(PathVertex newVertex)
    {
        if (closeList.ContainsKey(newVertex.X * 1000 + newVertex.Y))
        {
            return;
        }
        openList.Add(newVertex);
        newVertex.myTilePos.AddedOpenList();
    }

    public void SetCurTile(Tile t)
    {
        myCurPos = t;
    }

    public Tile GetDestination()
    {
        return destination;
    }
    public void SetDestination(Tile t)
    {
        destination = t;
    }
    public List<PathVertex> GetPath()
    {
        return path;
    }
    /*
    public List<Tile> GetTilePath()
    {
        List<Tile> tilePath = new List<Tile>();
        foreach (PathVertex vertex in path)
        {
            tilePath.Add(vertex.myTilePos);
        }
        return tilePath;
    }*/
	public void ClearVisited()
	{
		for (int i = 0; i < visited.GetLength(0); i++)
		{
			for (int j = 0; j < visited.GetLength(1); j++)
			{
				visited[i, j].isClosed = false;
				visited[i, j].isVisited = false;
			}
		}
	}
	public void SetNotifyEvent(NotifyToActor success, NotifyToActor fail)
	{
		pathFindSuccess = success;
		pathFindFail = fail;
	}
	public void SetValidateTile(TileValidation validatingFunc)
	{
		validateTile = validatingFunc;
	}
}

