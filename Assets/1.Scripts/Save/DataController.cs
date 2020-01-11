using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

public class DataController : MonoBehaviour
{
    static GameObject _container;

    static GameObject Container
    {
        get
        {
            return _container;
        }
    }
    static DataController _instance;

    public static DataController Instance
    {
        get
        {
            if (_instance == null)
            {
                Debug.Log("데이터 컨트롤러 없음. 에러");
            }

            return _instance;
        }
    }


    private void Awake()
    {
        _instance = this;
    }

    public string GameDataFileName = "save.json";

    GameData _data;
    GameData Data
    {
        get
        {
            if(_data == null)
            {
                LoadGameData();
                SaveGameData();
            }
            return _data;
        }
        set
        {
            _data = value;
        }
    }

    public void LoadGameData()
    {
        string filePath = Application.persistentDataPath + GameDataFileName;

        if (File.Exists(filePath))
        {
            Debug.Log("불러오기 성공!");
            string FromJsonData = File.ReadAllText(filePath);
            _data = JsonUtility.FromJson<GameData>(FromJsonData);
            Debug.Log(_data);
        }
        else
        {
            Debug.Log("새 파일 생성");

            _data = new GameData("테스트");
        }
    }

    public void SaveGameData()
    {
        string toJsonData = JsonUtility.ToJson(Data);
        Debug.Log(toJsonData);
        string filePath = Application.persistentDataPath + GameDataFileName;
        File.WriteAllText(filePath, toJsonData);
        Debug.Log("저장 완료");
    }

    private void Start()
    {
        LoadGameData();
    }

    private void OnApplicationQuit()
    {
        SaveGameData();
    }
}

[Serializable]
public class GameData
{
    public string str;

    public GameData(string input)
    {
        str = input;
    }
}

