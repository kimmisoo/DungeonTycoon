using UnityEngine;
using System.Collections;
using System.IO;
using SimpleJSON;
using System.Collections.Generic;

public class SaveFileLoader : MonoBehaviour {

    public JSONNode saveData;
    List<GameObject> loadObjects;
    public GameObject scroll;

    public float objectTransformY = -570.0f;
    public float objectTransformX = 315.0f;
    public float objectMarginX = 90.0f;
    // Use this for initialization

    void Start()
    {
        
        
    }
    private void OnEnable()
    {
        LoadAll();
    }
    private void OnDisable()
    {
        DestroyAll();
    }
    public void LoadAll()
    {
        loadObjects = new List<GameObject>();
        
        if(File.Exists(Application.persistentDataPath + "/saves.json") == true)
        {
            
            FileStream fs = new FileStream(Application.persistentDataPath + "/saves.json", FileMode.Open, FileAccess.ReadWrite);
            StreamReader sr = new StreamReader(fs);
            saveData = JSONNode.Parse(sr.ReadToEnd());
            sr.Close();
            fs.Close();
            
        }
        else
        {
            Debug.Log(Resources.Load<TextAsset>("saveform").text);
            saveData = JSON.Parse(Resources.Load<TextAsset>("saveform").text);
            Debug.Log(saveData.ToString());
            FileStream fs = new FileStream(Application.persistentDataPath + "/saves.json", FileMode.Create, FileAccess.ReadWrite);
            StreamWriter sw = new StreamWriter(fs);
            Debug.Log(saveData.ToString());
            sw.Write(saveData.ToString());
            sw.Flush();
            fs.Flush();
            sw.Close();
            fs.Close();
        }
        if (saveData.Count == 0)
        {
            RectTransform rt;

            loadObjects.Add((GameObject)Instantiate(Resources.Load("UIPrefabs/LoadObject")));
            rt = loadObjects[0].GetComponent<RectTransform>();

            rt.parent = scroll.GetComponent<RectTransform>();
            rt.anchoredPosition = new Vector3(315.0f, -570.0f, 0.0f);
            rt.localScale = new Vector3(1.0f, 1.0f, 1.0f);
            loadObjects[0].GetComponent<LoadObject>().LoadThisObject();
        }
        else
        {
            for (int i = 0; i < saveData.Count; i++)
            {
                RectTransform rt;

                loadObjects.Add((GameObject)Instantiate(Resources.Load("UIPrefabs/LoadObject")));
                rt = loadObjects[i].GetComponent<RectTransform>();

                rt.parent = scroll.GetComponent<RectTransform>();
                rt.anchoredPosition = new Vector3(i * 500.0f + 315.0f, -570.0f, 0.0f);
                rt.localScale = new Vector3(1.0f, 1.0f, 1.0f);
                loadObjects[i].GetComponent<LoadObject>().LoadThisObject(saveData[i]);
            }
        }
    }

    public void DestroyAll()
    {
        for(int i  = 0; i<loadObjects.Count; i++)
        {
            Destroy(loadObjects[i]);
        }
    }

}
