﻿using UnityEngine;
using System.Collections;
using SimpleJSON;
using System.Collections.Generic;

public class StructureManager : MonoBehaviour {
	static StructureManager _instance;
    
	public static StructureManager Instance
	{
		get
		{
			if (_instance == null)
			{
				Debug.Log("StructureManager is null");
				return null;
			}
			else
				return _instance;
		}
	}
	public GameObject[] ConstructingAreas;
	public GameObject constructing; 
	public GameObject rootStructureObject;
	public bool isConstructing = false;
	public JSONNode structureJson;
	string tempStructureCategory;
	int tempStructureNumber;
    

	public List<Structure> structures;
	// Use this for initialization
	void Start () {
		_instance = this;
		LoadStructureData();
		structures = new List<Structure>();
		
	}
	
	
	void LoadStructureData()
	{
		TextAsset structureText = Resources.Load<TextAsset>("Structure/structures");
		structureJson = JSON.Parse(structureText.text);
		Debug.Log(structureText.text);
	}

	//setStructureCategory -> setStructureNumber -> instantiateStructure // onClick 이벤트 정적 설정이 파라미터가 한개인 함수만 설정 가능하기 때문에 .. 번거롭더라도~~
	public void setStructureCategory(string structureCategory)
	{
		tempStructureCategory = structureCategory;
	}
	public void setStructureNumber(int num)
	{
		tempStructureNumber = num;
	}
	public void InstantiateStructure()
	{
		
		if (constructing != null)
			DestroyConstructing();
        isConstructing = true;

        constructing = (GameObject)Instantiate(Resources.Load("Structure/StructurePrefabs/"+ tempStructureCategory + "/" + tempStructureCategory + tempStructureNumber.ToString()) );
		constructing.transform.parent = rootStructureObject.transform;
		constructing.tag = "Movable";

		//임시
		Structure structure = constructing.GetComponent<Structure>();
		structure.name = structureJson[tempStructureCategory][tempStructureNumber]["name"];
		structure.type = structureJson[tempStructureCategory][tempStructureNumber]["type"];
		structure.capacity = structureJson[tempStructureCategory][tempStructureNumber]["capacity"].AsInt;
		structure.duration = structureJson[tempStructureCategory][tempStructureNumber]["duration"].AsInt;
		structure.charge = structureJson[tempStructureCategory][tempStructureNumber]["charge"].AsInt;

        //preference
        structure.preference.SetPrefAdventurer(structureJson[tempStructureCategory][tempStructureNumber]["preference"]["adventurer"].AsFloat);
        structure.preference.SetPrefTourist(structureJson[tempStructureCategory][tempStructureNumber]["preference"]["tourist"].AsFloat);
        structure.preference.SetPrefHuman(structureJson[tempStructureCategory][tempStructureNumber]["preference"]["human"].AsFloat);
        structure.preference.SetPrefElf(structureJson[tempStructureCategory][tempStructureNumber]["preference"]["elf"].AsFloat);
        structure.preference.SetPrefDwarf(structureJson[tempStructureCategory][tempStructureNumber]["preference"]["dwarf"].AsFloat);
        structure.preference.SetPrefOrc(structureJson[tempStructureCategory][tempStructureNumber]["preference"]["orc"].AsFloat);
        structure.preference.SetPrefDog(structureJson[tempStructureCategory][tempStructureNumber]["preference"]["cat"].AsFloat);
        structure.preference.SetPrefUpperclass(structureJson[tempStructureCategory][tempStructureNumber]["preference"]["upperclass"].AsFloat);
        structure.preference.SetPrefMiddleclass(structureJson[tempStructureCategory][tempStructureNumber]["preference"]["middleclass"].AsFloat);
        structure.preference.SetPrefLowerclass(structureJson[tempStructureCategory][tempStructureNumber]["preference"]["lowerclass"].AsFloat);
        //desire
        structure.resolveDesire.resolveThirsty = structureJson[tempStructureCategory][tempStructureNumber]["desire"]["thirsty"].AsFloat;
        structure.resolveDesire.resolveHungry = structureJson[tempStructureCategory][tempStructureNumber]["desire"]["hungry"].AsFloat;
        structure.resolveDesire.resolveSleep = structureJson[tempStructureCategory][tempStructureNumber]["desire"]["sleepy"].AsFloat;
        structure.resolveDesire.resolveEquipment = structureJson[tempStructureCategory][tempStructureNumber]["desire"]["equipment"].AsFloat;
        structure.resolveDesire.resolveTour = structureJson[tempStructureCategory][tempStructureNumber]["desire"]["tour"].AsFloat;
        structure.resolveDesire.resolveFun = structureJson[tempStructureCategory][tempStructureNumber]["desire"]["fun"].AsFloat;
        structure.resolveDesire.resolveConvenience = structureJson[tempStructureCategory][tempStructureNumber]["desire"]["convenience"].AsFloat;
        structure.resolveDesire.resolveHealth = structureJson[tempStructureCategory][tempStructureNumber]["desire"]["helath"].AsFloat;


        structure.genre = structureJson[tempStructureCategory][tempStructureNumber]["genre"];
		structure.expenses = structureJson[tempStructureCategory][tempStructureNumber]["expenses"].AsInt;

		int x = structureJson[tempStructureCategory][tempStructureNumber]["sitewidth"].AsInt;
		structure.extentWidth = x;
		
		int y = structureJson[tempStructureCategory][tempStructureNumber]["sitelheight"].AsInt;
		structure.extentHeight = y;

		structure.extent = new int[x, y];
		for(int i =0; i< x*y; i++)
		{
			structure.extent[i % x, i / x] = structureJson[tempStructureCategory][tempStructureNumber]["site"][i].AsInt;
			Debug.Log(structure.extent[i % x, i / x]);
        }
		AllocateStructure();
	}
	
	public void AllocateStructure()
	{
		GameObject nearest = null;
		GameObject[] gos = GameObject.FindGameObjectsWithTag("Tile");
		Vector2 pos = Camera.main.ScreenToWorldPoint(new Vector3(Camera.main.pixelWidth / 2, Camera.main.pixelHeight / 2, 0.0f));
		float minDiff = Mathf.Infinity;
		Debug.Log("Allocated!!!!");

		foreach (GameObject go in gos)
		{
			float diff = ((Vector2)go.transform.position - pos).magnitude;
			if (minDiff > diff && go.GetComponent<Tile>().GetBuildable())
			{
				minDiff = diff;
				nearest = go;
			}
		}
		constructing.transform.position = new Vector3(nearest.transform.position.x, nearest.transform.position.y, -50.0f) ;

		//////////////////////////////////
		SetStructurePoint(nearest.GetComponent<Tile>());
		MoveStructure(nearest.GetComponent<Tile>());
		
		if (constructing.GetComponent<Structure>().GetisConstructable() == false)
		{
			constructing.GetComponent<SpriteRenderer>().color = new Color(1, 0, 0);
		}
		else
		{
			constructing.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1);
		}
		////////////////////////////////////
		constructing.GetComponent<Structure>().StartMove();
		
	}
	public void ConstructStructure()
	{
		//Gold 소모
        
		Structure structure = constructing.GetComponent<Structure>();

        if (GameManager.Instance.GetPlayerGold() >= structure.expenses)
        {
            GameManager.Instance.playerGold -= structure.expenses;
        }
        else
            return;
        Tile tile = structure.point;
		int[,] extent = structure.GetExtent();
		TileLayer tileLayer = tile.GetLayer();

		Debug.Log("Constructed!!!!");
		constructing.tag = "Structure";
		
		structure.EndMove();
		ResetConstructingAreas();
		structures.Add(constructing.GetComponent<Structure>());		
		for(int i = 0; i<structure.extentHeight; i++)
		{
			for(int j = 0; j<structure.extentWidth; j++)
			{
                Tile thatTile = tileLayer.GetTileAsComponent(tile.GetX() + j, tile.GetY() + i);

                if (extent[j,i] == 1)
				{
					thatTile.SetBuildable(false);
                    thatTile.SetStructed(true);
                    thatTile.SetStructure(structure);
                }
                else if(extent[j, i] == 2)
                {
                    thatTile.SetStructed(true);
                    if(thatTile.GetBuildable())
                    {
                        structure.addEntrance(thatTile);
                    }
                }
			}
		}

        CheckEntrance();

        constructing = null;
        isConstructing = false;
        

    }
	public bool GetisConstructing()
	{
		return isConstructing;
	}
	public void DestroyConstructing()
	{
		Destroy(constructing);
		constructing = null;
		isConstructing = false;
		ResetConstructingAreas();
	}
	public GameObject GetConstructing()
	{
		return constructing;
	}
	public void ResetConstructingAreas()
	{
		foreach(GameObject go in ConstructingAreas)
		{
			go.transform.position = new Vector3(1000, 1000, -50);
			go.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0.6f);
		}
	}
	public void MoveStructure(Tile tile)
	{
		if (constructing == null)
		{
			Debug.Log("GameObject constructing is NULL");
			return;
		}
		Structure conStructure = constructing.GetComponent<Structure>();

		
		int width = conStructure.extentWidth;
		int height = conStructure.extentHeight;

		Tile edge1 = tile.GetLayer().GetTileAsComponent(tile.GetX() + width - 1, tile.GetY());
		Tile edge2 = tile.GetLayer().GetTileAsComponent(tile.GetX(), tile.GetY() + height - 1);
		Tile edge3 = tile.GetLayer().GetTileAsComponent(tile.GetX() + width - 1, tile.GetY() + height - 1);

		if (tile == null || edge1 == null || edge2 == null || edge3 == null)
			return;

		constructing.transform.position = new Vector3(tile.transform.position.x, tile.transform.position.y, tile.transform.position.z - 10);
		conStructure.point = tile;
		/////////////////////////////////////////////////// place
		

		int x = tile.GetX();
		int y = tile.GetY();
		int[,] extent = conStructure.GetExtent(); // 현재 움직이는 건물의 영역 크기 불러옴.
																			

		int areaIndex = 0;
		conStructure.SetisConstructable(true);
		for (int i = 0; i < conStructure.extentHeight; i++)//StructureManager.Instance.GetConstructing().GetComponent<Structure>().extentHeight; i++)
		{
			for (int j = 0; j < conStructure.extentWidth; j++)//StructureManager.Instance.GetConstructing().GetComponent<Structure>().extentWidth; j++) //건물의 영역 크기 만큼의 2차원 배열
			{

				if (extent[j, i] == 0) // 건물의 영역에서 0인 부분
				{

				}
				else
				{
					Tile t = GameManager.Instance.GetMap().GetLayer(0).GetComponent<TileLayer>().GetTileAsComponent(x + j, y + i);
					if (t != null)
					{
						if (t.GetBuildable() == true && extent[j, i] == 1)
						{
							StructureManager.Instance.ConstructingAreas[areaIndex].transform.position = t.transform.position;
							StructureManager.Instance.ConstructingAreas[areaIndex++].GetComponent<SpriteRenderer>().color = new Color(0.0f, 1.0f, 0.0f);
							//constructing.GetComponent<Structure>().SetisConstructable(true);
						}
						else if (t.GetBuildable() == true && extent[j, i] == 2)
						{
							StructureManager.Instance.ConstructingAreas[areaIndex].transform.position = t.transform.position;
							StructureManager.Instance.ConstructingAreas[areaIndex++].GetComponent<SpriteRenderer>().color = new Color(0.0f, 0.0f, 1.0f);
							//constructing.GetComponent<Structure>().SetisConstructable(true);
						}
						else if (t.GetBuildable() == false && extent[j, i] == 1)
						{
							StructureManager.Instance.ConstructingAreas[areaIndex].transform.position = t.transform.position;
							StructureManager.Instance.ConstructingAreas[areaIndex++].GetComponent<SpriteRenderer>().color = new Color(1.0f, 0.0f, 0.0f);
							conStructure.SetisConstructable(false);

						}
						else if (t.GetBuildable() == false && extent[j, i] == 2)
						{
							StructureManager.Instance.ConstructingAreas[areaIndex].transform.position = t.transform.position;
							StructureManager.Instance.ConstructingAreas[areaIndex++].GetComponent<SpriteRenderer>().color = new Color(1.0f, 1.0f, 0.0f);
							//constructing.GetComponent<Structure>().SetisConstructable(true);
						}
						else
						{
							Debug.Log("Never gonna happen");
						}
					}
				}
			}
		}
		if (conStructure.GetisConstructable() == false)
		{
			constructing.GetComponent<SpriteRenderer>().color = new Color(1, 0, 0);
		}
		else
		{
			constructing.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1);
		}
	}
	

	public List<Structure> GetStructures()
	{
		return structures;
	}

	public void SetStructurePoint(Tile t)
	{
		constructing.GetComponent<Structure>().point = t;
	}

    public void CheckEntrance() // 건설 및 해체시 반드시 호출
    {

        foreach(Structure s in structures)
        {
            int[,] ex = s.GetExtent();
            int tilex = s.point.GetX();
            int tiley = s.point.GetY();
            
            for(int i = 0; i<s.extentHeight; i++)
            {
                for(int j = 0; j<s.extentWidth; j++)
                {
                    Tile t = s.point.GetLayer().GetTileAsComponent(s.point.GetX() + j, s.point.GetY() + i);
                    if (t.GetBuildable() && ex[j,i] == 2 && !t.GetStructed())
                    {
                        t.SetStructed(true);
                        s.entrance.Add(t);
                    }
                    else if(!t.GetBuildable() && ex[j,i] == 2 && t.GetStructed())
                    {
                        t.SetStructed(false);
                        s.entrance.Remove(t);
                    }
                    

                }
            }
            s.entCount = s.entrance.Count;
            
            if(s.entrance.Count <= 0)
            {
                //s.입구 막힘 알림.
                Debug.Log("입구 막힘. " + s.gameObject.name);

            }
        }
    }

    public void DestroyStructure(Structure s)
    {
        if (s == null)
            return;
        structures.Remove(s);
        int[,] ex = s.GetExtent();
        for (int i = 0; i<s.extentWidth; i++)
        {
            for(int j = 0; j<s.extentHeight; j++)
            {
                if (ex[i, j] == 1)
                {
                    s.point.GetLayer().GetTileAsComponent(s.point.GetX() + i, s.point.GetY() + j).SetStructure(null);
                    s.point.GetLayer().GetTileAsComponent(s.point.GetX() + i, s.point.GetY() + j).SetStructed(false);
                    s.point.GetLayer().GetTileAsComponent(s.point.GetX() + i, s.point.GetY() + j).SetBuildable(true);
                }
            }
        }
        Debug.Log("Structure " + s.gameObject.name + " Destroyed!");

        Destroy(s.gameObject);
        CheckEntrance();
    }

    public Structure FindStructureByTile(Tile t)
    {
        foreach(Structure s in structures)
        {
            if( (s.point.x <= t.x) && (s.point.x + s.extentWidth >= t.x) && (s.point.y <= t.y) && (s.point.y + s.extentHeight >= t.y))
            {
                return s;
            }
            
        }
        return null;
    }
	
}