using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;

public class MonsterManager : MonoBehaviour {
	public static MonsterManager Instance
	{
		get
		{
			if (_instance != null)
				return _instance;
			else
				return null;
		}
	}
	static MonsterManager _instance = null;
	JSONNode monsterData;
	TextAsset monsterDataText;
	List<GameObject> monsters;
	Moveto pos;
	public Transform monstersTransform;
	// Use this for initialization
	private void Awake()
	{
		_instance = this;
	}
	void Start () {
		monsters = new List<GameObject>();
		
		monsterDataText = Resources.Load<TextAsset>("Monsters/Monsters");
		//monsterData = JSON.Parse(monsterDataText.text);
		monsters.Add(Instantiate(Resources.Load<GameObject>("MonsterPrefabs/Monster_test")));
		monsters[0].GetComponent<Moveto>().SetCurPos(GameManager.Instance.GetMap().GetLayer(0).GetComponent<TileLayer>().GetTileAsComponent(21, 33));
		//monsters[0].transform.position = GameManager.Instance.GetMap().GetLayer(0).GetComponent<TileLayer>().GetTile(21, 33).gameObject.transform.position + (Vector3.forward * 0.1f);
		//monsters[0].transform.position += Vector3.up;
	}
	
	

	public GameObject GetRandomMonster_test( )
	{
		/*
		if (monsters.Count >= 1 && monsters[0].activeSelf == true)
			return monsters[0];
		else
			return null;*/
		return null;
	}
}
