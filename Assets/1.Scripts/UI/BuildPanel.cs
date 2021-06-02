using UnityEngine;
using System.Collections;
using SimpleJSON;

public class BuildPanel : UIObject {

    public GameObject drinkPanel;
    public GameObject foodPanel;
    public GameObject lodgePanel;
    public GameObject equipmentPanel;
    public GameObject tourPanel;
    public GameObject conveniencePanel;
    public GameObject funPanel;
    public GameObject santuaryPanel;
    public GameObject rescuePanel;

    GameObject currentShowingPanel;

	bool isInstantiated = false;
	JSONNode structuresInfo;
	JSONNode structuresMaxInfo;

	public GameObject structureUIEntity; // for dup

    public override void Awake()
    { 
        base.Awake();
    }

	public void Start()
	{
		StartCoroutine(LateStart());
	}
	IEnumerator LateStart()
	{
		yield return null;
		if (isInstantiated == true)
			yield break;
		else
		{
			//건물 ui Instantiate...
			structuresInfo = StructureManager.Instance.GetStructuresJSON();
			structuresMaxInfo = GameManager.Instance.GetStructureMaxInfo();

			isInstantiated = true;
		}
	}
	public override void Show()
    {
        drinkPanel.SetActive(true);
        foodPanel.SetActive(false);
        lodgePanel.SetActive(false);
        equipmentPanel.SetActive(false);
        tourPanel.SetActive(false);
        conveniencePanel.SetActive(false);
        funPanel.SetActive(false);
        santuaryPanel.SetActive(false);
        rescuePanel.SetActive(false);

        base.Show();
        currentShowingPanel = drinkPanel;
        OpenPanel(drinkPanel);
    }
    public override void Hide()
    {
        currentShowingPanel = null;
        base.Hide();
    }
    public void OpenPanel(GameObject panel)
    {
        SetInitialPosition(FindChildScroll(currentShowingPanel));
        currentShowingPanel.SetActive(false);

        currentShowingPanel = panel;
        SetInitialPosition(FindChildScroll(currentShowingPanel));
        currentShowingPanel.SetActive(true);
    }

    public void SetInitialPosition(GameObject scroll)
    {
        RectTransform rt = scroll.GetComponent<RectTransform>();
        rt.anchoredPosition = new Vector2((rt.sizeDelta.x / 2) - rt.GetChild(0).GetComponent<RectTransform>().sizeDelta.x, rt.anchoredPosition.y);
    }
    public GameObject FindChildScroll(GameObject panel)
    {
        for (int i = 0; i < currentShowingPanel.transform.childCount; i++)
        {
            if (currentShowingPanel.transform.GetChild(i).tag == "HorizontalScroll")
            {
                GameObject scroll = panel.transform.GetChild(i).gameObject;
                return scroll;
            }
        }
        Debug.Log("BuilPanel.FindChildScroll() returns null!!\n GameObject name == " + panel.name);
        return null;
    }
	
}
