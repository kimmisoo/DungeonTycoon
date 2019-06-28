using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DTObject : MonoBehaviour {

	public Tile currentTile;
	
	public void SetCurrentTile(Tile tile)
	{
		currentTile = tile;
	}
	public Tile GetCurrentTile()
	{
		return currentTile;
	}
}
