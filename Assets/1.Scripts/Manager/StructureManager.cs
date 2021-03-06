using UnityEngine;
using System.Collections;
using SimpleJSON;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.SceneManagement;

public class StructureManager : MonoBehaviour
{
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

	// 건설 대상 지역 표시
	public List<GameObject> constructingAreas;

    // 건설할 건물
	public GameObject constructing; 

    // 건물들의 부모가 될 오브젝트
	public GameObject rootStructureObject;

    // 건설 중인지 표시
    public bool isConstructing = false;

    // 건물 정보 읽어오기용
	JSONNode structureJson;
	string tempStructureCategory;
	int tempStructureNumber;

    #region 세이브!
    public List<Structure> structures;
	#endregion
	void Awake()
	{
        _instance = this;
        structures = new List<Structure>();
        LoadStructureData();
    }
	// Use this for initialization
	void Start ()
    {
		GameObject constructingAreaParent = GameObject.FindGameObjectWithTag("ConstructingArea");
		for(int i=0; i<constructingAreaParent.transform.childCount; i++)
		{
			constructingAreas.Add(constructingAreaParent.transform.GetChild(i).gameObject);
		}
	}
	
	
	void LoadStructureData()
	{
		TextAsset structureText = Resources.Load<TextAsset>("Structure/structures");
		structureJson = JSON.Parse(structureText.text);
		//Debug.Log(structureText.text);
		//Debug.Log(structureJson);
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

    // Resources에서 건물 정보 읽어오고 constructing에 올림
	public void InstantiateStructure()
	{
		if (constructing != null)
			DestroyConstructing();
        isConstructing = true;

        constructing = (GameObject)Instantiate(Resources.Load("Structure/StructurePrefabs/"+ tempStructureCategory + "/" + tempStructureCategory + tempStructureNumber.ToString()) );
		constructing.transform.parent = rootStructureObject.transform;
		constructing.tag = "Movable";


        SetStructureDataFromJSON(constructing.GetComponent<Structure>(), tempStructureCategory, tempStructureNumber);
        /*for(int i =0; i< x*y; i++)
		{
			structure.extent[i % x, i / x] = structureJson[tempStructureCategory][tempStructureNumber]["site"][i].AsInt;
			Debug.Log(structure.extent[i % x, i / x]);
        }*/

        AllocateStructure();
	}

    // JSON 파일에서 건물 정보 읽어서 structure에 저장.
    public void SetStructureDataFromJSON(Structure structure, string category, int num)
    {
        DesireType tempType = DesireType.Base;
        int tempResolveAmount = 0;
        switch (tempStructureCategory)
        {
            case "Drink":
                tempType = DesireType.Thirsty;
                tempResolveAmount = structureJson[tempStructureCategory][tempStructureNumber]["desire"]["thirsty"].AsInt;
                break;

            case "Food":
                tempType = DesireType.Hungry;
                tempResolveAmount = structureJson[tempStructureCategory][tempStructureNumber]["desire"]["hungry"].AsInt;
                break;

            case "Lodge":
                tempType = DesireType.Sleep;
                tempResolveAmount = structureJson[tempStructureCategory][tempStructureNumber]["desire"]["sleepy"].AsInt;
                break;

            case "Equipment":
                tempType = DesireType.Equipment;
                tempResolveAmount = structureJson[tempStructureCategory][tempStructureNumber]["desire"]["equipment"].AsInt;
                break;

            case "Tour":
                tempType = DesireType.Tour;
                tempResolveAmount = structureJson[tempStructureCategory][tempStructureNumber]["desire"]["tour"].AsInt;
                break;

            case "Convenience":
                tempType = DesireType.Convenience;
                tempResolveAmount = structureJson[tempStructureCategory][tempStructureNumber]["desire"]["convenience"].AsInt;
                break;

            case "Fun":
                tempType = DesireType.Fun;
                tempResolveAmount = structureJson[tempStructureCategory][tempStructureNumber]["desire"]["fun"].AsInt;
                break;

            case "Santuary":
                tempType = DesireType.Health;
                tempResolveAmount = structureJson[tempStructureCategory][tempStructureNumber]["desire"]["health"].AsInt;
                break;

            case "Rescue":
                tempType = DesireType.Rescue;
                break;
        }

        structure.name = structureJson[tempStructureCategory][tempStructureNumber]["name"];
        structure.type = structureJson[tempStructureCategory][tempStructureNumber]["type"];
        structure.capacity = structureJson[tempStructureCategory][tempStructureNumber]["capacity"].AsInt;
        structure.duration = structureJson[tempStructureCategory][tempStructureNumber]["duration"].AsInt;
        structure.charge = structureJson[tempStructureCategory][tempStructureNumber]["charge"].AsInt;
        structure.resolveType = tempType;
        structure.resolveAmount = tempResolveAmount;
        // 저장용.
        structure.structureCategory = tempStructureCategory;
        structure.structureNumber = tempStructureNumber;

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


        structure.genre = structureJson[tempStructureCategory][tempStructureNumber]["genre"];
        structure.expenses = structureJson[tempStructureCategory][tempStructureNumber]["expenses"].AsInt;

        int x = structureJson[tempStructureCategory][tempStructureNumber]["sitewidth"].AsInt;
        structure.extentWidth = x;

        int y = structureJson[tempStructureCategory][tempStructureNumber]["sitelheight"].AsInt;
        structure.extentHeight = y;

        structure.extent = new int[x, y];
        //Debug.Log(x + " , " + y);
        for (int i = 0; i < y; i++)
        {
            for (int j = 0; j < x; j++)
            {
                structure.extent[j, i] = structureJson[tempStructureCategory][tempStructureNumber]["site"][j + (i * x)].AsInt;
            }
        }
    }
	
	public void AllocateStructure()
	{
		GameObject nearest = null;

		GameObject[] gos = GameObject.FindGameObjectsWithTag("Tile");
		Vector2 pos = Camera.main.ScreenToWorldPoint(new Vector3(Camera.main.pixelWidth / 2, Camera.main.pixelHeight / 2, 0.0f));
		float minDiff = Mathf.Infinity;
		Debug.Log("Allocated!!!!");

        //가장 가까운 타일 찾기(카메라 중심의 타일 찾기)
		foreach (GameObject go in gos)
		{
			float diff = ((Vector2)go.transform.position - pos).magnitude;
			if (minDiff > diff && go.GetComponent<Tile>().GetBuildingArea())
			{
				minDiff = diff;
				nearest = go;
			}
		}
        // 건설할 건물의 position 설정(가장 가까운 타일에 맞춰)
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

    // 위까지 생략하되 constructing에 설정하는 건 가져와줘야.
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

        // 인덱스 값 넣어줌.
        structure.index = structures.Count;

        // 리스트에 추가
        structures.Add(structure);
        
		
		for(int i = 0; i<structure.extentHeight; i++)
		{
			for(int j = 0; j<structure.extentWidth; j++)
			{
                Tile tempTile = tileLayer.GetTileAsComponent(tile.GetX() + j, tile.GetY() + i);

                if (extent[j,i] == 1)
				{
					tempTile.SetIsBuildingArea(false);
                    tempTile.SetIsStructed(true);
                    tempTile.SetStructure(structure);
					tempTile.SetTileColor(new Color(1.0f, 0.3f, 0.2f));
                }
                else if(extent[j, i] == 2)
                {
					//tempTile.SetIsStructed(false);
					//tempTile.SetIsEntrance(true);
                    if(tempTile.GetBuildingArea())
                    {
                        structure.addEntrance(tempTile);
						tempTile.SetTileColor(new Color(0.2f, 0.2f, 1.0f));
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
		foreach(GameObject go in constructingAreas)
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

        // 건설영역을 돌면서 건설 가능한가를 봄.
		for (int i = 0; i < conStructure.extentHeight; i++)//StructureManager.Instance.GetConstructing().GetComponent<Structure>().extentHeight; i++)
		{
			for (int j = 0; j < conStructure.extentWidth; j++)//StructureManager.Instance.GetConstructing().GetComponent<Structure>().extentWidth; j++) //건물의 영역 크기 만큼의 2차원 배열
			{
				if (extent[j, i] == 0) // 건물의 영역에서 0인 부분
				{

				}
				else
				{
                    // 해당 영역의 타일 받기.
					Tile t = GameManager.Instance.GetMap().GetLayer(0).GetComponent<TileLayer>().GetTileAsComponent(x + j, y + i);

                    // 건설 가능한가, 입구인가에 따라 색칠하고 건설 불가면 bool 변수 변경해줌.
					if (t != null)
					{
						Debug.Log("Area Index = " + areaIndex + " ConstructingAreasCount = " + constructingAreas.Count) ;
						if (t.GetBuildingArea() == true && extent[j, i] == 1) // 입구 아니고 타일 건설가능할때
						{
							StructureManager.Instance.constructingAreas[areaIndex].transform.position = t.transform.position;
							StructureManager.Instance.constructingAreas[areaIndex++].GetComponent<SpriteRenderer>().color = Color.green; //new Color(0.0f, 1.0f, 0.0f);
							//constructing.GetComponent<Structure>().SetisConstructable(true);
						}
						else if (t.GetBuildingArea() == true && extent[j, i] == 2) // 입구 이고 타일 길일때
						{
							StructureManager.Instance.constructingAreas[areaIndex].transform.position = t.transform.position;
							StructureManager.Instance.constructingAreas[areaIndex++].GetComponent<SpriteRenderer>().color = Color.blue;//new Color(0.0f, 0.0f, 1.0f);
							//constructing.GetComponent<Structure>().SetisConstructable(true);
						}
						else if (t.GetBuildingArea() == false && extent[j, i] == 1) // 입구 아니고 타일 건설 불가능일때
						{
							StructureManager.Instance.constructingAreas[areaIndex].transform.position = t.transform.position;
							StructureManager.Instance.constructingAreas[areaIndex++].GetComponent<SpriteRenderer>().color = Color.red;//new Color(1.0f, 0.0f, 0.0f);
							conStructure.SetisConstructable(false); // 입구 아닌 부분이 건설 불가능일때만 건설 불가 처리.

						}
						else if (t.GetBuildingArea() == false && extent[j, i] == 2) // 입구 이고 타일 길이아닐때
						{
							StructureManager.Instance.constructingAreas[areaIndex].transform.position = t.transform.position;
							StructureManager.Instance.constructingAreas[areaIndex++].GetComponent<SpriteRenderer>().color = Color.yellow;//new Color(1.0f, 1.0f, 0.0f);
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
                    if (t.GetBuildingArea() && ex[j,i] == 2 && !t.GetStructed())
                    {
						//t.SetIsStructed(true);
						if (s.entrance.Exists(
							delegate (Tile element)
							{
								if (element.Equals(t))
									return true;
								else
									return false;
							}))
						{
							//exist ! do nothing;
						}
						else
							s.entrance.Add(t);
					}
                    else if(/*!t.GetBuildingArea() &&*/ ex[j,i] == 2 && t.GetStructed())
                    {
                        t.SetIsStructed(false);
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
                    s.point.GetLayer().GetTileAsComponent(s.point.GetX() + i, s.point.GetY() + j).SetIsStructed(false);
                    s.point.GetLayer().GetTileAsComponent(s.point.GetX() + i, s.point.GetY() + j).SetIsBuildingArea(true);
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

	public List<Structure> FindStructureByDesire(DesireType type, Traveler t) // 전달받은 stat의 종족, 부, 소지골드, 직업에따라 최적의 건물 반환.
	{
		
		return new List<Structure> ((from s in structures where s.resolveType == type && s.GetWaitSeconds() < 120.0f && t.stat.gold > s.charge orderby s.preference.GetPrefSum(t.stat.race, t.stat.wealth, t.stat.job) descending select s).ToArray());
		//기존 preferences 최대값 150.0f
		//이용 요금 고려
		//욕구 타입과 대기 시간 고려
		//순서는 종족, 부, 직업 고려
		//거리?
		
	}

    public Structure[] FindRescue(Traveler t) // 수정필요할듯?
    {
        return(from s in structures where s.resolveType == DesireType.Rescue && s.GetWaitSeconds() < 15.0f && t.stat.gold > s.charge orderby s.preference.GetPrefSum(t.stat.race, t.stat.wealth, t.stat.job) descending select s).ToArray();
    }

    public void LoadStructuresFromSave(GameSavedata savedata)
    {
        ClearStructureManager();
        for(int i=0; i<savedata.structureDatas.Count; i++)
        {
            ConstructStructureFromData(savedata.structureDatas[i]);
        }
    }

    public void SetStructuresGuest(GameSavedata savedata)
    {
        for (int i = 0; i < savedata.structureDatas.Count; i++)
        {
            SetSingleStructureGuest(structures[i], savedata.structureDatas[i]);
        }
    }

    public void ClearStructureManager()
    {
        ResetConstructingAreas();

        if (constructing != null)
            DestroyConstructing();

        structures.Clear();
        int childCount = rootStructureObject.transform.childCount;
        for (int i = 0; i < childCount; i++)
            Destroy(rootStructureObject.transform.GetChild(i).gameObject);

        //structureJson 는 일단 손 안댐.
    }

    public void ConstructStructureFromData(StructureData input) // Resolve Type, ResolveAmount 내용 추가해야됨. 20200528
    {
        #region InstantiateStructure()
        tempStructureCategory = input.structureCategory;
        tempStructureNumber = input.structureNumber;

        constructing = (GameObject)Instantiate(Resources.Load("Structure/StructurePrefabs/" + tempStructureCategory + "/" + tempStructureCategory + tempStructureNumber.ToString()));
        constructing.transform.parent = rootStructureObject.transform;

        Structure structure = constructing.GetComponent<Structure>();
        SetStructureDataFromJSON(structure, tempStructureCategory, tempStructureNumber);
        //임시
        
        //structure.name = structureJson[tempStructureCategory][tempStructureNumber]["name"];
        //structure.type = structureJson[tempStructureCategory][tempStructureNumber]["type"];
        //structure.capacity = structureJson[tempStructureCategory][tempStructureNumber]["capacity"].AsInt;
        //structure.duration = structureJson[tempStructureCategory][tempStructureNumber]["duration"].AsInt;
        //structure.charge = structureJson[tempStructureCategory][tempStructureNumber]["charge"].AsInt;

        //// 저장용.
        //structure.structureCategory = tempStructureCategory;
        //structure.structureNumber = tempStructureNumber;

        ////preference
        //structure.preference.SetPrefAdventurer(structureJson[tempStructureCategory][tempStructureNumber]["preference"]["adventurer"].AsFloat);
        //structure.preference.SetPrefTourist(structureJson[tempStructureCategory][tempStructureNumber]["preference"]["tourist"].AsFloat);
        //structure.preference.SetPrefHuman(structureJson[tempStructureCategory][tempStructureNumber]["preference"]["human"].AsFloat);
        //structure.preference.SetPrefElf(structureJson[tempStructureCategory][tempStructureNumber]["preference"]["elf"].AsFloat);
        //structure.preference.SetPrefDwarf(structureJson[tempStructureCategory][tempStructureNumber]["preference"]["dwarf"].AsFloat);
        //structure.preference.SetPrefOrc(structureJson[tempStructureCategory][tempStructureNumber]["preference"]["orc"].AsFloat);
        //structure.preference.SetPrefDog(structureJson[tempStructureCategory][tempStructureNumber]["preference"]["cat"].AsFloat);
        //structure.preference.SetPrefUpperclass(structureJson[tempStructureCategory][tempStructureNumber]["preference"]["upperclass"].AsFloat);
        //structure.preference.SetPrefMiddleclass(structureJson[tempStructureCategory][tempStructureNumber]["preference"]["middleclass"].AsFloat);
        //structure.preference.SetPrefLowerclass(structureJson[tempStructureCategory][tempStructureNumber]["preference"]["lowerclass"].AsFloat);
        ////desire

        //structure.genre = structureJson[tempStructureCategory][tempStructureNumber]["genre"];
        //structure.expenses = structureJson[tempStructureCategory][tempStructureNumber]["expenses"].AsInt;

        //int x = structureJson[tempStructureCategory][tempStructureNumber]["sitewidth"].AsInt;
        //structure.extentWidth = x;

        //int y = structureJson[tempStructureCategory][tempStructureNumber]["sitelheight"].AsInt;
        //structure.extentHeight = y;

        //structure.extent = new int[x, y];
        //for (int i = 0; i < x * y; i++)
        //{
        //    structure.extent[i % x, i / x] = structureJson[tempStructureCategory][tempStructureNumber]["site"][i].AsInt;
        //}
        //#endregion

        #region AllocateStructure()
        // 건설할 건물의 position 설정
        constructing.transform.position = new Vector3(input.position.x, input.position.y, input.position.z);

        TileLayer tileLayer = TileMapGenerator.Instance.tileMap_Object.transform.GetChild(0).GetComponent<TileLayer>();

        SetStructurePoint(tileLayer.transform.GetChild(input.pointTile).GetComponent<Tile>());
        #endregion

        #region ConstructStructure()
        Tile tile = structure.point;
        int[,] extent = structure.GetExtent();

        constructing.tag = "Structure";

        ResetConstructingAreas();

        // 인덱스 값 넣어줌.
        structure.index = structures.Count;

        // 리스트에 추가
        structures.Add(structure);

        for (int i = 0; i < input.entranceList.Count; i++)
            structure.addEntrance(tileLayer.transform.GetChild(input.entranceList[i]).GetComponent<Tile>());

        constructing = null;
        isConstructing = false;
        #endregion

        
        //// 사용중 모험가, 대기중 모험가 큐에 넣어줌.
        //while (input.curUsingQueue.Count()>0)
        //{
        //    //structure.LoadEnteredTraveler(GameManager.Instance.travelersEnabled[input.curUsingQueue.Dequeue()].GetComponent<Traveler>(), input.elapsedTimeQueue.Dequeue());
        //    structure.LoadEnteredTraveler(input.curUsingQueue.Dequeue());
        //}

        //while(input.curWaitingQueue.Count()>0)
        //{
        //    //structure.AddWaitTraveler(GameManager.Instance.travelersEnabled[input.curWaitingQueue.Dequeue()].GetComponent<Traveler>());
        //    structure.LoadWaitingTraveler(input.curWaitingQueue.Dequeue());
        //}
    }

    private void SetSingleStructureGuest(Structure structure, StructureData data)
    {
        // 사용중 모험가, 대기중 모험가 큐에 넣어줌.
        while (data.curUsingQueue.Count() > 0)
        {
            //structure.LoadEnteredTraveler(GameManager.Instance.travelersEnabled[input.curUsingQueue.Dequeue()].GetComponent<Traveler>(), input.elapsedTimeQueue.Dequeue());
            structure.LoadEnteredTraveler(data.curUsingQueue.Dequeue());
        }

        while (data.curWaitingQueue.Count() > 0)
        {
            //structure.AddWaitTraveler(GameManager.Instance.travelersEnabled[input.curWaitingQueue.Dequeue()].GetComponent<Traveler>());
            structure.LoadWaitingTraveler(data.curWaitingQueue.Dequeue());
        }
    }
    #endregion
	public JSONNode GetStructuresJSON()
	{
		return structureJson;
	}
}