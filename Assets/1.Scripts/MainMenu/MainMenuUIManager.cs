using UnityEngine;
using System.Collections;

public class MainMenuUIManager : MonoBehaviour {

    public GameObject loadPanel;
    
    public GameObject settingsPanel;
    public GameObject staffPanel;

    public GameObject loadPopup;
    public GameObject deletePopup;
    public GameObject exitPopup;

    public GameObject[] mainMenuButtons;

    public SaveFileLoader sfl;

    Stack uiStack;

    private void Awake()
    {
        uiStack = new Stack();
    }
    
	// Use this for initialization
	void Start () {
            
	}
	
	// Update is called once per frame
	void Update () {
	    if(Input.GetKeyDown(KeyCode.Escape))
        {
            HideUI();
        }    
	}
    public void HideUI()
    {
        if (uiStack.Count == 0)
            ShowUI(exitPopup);
        else if (uiStack.Count == 1)
        {
            GameObject t = (GameObject)uiStack.Pop();
            t.SetActive(false);
            foreach (GameObject go in mainMenuButtons)
            {
                go.SetActive(true);
            }
        }
        else
        {
            GameObject t = (GameObject)uiStack.Pop();
            t.SetActive(false);
        }

    }
    public void ShowUI(GameObject ui)
    {
        foreach (GameObject go in mainMenuButtons)
        {
            go.SetActive(false);
        }

        ui.SetActive(true);
        uiStack.Push(ui);
    }
    
    public void StartNewGame()
    {
        GameManager.Instance.LoadScene("1");
    }

    


}
