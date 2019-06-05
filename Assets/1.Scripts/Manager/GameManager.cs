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

    public List<GameObject> characters;
	public List<GameObject> travelers;
	public List<GameObject> adventurers;
	public List<GameObject> specialAdventurers;

    public int corporateNum = 1;
    public List<float> popular;

    public JSONNode items;
	private object lockObject = new object();
	private object lockObject2 = new object();
	//////////////////////////////////////////SceneData
	int traveler_Max = 0;
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
		
        for (int i = 0; i < traveler_Max; i++)
        {
			//씬마다 ~~~ Traveler , Adventurer 숫자 정해줘야함.
			
            GameObject go = (GameObject)Resources.Load("CharacterPrefabs/Traveler_test");
            go.SetActive(false);
            travelers.Add(Instantiate(go));
            go.transform.position = new Vector3(5000.0f, 5000.0f, 5000.0f);
            travelers[i].transform.parent = GameObject.FindGameObjectWithTag("Characters").transform;
			   
        }
        
		

		for (int i = 0; i < adventurer_Max; i++)
		{
			//씬마다 ~~~ Traveler , Adventurer 숫자 정해줘야함.
            
			GameObject go = (GameObject)Resources.Load("CharacterPrefabs/Adventurer_test");
			go.SetActive(false);
			adventurers.Add(Instantiate(go));
			go.transform.position = new Vector3(5000.0f, 5000.0f, 5000.0f);
			adventurers[i].transform.parent = GameObject.FindGameObjectWithTag("Characters").transform;

		}

		for (int i = 0; i < specialAdventurer_Max; i++)
		{
			GameObject go;
			//씬마다 ~~~ Traveler , Adventurer 숫자 정해줘야함.
			int randNumber = Random.Range(0, 15);
			switch(randNumber)
			{
				case 0:
					go = (GameObject)Resources.Load("CharacterPrefabs/Character01_c01_test");//SpecialAdventurer_test");
					break;
				case 1:
					go = (GameObject)Resources.Load("CharacterPrefabs/Character01_c02_test");
					break;
				case 2:
					go = (GameObject)Resources.Load("CharacterPrefabs/Character01_c03_test");
					break;
				case 3:
					go = (GameObject)Resources.Load("CharacterPrefabs/Character02_c01_test");
					break;
				case 4:
					go = (GameObject)Resources.Load("CharacterPrefabs/Character02_c02_test");
					break;
				case 5:
					go = (GameObject)Resources.Load("CharacterPrefabs/Character02_c03_test");
					break;
				case 6:
					go = (GameObject)Resources.Load("CharacterPrefabs/Character03_c01_test");
					break;
				case 7:
					go = (GameObject)Resources.Load("CharacterPrefabs/Character03_c02_test");
					break;
				case 8:
					go = (GameObject)Resources.Load("CharacterPrefabs/Character03_c03_test");
					break;
				case 9:
					go = (GameObject)Resources.Load("CharacterPrefabs/Character04_c01_test");
					break;
				case 10:
					go = (GameObject)Resources.Load("CharacterPrefabs/Character04_c02_test");
					break;
				case 11:
					go = (GameObject)Resources.Load("CharacterPrefabs/Character04_c03_test");
					break;
				case 12:
					go = (GameObject)Resources.Load("CharacterPrefabs/Character05_c01_test");
					break;
				case 13:
					go = (GameObject)Resources.Load("CharacterPrefabs/Character05_c02_test");
					break;
				case 14:
					go = (GameObject)Resources.Load("CharacterPrefabs/Character05_c03_test");
					break;
				default:
					go = (GameObject)Resources.Load("CharacterPrefabs/Character01_c01_test");
					break;
			}
			

			go.SetActive(false);
			specialAdventurers.Add(Instantiate(go));
			go.transform.position = new Vector3(5000.0f, 5000.0f, 5000.0f);
			specialAdventurers[i].transform.parent = GameObject.FindGameObjectWithTag("Characters").transform;
		}


		//StartCoroutine(EnteringCharacter());
		StartCoroutine(TEnter());
		
		//StartCoroutine(GCcall());
        for(int i = 0; i<corporateNum; i++)
        {
            popular.Add(0);
        }
    }
	

	IEnumerator TEnter()
	{
		Debug.Log(specialAdventurer_Max);
		for (int i = 0; i < specialAdventurer_Max; i++)
		{
			//yield return wait;
			yield return null;
			specialAdventurers[i].SetActive(true);
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
	IEnumerator EnteringCharacter()
	{
		
		int remain;
        int traveler_Offset = 0;
		int adventurer_Offset = 0;
		int specialAdventurer_Offset = 0;
		bool isDup = false;
		
		for(int i=0; i<specialAdventurer_Max; i++)
		{
			yield return wait;
			specialAdventurers[i].SetActive(true);
		}

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
}
