using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpecificationPanel : UIObject {

	public GameObject specPanelBase;
	//스펙 패널 루트

	public GameObject statPanelBase;
	public Text characterNameText;
	public Image chracterImage;
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

	//캐릭터 클릭할떄 -> InputManager -> UIManager -> SpecificationPanel 로 캐릭터 오브젝트 전달
	//캐릭터 유형에 따라 탭 활성화
	//
	public Traveler curCharacter;
	public void Awake()
	{
		specPanelBase = gameObject;
	}

	public void OnCharacterSelected(Traveler traveler)
	{
		if(traveler is SpecialAdventurer)
		{
			//desire, stat, item 오픈
			SpecialAdventurer sp = (SpecialAdventurer)traveler;
			
		}
		else if(traveler is Adventurer)
		{
			//desire, stat 오픈
		}
		else
		{
			//desire 오픈
		}
	}
	


}
