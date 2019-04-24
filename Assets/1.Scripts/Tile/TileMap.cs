using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TileMap : MonoBehaviour {

	
	int width;
	int height;
	int layer_Count = 0;
	string orientation;
	string renderorder;

	private int layer_Count_Offset = 0;

	//Dictionary<int, TileLayer> layer; // 0 = Ground,  1 = 2층, 2 = 3층.....
	GameObject[] layers;

	
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
	public void AssignLayerArray(int _layer_Count)
	{
		layers = new GameObject[_layer_Count];
	}
	public void AddLayer(GameObject _layer)
	{
		if (layer_Count_Offset >= layer_Count)
			Debug.Log("Layer Overflow");
		else
			layers[layer_Count_Offset++] = _layer;
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
	
}
