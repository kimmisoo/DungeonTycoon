using SimpleJSON;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;
using System.Runtime.Serialization.Json;

/*
 * # 이펙트번호
 * 1 : 쉐이크
 * 2 : 화면페이드인
 * 3 : 화면페이드아웃
 * 4 : 대화시작 이펙트
 * 5 : 대화끝 이펙트
 * 6 : 그레이스케일
 * 7 : 그레이스케일해제
 * 
 * # 일러스트 번호
 * 1 : 카트린
 * 2 : 막시밀리안
 * 3 : 아이리스
 * 4 : 에밀
 * 5 : 하나
 * 6 : 존
 * 7 : 장연화
 * 8 : 뮈라
 * 9 : 왈멍멍
 * 10 : 냥나리우스
 */
[Serializable]
class DialogList
{
	List<Dialogs> dialog;
}
[Serializable]
class DialogSaveData
{
	public Dictionary<int, List<int>> effects;
	public Dictionary<int, List<int>> illustrationsL;
	public Dictionary<int, List<int>> illustrationsR;
	public Dictionary<int, List<string>> names;
	public Dictionary<int, int> backgrounds;
	public Dictionary<int, int> smallIllustrations;
	public Dictionary<int, int> bigIllustrations;
	public List<Vector3> markerPositionList;
	public string fullText;
	public int textType;
}
public class DialogEditor : MonoBehaviour
{
	public GameObject dialogUI; //Hide , Show 에 사용
	public Vector3 dialogUIOriginPosition; //Hide, Show 에 사용
	public List<Sprite> preLoadedIllustrations; // Illustration sprite 들
	public List<Sprite> preLoadedSmallIllustrations;
	public List<Sprite> preLoadedBigIllustrations;
	public List<Sprite> preLoadedBackgrounds;
	public GameObject forCloneTextObject; // 다이얼로그 튜플 원본, Instantiate에 사용됨
	public List<Dialogs> dialogList;

	public RectTransform savedSentenceContent; //다이얼로그 리스트 위치..

	public int illustrationPosition = 0; // 일러스트 추가시 or 
										 // 버튼 이미지 바꾸는 방식으로 변경함. 18.06.26 김미수
	public Button illustrationPositionBtn;
	public Sprite leftBtnSprite;
	public Sprite rightBtnSprite;
	//
	public float tempTimeBetween = 0.08f; // 대화시작 이펙트 연출 시 이동 시간 간격
	public RectTransform dialogContentTransform; // Shake 이펙트 시 대화창 위치
	public Image fadePanel; // fade 효과시 패널
	public Grayscale grayscale; // 화면 전체 흑백효과용
	public Material illustrationGrayScale;
	public GameObject dialogPanel;
	public Image illustrationL;
	public Image illustrationR;
	public Text editorsTextBox;
	public CustomInputField editorsTextInputField;
	public InputField loadFileName; // 로드할 파일 이름 - 파일목록 List Panel 만들기
	public InputField smallIllustrationNumber;
	public InputField bigIllustrationNumber;
	public InputField backgroundNumber;
	public Text showingTextBox; // 실제 출력되는 텍스트
	public Text showingNameBox; // 실제 출력되는 이름
								// 명찰 색 바꾸기용 18.06.26 김미수
	public String[] nameArray;
	public Image showingNameBG;
	//
	public GameObject smallIllustrationObject;
	public Image smallIllustrationImage;
	public GameObject bigIllustrationObject;
	public Image bigIllustrationImage;

	public List<GameObject> clonedObjects; // 표시되는 대화내용<GameObject> - 
	public int dialogSelectIndex= 0; // 선택한 대화내용 인덱스
	public bool dialogUIHide = false;
	// 대사 전환 버튼용 18.06.26 김미수
	public String[] changeBtnStringArray;
	public Button changeBtn;
	public Button addBtn;
	public Button deleteBtn;
	public Button replaceBtn;
	public Button playBtn;
	public Button saveBtn;
	//
	public int caretPositionRecord = 0; // 기록된 커서 포지션
	public StringBuilder forShowTextBox;

	

	public InputField customNameInput; // 엑스트라 등 대화 추가할때 이름입력

	Vector3 origin; //대화창 anchroed position
	Vector3 originLocal; // 대화창 local position

	Color narrationTextColor;
	Color monologueTextColor;
	Color dialogTextColor;

	public Text dialogType;
	int dialogTypeNum = 2; // 0 = ㄷㄱ백 / 1 = 나레이션 / 2 = 대사

	public Dialogs currentDialogs;
	public GameObject currentDialogsObject;
	public Dialogs selectedDialogs;
	public GameObject selectedDialogsObject;
	public Vector2 caretPositionVector=new Vector3(-465.0f, 95.0f);
	public GameObject effectOrigin;
	public Transform effectListTransform;
	public Markers selectedMarkers = null;

	public bool isReplacing = false;

	public Coroutine waitForTouch;
	public Coroutine doPlay;
	public float typeWriteBetween = 0.055f;
	RaycastHit2D hit;
	Touch[] touches;
	Vector3 touchPos;
	public bool isTouchDown = false;
	public bool isPlaying = false;
	public bool isSkipDown = false;
	public bool isEffectBlocked = false;
	public GameObject nextButton;
	public Vector3 nextButtonPosition;
	public int emotionOffset = 0;
	public Image[] emotionButtons;
	public Image backGroundImage;
	

	// Use this for initialization 
	void Start()
	{
		
		monologueTextColor = new Color(0.647f, 0.647f, 1.0f);
		dialogTextColor = Color.white;
		narrationTextColor = new Color(1.0f, 1.0f, 0.647f);
		forShowTextBox = new StringBuilder();
		RectTransform tr = dialogPanel.GetComponent<RectTransform>();
		origin = tr.position;
		originLocal = tr.localPosition;
		nextButtonPosition = nextButton.transform.localPosition;
		grayscale = Camera.main.gameObject.GetComponent<Grayscale>();
		//savedSentence = new List<string>();
		clonedObjects = new List<GameObject>();

		dialogList = new List<Dialogs>();
		dialogUIOriginPosition = dialogUI.GetComponent<RectTransform>().anchoredPosition3D;
		preLoadedIllustrations = new List<Sprite>();
		preLoadedSmallIllustrations = new List<Sprite>();
		preLoadedBigIllustrations = new List<Sprite>();
		Sprite[] a = Resources.LoadAll<Sprite>("Illustration/"); //Load All 후 Sort 필요..
		Sprite[] b = Resources.LoadAll<Sprite>("PopupIllustration/Small");
		Sprite[] c = Resources.LoadAll<Sprite>("PopupIllustration/Big");
		Sprite[] d = Resources.LoadAll<Sprite>("Backgrounds/");
		for (int i = 0; i < a.Length; i++)
		{
			preLoadedIllustrations.Add(Resources.Load<Sprite>("Illustration/" + i));
		}
		for(int i=0; i<b.Length; i++)
		{
			preLoadedSmallIllustrations.Add(Resources.Load<Sprite>("PopupIllustration/Small/" + i));
		}
		for (int i = 0; i < c.Length; i++)
		{
			preLoadedBigIllustrations.Add(Resources.Load<Sprite>("PopupIllustration/Big/" + i));
		}
		for (int i = 0; i < d.Length; i++)
		{
			preLoadedBackgrounds.Add(Resources.Load<Sprite>("Backgrounds/" + i));
		}
		//Debug.Log(preLoadedIllustrations.Count + "aaaaa");
		// 버튼 스프라이트 불러오기 18.06.26 김미수
		rightBtnSprite = Resources.Load<Sprite>("ScriptEditor#/Right");
		leftBtnSprite = Resources.Load<Sprite>("ScriptEditor#/Left");
		//

		//clonedObjects.Add(Instantiate(forCloneTextObject));
		//selectedDialogs = clonedObjects[0].GetComponent<Dialogs>();
		CreateCurrentDialogs();

	}

	// Update is called once per frame
	void Update()
	{
		/*if (illustrationPosition == 0)
        {
            illustrationR.color = Color.gray;
            illustrationL.color = Color.white;
        }
        else
        {
            illustrationL.color = Color.gray;
            illustrationR.color = Color.white;
        }
       */
		if (editorsTextInputField.caretPosition >= 0 && editorsTextInputField.text.Length > 0 && editorsTextInputField.isFocused)
		{
			caretPositionRecord = editorsTextInputField.caretPosition;
			caretPositionVector = editorsTextInputField.GetLocalCaretPosition();
		}
		if(editorsTextInputField.isFocused == false && caretPositionRecord == 0)
		{
			caretPositionVector = new Vector3(-465.0f, 95.0f);
		}

		if(selectedDialogs != null)
		{
			replaceBtn.interactable = true;
		}
		else
		{
			replaceBtn.interactable = false;
		}
		/*
		if (Input.touchCount > 0)
		{
			touches = Input.touches;
			touchPos = Camera.main.ScreenToWorldPoint(touches[0].position);
			hit = Physics2D.Raycast(touchPos, Vector2.zero, 0.0f);
			if (touches[0].phase == TouchPhase.Moved || touches[0].phase == TouchPhase.Stationary)
			{
				if (hit.collider.gameObject.tag.Equals("guard"))
				{
					typeWriteBetween = 0.015f;
				}
			}
		}*/
		if(Input.GetMouseButtonDown(0) && isPlaying)
		{
			Vector3 clickPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			hit = Physics2D.Raycast(clickPos, Vector2.zero);
			if (hit.collider != null &&hit.collider.gameObject.tag.Equals("guard"))
			{
				isTouchDown = true;
				Debug.Log("hit");
			}
			if (hit.collider != null && hit.collider.gameObject.tag.Equals("skipButton"))
			{
				typeWriteBetween = 0.0f;
				isTouchDown = true;
				isSkipDown = true;
				
			}
		}
		
		if(Input.GetMouseButtonUp(0) && isPlaying)
		{
			typeWriteBetween = 0.055f;
			isSkipDown = false;
			
		}
	}

	public void MoveDialogList()
	{
		if (dialogUIHide == false)
		{
			dialogUIHide = true;
			dialogUI.GetComponent<RectTransform>().anchoredPosition3D = new Vector3(5000.0f, 5000.0f, 5000.0f);
		}
		else
		{
			dialogUIHide = false;
			dialogUI.GetComponent<RectTransform>().anchoredPosition3D = dialogUIOriginPosition;
		}
	}
	public void AddIllustrationChange(int illustrationNum)
	{
		
		if (illustrationPosition == 0)
			AddIllustrationChangeL(illustrationNum + emotionOffset);
		else
			AddIllustrationChangeR(illustrationNum + emotionOffset);
	}
	public void AddIllustrationChangeL(int illustrationNum)
	{
		//editorsTextInputField.text = editorsTextInputField.text.Insert(caretPositionRecord, "@@illustrationchangeL#" + illustrationNum + "#");
		//caretPositionRecord = caretPositionRecord + ("@@illustrationchangeL#" + illustrationNum + "#").Length > editorsTextInputField.text.Length ? editorsTextInputField.text.Length : caretPositionRecord + ("@@illustrationchangeL#" + illustrationNum + "#").Length;
		//Debug.Log("illL" + caretPositionRecord);
		//editorsTextInputField.text = editorsTextInputField.text + "@@illustrationchangeL#" + illustrationNum + "#";
		if(isReplacing == true && selectedDialogs != null)
			selectedDialogs.AddIllustrationL(caretPositionRecord, illustrationNum, caretPositionVector);
		else
			currentDialogs.AddIllustrationL(caretPositionRecord, illustrationNum, caretPositionVector);
	}
	public void AddIllustrationChangeR(int illustrationNum)
	{
		//editorsTextInputField.text = editorsTextInputField.text.Insert(caretPositionRecord, "@@illustrationchangeR#" + illustrationNum + "#");
		//caretPositionRecord = caretPositionRecord + ("@@illustrationchangeR#" + illustrationNum + "#").Length > editorsTextInputField.text.Length ? editorsTextInputField.text.Length : caretPositionRecord + ("@@illustrationchangeR#" + illustrationNum + "#").Length;
		//Debug.Log("illR" + caretPositionRecord);
		//editorsTextInputField.text = editorsTextInputField.text + "@@illustrationchangeR#" + illustrationNum + "#";
		if (isReplacing == true && selectedDialogs != null)
			selectedDialogs.AddIllustrationR(caretPositionRecord, illustrationNum, caretPositionVector);
		else
			currentDialogs.AddIllustrationR(caretPositionRecord, illustrationNum, caretPositionVector);
		
	}
	public void AddEffect(int effectNum)
	{

		//editorsTextInputField.text = editorsTextInputField.text.Insert(caretPositionRecord, "@@effect#" + effectNum + "#");
		//caretPositionRecord = caretPositionRecord + ("@@effect#" + effectNum + "#").Length > editorsTextInputField.text.Length ? editorsTextInputField.text.Length : caretPositionRecord + ("@@effect#" + effectNum + "#").Length;
		if (isReplacing == true && selectedDialogs != null)
			selectedDialogs.AddEffect(caretPositionRecord, effectNum, caretPositionVector);
		else
			currentDialogs.AddEffect(caretPositionRecord, effectNum, caretPositionVector);
	}


	public void AddName(string name)
	{
		/*Debug.Log(caretPositionRecord + name);
		editorsTextInputField.text = editorsTextInputField.text.Insert(caretPositionRecord, "@@name#" + name + "#");
		caretPositionRecord = caretPositionRecord + ("@@name#" + name + "#").Length > editorsTextInputField.text.Length ? editorsTextInputField.text.Length : caretPositionRecord + ("@@name#" + name + "#").Length;
		Debug.Log("name" + caretPositionRecord);
		DoChangeName(name);*/
		if (isReplacing == true && selectedDialogs != null)
			selectedDialogs.AddName(caretPositionRecord, name, caretPositionVector);
		else
			currentDialogs.AddName(caretPositionRecord, name, caretPositionVector);
		
	}
	public void AddCustomName()
	{
		if (isReplacing == true && selectedDialogs != null)
			selectedDialogs.AddName(caretPositionRecord, customNameInput.text, caretPositionVector);
		else
			currentDialogs.AddName(caretPositionRecord, customNameInput.text, caretPositionVector);
		customNameInput.text = "";
	}
	public void AddSmallIllustration()
	{
		if (isReplacing == true && selectedDialogs != null)
			selectedDialogs.AddSmallIllustration(caretPositionRecord, int.Parse(smallIllustrationNumber.text), caretPositionVector);
		else
			currentDialogs.AddSmallIllustration(caretPositionRecord, int.Parse(smallIllustrationNumber.text), caretPositionVector);
	}
	public void AddBigIllustration()
	{
		if (isReplacing == true && selectedDialogs != null)
			selectedDialogs.AddBigIllustration(caretPositionRecord, int.Parse(bigIllustrationNumber.text), caretPositionVector);
		else
			currentDialogs.AddBigIllustration(caretPositionRecord, int.Parse(bigIllustrationNumber.text), caretPositionVector);
	}
	public void AddBackground() //0 번은 투명으로 준비할것
	{
		if (isReplacing == true && selectedDialogs != null)
			selectedDialogs.AddBackground(caretPositionRecord, int.Parse(backgroundNumber.text), caretPositionVector);
		else
			currentDialogs.AddBackground(caretPositionRecord, int.Parse(backgroundNumber.text), caretPositionVector);
	}
	public void DoRecord()
	{
		if (editorsTextBox.text.Length > 0)
		{
			currentDialogs.SetFullText(editorsTextBox.text);
			currentDialogs.SetIsRecorded();
			dialogList.Insert(dialogSelectIndex <= 0 ? 0 : dialogSelectIndex, currentDialogs);
			clonedObjects.Insert(dialogSelectIndex <= 0 ? 0 : dialogSelectIndex, currentDialogsObject);
			currentDialogs.SetIndex(dialogSelectIndex <= 0 ? 0 : dialogSelectIndex);
			currentDialogsObject.GetComponent<RectTransform>().SetParent(savedSentenceContent.GetComponent<RectTransform>());
			currentDialogsObject.GetComponent<RectTransform>().localScale = Vector3.one;
			//newClone.GetComponent<RectTransform>().SetParent(savedSentenceContent.GetComponent<RectTransform>());
			//newClone.GetComponent<RectTransform>().localScale = Vector3.one;
			currentDialogs.HideMarker();

			
			CreateCurrentDialogs();


			editorsTextInputField.text = "";
			caretPositionRecord = 0;
			caretPositionVector = new Vector3(-465.0f, 95.0f);
			DoRemap();
			SetDialogSelectIndex(clonedObjects.Count);
			//clonedObjects[clonedObjects.Count - 1].GetComponent<Text>().text = selectedDialogs.
		}
		
	}
	public void DoReplace()
	{
		if (selectedDialogs != null)
		{
			if (isReplacing == true)
			{
				selectedDialogs.SetFullText(editorsTextInputField.text);
				editorsTextInputField.text = "";
				caretPositionRecord = 0;
				caretPositionVector = new Vector3(-465.0f, 95.0f);
				//markers 내용 복사하기!
				DoRemap();
				isReplacing = false;
				//버튼활성화
				addBtn.interactable = true;

				playBtn.interactable = true;
				replaceBtn.interactable = false;
				saveBtn.interactable = true;
				//current dialog 새로만들기
				CreateCurrentDialogs();
				DoIllustrationChangeL(0);
				DoIllustrationChangeR(0);
				foreach (Dialogs d in dialogList)
				{
					d.GetComponent<Selectable>().interactable = true;
				}
			}
			else
			{
				isReplacing = true;
				//버튼비활성화.
				addBtn.interactable = false;

				playBtn.interactable = false;
				replaceBtn.interactable = true;
				saveBtn.interactable = false;

				DoIllustrationChangeL(selectedDialogs.representImageLNum);
				DoIllustrationChangeR(selectedDialogs.representImageRNum);

				editorsTextInputField.text = selectedDialogs.fullText;
				selectedDialogs.ShowMarker();
				foreach(Dialogs d in dialogList)
				{
					d.GetComponent<Selectable>().interactable = false;
				}

			}
		}
	}
	public void DoDelete()
	{
		
		if(selectedMarkers != null && selectedDialogs == null) //== current effect del
		{
			
			Markers tp = selectedMarkers;
			if(tp.type.Equals("Effect"))
			{
				foreach (Markers k in selectedMarkers.dialog.markersList)
				{
					if (k.type.Equals("Effect") && k.key == selectedMarkers.key && k.valueIndex > selectedMarkers.valueIndex)
					{
						k.valueIndex--;
					}
				}
				selectedMarkers.dialog.effects[tp.key].RemoveAt(tp.valueIndex);
				
				
			}
			else if(tp.type.Equals("IllustrationL"))
			{
				foreach (Markers k in selectedMarkers.dialog.markersList)
				{
					if (k.type.Equals("IllustrationL") && k.key == selectedMarkers.key && k.valueIndex > selectedMarkers.valueIndex)
					{
						k.valueIndex--;
					}
				}
				selectedMarkers.dialog.illustrationsL[tp.key].RemoveAt(tp.valueIndex);
				
			}
			else if(tp.type.Equals("IllustrationR"))
			{
				foreach (Markers k in selectedMarkers.dialog.markersList)
				{
					if (k.type.Equals("IllustrationR") && k.key == selectedMarkers.key && k.valueIndex > selectedMarkers.valueIndex)
					{
						k.valueIndex--;
					}
				}
				selectedMarkers.dialog.illustrationsR[tp.key].RemoveAt(tp.valueIndex);
				
			}
			else if(tp.type.Equals("Name"))
			{
				foreach (Markers k in selectedMarkers.dialog.markersList)
				{
					if (k.type.Equals("Name") && k.key == selectedMarkers.key && k.valueIndex > selectedMarkers.valueIndex)
					{
						k.valueIndex--;
					}
				}
				selectedMarkers.dialog.names[tp.key].RemoveAt(tp.valueIndex);
			}
			else if(tp.type.Equals("Background"))
			{
				
				selectedMarkers.dialog.backgrounds.Remove(tp.key);
			}
			else if (tp.type.Equals("SmallIllustration"))
			{
				selectedMarkers.dialog.smallIllustrations.Remove(tp.key);
			}
			else if (tp.type.Equals("BigIllustration"))
			{
				selectedMarkers.dialog.bigIllustrations.Remove(tp.key);
			}
			currentDialogs.markersList.Remove(tp);
			Destroy(tp.gameObject);
			selectedMarkers = null;
		}
		else if(isReplacing == true && selectedMarkers != null) // == selected effect del
		{

			Markers tp = selectedMarkers;
			if (tp.type.Equals("Effect"))
			{
				foreach (Markers k in selectedMarkers.dialog.markersList)
				{
					if (k.type.Equals("Effect") && k.key == selectedMarkers.key && k.valueIndex > selectedMarkers.valueIndex)
					{
						k.valueIndex--;
					}
				}
				selectedMarkers.dialog.effects[tp.key].RemoveAt(tp.valueIndex);
			}
			else if (tp.type.Equals("IllustrationL"))
			{
				foreach (Markers k in selectedMarkers.dialog.markersList)
				{
					if (k.type.Equals("IllustrationL") && k.key == selectedMarkers.key && k.valueIndex > selectedMarkers.valueIndex)
					{
						k.valueIndex--;
					}
				}
				selectedMarkers.dialog.illustrationsL[tp.key].RemoveAt(tp.valueIndex);
			}
			else if (tp.type.Equals("IllustrationR"))
			{
				foreach (Markers k in selectedMarkers.dialog.markersList)
				{
					if (k.type.Equals("IllustrationR") && k.key == selectedMarkers.key && k.valueIndex > selectedMarkers.valueIndex)
					{
						k.valueIndex--;
					}
				}
				selectedMarkers.dialog.illustrationsR[tp.key].RemoveAt(tp.valueIndex);
			}
			else if (tp.type.Equals("Name"))
			{
				foreach (Markers k in selectedMarkers.dialog.markersList)
				{
					if (k.type.Equals("Name") && k.key == selectedMarkers.key && k.valueIndex > selectedMarkers.valueIndex)
					{
						k.valueIndex--;
					}
				}
				selectedMarkers.dialog.names[tp.key].RemoveAt(tp.valueIndex);
			}
			selectedDialogs.markersList.Remove(tp);
			Destroy(tp.gameObject);
			selectedMarkers = null;

			//Dialog.names, effect 에서 제거 해야함.
			
		}
		else if(isReplacing != true && selectedMarkers == null && selectedDialogs != null) // selected dialog del
		{
			if (dialogSelectIndex < clonedObjects.Count && selectedDialogs != null)
			{
				GameObject del = selectedDialogs.gameObject;
				Dialogs deldialog = selectedDialogs;
				clonedObjects.Remove(del);
				dialogList.Remove(deldialog);
				Destroy(deldialog);
				Destroy(del);

				dialogSelectIndex = clonedObjects.Count - 1;
				selectedDialogs = null;
				selectedDialogsObject = null;
				DoRemap();
				CreateCurrentDialogs();
			}
		}
	}
	public void SetDialogSelectIndex(int index)
	{
		dialogSelectIndex = index;
	}
	
	public void DoRemap()
	{
		int i = 0;
		foreach (GameObject go in clonedObjects)
		{
			go.GetComponent<Dialogs>().SetIndex(i);
			i++;
		}
		foreach (GameObject go in clonedObjects)
		{
			go.transform.SetSiblingIndex(go.GetComponent<Dialogs>().GetIndex());
		}
		dialogList.Sort(delegate (Dialogs A, Dialogs B)
		{
			if (A.GetIndex() > B.GetIndex())
				return 1;
			else if (A.GetIndex() < B.GetIndex())
				return -1;
			else
				return 0;
		});
		
	}

	public void DoPlay_Selected()
	{
		
	}
	public void DoPlay()
	{
		if (selectedDialogs == null)
			return;

		showingNameBox.text = "";
		showingTextBox.text = "";
		DoIllustrationChangeL(0);
		DoIllustrationChangeR(0);
		isPlaying = true;
		
		if (doPlay != null)
			StopCoroutine(doPlay);
		selectedDialogs.GetComponent<Image>().color = new Color32(0xFF, 0xFF, 0xFF, 0x84);
		doPlay = StartCoroutine(_DoPlay());
		
	}
	/*
	IEnumerator _DoPlay()
	{
		List<string> playText = new List<string>();
		playText.Add(string.Empty); //출력될
		playText.Add("<color=#00000000>"); // 태그
		playText.Add(string.Empty);//원본텍스트
		playText.Add("</color>");//태그

		int dialogsIndex = selectedDialogs.GetIndex();
		for(int i = dialogsIndex; i<dialogList.Count; i++) // dialog
		{
			selectedDialogs = dialogList[i];
			playText[0] = string.Empty;
			playText[2] = selectedDialogs.fullText;
			
			int progress = 0;
			DoChangeTextColor(dialogList[i].type);
			if(dialogList[i].type != 2)
			{
				//showingNameBG.color = new Color32(0x74, 0x5D, 0x5D, 0xB4);
				showingNameBG.gameObject.SetActive(false);
				showingNameBox.enabled = false;

			}
			else
			{
				showingNameBG.gameObject.SetActive(true);
				showingNameBox.enabled = true;
			}
			foreach (char c in selectedDialogs.fullText)
			{
				int smallIndex = 0;
				int bigIndex = 0;
				
				if(dialogList[i].names.ContainsKey(progress))
				{
					foreach (string s in dialogList[i].names[progress])
					{
						DoChangeName(s);
						//showingNameBox.text = s;
					}
				}
				if(dialogList[i].illustrationsL.ContainsKey(progress))
				{
					foreach (int il in dialogList[i].illustrationsL[progress])
					{
						illustrationL.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
						illustrationR.color = new Color(0.6f, 0.6f, 0.6f, 1.0f);
						DoIllustrationChangeL(il);
					}
				}
				if(dialogList[i].illustrationsR.ContainsKey(progress))
				{
					foreach (int il in dialogList[i].illustrationsR[progress])
					{
						illustrationR.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
						illustrationL.color = new Color(0.6f, 0.6f, 0.6f, 1.0f);
						DoIllustrationChangeR(il);
					}
				}
				if(dialogList[i].effects.ContainsKey(progress))
				{
					foreach(int ef in dialogList[i].effects[progress])
					{
						yield return StartCoroutine(DoEffect(ef));
						
					}
				}
				if(dialogList[i].backgrounds.ContainsKey(progress))
				{
					//change background
					StartCoroutine(ShowBackground(dialogList[i].backgrounds[progress]));
				}
				if(dialogList[i].smallIllustrations.ContainsKey(progress))
				{
					//popup smallillustrations
					StartCoroutine(ShowSmallIllustration(dialogList[i].smallIllustrations[progress]));

				}
				if(dialogList[i].bigIllustrations.ContainsKey(progress))
				{
					StartCoroutine(ShowBigIllustration(dialogList[i].bigIllustrations[progress]));
					//popup BigIllustrations
				}


				if (isTouchDown == true)
				{
					//대화 전체 출력하면서 해야할것들(그레이스케일 이펙트, 일러스트 변경, 
					
					foreach(char ch in playText[2])
					{
						if (dialogList[i].names.ContainsKey(progress))
						{
							foreach (string s in dialogList[i].names[progress])
							{
								showingNameBox.text = s;
							}
						}
						if (dialogList[i].illustrationsL.ContainsKey(progress))
						{
							foreach (int il in dialogList[i].illustrationsL[progress])
							{
								DoIllustrationChangeL(il);
							}
						}
						if (dialogList[i].illustrationsR.ContainsKey(progress))
						{
							foreach (int il in dialogList[i].illustrationsL[progress])
							{
								DoIllustrationChangeR(il);
							}
						}
						if (dialogList[i].effects.ContainsKey(progress))
						{
							foreach (int ef in dialogList[i].effects[progress])
							{
								//block or nonblock
								yield return StartCoroutine(DoEffect(ef));
							}
						}
						if (dialogList[i].backgrounds.ContainsKey(progress))
						{
							//change background
							StartCoroutine(ShowBackground(dialogList[i].backgrounds[progress]));

						}
						if (dialogList[i].smallIllustrations.ContainsKey(progress))
						{
							//popup smallillustrations
							StartCoroutine(ShowSmallIllustration(dialogList[i].smallIllustrations[progress]));
						}
						if (dialogList[i].bigIllustrations.ContainsKey(progress))
						{
							//popup BigIllustrations
							StartCoroutine(ShowBigIllustration(dialogList[i].bigIllustrations[progress]));
						}
						progress++;
					}
					showingTextBox.text = selectedDialogs.fullText;
					isTouchDown = false;
					break;
				}
				playText[0] += c;
				playText[2] = playText[2].Remove(0, 1);
				showingTextBox.text = playText[0] + playText[1] + playText[2] + playText[3];
				progress++;
				yield return new WaitForSeconds(typeWriteBetween);
				

			}//--한칸
			isTouchDown = false;
			if(i < dialogList.Count-1 && !isSkipDown)
				yield return waitForTouch = StartCoroutine(_Wait());
			if (isSkipDown)
				yield return new WaitForSeconds(0.18f);
			
			bigIllustrationObject.SetActive(false);
			//smallIllustrationObject.SetActive(false);
		}
		isPlaying = false;
		
		yield return null;
		
	}*/
	IEnumerator _DoPlay()
	{
		List<string> playText = new List<string>();
		playText.Add(string.Empty); //출력될
		playText.Add("<color=#00000000>"); // 태그
		playText.Add(string.Empty);//원본텍스트
		playText.Add("</color>");//태그

		int dialogsIndex = selectedDialogs.GetIndex();
		for (int i = dialogsIndex; i < dialogList.Count; i++) // dialog
		{
			selectedDialogs = dialogList[i];
			playText[0] = string.Empty;
			playText[2] = selectedDialogs.fullText;

			int progress = 0;
			DoChangeTextColor(dialogList[i].type);
			if (dialogList[i].type != 2)
			{
				//showingNameBG.color = new Color32(0x74, 0x5D, 0x5D, 0xB4);
				showingNameBG.gameObject.SetActive(false);
				showingNameBox.enabled = false;

			}
			else
			{
				showingNameBG.gameObject.SetActive(true);
				showingNameBox.enabled = true;
			}
			foreach (char c in selectedDialogs.fullText)
			{
				int smallIndex = 0;
				int bigIndex = 0;
				
				if (dialogList[i].names.ContainsKey(progress))
				{
					foreach (string s in dialogList[i].names[progress])
					{
						DoChangeName(s);
						//showingNameBox.text = s;
					}
				}
				if (dialogList[i].illustrationsL.ContainsKey(progress))
				{
					foreach (int il in dialogList[i].illustrationsL[progress])
					{
						illustrationL.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
						illustrationR.color = new Color(0.6f, 0.6f, 0.6f, 1.0f);
						DoIllustrationChangeL(il);
					}
				}
				if (dialogList[i].illustrationsR.ContainsKey(progress))
				{
					foreach (int il in dialogList[i].illustrationsR[progress])
					{
						illustrationR.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
						illustrationL.color = new Color(0.6f, 0.6f, 0.6f, 1.0f);
						DoIllustrationChangeR(il);
					}
				}
				if (dialogList[i].effects.ContainsKey(progress))
				{
					foreach (int ef in dialogList[i].effects[progress])
					{
						yield return StartCoroutine(DoEffect(ef));

					}
				}
				if (dialogList[i].backgrounds.ContainsKey(progress))
				{
					//change background
					StartCoroutine(ShowBackground(dialogList[i].backgrounds[progress]));
				}
				if (dialogList[i].smallIllustrations.ContainsKey(progress))
				{
					//popup smallillustrations
					StartCoroutine(ShowSmallIllustration(dialogList[i].smallIllustrations[progress]));

				}
				if (dialogList[i].bigIllustrations.ContainsKey(progress))
				{
					StartCoroutine(ShowBigIllustration(dialogList[i].bigIllustrations[progress]));
					
					//popup BigIllustrations
				}


				if (isTouchDown == true)
				{
					//대화 전체 출력하면서 해야할것들(그레이스케일 이펙트, 일러스트 변경, 

					foreach (char ch in playText[2])
					{
						progress++;
						if (dialogList[i].names.ContainsKey(progress))
						{
							foreach (string s in dialogList[i].names[progress])
							{
								showingNameBox.text = s;
							}
						}
						if (dialogList[i].illustrationsL.ContainsKey(progress))
						{
							foreach (int il in dialogList[i].illustrationsL[progress])
							{
								DoIllustrationChangeL(il);
							}
						}
						if (dialogList[i].illustrationsR.ContainsKey(progress))
						{
							foreach (int il in dialogList[i].illustrationsL[progress])
							{
								DoIllustrationChangeR(il);
							}
						}
						if (dialogList[i].effects.ContainsKey(progress))
						{
							foreach (int ef in dialogList[i].effects[progress])
							{
								
								yield return StartCoroutine(DoEffect(ef));
							}
						}
						if (dialogList[i].backgrounds.ContainsKey(progress))
						{
							//change background
							StartCoroutine(ShowBackground(dialogList[i].backgrounds[progress]));

						}
						if (dialogList[i].smallIllustrations.ContainsKey(progress))
						{
							//popup smallillustrations
							StartCoroutine(ShowSmallIllustration(dialogList[i].smallIllustrations[progress]));
						}
						if (dialogList[i].bigIllustrations.ContainsKey(progress))
						{
							//popup BigIllustrations
							StartCoroutine(ShowBigIllustration(dialogList[i].bigIllustrations[progress]));
						}
						
					}
					showingTextBox.text = selectedDialogs.fullText;
					isTouchDown = false;
					break;
				}
				playText[0] += c;
				playText[2] = playText[2].Remove(0, 1);
				showingTextBox.text = playText[0] + playText[1] + playText[2] + playText[3];
				progress++;
				yield return new WaitForSeconds(typeWriteBetween);


			}//--한칸
			isTouchDown = false;
			if (i <= dialogList.Count - 1 && !isSkipDown)
				yield return waitForTouch = StartCoroutine(_Wait());
			if (isSkipDown)
				yield return new WaitForSeconds(0.18f);
			//bigIllustrationObject.SetActive(false);


			//smallIllustrationObject.SetActive(false);
		}
		//yield return waitForTouch = StartCoroutine(_Wait());
		isPlaying = false;
		
		

	}
	IEnumerator _DoPlay_Selected()
	{
		yield return null;
	}
	IEnumerator _Wait()
	{
		StartCoroutine(GiveMovementNextButton());
		while(isPlaying == true)
		{
			if (isTouchDown == true)
			{
				isTouchDown = false;
				
				break;
			}
			yield return null;
		}
		
		Debug.Log("WaitOut");
	}
	IEnumerator GiveMovementNextButton()
	{
		nextButton.transform.localPosition = nextButtonPosition;
		int coeff = 1;
		while(isPlaying == true && isTouchDown == false)
		{
			if (nextButton.transform.localPosition.x > nextButtonPosition.x + 13.0f)
			{
				coeff = -1;
			}
			else if(nextButton.transform.localPosition.x <= nextButtonPosition.x)
			{
				coeff = 1;
			}
			nextButton.transform.localPosition += new Vector3(55.0f, 0.0f, 0.0f) * Time.deltaTime * coeff;
			//nextButton.transform.Translate(1.0f, 0.0f, 0.0f);
			yield return null;
		}
		nextButton.transform.localPosition = nextButtonPosition;
	}
	public void SetCaretPosition()
	{
		if (editorsTextInputField.text.Length == 0)
		{
			caretPositionRecord = 0;
			caretPositionVector = editorsTextInputField.GetLocalCaretPosition();
		}
	}
	public void DoPlayAll()
	{
		StartCoroutine(_DoPlayAll());
	}
	public IEnumerator _DoPlayAll()
	{
		yield return null;
		/*
		  yield return null;
		  foreach (GameObject go in clonedObjects)
		  {
			  editorsTextInputField.text = go.GetComponent<Text>().text;
			  DoEnter();
			  showingTextBox.text = "";
			  yield return StartCoroutine(_DoPlay());
			  yield return new WaitForSeconds(1.0f);
		  }
		*/
	}
	public void StopAll()
	{
		//StopAllCoroutines();
		//코루틴 변수 배정... 개별 정지 하기
	}
	
	public void IllustrationPositionChange()
	{
		if (illustrationPosition == 0)
		{
			illustrationPosition = 1;
			illustrationPositionBtn.image.sprite = rightBtnSprite; // 버튼 스프라이트 바꾸기로 변경함 18.06.26 김미수
		}
		else
		{
			illustrationPosition = 0;
			illustrationPositionBtn.image.sprite = leftBtnSprite; // 버튼 스프라이트 바꾸기로 변경함 18.06.26 김미수
		}
	}
	public void DoChangeCustomName()
	{
		DoChangeName(customNameInput.text);
	}
	public void DoChangeName(string name)
	{
		showingNameBox.text = name;
		/* 명찰 색상 변경 기능 추가함. 18.06.26 김미수
         * 색상표
         * 1) 카트린     : BD0000B4
         * 2) 막시         : FF4E00B4
         * 3) 아이리스    : 9B92FFB4, FFA9C2B4, FF96B5B4
         * 4) 에밀         : 075BE5B4
         * 5) 하나         : FFFD4FB4
         * 6) 존         : 1C1C1CB4
         * 7) 장연화     : 7BF6EAB4
         * 8) 뮈라         : E1E1E1B4
         * 9) 왈멍멍      : 83CA0FB4
         * 10) 냥냐리우스 : 6223B2B4
         * 11) 주인공     : 74A0CCB4
         * Default) 기타  : 745D5DB4
        */

		if (Equals(showingNameBox.text, nameArray[1]))
			showingNameBG.color = new Color32(0xBD, 0x00, 0x00, 0xB4);
		else if (Equals(showingNameBox.text, nameArray[2]))
			showingNameBG.color = new Color32(0xFF, 0x4E, 0x00, 0xB4);
		else if (Equals(showingNameBox.text, nameArray[3]))
			showingNameBG.color = new Color32(0xFF, 0x96, 0xB5, 0xB4);
		else if (Equals(showingNameBox.text, nameArray[4]))
			showingNameBG.color = new Color32(0x07, 0x5B, 0xE5, 0xB4);
		else if (Equals(showingNameBox.text, nameArray[5]))
			showingNameBG.color = new Color32(0xFF, 0xFD, 0x4F, 0xB4);
		else if (Equals(showingNameBox.text, nameArray[6]))
			showingNameBG.color = new Color32(0x1C, 0x1C, 0x1C, 0xB4);
		else if (Equals(showingNameBox.text, nameArray[7]))
			showingNameBG.color = new Color32(0x7B, 0xF6, 0xEA, 0xB4);
		else if (Equals(showingNameBox.text, nameArray[8]))
			showingNameBG.color = new Color32(0xE1, 0xE1, 0xE1, 0xB4);
		else if (Equals(showingNameBox.text, nameArray[9]))
			showingNameBG.color = new Color32(0x83, 0xCA, 0x0F, 0xB4);
		else if (Equals(showingNameBox.text, nameArray[10]))
			showingNameBG.color = new Color32(0x62, 0x23, 0xB2, 0xB4);
		else if (Equals(showingNameBox.text, nameArray[11]))
			showingNameBG.color = new Color32(0x74, 0xA0, 0xCC, 0xB4);
		else
			showingNameBG.color = new Color32(0x74, 0x5D, 0x5D, 0xB4);
		//
		Debug.Log("n" + name);
	}
	public void DoChangeTextColor(int type)
	{
		switch(type)
		{
			case 0:
				
				showingTextBox.color = new Color32(0x78, 0x8C, 0xF1, 0xDC);//new Color32(0x1F, 0x34, 0x9E, 0xDC);
				break;
			case 1:
				showingTextBox.color = new Color32(0xFF, 0xB9, 0x00, 0xDC);
				break;
			case 2:
				showingTextBox.color = new Color32(0xFF, 0xFF, 0xFF, 0xDC);
				break;
			default:
				break;
		}
	}
	public void DoIllustrationChange(int illustrationNum)
	{
		if (illustrationPosition == 0)
			DoIllustrationChangeL(illustrationNum + emotionOffset);
		else
			DoIllustrationChangeR(illustrationNum = emotionOffset);
	}
	public void DoIllustrationChangeL(int illustrationNum)
	{
		if (preLoadedIllustrations.Count > illustrationNum)
			illustrationL.sprite = preLoadedIllustrations[illustrationNum];
		else
			illustrationL.sprite = preLoadedIllustrations[0];
	}
	public void DoIllustrationChangeR(int illustrationNum)
	{
		if (preLoadedIllustrations.Count > illustrationNum)
			illustrationR.sprite = preLoadedIllustrations[illustrationNum];
		else
			illustrationR.sprite = preLoadedIllustrations[0];
		
	}
	IEnumerator DoEffect(int effectNum)
	{
		
		Debug.Log("EFFFFF" + effectNum);
		switch (effectNum)
		{
			case 1:
				StartCoroutine(_CameraShake());
				break;
			case 2:
				yield return StartCoroutine(_FadeIn());
				break;
			case 3:
				yield return StartCoroutine(_FadeOut());
				break;
			case 4:
				yield return StartCoroutine(_StartDialog());
				break;
			case 5:
				yield return StartCoroutine(_EndDialog());
				break;
			case 6:
				GrayScaleOn();
				break;
			case 7:
				GrayScaleOff();
				break;
			case 8:
				//둘만 흑백
				AllIllustrationsGrayScaleOn();
				break;
			case 9:
				//둘만 흑백 해제
				AllIllustrationsGrayScaleOff();
				break;
			case 10:
				//둘다 컬러
				yield return StartCoroutine(_EnableAllIllustrations());
				break;
			case 13:
				yield return StartCoroutine(HideSmallIllustration());
				break;
			case 14:
				yield return StartCoroutine(HideBigIllustration());
				break;
			default:
				break;
		}
	}
	public void DoSave()
	{
		
		
		List<DialogSaveData> sdList = new List<DialogSaveData>();
		//List<TestClass> tList = new List<TestClass>();
		/*TestClass t1 = new global::TestClass();
		TestClass t2 = new global::TestClass();

		t1.names = new Hashtable();
		t1.effects = new Hashtable();
		t2.names = new Hashtable();
		t2.effects = new Hashtable();

		t1.names.Add(1, "ganada");
		t1.names.Add(2, "dada");
		t1.effects.Add(10, "haha");
		t2.names.Add(1, "jkakak");
		t2.effects.Add(10, "hoho");*/
		for(int i=0; i<dialogList.Count; i++)
		{
			DialogSaveData tp = new DialogSaveData();
			tp.effects = dialogList[i].effects;
			tp.illustrationsL = dialogList[i].illustrationsL;
			tp.illustrationsR = dialogList[i].illustrationsR;
			tp.fullText = dialogList[i].fullText;
			tp.names = dialogList[i].names;
			tp.textType = dialogList[i].type;
			tp.markerPositionList = dialogList[i].markerPositionList;
			tp.smallIllustrations = dialogList[i].smallIllustrations;
			tp.bigIllustrations = dialogList[i].bigIllustrations;
			tp.backgrounds = dialogList[i].backgrounds;
			tp.markerPositionList = new List<Vector3>();
			foreach(Markers mk in dialogList[i].markersList)
			{
				tp.markerPositionList.Add(mk.position);
			}
			sdList.Add(tp);
		}

		//string jsonTexts = JsonUtility.ToJson(tList, true);
		//Debug.Log(jsonTexts);
		DataContractJsonSerializer serializer = new DataContractJsonSerializer(sdList.GetType());
		
		
		FileStream fs = new FileStream(Application.persistentDataPath + "\\DialogListData.json", FileMode.OpenOrCreate);
		serializer.WriteObject(fs, sdList);
		fs.Close();
		Debug.Log("Saved");
		//Debug.Log(Encoding.Default.GetString(ms.ToArray()));
		//string jsonText = Encoding.Default.GetString(ms.ToArray());
		//MemoryStream ms2 = new MemoryStream(Encoding.Default.GetBytes(jsonText));

		//FileStream fs2 = new FileStream(Application.persistentDataPath + "\\metest.json", FileMode.OpenOrCreate);
	}
	public void DoLoad()
	{
		/*Debug.Log(Application.persistentDataPath);
		if (File.Exists(Application.persistentDataPath + "/" + loadFileName.text + ".json"))
		{
			string jsonText = File.ReadAllText(Application.persistentDataPath + "/" + loadFileName.text + ".json");
			JSONNode json = JSON.Parse(jsonText);
			for (int i = 0; i < json["savedSentence"].Count; i++)
			{
				GameObject newClone = Instantiate(forCloneTextObject);
				clonedObjects.Add(newClone);
				newClone.GetComponent<RectTransform>().SetParent(savedSentenceContent.GetComponent<RectTransform>());
				newClone.GetComponent<RectTransform>().localScale = Vector3.one;
				newClone.GetComponent<Dialogs>().SetIndex(i);
				newClone.GetComponent<Dialogs>().SetDialogEditor(this);
				newClone.GetComponent<Text>().text = json["savedSentence"][i];
			}
		}*/
		if(File.Exists(Application.persistentDataPath+"/"+loadFileName.text+".json"))
		{
			List<DialogSaveData> sdList = new List<DialogSaveData>();
			FileStream fs = new FileStream(Application.persistentDataPath + "/" + loadFileName.text + ".json", FileMode.OpenOrCreate);
			DataContractJsonSerializer serializer = new DataContractJsonSerializer(sdList.GetType());
			sdList = (List<DialogSaveData>)serializer.ReadObject(fs);
			for(int i =0; i<sdList.Count; i++)
			{
				currentDialogs.type = sdList[i].textType;

				int cCount = 0;
				int posCount = 0;
				foreach(char c in sdList[i].fullText)
				{
					if (sdList[i].effects.ContainsKey(cCount))
					{
						foreach (int a in sdList[i].effects[cCount])
						{
							currentDialogs.AddEffect(cCount, a, new Vector2(sdList[i].markerPositionList[posCount].x, sdList[i].markerPositionList[posCount++].y+20));
						}
					}
					if (sdList[i].illustrationsL.ContainsKey(cCount))
					{
						foreach (int a in sdList[i].illustrationsL[cCount])
						{
							currentDialogs.AddIllustrationL(cCount,a, new Vector2(sdList[i].markerPositionList[posCount].x, sdList[i].markerPositionList[posCount++].y + 20));
						}
					}
					if (sdList[i].illustrationsR.ContainsKey(cCount))
					{
						foreach (int a in sdList[i].illustrationsR[cCount])
						{
							currentDialogs.AddIllustrationR(cCount, a, new Vector2(sdList[i].markerPositionList[posCount].x, sdList[i].markerPositionList[posCount++].y + 20));
						}
					}
					if (sdList[i].names.ContainsKey(cCount))
					{
						foreach (string a in sdList[i].names[cCount])
						{
							currentDialogs.AddName(cCount, a, new Vector2(sdList[i].markerPositionList[posCount].x, sdList[i].markerPositionList[posCount++].y + 20));
						}
					}
					if(sdList[i].backgrounds.ContainsKey(cCount))
					{
						currentDialogs.AddBackground(cCount, sdList[i].backgrounds[cCount], new Vector2(sdList[i].markerPositionList[posCount].x, sdList[i].markerPositionList[posCount++].y + 20));
					}
					if (sdList[i].smallIllustrations.ContainsKey(cCount))
					{
						currentDialogs.AddSmallIllustration(cCount, sdList[i].smallIllustrations[cCount], new Vector2(sdList[i].markerPositionList[posCount].x, sdList[i].markerPositionList[posCount++].y + 20));
					}
					if (sdList[i].bigIllustrations.ContainsKey(cCount))
					{
						currentDialogs.AddBigIllustration(cCount, sdList[i].bigIllustrations[cCount], new Vector2(sdList[i].markerPositionList[posCount].x, sdList[i].markerPositionList[posCount++].y + 20));
					}
					cCount++;
				}
				currentDialogs.SetFullText(sdList[i].fullText);
				currentDialogs.SetIsRecorded();
				dialogList.Insert(dialogSelectIndex <= 0 ? 0 : dialogSelectIndex, currentDialogs);
				clonedObjects.Insert(dialogSelectIndex <= 0 ? 0 : dialogSelectIndex, currentDialogsObject);
				currentDialogs.SetIndex(dialogSelectIndex <= 0 ? 0 : dialogSelectIndex);
				currentDialogsObject.GetComponent<RectTransform>().SetParent(savedSentenceContent.GetComponent<RectTransform>());
				currentDialogsObject.GetComponent<RectTransform>().localScale = Vector3.one;
				//newClone.GetComponent<RectTransform>().SetParent(savedSentenceContent.GetComponent<RectTransform>());
				//newClone.GetComponent<RectTransform>().localScale = Vector3.one;
				currentDialogs.HideMarker();


				CreateCurrentDialogs();
				

				editorsTextInputField.text = "";
				caretPositionRecord = 0;
				caretPositionVector = new Vector3(-465.0f, 95.0f);
				DoRemap();
				SetDialogSelectIndex(clonedObjects.Count);
				currentDialogs.Start();

			}
			fs.Close();

		}
		else
		{
			Debug.Log("File Does Not Exist!");
		}
	}
	

	IEnumerator _CameraShake()
	{
		
		RectTransform tr = dialogPanel.GetComponent<RectTransform>();
		float shakeAmount = 1.0f;
		while (shakeAmount > 0)
		{
			tr.localPosition = originLocal + (Vector3)(UnityEngine.Random.insideUnitCircle * shakeAmount * 60);
			yield return new WaitForSeconds(typeWriteBetween/1.8f);
			shakeAmount -= 0.1f;
		}
		tr.position = origin;
		//yield return null;
	}
	IEnumerator _FadeIn()
	{
		//yield return null;
		float Alpha = 1.0f;
		fadePanel.color = new Color(fadePanel.color.r, fadePanel.color.g, fadePanel.color.b, 1.0f);
		while (fadePanel.color.a > 0.0f)
		{
			yield return new WaitForSeconds(typeWriteBetween/3.6f);
			fadePanel.color = new Color(fadePanel.color.r, fadePanel.color.g, fadePanel.color.b, Alpha);
			Alpha -= 0.01f;
		}
	}

	IEnumerator _FadeOut()
	{
		//yield return null;
		Debug.Log("Ha");
		float Alpha = 0.0f;
		fadePanel.color = new Color(fadePanel.color.r, fadePanel.color.g, fadePanel.color.b, 0.0f);
		while (fadePanel.color.a < 1.0f)
		{
			yield return new WaitForSeconds(typeWriteBetween/3.6f);
			fadePanel.color = new Color(fadePanel.color.r, fadePanel.color.g, fadePanel.color.b, Alpha);
			Alpha += 0.01f;
		}
	}

	IEnumerator _StartDialog()
	{
		//yield return null;
		Vector2 origin;
		float distance = 0.0f;
		origin = dialogContentTransform.anchoredPosition;
		dialogContentTransform.anchoredPosition = new Vector2(origin.x, origin.y - 320.0f);
		while (dialogContentTransform.anchoredPosition.y < origin.y - 5.0f)
		{
			yield return new WaitForSeconds(tempTimeBetween);
			dialogContentTransform.anchoredPosition = Vector2.Lerp(origin, dialogContentTransform.anchoredPosition, 0.9f);
		}
		dialogContentTransform.anchoredPosition = origin;
	}

	IEnumerator _EndDialog()
	{
		yield return null;
	}
	
	IEnumerator ShowBigIllustration(int idx)
	{
		
		bigIllustrationImage.sprite = preLoadedBigIllustrations[idx];
		bigIllustrationObject.SetActive(true);
		yield return null;
		/*while(true)
		{
			yield return null;
			if (Input.touches.Length > 0 || Input.GetMouseButtonUp(0))
				break;
		}*/
		//yield return StartCoroutine(_Wait()); 
		//bigIllustrationObject.SetActive(false);

	}
	IEnumerator ShowSmallIllustration(int idx)
	{
		smallIllustrationImage.sprite = preLoadedSmallIllustrations[idx];
		smallIllustrationObject.SetActive(true);
		yield return null;
		/*while(true)
		{
			yield return null;
			if(Input.touches.Length > 0 || Input.GetMouseButtonUp(0))
			{
				break;
			}
		}*/
		//yield return StartCoroutine(_Wait());
		//smallIllustrationObject.SetActive(false);
	}
	IEnumerator HideSmallIllustration()
	{
		yield return null;
		smallIllustrationObject.SetActive(false);
	}
	IEnumerator HideBigIllustration()
	{
		yield return null;
		bigIllustrationObject.SetActive(false);
	}
	IEnumerator ShowBackground(int idx)
	{
		//smallIllustrationImage.sprite = preLoadedSmallIllustrations[idx];
		backGroundImage.sprite = preLoadedBackgrounds[idx];
		
		yield return null;

	}
	public void GrayScaleOn()
	{
		StartCoroutine(_GrayScaleOn());
	}
	IEnumerator _GrayScaleOn()
	{
		
		grayscale.enabled = true;
		yield return null;
	}
	public void GrayScaleOff()
	{
		StartCoroutine(_GrayScaleOff());
	}
	IEnumerator _GrayScaleOff()
	{
		
		grayscale.enabled = false;
		yield return null;
	}
	public void AllIllustrationsGrayScaleOn()
	{
		StartCoroutine(_AllIllustrationsGrayScaleOn());
	}
	IEnumerator _AllIllustrationsGrayScaleOn()
	{
		illustrationL.material = illustrationGrayScale;
		illustrationR.material = illustrationGrayScale;
		yield return null;
	}
	public void AllIllustrationsGrayScaleOff()
	{
		StartCoroutine(_AllIllustrationsGrayScaleOff());
	}
	IEnumerator _AllIllustrationsGrayScaleOff()
	{
		illustrationL.material = null;
		illustrationR.material = null;
		yield return null;
	}
	public void EnableAllIllustrations()
	{
		StartCoroutine(_EnableAllIllustrations());
	}
	IEnumerator _EnableAllIllustrations()
	{
		illustrationL.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
		illustrationR.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
		showingNameBG.gameObject.SetActive(false);
		showingNameBox.enabled = false;
		yield return null;
	}
	public void ChangeTextType()
	{
		switch (dialogTypeNum)
		{
			/* 
             * 0) 독백
             * 1) 나레이션
             * 2) 대사
             */
			case 0:
				//독백 -> 나레이션
				dialogTypeNum = 1;
				dialogType.text = changeBtnStringArray[dialogTypeNum];
				
				changeBtn.image.color = new Color32(0xFF, 0xB9, 0x00, 0xDC);
				currentDialogs.SetDialogType(dialogTypeNum);
				editorsTextInputField.textComponent.color = new Color32(0xFF, 0xB9, 0x00, 0xDC);
				break;
			case 1:
				//나레이션 -> 대사
				dialogTypeNum = 2;
				dialogType.text = changeBtnStringArray[dialogTypeNum];
				
				changeBtn.image.color = new Color32(0x26, 0x26, 0x26, 0xDC);
				currentDialogs.SetDialogType(dialogTypeNum);
				editorsTextInputField.textComponent.color = new Color32(0x11, 0x11, 0x11, 0xDC);
				break;
			case 2:
				//대사 -> 독백
				dialogTypeNum = 0;
				dialogType.text = changeBtnStringArray[dialogTypeNum];
				changeBtn.image.color = new Color32(0x1F, 0x34, 0x9E, 0xDC);
				currentDialogs.SetDialogType(dialogTypeNum);
				editorsTextInputField.textComponent.color = new Color32(0x1F, 0x34, 0x9E, 0xDC);
				break;
		}
	}
	
	public void ChangeTextType(int idx)
	{
		dialogTypeNum = idx;
		switch (dialogTypeNum)
		{
			/* 
             * 0) 독백
             * 1) 나레이션
             * 2) 대사
             */
			case 0:
				dialogTypeNum = 0;
				dialogType.text = changeBtnStringArray[dialogTypeNum];

				changeBtn.image.color = new Color32(0x1F, 0x34, 0x9E, 0xDC);
				currentDialogs.SetDialogType(dialogTypeNum);
				editorsTextInputField.textComponent.color = new Color32(0x1F, 0x34, 0x9E, 0xDC);
				break;
			case 1:
				dialogTypeNum = 1;
				dialogType.text = changeBtnStringArray[dialogTypeNum];

				changeBtn.image.color = new Color32(0xFF, 0xB9, 0x00, 0xDC);
				currentDialogs.SetDialogType(dialogTypeNum);
				editorsTextInputField.textComponent.color = new Color32(0xFF, 0xB9, 0x00, 0xDC);
				break;
			case 2:
				dialogTypeNum = 2;
				dialogType.text = changeBtnStringArray[dialogTypeNum];

				changeBtn.image.color = new Color32(0x26, 0x26, 0x26, 0xDC);
				currentDialogs.SetDialogType(dialogTypeNum);
				editorsTextInputField.textComponent.color = new Color32(0x11, 0x11, 0x11, 0xDC);
				break;
		}
	}
	public void CreateCurrentDialogs()
	{
		if (currentDialogs != null)
			currentDialogs.HideMarker();
		if (currentDialogs != null && currentDialogs.isRecorded == false)
		{
			Destroy(currentDialogs.gameObject);
		}
		currentDialogsObject = Instantiate(forCloneTextObject);
		currentDialogs = currentDialogsObject.GetComponent<Dialogs>();
		currentDialogs.SetDialogEditor(this);
		currentDialogs.SetDialogType(dialogTypeNum);
		editorsTextInputField.text = "";
		if (selectedDialogs != null)
		{
			selectedDialogs.HideMarker();
			selectedDialogs.GetComponent<Image>().color = new Color32(0xFF, 0xFF, 0xFF, 0x84);
		}
		selectedDialogs = null;
		selectedDialogsObject = null;
		selectedMarkers = null;
		DoIllustrationChangeL(0);
		DoIllustrationChangeR(0);
		DoChangeName("");
		

	}
	/*public void CopyToCurrentDialogs()
	{
		currentDialogs.fullText = selectedDialogs.fullText;
		foreach(KeyValuePair<int, List<int>> kv in selectedDialogs.effects)
		{
			foreach(int v in selectedDialogs.effects[kv.Key])
			{
				currentDialogs.AddEffect(kv.Key, v);
			}
		}
		foreach(KeyValuePair<int, List<int>> kv in selectedDialogs.illustrationsL)
		{
			foreach(int v in selectedDialogs.illustrationsL[kv.Key])
			{
				currentDialogs.AddIllustrationL(kv.Key, v);
			}
		}
		foreach (KeyValuePair<int, List<int>> kv in selectedDialogs.illustrationsR)
		{
			foreach (int v in selectedDialogs.illustrationsR[kv.Key])
			{
				currentDialogs.AddIllustrationR(kv.Key, v);
			}
		}

		foreach (KeyValuePair<int, List<string>> kv in selectedDialogs.names)
		{
			foreach (string v in selectedDialogs.names[kv.Key])
			{
				currentDialogs.AddName(kv.Key, v);
			}
		}
		ChangeTextType(selectedDialogs.type);

	}*/
	public void StartEdit()
	{
		editorsTextInputField.text = currentDialogs.fullText;
		currentDialogs.ShowMarker();
	}
	public void EndEdit()
	{
		currentDialogs.SetFullText(editorsTextInputField.text);
		currentDialogs.HideMarker();
		editorsTextInputField.text = "";
	}
	public void Swap(int dir)
	{
		//0 : up / 1 : down
		if(selectedDialogs != null)
		{
			if(dir == 0 && selectedDialogs.GetIndex() > 0)
			{
				GameObject tp = clonedObjects[selectedDialogs.GetIndex()];
				clonedObjects[selectedDialogs.GetIndex()] = clonedObjects[selectedDialogs.GetIndex() - 1];
				clonedObjects[selectedDialogs.GetIndex() - 1] = tp;
				
				DoRemap();
				//clonedObjects[selectedDialogs.GetIndex()]
			}
			else if(dir == 1 && selectedDialogs.GetIndex() < clonedObjects.Count -1)
			{
				GameObject tp = clonedObjects[selectedDialogs.GetIndex()];
				clonedObjects[selectedDialogs.GetIndex()] = clonedObjects[selectedDialogs.GetIndex() + 1];
				clonedObjects[selectedDialogs.GetIndex() + 1] = tp;
				DoRemap();
			}
		}
	}
	public void SelectNewDialog()
	{
		if(isReplacing)
		{
			//수정 중에는 새로 생성 불가
		}
		else
		{
			SetDialogSelectIndex(dialogList.Count);
			CreateCurrentDialogs();
		}
	}
	public void SetDialogReplace()
	{
		if (isReplacing == true)
		{
			isReplacing = false;
		}
		else
		{
			isReplacing = true;			
		}
	}
	public void SetEmotionOffset(int _offset)
	{
		emotionButtons[emotionOffset].color = new Color32(0xCC, 0xD5, 0xF5, 0xFF);
		emotionOffset = _offset;
		emotionButtons[emotionOffset].color = new Color32(0xFF, 0xAF, 0xAF, 0xFF);

	}
	
}