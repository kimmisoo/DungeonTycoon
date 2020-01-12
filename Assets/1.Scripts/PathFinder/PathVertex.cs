using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

// A*용 클래스
public class PathVertex : IComparable
{
	public float Cost { get { return F; } }
	public PathVertex Parent { get { return prevVertex; } }
	public Tile myTilePos { get { return curTile; } }
	public int X = 0;
	public int Y = 0;
	PathVertex prevVertex = null;
	private Tile curTile = null;
	public int F = 0;
	public int G = 0;
	public int H = 0;

	public PathVertex()
	{

	}

    // 생성자이며 비용계산도 함.
	public PathVertex(PathVertex _prevVertex, Tile _curTile, Tile _endTile)
	{
		curTile = _curTile;
		prevVertex = _prevVertex;
		if (prevVertex == null)
			G = 1;
		else
			G = prevVertex.G + 1;
		H = Mathf.Abs(_endTile.GetX() - curTile.GetX()) + Mathf.Abs(_endTile.GetY() - curTile.GetY());
		F = G + H;
		X = curTile.GetX();
		Y = curTile.GetY();

	}

	public void ReUse(PathVertex _prevVertex, Tile _curTile, Tile _endTile)
	{
		curTile = _curTile;
		prevVertex = _prevVertex;
		if (prevVertex == null)
			G = 1;
		else
			G = prevVertex.G + 1;
		H = Mathf.Abs(_endTile.GetX() - curTile.GetX()) + Mathf.Abs(_endTile.GetY() - curTile.GetY());
		F = G + H;
		X = curTile.GetX();
		Y = curTile.GetY();
	}

	public void ClearReference()
	{
		prevVertex = null;
	}

    // F값 비교
	public int CompareTo(object other)
	{
		if ((other is PathVertex) == false) return 0;

		return F.CompareTo((other as PathVertex).F);
	}
}
