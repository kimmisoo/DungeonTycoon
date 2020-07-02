using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine.SceneManagement;

public class SaveLoadManager : MonoBehaviour
{
    private static SaveLoadManager _instance;

    public static SaveLoadManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<SaveLoadManager>();

                if(_instance == null)
                {
                    GameObject container = new GameObject("SaveLoadManager");
                    _instance = container.AddComponent<SaveLoadManager>();
                }
            }

            return _instance;
        }
    }
    string savedataPath;
    GameSavedata savedata;

    private void Start()
    {
        if (_instance != null && _instance != this)
        {
            Debug.Log("이미 있음." + this.gameObject.GetInstanceID());
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
    }

    // 현 상태 저장
    public void Save()
    {
        // 데이터 패스 설정
        savedataPath = Application.persistentDataPath + "/test.sav"; // 임시
        string sceneName = SceneManager.GetActiveScene().name; 

        // 저장
        BinaryFormatter bf = new BinaryFormatter();
        FileStream stream = new FileStream(savedataPath, FileMode.Create);

        GameSavedata data = new GameSavedata(sceneName);

        bf.Serialize(stream, data);
        stream.Close();
    }

    public void Load()
    {
        LoadFromSave();
        LoadScene(savedata.sceneName);
        //InstantiateFromSave();
    }

    // 세이브 파일에서 로드
    public GameSavedata LoadFromSave()
    {
        // 데이터 패스 설정
        savedataPath = Application.persistentDataPath + "/test.sav"; // 임시

        // 불러오기
        if (File.Exists(savedataPath))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream stream = new FileStream(savedataPath, FileMode.Open);

            savedata = bf.Deserialize(stream) as GameSavedata;

            stream.Close();
            return savedata;
        }
        else
        {
            return null;
        }
    }

    /// <summary>
    /// 세이브 파일에서 정보 읽어서 로드
    /// </summary>
    /// <returns>로드 되었는지 아닌지</returns>
    public bool InstantiateFromSave()
    {
        if (savedata == null)
            return false;

        // 세이브에서 받아서 결과값 대입
        if (savedata != null)
        {
            GameManager.Instance.LoadPlayerData(savedata);

            GameManager.Instance.LoadTileMap(savedata);
            GameManager.Instance.LoadTravelerList(savedata);

            StructureManager.Instance.LoadStructuresFromSave(savedata);

            TileMapGenerator.Instance.SetTileStructure(savedata);

            Debug.Log("불러오기 성공");

            savedata = null;

            return true;
        }
        // 실패 메시지 출력
        else
        {
            Debug.Log("불러오기 실패");
            return false;
        }
    }

    // Scene 로드
    public void LoadScene(int sceneNum)
    {
        SceneManager.LoadScene(sceneNum);
    }
    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}

