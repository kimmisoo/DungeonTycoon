using UnityEngine;
using System.Collections;

public class TileLayer : MonoBehaviour  {

	int layer_Num = -1;
	int offset_X;
	int offset_Y;
	int opacity;
	int layer_Width;
	int layer_Height;

	string layer_Name;
	string layer_Type;
	
	GameObject[ , ] tiles;
	Tile[,] tilesComponent;
	TileForMove [ , ] tilesforMove;
    
    // Structure 세팅을 위해 Get가능
    public GameObject[ , ] GetTiles()
    {
        return tiles;
    }

	public void SetLayerNum(int _layer_Num)
	{
		layer_Num = _layer_Num;
	}
	public void SetLayerWidth(int _layer_Width)
	{
		layer_Width = _layer_Width;
	}
	public void SetLayerHeight(int _layer_Height)
	{
		layer_Height = _layer_Height;
	}
	public void SetOffsetX(int _offset_X)
	{
		offset_X = _offset_X;
	}
	public void SetOffsetY(int _offset_Y)
	{
		offset_Y = _offset_Y;
	}
	public void SetOpacity(int _opacity)
	{
		opacity = _opacity;
	}
	public void SetLayerName(string _layer_Name)
	{
		layer_Name = _layer_Name;
	}
	public void SetLayerType(string _layer_Type)
	{
		layer_Type = _layer_Type;
	}
	public void AssignTileArray(int _layer_Width, int _layer_Height)
	{
		tiles = new GameObject[_layer_Height, _layer_Width];
		for(int i=0; i< _layer_Height; i++)
		{
			for(int j = 0; j< _layer_Width; j++)
			{
				tiles[i, j] = null;
			}
		}
		tilesComponent = new Tile[_layer_Height, _layer_Width];
		for(int i=0; i<_layer_Height; i++)
		{
			for(int j = 0; j<_layer_Width; j++)
			{
				tilesComponent[i, j] = null;
			}
		}
	}
	public void AssignTileForMoveArray(int _layer_Width, int _layer_Height)
	{
		tilesforMove = new TileForMove[_layer_Height * 2, _layer_Width * 2];
		for (int i = 0; i < _layer_Height * 2; i++)
		{
			for (int j = 0; j < _layer_Width * 2; j++)
			{
				tilesforMove[i, j] = null;
			}
		}
	}
	public void AddTileForMove(int x, int y, Vector3 _pos, Tile _parent)
	{
		tilesforMove[x, y] = new TileForMove(x, y, _pos, _parent);
	}

	public void AddTile(int x, int y, GameObject tile)
	{
		tiles[x,y] = tile;
		tilesComponent[x, y] = tile.GetComponent<Tile>();
	}
	
	public void PrintMyAttr()
	{
		for(int i = 0; i< layer_Height; i++)
		{
			string t = layer_Name + "\n";
			for (int j = 0; j< layer_Width; j++)
			{
				//Debug.Log(" " + tiles[i, j].GetNum());
				if(tiles[i, j] != null)
					t = t + tiles[i, j].GetComponent<Tile>().GetTileType().ToString() + "  ";

            }
			//Debug.Log(t);
		}
	}
	public int GetLayerWidth()
	{
		return layer_Width;
	}
	public int GetLayerHeight()
	{
		return layer_Height;
	}
	public int GetOffsetY()
	{
		return offset_Y;
	}
	public GameObject GetTile(int x, int y)
	{
		if ((0 <= x && x < layer_Width) && (0 <= y && y < layer_Height) && tiles[x, y] != null)
			return tiles[x, y];
		else
			return null;
	}
	public Tile GetTileAsComponent(int x, int y)
	{
        if ((0 <= x && x < layer_Width) && (0 <= y && y < layer_Height) && tiles[x, y] != null)
            return tilesComponent[x, y];
        else
        {
            //Debug.Log("GetTileAsComponent Returns Null!! // " + x + " , " + y);
            //Debug.Log("layer_Width : " + layer_Width + " , layer_Height : " + layer_Height);
            return null;
        }
	}
    public Tile GetTileDebug()
    {
        return GetComponentInChildren<Tile>();
    }

	public TileForMove GetTileForMove(int x, int y)
	{
		if ((0 <= x && x < layer_Width * 2) && (0 <= y && y < layer_Height * 2) && tilesforMove[x, y] != null)
			return tilesforMove[x, y];
		else
		{
			Debug.Log("GetTileForMove Returns Null!! // " + x + " , " + y);
			return null;			
		}
	}

	public string GetLayerName()
	{
		return layer_Name;
	}
	public string GetLayerType()
	{
		return layer_Type;
	}
	public int GetLayerNum()
	{
		return layer_Num;
	}
	public int GetOpacity()
	{
		return opacity;
	}
}
