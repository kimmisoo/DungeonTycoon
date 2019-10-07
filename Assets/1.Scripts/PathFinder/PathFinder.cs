using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

public delegate void NotifyToActor();
public delegate bool TileValidation(Tile tile);

public class PathFinder : MonoBehaviour
{

	
	NotifyToActor pathFindSuccess;
	NotifyToActor pathFindFail;
	TileValidation validateTile;

	public struct openListVisited
	{
		public bool isVisited;
		public bool isClosed;
		public int F;
		public openListVisited(bool _isVisited, bool _isClosed, int _F)
		{
			isVisited = _isVisited;
			isClosed = _isClosed;
			F = _F;
		}
	}
	public bool isNoPath = false;
	enum Direction {left, right, up, down};
	static Vector2[] DirectionVectors = { new Vector2(-1.0f, 0.0f), new Vector2(1.0f, 0.0f), new Vector2(0.0f, 1.0f), new Vector2(0.0f, -1.0f) };
	//
	Dictionary<int, PathVertex> closeList = new Dictionary<int, PathVertex>();
	PriorityQueue<PathVertex> openList = new PriorityQueue<PathVertex>();
	List<PathVertex> path = new List<PathVertex>();
	public openListVisited[,] visited;

	int bestKey = 0;
	PathVertex latest = null;
	PathVertex best = null;
	PathVertex latest_simulate = null;

	public Tile myCurPos;
	public Tile destination;
	public Tile nextMovePos;
	Tile next;

	public TileLayer tileLayer; //for Test
	bool isCal = false;
	public bool found = false;
	public bool isAdventurer = false;
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
		tileLayer = GameManager.Instance.GetMap().GetLayer(0).GetComponent<TileLayer>();
		id = gameObject.GetInstanceID();
		wait = new WaitForSeconds(0.5f);
		visited = new openListVisited[GameManager.Instance.GetMap().GetLayer(0).GetComponent<TileLayer>().GetLayerHeight(), GameManager.Instance.GetMap().GetLayer(0).GetComponent<TileLayer>().GetLayerWidth()];
		for (int i = 0; i < GameManager.Instance.GetMap().GetLayer(0).GetComponent<TileLayer>().GetLayerHeight(); i++)
		{
			for (int j = 0; j < GameManager.Instance.GetMap().GetLayer(0).GetComponent<TileLayer>().GetLayerWidth(); j++)
			{
				visited[i, j] = new openListVisited(false, false, 0);
			}
		}	
	}
	
	public IEnumerator Moves(Tile pcurPos, Tile pdestination)
	{
		yield return null;
		myCurPos = pcurPos;
		destination = pdestination;
		for (int i = 0; i < path.Count; i++)
		{
			path[i].ClearReference();
		}
		path.Clear();
		closeList.Clear();
		openList.HalfClear();
		ClearVisited();
		found = false;
		ThreadPool.UnsafeQueueUserWorkItem(this.Simulate, null);
		while (found == false)
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
			openList[i].ClearReference();
		}
		for (int i = 0; i < closeList.Count; i++)
		{
			closeList[i].ClearReference();
		}
		
	}

	public void Simulate(System.Object threadContext)
	{
		latest_simulate = new PathVertex(null, myCurPos, destination);
		bestKey = latest_simulate.X * 1000 + latest_simulate.Y;
		closeList.Add(latest_simulate.X * 1000 + latest_simulate.Y, latest_simulate);

		visited[latest_simulate.Y, latest_simulate.X].isClosed = true;
		Debug.Log("in");
		while (!latest_simulate.curTile.Equals(destination))
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
				found = true;
				//not found Callback
				break;
			}
			if (!closeList.ContainsKey(bestKey))
			{
				closeList.Add(bestKey, best);
				best.curTile.AddedCloseList();
			}
			latest_simulate = best;
		}
		latest = latest_simulate;
		found = true;
	}

	void AddOpenList(PathVertex newVertex)
	{
		if (closeList.ContainsKey(newVertex.X * 1000 + newVertex.Y))
		{
			return;
		}
		openList.Add(newVertex);
		newVertex.curTile.AddedOpenList();
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

