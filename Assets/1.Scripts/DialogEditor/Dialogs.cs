using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
/*Onselected ; 
Delete*/ //2018-03-10


public class Dialogs : MonoBehaviour, ISelectHandler
{
	int index;
	public DialogEditor de;
	Selectable se;
	public string fullText;
	public int representImageLNum = 0;
	public int representImageRNum = 0;

	public Dictionary<int, List<int>> effects = new Dictionary<int, List<int>>();	
	public Dictionary<int, List<int>> illustrationsL = new Dictionary<int, List<int>>();
	public Dictionary<int, List<int>> illustrationsR = new Dictionary<int, List<int>>();
	public Dictionary<int, List<string>> names = new Dictionary<int, List<string>>();

	public Dictionary<int, int> smallIllustrations = new Dictionary<int, int>();
	public Dictionary<int, int> bigIllustrations = new Dictionary<int, int>();
	public Dictionary<int, int> backgrounds = new Dictionary<int, int>();
	public int type;
	public List<Markers> markersList = new List<Markers>();
	public List<Vector3> markerPositionList = new List<Vector3>();
	public Markers selected;
	public GameObject effectObjectOrigin;
	public GameObject effectListContent;
	public GameObject smallMarkerOrigin;
	public bool isRecorded = false;
	public bool isStart = false;
	
	public void Start()
	{
		
		se = gameObject.GetComponent<Selectable>();
				
	}
	public void SetIndex(int i)
	{
		index = i;
		
	}
	public void SetDialogEditor(DialogEditor d)
	{
		de = d;
	}
	public int GetIndex()
	{
		return index;
	}
	
	public void OnSelect(BaseEventData eventData)
	{
		if(de.selectedDialogs != null)
		{
			de.selectedDialogs.GetComponent<Image>().color  = new Color32(0xFF, 0xFF, 0xFF, 0x84);
		}
		GetComponent<Image>().color = new Color32(0x70, 0xFF, 0x7A, 0xD8);
		de.SetDialogSelectIndex(index);
		de.selectedDialogsObject = gameObject;
		de.selectedDialogs = this;
		de.selectedMarkers = null;
		Debug.Log("aaa");
		
		
		
		//throw new NotImplementedException();
		//de.editorsTextInputField.text = fullText;
		//de.selectedDialogs.HideMarker();
		//ShowMarker();
	}
	
	public string GetFullText()
	{
		return fullText;
	}
	public void SetFullText(string _t)
	{
		if(!_t.EndsWith("　"))
			_t = _t + "　";
		fullText = _t;
		switch (type)
		{
			case 1:
				gameObject.GetComponentInChildren<Text>().text = "나레이션 : " +_t;
				break;
			default:
				if (names.ContainsKey(0) && names[0] != null && names[0].Count != 0) // 캐릭터 이름이 존재하는경우
				{
					Debug.Log("1");
					gameObject.GetComponentInChildren<Text>().text = names[0][0] + " : " + _t;
					if(illustrationsL.ContainsKey(0) && illustrationsL[0] != null) // 일러스트L인경우
					{
						Debug.Log("2");
						//gameObject.GetComponentInChildren<Image>().sprite = de.preLoadedIllustrations[int.Parse(illustrations[0][1])];
						Image[] img = gameObject.GetComponentsInChildren<Image>();
						foreach(Image i in img)
						{
							if(!(i.gameObject.Equals(gameObject)))
							{
								i.sprite = de.preLoadedIllustrations[illustrationsL[0][0]];
								representImageLNum = illustrationsL[0][0];
								break;
							}
						}
					}
					else if (illustrationsR.ContainsKey(0) && illustrationsR[0] != null) // 일러스트 R인경우
					{
						Debug.Log("3");
						//gameObject.GetComponentInChildren<Image>().sprite = de.preLoadedIllustrations[int.Parse(illustrations[0][1])];
						Image[] img = gameObject.GetComponentsInChildren<Image>();
						foreach (Image i in img)
						{
							if (!(i.gameObject.Equals(gameObject)))
							{
								i.sprite = de.preLoadedIllustrations[illustrationsR[0][0]];
								representImageRNum = illustrationsR[0][0];
								break;
							}
						}
					}
					else //일러스트가 없는경우
					{
						Debug.Log("4");
						Image[] img = gameObject.GetComponentsInChildren<Image>();
						foreach (Image i in img)
						{
							if (!(i.gameObject.Equals(gameObject)))
							{
								i.sprite = de.preLoadedIllustrations[0];
								break;
							}
						}
					}
				}
				else
				{
					gameObject.GetComponentInChildren<Text>().text = " : " +  _t;
				}
				break;
		}
		
		
		
	}
	public void AddBackground(int key, int bgNum, Vector2 markerPosition)
	{
		//markerPositionList.Add(new Vector3(markerPosition.x, markerPosition.y));
		backgrounds[key] = bgNum;
		
		
		GameObject clone = Instantiate(effectObjectOrigin);
		clone.transform.SetParent(effectListContent.transform);
		clone.GetComponent<RectTransform>().localScale = Vector3.one;
		Markers clonem = clone.GetComponent<Markers>();
		clonem.type = "Background";
		clonem.key = key;
		clonem.valueIndex = 0;
		clonem.position = markerPosition;
		markersList.Add(clonem);
		
		clonem.SetDialog(this);
		
		clone.GetComponentInChildren<Text>().text = key.ToString() + " : " + " BackgroundChange - " + bgNum;
		GameObject smallMarker = Instantiate(smallMarkerOrigin);
		smallMarker.transform.SetParent(de.editorsTextInputField.transform);
		smallMarker.GetComponent<RectTransform>().anchoredPosition = new Vector3(markerPosition.x, markerPosition.y - 20);
		smallMarker.GetComponent<RectTransform>().localScale = Vector3.one;
		clonem.SetSmallMarker(smallMarker);
		smallMarker.GetComponent<Image>().color = new Color(1.0f, 1.0f, 1.0f, 0.5f); //bg => white

	}
	public void AddBackground(int key, int bgNum)
	{
		backgrounds[key] = bgNum;
	}
	public void AddSmallIllustration(int key, int illustNum, Vector2 markerPosition)
	{
		//markerPositionList.Add(new Vector3(markerPosition.x, markerPosition.y));
		smallIllustrations[key] = illustNum;


		GameObject clone = Instantiate(effectObjectOrigin);
		clone.transform.SetParent(effectListContent.transform);
		clone.GetComponent<RectTransform>().localScale = Vector3.one;
		Markers clonem = clone.GetComponent<Markers>();
		clonem.type = "SmallIllustration";
		clonem.key = key;
		clonem.valueIndex = 0;
		clonem.position = markerPosition;
		markersList.Add(clonem);
		clonem.SetDialog(this);

		clone.GetComponentInChildren<Text>().text = key.ToString() + " : " + " SmallIllustrationPopUp - " + illustNum;
		GameObject smallMarker = Instantiate(smallMarkerOrigin);
		smallMarker.transform.SetParent(de.editorsTextInputField.transform);
		smallMarker.GetComponent<RectTransform>().anchoredPosition = new Vector3(markerPosition.x, markerPosition.y - 20);
		smallMarker.GetComponent<RectTransform>().localScale = Vector3.one;
		clonem.SetSmallMarker(smallMarker);
		smallMarker.GetComponent<Image>().color = new Color(1.0f, 1.0f, 1.0f, 0.5f); //bg => white

	}
	public void AddSmallIllustration(int key, int illustNum)
	{
		smallIllustrations[key] = illustNum;
	}
	public void AddBigIllustration(int key, int illustNum, Vector2 markerPosition)
	{
		//markerPositionList.Add(new Vector3(markerPosition.x, markerPosition.y));
		bigIllustrations[key] = illustNum;


		GameObject clone = Instantiate(effectObjectOrigin);
		clone.transform.SetParent(effectListContent.transform);
		clone.GetComponent<RectTransform>().localScale = Vector3.one;
		Markers clonem = clone.GetComponent<Markers>();
		clonem.type = "BigIllustration";
		clonem.key = key;
		clonem.valueIndex = 0;
		clonem.position = markerPosition;
		markersList.Add(clonem);
		clonem.SetDialog(this);

		clone.GetComponentInChildren<Text>().text = key.ToString() + " : " + " BigIllustrationPopUp - " + illustNum;
		GameObject smallMarker = Instantiate(smallMarkerOrigin);
		smallMarker.transform.SetParent(de.editorsTextInputField.transform);
		smallMarker.GetComponent<RectTransform>().anchoredPosition = new Vector3(markerPosition.x, markerPosition.y - 20);
		smallMarker.GetComponent<RectTransform>().localScale = Vector3.one;
		clonem.SetSmallMarker(smallMarker);
		smallMarker.GetComponent<Image>().color = new Color(1.0f, 1.0f, 1.0f, 0.5f); //bg => white

	}
	public void AddBigIllustration(int key, int illustNum)
	{
		bigIllustrations[key] = illustNum;
	}
	public void AddEffect(int key, int effectNum)
	{
		if(effects.ContainsKey(key))
		{
			effects[key].Add(effectNum);
		}
		else
		{
			effects[key] = new List<int>();
			effects[key].Add(effectNum);
		}
	}
	public void AddEffect(int key, int effectNum, Vector2 markerPosition)
	{
		//markerPositionList.Add(new Vector3(markerPosition.x, markerPosition.y));
		if (effects.ContainsKey(key))
		{
			effects[key].Add(effectNum);
		}
		else
		{
			effects[key] = new List<int>();
			effects[key].Add(effectNum);
		}
		GameObject clone = Instantiate(effectObjectOrigin);
		clone.transform.SetParent(effectListContent.transform);
		clone.GetComponent<RectTransform>().localScale = Vector3.one;
		Markers clonem = clone.GetComponent<Markers>();
		clonem.type = "Effect";
		clonem.key = key;
		clonem.valueIndex = effects[key].Count - 1;
		clonem.position = markerPosition;
		markersList.Add(clonem);
		clonem.SetDialog(this);
		string effectName = string.Empty;
		switch(effectNum)
		{
			case 0:
				break;
			case 1:
				effectName = "화면흔들림";
				break;
			case 2:
				effectName = "페이드인";
				break;
			case 3:
				effectName = "페이드아웃";
				break;
			case 4:
				effectName = "대화창시작";
				break;
			case 5:
				effectName = "대화창종료";
				break;
			case 6:
				effectName = "화면흑백 켜기";
				break;
			case 7:
				effectName = "화면흑백 끄기";
				break;
			case 8:
				effectName = "일러스트 모두 흑백";
				break;
			case 9:
				effectName = "일러스트 모두 흑백 끄기";
				break;
			case 10:
				effectName = "일러스트 모두 활성";
				break;
			
			default:
				break;
		}
		clone.GetComponentInChildren<Text>().text = key.ToString() + " : " + " Effect - " + effectName;
		GameObject smallMarker = Instantiate(smallMarkerOrigin);
		smallMarker.transform.SetParent(de.editorsTextInputField.transform);
		smallMarker.GetComponent<RectTransform>().anchoredPosition = new Vector3(markerPosition.x, markerPosition.y-20);
		smallMarker.GetComponent<RectTransform>().localScale = Vector3.one;
		clonem.SetSmallMarker(smallMarker);
		smallMarker.GetComponent<Image>().color = new Color(1.0f, 0.5f, 0.5f, 0.5f); //effect => red
		
	}
	public void AddIllustrationL(int key, int illustrationNum)
	{
		if (illustrationsL.ContainsKey(key))
		{
			illustrationsL[key].Add(illustrationNum);
		}
		else
		{
			illustrationsL[key] = new List<int>();
			illustrationsL[key].Add(illustrationNum);
		}
	}
	public void AddIllustrationL(int key, int illustrationNum, Vector2 markerPosition)
	{
		//markerPositionList.Add(new Vector3(markerPosition.x, markerPosition.y));
		if (illustrationsL.ContainsKey(key))
		{
			illustrationsL[key].Add(illustrationNum);
		}
		else
		{
			illustrationsL[key] = new List<int>();
			illustrationsL[key].Add(illustrationNum);
		}
		GameObject clone = Instantiate(effectObjectOrigin);
		clone.transform.SetParent(effectListContent.transform);
		clone.GetComponent<RectTransform>().localScale = Vector3.one;
		Markers clonem = clone.GetComponent<Markers>();
		clonem.type = "IllustrationL";
		clonem.key = key;
		clonem.valueIndex = illustrationsL[key].Count - 1;
		clonem.position = markerPosition;
		markersList.Add(clonem);
		clonem.SetDialog(this);
		string effectName = string.Empty;
		int moded = illustrationNum / 10;
		switch (moded)
		{
			case 0:
				effectName = "None";
				break;
			case 1:
				effectName = "카트린";
				break;
			case 2:
				effectName = "막시밀리안";
				break;
			case 3:
				effectName = "아이리스";
				break;
			case 4:
				effectName = "에밀";
				break;
			case 5:
				effectName = "하나";
				break;
			case 6:
				effectName = "존";
				break;
			case 7:
				effectName = "장연화";
				break;
			case 8:
				effectName = "뮈라";
				break;
			case 9:
				effectName = "왈멍멍";
				break;
			case 10:
				effectName = "냥냐리우스";
				break;
			case 11:
				effectName = "주인공";
				break;
			default:
				effectName = "None";
				break;
		}
		clone.GetComponentInChildren<Text>().text = key.ToString() + " : " + " IllustrationL - " + effectName;
		GameObject smallMarker = Instantiate(smallMarkerOrigin);
		smallMarker.transform.SetParent(de.editorsTextInputField.transform);
		smallMarker.GetComponent<RectTransform>().anchoredPosition = new Vector3(markerPosition.x, markerPosition.y - 20);
		smallMarker.GetComponent<RectTransform>().localScale = Vector3.one;
		clonem.SetSmallMarker(smallMarker);
		smallMarker.GetComponent<Image>().color = new Color(0.5f, 1.0f, 0.5f, 0.5f); //Illustration - > green
	}
	public void AddIllustrationR(int key, int illustrationNum)
	{
		if (illustrationsR.ContainsKey(key))
		{
			illustrationsR[key].Add(illustrationNum);
		}
		else
		{
			illustrationsR[key] = new List<int>();
			illustrationsR[key].Add(illustrationNum);
		}
	}
	public void AddIllustrationR(int key, int illustrationNum, Vector2 markerPosition)
	{
		//markerPositionList.Add(new Vector3(markerPosition.x, markerPosition.y));
		if (illustrationsR.ContainsKey(key))
		{
			illustrationsR[key].Add(illustrationNum);
		}
		else
		{
			illustrationsR[key] = new List<int>();
			illustrationsR[key].Add(illustrationNum);
		}
		GameObject clone = Instantiate(effectObjectOrigin);
		clone.transform.SetParent(effectListContent.transform);
		clone.GetComponent<RectTransform>().localScale = Vector3.one;
		Markers clonem = clone.GetComponent<Markers>();
		clonem.type = "IllustrationR";
		clonem.key = key;
		clonem.valueIndex = illustrationsR[key].Count - 1;
		clonem.position = markerPosition;
		markersList.Add(clonem);
		clonem.SetDialog(this);
		string effectName = string.Empty;
		int moded = illustrationNum / 10;
		switch (moded)
		{
			case 0:
				effectName = "None";
				break;
			case 1:
				effectName = "카트린";
				break;
			case 2:
				effectName = "막시밀리안";
				break;
			case 3:
				effectName = "아이리스";
				break;
			case 4:
				effectName = "에밀";
				break;
			case 5:
				effectName = "하나";
				break;
			case 6:
				effectName = "존";
				break;
			case 7:
				effectName = "장연화";
				break;
			case 8:
				effectName = "뮈라";
				break;
			case 9:
				effectName = "왈멍멍";
				break;
			case 10:
				effectName = "냥냐리우스";
				break;
			case 11:
				effectName = "주인공";
				break;
			default:
				effectName = "None";
				break;
		}
		clone.GetComponentInChildren<Text>().text = key.ToString() + " : " + " IllustrationR - " + effectName;
		GameObject smallMarker = Instantiate(smallMarkerOrigin);
		smallMarker.transform.SetParent(de.editorsTextInputField.transform);
		smallMarker.GetComponent<RectTransform>().anchoredPosition = new Vector3(markerPosition.x, markerPosition.y - 20);
		smallMarker.GetComponent<RectTransform>().localScale = Vector3.one;
		clonem.SetSmallMarker(smallMarker);
		smallMarker.GetComponent<Image>().color = new Color(0.5f, 1.0f, 0.5f, 0.5f); //Illustration - > green
	}
	public void AddName(int key, string name)
	{
		if (names.ContainsKey(key))
		{
			names[key].Add(name);
		}
		else
		{
			names[key] = new List<string>();
			names[key].Add(name);
		}
	}
	public void AddName(int key, string name, Vector2 markerPosition)
	{
		//markerPositionList.Add(new Vector3(markerPosition.x, markerPosition.y));
		if (names.ContainsKey(key))
		{
			names[key].Add(name);
		}
		else
		{
			names[key] = new List<string>();
			names[key].Add(name);
		}

		GameObject clone = Instantiate(effectObjectOrigin);
		clone.transform.SetParent(effectListContent.transform);
		clone.GetComponent<RectTransform>().localScale = Vector3.one;
		Markers clonem = clone.GetComponent<Markers>();
		clonem.type = "Name";
		clonem.key = key;
		clonem.valueIndex = names[key].Count - 1;
		clonem.position = markerPosition;
		markersList.Add(clonem);
		clonem.SetDialog(this);
		string effectName = name;
		
		clone.GetComponentInChildren<Text>().text = key.ToString() + " : " + " Name - " + effectName;
		GameObject smallMarker = Instantiate(smallMarkerOrigin);
		smallMarker.transform.SetParent(de.editorsTextInputField.transform);
		smallMarker.GetComponent<RectTransform>().anchoredPosition = new Vector3(markerPosition.x, markerPosition.y - 20);
		smallMarker.GetComponent<RectTransform>().localScale = Vector3.one;
		clonem.SetSmallMarker(smallMarker);
		smallMarker.GetComponent<Image>().color = new Color(0.5f, 0.5f, 1.0f, 0.5f); //name - > blue
	}
	public void SetDialogType(int tp)
	{
		type = tp;
	}

	//marker는 이펙트 목록과 editorstextinputfield 에서 마커 2가지...
	public void HideMarker()
	{
		foreach(Markers m in markersList)
		{
			m.gameObject.SetActive(false);
		}
	}
	public void ShowMarker()
	{
		foreach (Markers m in markersList)
		{
			m.gameObject.SetActive(true);
		}
	}
	
	public void SetIsRecorded()
	{
		isRecorded = true;
	}
	public void OnDestroy()
	{
		foreach (Markers m in markersList)
		{
			Destroy(m.gameObject);
		}
	}
	/*
	public void MakeMarkersOnLoad()
	{
		int cCount = 0;
		foreach (char c in fullText)
		{
			if (effects.ContainsKey(cCount))
			{
				foreach(int a in effects[cCount])
				{
					GameObject clone = Instantiate(effectObjectOrigin);
					Markers clonem = clone.GetComponent<Markers>();
					markersList.Add(clonem);
					clonem.SetDialog(this);
					string effectName = name;

					clone.GetComponentInChildren<Text>().text = key.ToString() + " : " + " Name - " + effectName;
					GameObject smallMarker = Instantiate(smallMarkerOrigin);
					smallMarker.transform.SetParent(de.editorsTextInputField.transform);
					smallMarker.GetComponent<RectTransform>().anchoredPosition = new Vector3(markerPosition.x, markerPosition.y - 20);
					smallMarker.GetComponent<RectTransform>().localScale = Vector3.one;
					clonem.SetSmallMarker(smallMarker);
					smallMarker.GetComponent<Image>().color = new Color(0.5f, 0.5f, 1.0f, 0.5f); //name - > blue
				}
			}
			if (illustrationsL.ContainsKey(cCount))
			{

			}
			if (illustrationsR.ContainsKey(cCount))
			{

			}
			if (names.ContainsKey(cCount))
			{

			}
			cCount++;
		}
	}*/
}