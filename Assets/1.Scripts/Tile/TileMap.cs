using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TileMap : MonoBehaviour {

	
	int width;
	int height;
	int layer_Count = 0;
	string orientation;
	string renderorder;
	Sprite unknownSprite;
	//Dictionary<int, TileLayer> layer; // 0 = Ground,  1 = 2층, 2 = 3층.....
	List<GameObject> layers = new List<GameObject>();
	List<List<Tile>> tilesForActivate;
	public void SetWidth(int _width)
	{
		width = _width;
	}
	public void SetHeight(int _height)
	{
		height = _height;
	}
	public void SetLayerCount(int _layer_Count)
	{
		layer_Count = _layer_Count;
	}
	public void SetOrientation(string _orientation)
	{
		orientation = _orientation;
	}
	public void SetRenderOrder(string _renderorder)
	{
		renderorder = _renderorder;
	}
	public int GetLayerCount()
	{
		return layer_Count;
	}
	public void AddLayer(GameObject _layer)
	{
		layers.Add(_layer);
	}

	public void PrintLayer()
	{
		for(int i=0; i< layer_Count; i++)
		{
			layers[i].GetComponent<TileLayer>().PrintMyAttr();
		}
	}

	public GameObject GetLayer(int n)
	{
		return layers[n];
	}
	public void SetTilesForActivate(List<List<Tile>> forActivate)
	{
		tilesForActivate = forActivate;
	}
	public void ActivateSpecificLayer(int num)
	{
		if(layer_Count<num || num < 0)
		{
			return;
		}
		foreach(Tile tile in tilesForActivate[num])
		{
			tile.SetIsActive(true);
		}
	}
}
