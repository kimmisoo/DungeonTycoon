/*
   타일⊂타일레이어⊂타일맵
   227 ~ 238 길
239 >= 몬스터
~226 건설
*/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SimpleJSON;
using System.IO;


public class TileMapGenerator : MonoBehaviour
{

	Dictionary<int, GameObject> tile_Resources; //타일 생성할때 . . .
												// 딕셔너리 찾아봄.   있음 -> 사용,   없음 -> 로드 후 사용 및 딕셔너리 삽입

	public GameObject tileMap_Object;
	JSONNode mapData;
	JSONNode mappingData;
	public TileMap tileMap;
	public const int road_Start = 227;
	public List<GameObject> preLoadedTileObject;
	int i = 0;
	public GameObject GenerateMap(string path)  //맵 생성
	{
		LoadMapData(path);
		CreateMap();
		return tileMap_Object;
	}

	private void LoadMapData(string file_Path)
	{
		//file path에 따라 json file load.
		TextAsset jsontext = Resources.Load<TextAsset>(file_Path);
		TextAsset jsonmappingtext = Resources.Load<TextAsset>("TileMap/" + "Tile_Mapping");
		mapData = JSON.Parse(jsontext.text);
		preLoadedTileObject = new List<GameObject>();

		for (int i = 0; i <= 298; i++) //hard -> 로딩시간 최적화하려면 씬에 등장하는것만 로드.
		{
			preLoadedTileObject.Add(Resources.Load<GameObject>("TilePrefabs/" + i));
			//Debug.Log("TilePrefabs/" + i);
		}

		//mappingData = JSON.Parse(jsonmappingtext.text);
	}

	private void CreateMap()
	{
		int layer_Count = 0;
		string t;

		tile_Resources = new Dictionary<int, GameObject>();

		do
		{
			t = mapData["layers"][layer_Count++];
		} while (t != null);

		layer_Count--;

		int a;
		//맵 데이터 읽기 완료

		tileMap_Object = new GameObject("TileMap"); // instantiate 할 필요 없음. 씬에 추가까찌 자동으로 됨
		tileMap_Object.transform.position = new Vector3(0.0f, 0.0f, 0.0f);
		tileMap = tileMap_Object.AddComponent<TileMap>();
		tileMap.SetWidth(mapData["width"].AsInt);
		tileMap.SetHeight(mapData["height"].AsInt);
		tileMap.SetLayerCount(layer_Count);
		tileMap.SetOrientation(mapData["orientation"]);
		tileMap.SetRenderOrder(mapData["renderorder"]);
		tileMap.AssignLayerArray(layer_Count);

		//타일맵 생성 완료. 레이어 추가는 아직

		float pivotX = 0.0f;
		float pivotY = 0.0f;
		float pivotZ = 0.0f;
		float offsety = 0.0f;
		//Debug.Log("layerCount = " + layer_Count);

		////////////////////////////////////////////////////////////

		GameObject layer_Object = new GameObject("TileLayer");
		layer_Object.transform.parent = tileMap_Object.transform;
		layer_Object.transform.position = new Vector3(0.0f, 0.0f, 0.0f);
		TileLayer layer = layer_Object.AddComponent<TileLayer>();
		layer.SetLayerNum(0);
		layer.SetLayerWidth(mapData["layers"][0]["width"].AsInt);
		layer.SetLayerHeight(mapData["layers"][0]["height"].AsInt);
		layer.SetOffsetX(mapData["layers"][0]["offsetx"] == null ? 0 : mapData["layers"][0]["offsetx"].AsInt);
		layer.SetOffsetY(mapData["layers"][0]["offsety"] == null ? 0 : mapData["layer"][0]["offsety"].AsInt);
		layer.SetOpacity(mapData["layers"][0]["opacity"].AsInt);
		layer.SetLayerName(mapData["layers"][0]["name"]);
		layer.SetLayerType(mapData["layers"][0]["type"]);
		layer.AssignTileArray(mapData["layers"][0]["width"].AsInt, mapData["layers"][0]["height"].AsInt);
		layer.AssignTileForMoveArray(mapData["layers"][0]["width"].AsInt, mapData["layers"][0]["height"].AsInt);
		tileMap.AddLayer(layer_Object);
		//layer는 0번 하나만~]
		//layer_Count = 1;
		for (int y = 0; y < layer.GetLayerHeight(); y++)
		{
			for (int x = 0; x < layer.GetLayerWidth(); x++)
			{
				for (i = 0; i < layer_Count; i++)//layer_Count; i++)
				{
					if (mapData["layers"][i]["data"][y * layer.GetLayerWidth() + x].AsInt != 0)
					{
						GameObject tile_Object;
						tile_Object = (GameObject)Instantiate(preLoadedTileObject[mapData["layers"][i]["data"][y * layer.GetLayerWidth() + x].AsInt]);//Resources.Load("TilePrefabs/" + mapData["layers"][i]["data"][y * layer.GetLayerWidth() + x].AsInt));                                                                                            //tile_Resources.Add(mapData["layers"][i]["data"][x * layer.GetLayerWidth() + y].AsInt, tile_Object);
						tile_Object.name = (y * layer.GetLayerWidth() + x).ToString();

						tile_Object.transform.parent = layer_Object.transform;
						//Tile tile = tile_Object.GetComponent<Tile>();
						Tile tile = tile_Object.AddComponent<Tile>();
						if (mapData["layers"][i]["data"][y * layer.GetLayerWidth() + x].AsInt >= 227 && mapData["layers"][i]["data"][y * layer.GetLayerWidth() + x].AsInt <= 238)//~238 //길
						{
							tile.SetPassable(true);
							tile.SetBuildable(false);
							tile.SetMonsterArea(false);
							tile.SetNonTile(false);
						}
						else if (mapData["layers"][i]["data"][y * layer.GetLayerWidth() + x].AsInt >= 239) // 몬스터
						{
							tile.SetPassable(true);
							tile.SetBuildable(false);
							tile.SetMonsterArea(true);
							tile.SetNonTile(false);
						}
						else // 건설
						{
							tile.SetPassable(false);
							tile.SetBuildable(true);
							tile.SetMonsterArea(false);
							tile.SetNonTile(false);
						}
						tile.SetX(x);
						tile.SetY(y);
						tile.SetLayerNum(i);
						tile.SetLayer(layer);
						tile.SetTileType(mapData["layers"][i]["data"][y * layer.GetLayerWidth() + x].AsInt);

						tile_Object.transform.position = new Vector3((pivotX), (pivotY), (pivotZ));
						layer.AddTile(x, y, tile_Object);

						////////////////////////////////
						int ui = y * layer.GetLayerWidth() * 4 + 2 * x;
						int di = y * layer.GetLayerWidth() * 4 + 2 * x + 2 * layer.GetLayerWidth() + 1;
						int li = y * layer.GetLayerWidth() * 4 + 2 * x + 2 * layer.GetLayerWidth();
						int ri = y * layer.GetLayerWidth() * 4 + 2 * x + 1;

						Vector3 p = tile.transform.position;

						layer.AddTileForMove(x * 2, y * 2, new Vector3(p.x, p.y + 0.666f, p.z), tile); // u
						tile.SetChild(0, layer.GetTileForMove(x * 2, y * 2));
						layer.AddTileForMove(x * 2 + 1, y * 2, new Vector3(p.x + 0.333f, p.y + 0.5f, p.z), tile); // r
						tile.SetChild(1, layer.GetTileForMove(x * 2 + 1, y * 2));
						layer.AddTileForMove(x * 2, y * 2 + 1, new Vector3(p.x - 0.333f, p.y + 0.5f, p.z), tile); // l
						tile.SetChild(2, layer.GetTileForMove(x * 2, y * 2 + 1));
						layer.AddTileForMove(x * 2 + 1, y * 2 + 1, new Vector3(p.x, p.y + 0.333f, p.z), tile); // d
						tile.SetChild(3, layer.GetTileForMove(x * 2 + 1, y * 2 + 1));

						break; // Layer 별로 겹치는 타일 없으니 바로 break;
					}
				}
				if (i >= layer_Count) // 마지막 까지 왔지만 타일 없을때
				{
					//0으로 생성
					GameObject tile_Object;

					tile_Object = (GameObject)Instantiate(preLoadedTileObject[1]);//Resources.Load("TilePrefabs/" + 1));
																				  //tile_Resources.Add(mapData["layers"][i]["data"][x * layer.GetLayerWidth() + y].AsInt, tile_Object);
					tile_Object.name = (y * layer.GetLayerWidth() + x).ToString();
					tile_Object.transform.parent = layer_Object.transform;

					Tile tile = tile_Object.AddComponent<Tile>();
					//Tile tile = tile_Object.GetComponent<Tile>();
					tile.SetPassable(false);
					tile.SetX(x);
					tile.SetY(y);
					tile.SetLayerNum(0);
					tile.SetLayer(layer);
					tile.SetTileType(mapData["layers"][i]["data"][y * layer.GetLayerWidth() + x].AsInt);
					tile_Object.GetComponent<SpriteRenderer>().enabled = false;
					//2016-12-30
					tile_Object.tag = "non_Tile";
					tile.SetNonTile(true);
					//2016-12-30
					tile_Object.transform.position = new Vector3((pivotX), (pivotY), (pivotZ));
					layer.AddTile(x, y, tile_Object);
				}
				pivotX = pivotX + 0.5f * 100 / 75; // 2/3
				pivotZ = pivotZ - 0.3f * 100 / 75; // 0.4
				pivotY = pivotY - 0.25f * 100 / 75; // 1/3
			}
			pivotX = -0.5f * 100 / 75 * (y + 1);
			pivotZ = -0.3f * 100 / 75 * (y + 1);
			pivotY = -0.25f * 100 / 75 * (y + 1);
		}



		/*
		for (int i = 0; i < 2; i++)//layer_Count; i++)////// 타일레이어 중첩?
		{
		   GameObject layer_Object = new GameObject("TileLayer" + i.ToString());
		   layer_Object.transform.parent = tileMap_Object.transform;
		   layer_Object.transform.position = new Vector3(0.0f, 0.0f, 0.0f);
		   TileLayer layer = layer_Object.AddComponent<TileLayer>();
		   GameObject tilesforMove = new GameObject("TilesForMove" + i.ToString());

			  layer.SetLayerNum(i);
		   layer.SetLayerWidth(mapData["layers"][i]["width"].AsInt);
		   layer.SetLayerHeight(mapData["layers"][i]["height"].AsInt);
		   layer.SetOffsetX(mapData["layers"][i]["offsetx"] == null ? 0 : mapData["layers"][i]["offsetx"].AsInt);
		   layer.SetOffsetY(mapData["layers"][i]["offsety"] == null ? 0 : mapData["layer"][i]["offsety"].AsInt);
		   layer.SetOpacity(mapData["layers"][i]["opacity"].AsInt);
		   layer.SetLayerName(mapData["layers"][i]["name"]);
		   layer.SetLayerType(mapData["layers"][i]["type"]);
		   layer.AssignTileArray(mapData["layers"][i]["width"].AsInt, mapData["layers"][i]["height"].AsInt);
		   layer.AssignTileForMoveArray(mapData["layers"][i]["width"].AsInt, mapData["layers"][i]["height"].AsInt);
		   tileMap.AddLayer(layer_Object);
		   //레이어 생성완료

		   for (int y = 0; y < layer.GetLayerHeight(); y++)
		   {
			  for (int x = 0; x < layer.GetLayerWidth(); x++)
			  {
				 if (mapData["layers"][i]["data"][y * layer.GetLayerWidth() + x].AsInt != 0) // json 에서 tile 정보가 0이 아닐때
				 {
					GameObject tile_Object;

					tile_Object = (GameObject)Instantiate(preLoadedTileObject[mapData["layers"][i]["data"][y * layer.GetLayerWidth() + x].AsInt]);//Resources.Load("TilePrefabs/" + mapData["layers"][i]["data"][y * layer.GetLayerWidth() + x].AsInt));
					//tile_Resources.Add(mapData["layers"][i]["data"][x * layer.GetLayerWidth() + y].AsInt, tile_Object);
					tile_Object.name = (y * layer.GetLayerWidth() + x).ToString();

					tile_Object.transform.parent = layer_Object.transform;
					Tile tile = tile_Object.AddComponent<Tile>();
					if (mapData["layers"][i]["data"][y * layer.GetLayerWidth() + x].AsInt >= 227 && mapData["layers"][i]["data"][y * layer.GetLayerWidth() + x].AsInt <= 238)//~238
					{
					   tile.SetPassable(true);
					   tile.SetBuildable(false);
					}
					else if(mapData["layers"][i]["data"][y * layer.GetLayerWidth() + x].AsInt >= 239)
					{
					   tile.SetPassable(true);
					   tile.SetBuildable(false);
					}
					else
					{
					   tile.SetPassable(false);
					   tile.SetBuildable(true);
					}
					tile.SetX(x);
					tile.SetY(y);
					tile.SetLayerNum(layer.GetLayerNum());
					tile.SetLayer(layer);
					tile.SetTileType(mapData["layers"][i]["data"][y * layer.GetLayerWidth() + x].AsInt);

					tile_Object.transform.position = new Vector3((pivotX), (pivotY), (pivotZ));
					layer.AddTile(x, y, tile_Object);
				 }
				 else
				 {
					if (i == 1) // 
					{
					   GameObject tile_Object;

					   tile_Object = (GameObject)Instantiate(preLoadedTileObject[1]);//Resources.Load("TilePrefabs/" + 1));
					   //tile_Resources.Add(mapData["layers"][i]["data"][x * layer.GetLayerWidth() + y].AsInt, tile_Object);
					   tile_Object.name = (y * layer.GetLayerWidth() + x).ToString();

					   tile_Object.transform.parent = layer_Object.transform;
					   Tile tile = tile_Object.AddComponent<Tile>();

					   tile.SetPassable(false);
					   tile.SetX(x);
					   tile.SetY(y);
					   tile.SetLayerNum(layer.GetLayerNum());
					   tile.SetLayer(layer);
					   tile.SetTileType(mapData["layers"][i]["data"][y * layer.GetLayerWidth() + x].AsInt);
					   tile_Object.GetComponent<SpriteRenderer>().enabled = false;
					   //2016-12-30
					   tile_Object.tag = "non_Tile";
					   tile.SetNonTile(true);
					   //2016-12-30
					   tile_Object.transform.position = new Vector3((pivotX), (pivotY), (pivotZ));
					   layer.AddTile(x, y, tile_Object);
					}
				 }
				 pivotX = pivotX + 0.5f * 100 / 75; // 2/3
				 pivotZ = pivotZ - 0.3f * 100 / 75; // 0.4
				 pivotY = pivotY - 0.25f * 100 / 75; // 1/3
			  }
			  pivotX = -0.5f * 100 / 75 * (y + 1);
			  pivotZ = -0.3f * 100 / 75 * (y + 1);
			  pivotY = -0.25f * 100 / 75 * (y + 1);
		   }



		   //각 타일 마다 4개씩 ...이동용 맵 생성.
		   for (int y = 0; y < layer.GetLayerHeight(); y++)
		   {
			  for (int x = 0; x < layer.GetLayerWidth(); x++)
			  {
				 if (mapData["layers"][i]["data"][y * layer.GetLayerWidth() + x].AsInt != 0)
				 {
					int ui = y * layer.GetLayerWidth() * 4 + 2 * x;
					int di = y * layer.GetLayerWidth() * 4 + 2 * x + 2 * layer.GetLayerWidth() + 1;
					int li = y * layer.GetLayerWidth() * 4 + 2 * x + 2 * layer.GetLayerWidth();
					int ri = y * layer.GetLayerWidth() * 4 + 2 * x + 1;


					Tile tempt = layer.GetTileAsComponent(x, y);
					Vector3 p = tempt.transform.position;


					layer.AddTileForMove(x * 2, y * 2, new Vector3(p.x, p.y + 0.666f, p.z), tempt); // u
					layer.AddTileForMove(x * 2 + 1, y * 2 + 1, new Vector3(p.x, p.y + 0.333f, p.z), tempt); // d
					layer.AddTileForMove(x * 2, y * 2 + 1, new Vector3(p.x - 0.333f, p.y + 0.5f, p.z), tempt); // l
					layer.AddTileForMove(x * 2 + 1, y * 2, new Vector3(p.x + 0.333f, p.y + 0.5f, p.z), tempt); // r

				 }
			  }
		   }


		   pivotX = -0.85f * (i + 1);
		   pivotZ = 0.0f;
		   pivotY = 0.25f * (i + 1);
		}




	 */
	}
}