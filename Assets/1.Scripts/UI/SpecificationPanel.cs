using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpecificationPanel : UIObject {

	public GameObject specPanelBase;
	//스펙 패널 루트
	
	public GameObject statPanelBase;
	public Text characterNameText;
	public Image characterImage;
	public Text characterExplanationText;
	public Text characterCurrentStateText;
	public Text characterJobText;
	public Text characterRaceText;
	public Text characterGoldText;
	//기본정보(이름, 스프라이트, 설명,
	//종족, 직업, 소지금액)

	public GameObject desirePanelBase;
	public Text characterDesireThirstyText;
	public Text characterDesireHungryText;
	public Text characterDesireSleepText;
	public Text characterDesireTourText;
	public Text characterDesireConvenienceText;
	public Text characterDesireFunText;
	//캐릭터 욕구

	public GameObject battleStatPanelBase;
	public Text characterLevelText;
	public Text characterHealthText; // N/M 형식으로 출력할 것
	public Text characterAttackText;
	public Text characterAttackSpeedText;
	public Text characterDefenseText;
	public Text characterPenetrationText;
	public Text characterCriticalChanceText;
	public Text characterCriticalAttackText;
	public Text characterAvoidText;
	public Text characterAttackRangeText;
	//캐릭터 전투스탯

	public GameObject itemPanelBase;
	public Image characterEquipedItemImage_1;
	public Image characterEquipedItemImage_2;
	public Image characterEquipedItemImage_3;
	public Image characterEquipedItemImage_4;
	public GameObject itemExplanationPanelBase;
	public Text itemStatText;
	public Text itemExplanationText;
	//아이템 장착정보 및 세부정보

	public GameObject nextButton;
	public GameObject prevButton;

	GameObject curOpenPanel = null;
	RectTransform rectTransform;
	Vector3 far;
	GameObject[] viewablePanels;
	int viewingIndex = 0;
	Coroutine statUpdateCoroutine;
	WaitForSeconds updateTick = new WaitForSeconds(2.0f);
	//캐릭터 클릭할떄 -> InputManager -> UIManager -> SpecificationPanel 로 캐릭터 오브젝트 전달
	//캐릭터 유형에 따라 탭 활성화
	Coroutine tracing;
	public Traveler curCharacter;
	public void Awake()
	{
		specPanelBase = gameObject;
		rectTransform = GetComponent<RectTransform>();
		far = new Vector3(5000f, 5000f, 0.0f);
	}

	public void OnCharacterSelected(Traveler traveler)
	{
		//Child부터 캐스팅 가능한지 검사
		if(traveler is SpecialAdventurer)
		{
			//desire, stat, item 오픈
			viewablePanels = new GameObject[3];
			viewablePanels[0] = statPanelBase;
			viewablePanels[1] = battleStatPanelBase;
			viewablePanels[2] = itemPanelBase;
		}
		else if(traveler is Adventurer)
		{
			//desire, stat 오픈
			viewablePanels = new GameObject[2];
			viewablePanels[0] = statPanelBase;
			viewablePanels[1] = battleStatPanelBase;
			
		}
		else
		{
			//desire 오픈
			viewablePanels = new GameObject[1];
			viewablePanels[0] = statPanelBase;
		}
		curCharacter = traveler;
		OnOpenPanel();
	}
	public void OnCharacterDeselected()
	{
		StopCoroutine(tracing);
		rectTransform.position = far;
		nextButton.SetActive(false);
		prevButton.SetActive(false);
		StopCoroutine(statUpdateCoroutine);
		ClearUI();
		foreach (GameObject go in viewablePanels)
		{
			go.SetActive(false);
		}
		viewablePanels = null;
		viewingIndex = 0;
		curCharacter = null;
		
		
	}
	public void ClearUI()
	{
		characterNameText.text = string.Empty;
		characterImage.sprite = null;
		characterExplanationText.text = string.Empty;
		characterCurrentStateText.text = string.Empty;
		characterJobText.text = string.Empty;
		characterRaceText.text = string.Empty;
		characterGoldText.text = string.Empty;
		//
		characterDesireThirstyText.text = string.Empty;
		characterDesireHungryText.text = string.Empty;
		characterDesireSleepText.text = string.Empty;
		characterDesireTourText.text = string.Empty;
		characterDesireConvenienceText.text = string.Empty;
		characterDesireFunText.text = string.Empty;
		//
		characterLevelText.text = string.Empty;
		characterHealthText.text = string.Empty;
		characterAttackText.text = string.Empty;
		characterAttackSpeedText.text = string.Empty;
		characterDefenseText.text = string.Empty;
		characterPenetrationText.text = string.Empty;
		characterCriticalChanceText.text = string.Empty;
		characterCriticalAttackText.text = string.Empty;
		characterAvoidText.text = string.Empty;
		characterAttackRangeText.text = string.Empty;
		//
		characterEquipedItemImage_1.sprite = null;
		characterEquipedItemImage_2.sprite = null;
		characterEquipedItemImage_3.sprite = null;
		characterEquipedItemImage_4.sprite = null;
		itemStatText.text = string.Empty;
		itemExplanationText.text = string.Empty;

	}
	public void OpenNextPanel()
	{
		if (viewablePanels == null || viewablePanels.Length <= 0)
			return;
		viewablePanels[viewingIndex].SetActive(false);
		viewingIndex = (viewingIndex + 1) % viewablePanels.Length;
		viewablePanels[viewingIndex].SetActive(true);
	}
	
	public void OpenPrevPanel()
	{
		if (viewablePanels == null || viewablePanels.Length <= 0)
			return;
		viewablePanels[viewingIndex].SetActive(false);
		viewingIndex = viewingIndex - 1 < 0 ? viewablePanels.Length - 1 : viewingIndex - 1;
		viewablePanels[viewingIndex].SetActive(true);
	}
	public void OnOpenPanel() // StatPanel관련 초기화할것들
	{
		viewablePanels[viewingIndex].SetActive(true);
		statUpdateCoroutine = StartCoroutine(UpdateCharacterSpec());
		//StopCoroutine(tracing);
		tracing = StartCoroutine(TraceCharacter());
	}
	IEnumerator TraceCharacter()
	{
		Vector3 curPos;
		if (curCharacter == null)
			yield break;
		while(curCharacter != null)
		{
			yield return null;
			curPos = Camera.main.WorldToScreenPoint(curCharacter.gameObject.transform.position);
			rectTransform.localPosition = curPos;
		}
	}
	IEnumerator UpdateCharacterSpec()
	{
		if(curCharacter != null)
		{
			UpdateStatFirst();
			if(curCharacter is SpecialAdventurer)
			{
				UpdateStatFirst();
				UpdateDesireFirst();
				UpdateBattleStatFirst();
				UpdateItemFirst();
				while (curCharacter != null)
				{
					UpdateStat();
					UpdateDesire();
					UpdateBattleStat();
					UpdateItem();
					yield return updateTick;
				}
			}
			else if(curCharacter is Adventurer)
			{
				UpdateStatFirst();
				UpdateDesireFirst();
				UpdateBattleStatFirst();
				while(curCharacter != null)
				{
					UpdateStat();
					UpdateDesire();
					UpdateBattleStat();
					yield return updateTick;
				}
			}
			else // traveler
			{
				UpdateStatFirst();
				UpdateDesireFirst();
				while(curCharacter != null)
				{
					UpdateStat();
					UpdateDesire();
					yield return updateTick;
					
				}
			}
		}
	}
	public void UpdateStatFirst()
	{
		characterNameText.text = curCharacter.stat.name;
		characterImage.sprite = curCharacter.GetChracterSprite();
		characterExplanationText.text = curCharacter.stat.explanation;
		if(curCharacter is SpecialAdventurer)
		{
			characterJobText.text = "일선모험가";
		}
		else if(curCharacter is Adventurer)
		{
			characterJobText.text = "모험가";
		}
		else
		{
			characterJobText.text = "여행자";
		}
		characterRaceText.text = curCharacter.stat.race.ToString();
		
		
	}
	public void UpdateStat()
	{
		characterCurrentStateText.text = curCharacter.GetState().ToString();
		characterGoldText.text = curCharacter.stat.gold.ToString();
	}
	public void UpdateDesireFirst()
	{

	}
	public void UpdateDesire()
	{
		Stat s = curCharacter.stat;
		characterDesireThirstyText.text = s.GetSpecificDesire(DesireType.Thirsty).desireValue.ToString("0.0");
		characterDesireHungryText.text = s.GetSpecificDesire(DesireType.Hungry).desireValue.ToString("0.0");
		characterDesireSleepText.text = s.GetSpecificDesire(DesireType.Sleep).desireValue.ToString("0.0");
		characterDesireTourText.text = s.GetSpecificDesire(DesireType.Tour).desireValue.ToString("0.0");
		characterDesireConvenienceText.text = s.GetSpecificDesire(DesireType.Convenience).desireValue.ToString("0.0");
		characterDesireFunText.text = s.GetSpecificDesire(DesireType.Fun).desireValue.ToString("0.0");
	}
	public void UpdateBattleStatFirst()
	{

	}
	public void UpdateBattleStat()
	{
		if (!(curCharacter is Adventurer))
			return;
		Adventurer adv = (Adventurer)curCharacter;
		BattleStat bs = adv.GetBattleStat();
		characterLevelText.text = bs.ToString();
		characterHealthText.text = bs.ToString() + " / " + bs.HealthMax.ToString();
		characterAttackText.text = bs.ToString();
		characterAttackSpeedText.text = bs.AttackSpeed.ToString();
		characterDefenseText.text = bs.Defence.ToString();
		characterPenetrationText.text = bs.PenetrationFixed.ToString() + " / " + bs.PenetrationMult.ToString();
		characterCriticalChanceText.text = bs.CriticalChance.ToString();
		characterCriticalAttackText.text = bs.CriticalDamage.ToString();
		characterAvoidText.text = bs.BaseAvoid.ToString();
		characterAttackRangeText.text = bs.Range.ToString();
	}
	public void UpdateItemFirst()
	{
		if (!(curCharacter is SpecialAdventurer))
			return;
		SpecialAdventurer sadv = (SpecialAdventurer)curCharacter;
		characterEquipedItemImage_1.sprite = sadv.GetWeapon().GetItemImage();
		characterEquipedItemImage_2.sprite = sadv.GetArmor().GetItemImage();
		characterEquipedItemImage_3.sprite = sadv.GetAccessory1().GetItemImage();
		characterEquipedItemImage_4.sprite = sadv.GetAccessory2().GetItemImage();
	}
	//아이템 클릭시 OnClick 처리랑 끄기 어떻게??
	public void UpdateItem()
	{

	}
	public void OnClickItem(int ClickNum)
	{
		UpdateItemStat(ClickNum);
	}
	public void HideItemExplanationPanel()
	{
		itemExplanationPanelBase.GetComponent<RectTransform>().localPosition = new Vector2(5000.0f, 5000.0f);
	}
	public void UpdateItemStat(int itemNum) // button onclick
	{
		if (!(curCharacter is SpecialAdventurer))
			return;
		SpecialAdventurer sadv = (SpecialAdventurer)curCharacter;
		
		switch(itemNum)
		{
			case 0:
				if (sadv.GetWeapon() == null)
					return;
				itemStatText.text = sadv.GetWeapon().GetItemStatAsString();
				itemExplanationText.text = sadv.GetWeapon().Explanation;
				itemExplanationPanelBase.GetComponent<RectTransform>().localPosition = characterEquipedItemImage_1.GetComponent<RectTransform>().localPosition;
				
				break;
			case 1:
				if (sadv.GetArmor() == null)
					return;
				itemStatText.text = sadv.GetArmor().GetItemStatAsString();
				itemExplanationText.text = sadv.GetArmor().Explanation;
				itemExplanationPanelBase.GetComponent<RectTransform>().localPosition = characterEquipedItemImage_2.GetComponent<RectTransform>().localPosition;
				break;
			case 2:
				if (sadv.GetAccessory1() == null)
					return;
				itemStatText.text = sadv.GetAccessory1().GetItemStatAsString();
				itemExplanationText.text = sadv.GetAccessory1().Explanation;
				itemExplanationPanelBase.GetComponent<RectTransform>().localPosition = characterEquipedItemImage_3.GetComponent<RectTransform>().localPosition;
				break;
			case 3:
				if (sadv.GetAccessory2() == null)
					return;
				itemStatText.text = sadv.GetAccessory2().GetItemStatAsString();
				itemExplanationText.text = sadv.GetAccessory2().Explanation;
				itemExplanationPanelBase.GetComponent<RectTransform>().localPosition = characterEquipedItemImage_4.GetComponent<RectTransform>().localPosition;
				break;

			default:
				break;
		}
	}
	


}
