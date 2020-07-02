using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIManager : MonoBehaviour {
	static UIManager _instance;
	
	public static UIManager Instance
	{
		get
		{
			if (_instance != null)
				return _instance;
			else
            {
				Debug.Log("UIManager error!");
				return null;
			}
		}
	}
    private Stack uiStack;

	public GameObject trainPanel;
	public GameObject infoPanel;
	public GameObject buildPanel;
	public GameObject settingsPanel;
	public GameObject talkPanel;
	public GameObject muteButton;
	//-- 스택1

	public GameObject foodPanel;
	public GameObject drinkPanel;
	public GameObject lodgePanel;
	public GameObject equipmentPanel;
	public GameObject tourPanel;
	public GameObject conveniencePanel;
	public GameObject funPanel;
	public GameObject santuaryPanel;
	public GameObject rescuePanel;

	//Specification Panel
	public GameObject characterSpecPanel;
	public GameObject monsterSpecPanel;
	public GameObject tileSpecPanel;
	public GameObject structureSpecPanel;

	//---건설
	public GameObject structureObject;
	public GameObject structureScroll;
	public Text structureName;
	public Image genreImage;
	public Text genreText;
	public Image structureIamge;
	public Image structureArea;
	public Text structureCharge;
	public Text structureCapacity;
	public Text structureUsagetime;
	public Text structurePreferenceUp;
	public Text structurePreferenceDown;
	public Text structureExplanation;
	public Text structureBonus;
	public Text structureConstruction;
	


	//---건설 세부
	public Sprite sound_On;
	public Sprite sound_Off;

	

    public Sprite pauseNonHighlighted;
    public Sprite pauseHighlighted;
    public Sprite playNonHighlighted;
    public Sprite playHighlighted;
    public Sprite fastNonHighlighted;
    public Sprite fastHighlighted;

    public GameObject selectedTimeScale;

    public Text goldText;

	bool isShowing = false;
	bool isMute = false;


    public Material grayScaleMaterial;

	public SpecificationPanel specPanel;
	void Awake()
	{
		_instance = this;
        uiStack = new Stack();
	}
    
    
    
    void Start()
    {
        StartCoroutine("UIUpdate");
    }
	public void CharacterSelected(Traveler trv)
	{
		specPanel.OnCharacterSelected(trv);
	}
	public void CharacterDeselected()
	{
		specPanel.OnCharacterDeselected();
	}
    IEnumerator UIUpdate()
    {
        bool isInterpolatingGold = false;
        bool isInterpolatingPop = false;
        int goldTo = 0;
		
        while(true)
        {
            yield return null;
            if(goldText.text != GameManager.Instance.GetPlayerGold().ToString() && isInterpolatingGold == false) // interpolate중이 아니고 골드 변경이 있을때
            {
                isInterpolatingGold = true;
                goldTo = GameManager.Instance.GetPlayerGold();
                StartCoroutine("InterpolateGold", goldTo);
                
            }
            else if(isInterpolatingGold == true && goldTo != GameManager.Instance.GetPlayerGold()) //interpolate중인데 골드 변경이 있을때
            {
                StopCoroutine("InterpolateGold");
                goldTo = GameManager.Instance.GetPlayerGold();
                StartCoroutine("InterpolateGold", goldTo);
                

            }
            else if(isInterpolatingGold == true && Mathf.Abs(goldTo - int.Parse(goldText.text)) < 5.0f) // interpolate 종료
            {
                StopCoroutine("InterpolateGold");
                isInterpolatingGold = false;
                goldText.text = GameManager.Instance.GetPlayerGold().ToString();
                
            }


            

        }
    }
    
    IEnumerator InterpolateGold(int goldTo)
    {
        int txtGold = 0;
        int prev = 0;
        while (true)
        {
            
            yield return new WaitForSeconds(0.02f);

            txtGold = int.Parse(goldText.text);
            prev = txtGold;
            goldText.text = ((int)Mathf.Lerp((float)txtGold, (float)goldTo, 0.13f)).ToString();
            if(prev == int.Parse(goldText.text))
            {
                goldText.text = GameManager.Instance.GetPlayerGold().ToString();
            }
        }
    }



    public void ShowUI(GameObject go)
    {
        isShowing = true;
        go.GetComponent<UIObject>().Show();
        uiStack.Push(go);
        
    }
    public void HideUI()
    {
        if (uiStack.Count >= 1)
        {
            ((GameObject)uiStack.Pop()).GetComponent<UIObject>().Hide(); //스택 빼고 Hide
            if (uiStack.Count <= 0)
                isShowing = false;
        }
        
        
    }
    ////////////////////////////////////////////////////////////////////////




    public void OpenTrainPanel()
	{
		CloseAll();
		trainPanel.SetActive(true);
		isShowing = true;
	}

	public void OpenInfoPanel()
	{
		CloseAll();
		infoPanel.SetActive(true);
		isShowing = true;
	}

	public void OpenBuildPanel()
	{
		CloseAll();
		buildPanel.SetActive(true);
		OpenBuilidPanel(drinkPanel);
		isShowing = true;
	}

	public void OpenSettingsPanel()
	{
		if (isShowing == true)
		{
			CloseAll();
		}
		else
		{
			CloseAll();
			settingsPanel.SetActive(true);
			isShowing = true;
		}
	}
	public void OpenTalkPanel()
	{
		if (isShowing == true)
		{
			CloseAll();
		}
		else
		{
			CloseAll();
			talkPanel.SetActive(true);
			isShowing = true;
		}
	}
	public void SoundOnOff()
	{
		if(isMute == false)
		{
			muteButton.GetComponent<Image>().sprite = sound_Off;
			isMute = true;
		}
		else
		{
			muteButton.GetComponent<Image>().sprite = sound_On;
			isMute = false;
		}
	}
	public void OpenBuilidPanel(GameObject p)
	{
		CloseBuildPanel();
		p.SetActive(true);
	}
	public void CloseBuildPanel()
	{
		foodPanel.SetActive(false);
		drinkPanel.SetActive(false);
		lodgePanel.SetActive(false);
		equipmentPanel.SetActive(false);
		tourPanel.SetActive(false);
		funPanel.SetActive(false);
		rescuePanel.SetActive(false);
		santuaryPanel.SetActive(false);
		conveniencePanel.SetActive(false);
	}
	public void CloseAll()
	{
		trainPanel.SetActive(false);
		infoPanel.SetActive(false);
		buildPanel.SetActive(false);
		settingsPanel.SetActive(false);
		talkPanel.SetActive(false);
		CloseBuildPanel();
		isShowing = false;
	}
    public void SelectTimeScale(GameObject go)
    {

        if (selectedTimeScale.tag == go.tag)
            return;

        Image img = selectedTimeScale.GetComponent<Image>();
        switch(selectedTimeScale.tag)
        {
            case "timeScale_Pause":
                img.sprite = pauseNonHighlighted;
                break;
            case "timeScale_Play":
                img.sprite = playNonHighlighted;
                break;
            case "timeScale_Fast":
                img.sprite = fastNonHighlighted;
                break;
            default:
                break;
        }
        img = go.GetComponent<Image>();
        selectedTimeScale = go;
        switch(go.tag)
        {
            case "timeScale_Pause":
                img.sprite = pauseHighlighted;
                break;
            case "timeScale_Play":
                img.sprite = playHighlighted;
                break;
            case "timeScale_Fast":
                img.sprite = fastHighlighted;
                break;
            default:
                break;
        }
    }
	
	public bool GetisShowing()
	{
		return isShowing;
	}
	/////////////////////////////////////////////////////////////////////////////////////////
	public void RefreshCharacterInfo()
	{

	}
}
