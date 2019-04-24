using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using SimpleJSON;
using System.IO;
using System;




public class LoadObject : MonoBehaviour {



    
    public Image loadImage;
    public Text playerName;
    public Text companyName;
    public Text bar;
    public Text stageText;
    public Text stageNumber;
    public Text date;
    public Text time;
    public GameObject deleteButton;

    
    


    public void DeleteThisObject()
    {
        Debug.Log(this.ToString() + " Object is Deleted!");
    }
    public void LoadThisObject()
    {
        Sprite sp;
        sp = Resources.Load<Sprite>("nosave");
        loadImage.sprite = sp;
        stageText.text = " ";
        bar.text = " ";
        deleteButton.SetActive(false);
    }
    public void LoadThisObject(JSONNode js)
    {

        Sprite sp;
        
        byte[] bytes;
        try
        {
            bytes = File.ReadAllBytes(Application.persistentDataPath + "/" + js["screenshot"]);
            Texture2D texture = new Texture2D(4, 4, TextureFormat.RGBA32, false);
            texture.LoadImage(bytes);
            sp = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
            loadImage.sprite = sp;
        }
        catch (FileNotFoundException e)
        {
            Debug.Log("file not found exception");
            sp = Resources.Load<Sprite>("nosave");
            loadImage.sprite = sp;
            stageText.text = " ";
            bar.text = " ";
            deleteButton.SetActive(false);
        }
        catch(UnauthorizedAccessException e)
        {
            Debug.Log("unauthorizedAccess exception");
            sp = Resources.Load<Sprite>("nosave");
            loadImage.sprite = sp;
            stageText.text = " ";
            bar.text = " ";
            deleteButton.SetActive(false);
        }

        playerName.text = js["playername"];
        companyName.text = js["companyname"];
        stageNumber.text = js["stage"];
        date.text = js["date"];
        time.text = js["time"];
        
         



    }

    

    public void SelectThisObjectOnSave()
    {
        Debug.Log(this.ToString() + " Object is Selected!");
    }
    public void SelectThisObjectOnLoad()
    {
        Debug.Log(this.ToString() + " Object is Selected!");
    }



}
