using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using SimpleJSON;



public class GameManager : MonoBehaviour {

	public static GameManager _instance = null;

	public TileMapGenerator tmg;
    string sceneName;
	JSONNode sceneData;
	
	GameObject tileMap;
	//Tile
	bool isUI = false;
	bool isConstructing = false;
	public int enteringTimeMin = 1;
	public int enteringTimeMax = 5;
	public int enteringMax = 100;
    public int playerGold = 0;
    public int playerPopularity = 0;
    //Characters

	public List<GameObject> travelers;
	public List<GameObject> adventurers;
	public List<GameObject> specialAdventurers;
	public List<GameObject> inactiveTravelers;
	public List<GameObject> inactiveAdventurers;

    public int corporateNum = 1;
    public List<float> popular;

    public JSONNode items;
	private object lockObject = new object();
	private object lockObject2 = new object();
	//////////////////////////////////////////SceneData
	int traveler_Max = 100;
	int adventurer_Max = 0;
	int specialAdventurer_Max = 0;
	int drink_Max = 0;
	int food_Max = 0;
	int lodge_Max = 0;
	int equipment_Max = 0;
	int tour_Max = 0;
	int convenience_Max = 0;
	int fun_Max = 0;
	int santuary_Max = 0;
	int rescue_Max = 0;
	int complete_Popularity = 0;

	int mapEntranceCount = 0;
	public int vertexCount = 0;
	WaitForSeconds wait;
	List<TileForMove> mapEntrance = new List<TileForMove>();
	
	WaitForSeconds countLogWait;

	//////////////////////////////////////////EndofSceneData


	
	public static GameManager Instance
	{
		get
		{
			if (_instance == null)
				Debug.Log("gameManager find Error!");
			return _instance;
		}
	}
	// Use this for initialization
	void Awake()
	{
        _instance = this;
        sceneName = SceneManager.GetActiveScene().name;
        //load Items
        TextAsset itemText = Resources.Load<TextAsset>("Items/items");
        items = JSON.Parse(itemText.ToString());
		wait = new WaitForSeconds(0.11f);
		countLogWait = new WaitForSeconds(3.0f);
	}
	void Start () {
        
		TextAsset sceneTxt = Resources.Load<TextAsset>("SceneData/scenedata");
		SetMap();

		sceneData = JSON.Parse(sceneTxt.ToString());
		setSceneData(sceneData);
		//scene 정보 세팅 
		
        travelers = new List<GameObject>();
        adventurers = new List<GameObject>();
		specialAdventurers = new List<GameObject>();
		inactiveTravelers = new List<GameObject>();
		inactiveAdventurers = new List<GameObject>();
		
        for (int i = 0; i < traveler_Max; i++)
        {
			//씬마다 ~~~ Traveler , Adventurer 숫자 정해줘야함.
			
            GameObject go = (GameObject)Resources.Load("CharacterPrefabs/Traveler_test");
            go.SetActive(false);
            travelers.Add(Instantiate(go));
            go.transform.position = new Vector3(5000.0f, 5000.0f, 5000.0f);
            travelers[i].transform.parent = GameObject.FindGameObjectWithTag("Characters").transform;
			Debug.Log("character instantiate - " + i);
        }
        
		

		StartCoroutine(TEnter());
		
		//StartCoroutine(GCcall());
        for(int i = 0; i<corporateNum; i++)
        {
            popular.Add(0);
        }
    }
	

	IEnumerator TEnter()
	{
		
		for (int i = 0; i < traveler_Max; i++)
		{
			//yield return wait;
			yield return null;
			travelers[i].SetActive(true);
		}
	}
	public void AddPop(int who, float amount)
    {
        popular[who] += amount;
    }
	private void SetMap()
	{
		tileMap = tmg.GenerateMap("TileMap/" + sceneName);
	}
	public TileMap GetMap()
	{
		return tileMap.GetComponent<TileMap>();
	}

	public void Save()
	{
		/*----------------------------------------------------------------*/ // 2016 02 26

		Debug.Log("Saved . . .");

		/*----------------------------------------------------------------*/ // 2016 02 26
	}
	public void SetTimeScale(float ts)
	{
		Time.timeScale = ts;
	}
	

    public void AddGold(int amount)
    {
        playerGold += amount;
    }
    public int GetPlayerGold()
    {
        return playerGold;
    }

	public void setSceneData(JSONNode aData)
	{
		int sceneNumber = int.Parse(SceneManager.GetActiveScene().name);

		traveler_Max = aData["scene"][sceneNumber]["traveler_Max"].AsInt;
		adventurer_Max = aData["scene"][sceneNumber]["adventurer_Max"].AsInt;
		complete_Popularity = aData["scene"][sceneNumber]["complete_Popularity"].AsInt;
		drink_Max = aData["scene"][sceneNumber]["buildable"]["drink"].AsInt;
		food_Max = aData["scene"][sceneNumber]["buildable"]["food"].AsInt;
		lodge_Max = aData["scene"][sceneNumber]["buildable"]["lodge"].AsInt;
		equipment_Max = aData["scene"][sceneNumber]["buildable"]["equipment"].AsInt;
		tour_Max = aData["scene"][sceneNumber]["buildable"]["tour"].AsInt;
		convenience_Max = aData["scene"][sceneNumber]["buildable"]["convenience"].AsInt;
		fun_Max = aData["scene"][sceneNumber]["buildable"]["fun"].AsInt;
		santuary_Max = aData["scene"][sceneNumber]["buildable"]["santuary"].AsInt;
		rescue_Max = aData["scene"][sceneNumber]["buildable"]["rescue"].AsInt;

		mapEntranceCount = aData["scene"][sceneNumber]["mapEntranceCount"].AsInt;
		TileLayer layer = tileMap.GetComponent<TileMap>().GetLayer(0).GetComponent <TileLayer>();
		int x = 0;
		int y = 0;
		for (int i = 0; i< mapEntranceCount; i++)
		{
			x = aData["scene"][sceneNumber]["mapEntrance"][i]["x"].AsInt;
			y = aData["scene"][sceneNumber]["mapEntrance"][i]["y"].AsInt;

			Debug.Log(x + "   " + y);
			mapEntrance.Add(layer.GetTileForMove(x * 2, y * 2));
			mapEntrance.Add(layer.GetTileForMove((x * 2) + 1, y * 2));
			mapEntrance.Add(layer.GetTileForMove(x * 2, (y * 2) + 1));
			mapEntrance.Add(layer.GetTileForMove((x * 2) + 1, (y * 2) + 1));
		}

		traveler_Max = 1;
		adventurer_Max = 1;
		specialAdventurer_Max = 800;

	}
	public Tile GetRandomEntrance()
	{
		int rand = Random.Range(0, mapEntranceCount);
		
		return mapEntrance[rand].GetParent();
	}

    public string getCurrentDateForSave()
	{ 
        return "";
    }
    public void QuitGame()
    {
        Application.Quit();
    }
    public void LoadScene(int sceneNum)
    {
        Application.LoadLevel(sceneNum);
    }
    public void LoadScene(string sceneName)
    {
        Application.LoadLevel(sceneName);
    }

	public Stat ResetTravelerStat(Traveler owner)
	{
		
		Stat stat = owner.stat;
		
		//owner Type 별로 race, job, name, wealth, gender 부여
		//stat 생성하여 owner.Init
		return null;
	}
	
}
