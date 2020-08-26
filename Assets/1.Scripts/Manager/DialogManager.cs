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

public class DialogManager : MonoBehaviour
{
	public GameObject dialogUI; //Hide , Show 에 사용
	public Vector3 dialogUIOriginPosition; //Hide, Show 에 사용
	public List<Sprite> preLoadedIllustrations; // Illustration sprite 들
	public List<Sprite> preLoadedSmallIllustrations;
	public List<Sprite> preLoadedBigIllustrations;
	public List<Sprite> preLoadedBackgrounds;
	public List<Dialogs> dialogList;

	public int illustrationPosition = 0; // 일러스트 추가시 or 
										 // 버튼 이미지 바꾸는 방식으로 변경함. 18.06.26 김미수
										 //
	public float tempTimeBetween = 0.08f; // 대화시작 이펙트 연출 시 이동 시간 간격
	public RectTransform dialogContentTransform; // Shake 이펙트 시 대화창 위치
	public Image fadePanel; // fade 효과시 패널
	public Grayscale grayscale; // 화면 전체 흑백효과용
	public Material illustrationGrayScale;
	public GameObject dialogPanel;
	public Image illustrationL;
	public Image illustrationR;
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
	public bool dialogUIHide = false;
	// 대사 전환 버튼용 18.06.26 김미수
	public String[] changeBtnStringArray;
	public StringBuilder forShowTextBox;

	Vector3 origin; //대화창 anchroed position
	Vector3 originLocal; // 대화창 local position

	Color narrationTextColor;
	Color monologueTextColor;
	Color dialogTextColor;

	public Text dialogType;
	int dialogTypeNum = 2; // 0 = 독백 / 1 = 나레이션 / 2 = 대사

	public Dialogs selectedDialogs;

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
		for (int i = 0; i < b.Length; i++)
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
	}

	void Update()
	{
		if (Input.GetMouseButtonDown(0) && isPlaying)
		{
			Vector3 clickPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			hit = Physics2D.Raycast(clickPos, Vector2.zero);
			if (hit.collider != null && hit.collider.gameObject.tag.Equals("guard"))
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

		if (Input.GetMouseButtonUp(0) && isPlaying)
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

	IEnumerator _Wait()
	{
		StartCoroutine(GiveMovementNextButton());
		while (isPlaying == true)
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
		while (isPlaying == true && isTouchDown == false)
		{
			if (nextButton.transform.localPosition.x > nextButtonPosition.x + 13.0f)
			{
				coeff = -1;
			}
			else if (nextButton.transform.localPosition.x <= nextButtonPosition.x)
			{
				coeff = 1;
			}
			nextButton.transform.localPosition += new Vector3(55.0f, 0.0f, 0.0f) * Time.deltaTime * coeff;
			//nextButton.transform.Translate(1.0f, 0.0f, 0.0f);
			yield return null;
		}
		nextButton.transform.localPosition = nextButtonPosition;
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
		switch (type)
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
	public void LoadDialogs(string fileName)
	{
		TextAsset dialogText;
		List<DialogSaveData> loadedDataList = new List<DialogSaveData>();
		Dialogs tempDialog = null; ;

		if((dialogText = Resources.Load<TextAsset>("Dialogs/"+fileName)) != null)
		{
			MemoryStream ms = new MemoryStream(dialogText.bytes);
			DataContractJsonSerializer serializer = new DataContractJsonSerializer(loadedDataList.GetType());
			loadedDataList = (List<DialogSaveData>)serializer.ReadObject(ms);
			for (int i = 0; i < loadedDataList.Count; i++)
			{
				tempDialog = new Dialogs();
				tempDialog.type = loadedDataList[i].textType;

				int cCount = 0;
				int posCount = 0;
				foreach (char c in loadedDataList[i].fullText)
				{
					if (loadedDataList[i].effects.ContainsKey(cCount))
					{
						foreach (int a in loadedDataList[i].effects[cCount])
						{
							tempDialog.AddEffect(cCount, a);
						}
					}
					if (loadedDataList[i].illustrationsL.ContainsKey(cCount))
					{
						foreach (int a in loadedDataList[i].illustrationsL[cCount])
						{
							tempDialog.AddIllustrationL(cCount, a);
						}
					}
					if (loadedDataList[i].illustrationsR.ContainsKey(cCount))
					{
						foreach (int a in loadedDataList[i].illustrationsR[cCount])
						{
							tempDialog.AddIllustrationR(cCount, a);
						}
					}
					if (loadedDataList[i].names.ContainsKey(cCount))
					{
						foreach (string a in loadedDataList[i].names[cCount])
						{
							tempDialog.AddName(cCount, a);
						}
					}
					if (loadedDataList[i].backgrounds.ContainsKey(cCount))
					{
						tempDialog.AddBackground(cCount, loadedDataList[i].backgrounds[cCount]);
					}
					if (loadedDataList[i].smallIllustrations.ContainsKey(cCount))
					{
						tempDialog.AddSmallIllustration(cCount, loadedDataList[i].smallIllustrations[cCount]);
					}
					if (loadedDataList[i].bigIllustrations.ContainsKey(cCount))
					{
						tempDialog.AddBigIllustration(cCount, loadedDataList[i].bigIllustrations[cCount]);
					}
					cCount++;
				}
				tempDialog.SetFullText(loadedDataList[i].fullText);
				tempDialog.SetIsRecorded();
				//dialogList.Insert(dialogSelectIndex <= 0 ? 0 : dialogSelectIndex, tempDialog);
				dialogList.Add(tempDialog);
				tempDialog.SetIndex(i);
			}
		}
		else
		{
			Debug.Log("Dialog" + fileName + " does not exist");
		}
	}
	IEnumerator _CameraShake()
	{

		RectTransform tr = dialogPanel.GetComponent<RectTransform>();
		float shakeAmount = 1.0f;
		while (shakeAmount > 0)
		{
			tr.localPosition = originLocal + (Vector3)(UnityEngine.Random.insideUnitCircle * shakeAmount * 60);
			yield return new WaitForSeconds(typeWriteBetween / 1.8f);
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
			yield return new WaitForSeconds(typeWriteBetween / 3.6f);
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
			yield return new WaitForSeconds(typeWriteBetween / 3.6f);
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

	public void SetEmotionOffset(int _offset)
	{
		emotionButtons[emotionOffset].color = new Color32(0xCC, 0xD5, 0xF5, 0xFF);
		emotionOffset = _offset;
		emotionButtons[emotionOffset].color = new Color32(0xFF, 0xAF, 0xAF, 0xFF);

	}

}