using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SimpleJSON;
using UnityEngine.SceneManagement;

public class ConquerPanel : MonoBehaviour {

	#region StageProgress
	public Text progressCurrent;
	public Text progressTotal;
	public Text currentHuntingAreaKind; // ~~/공략 중 까지..
	public Text remainMonsterCount;
	const string commonConqueringText = "일반 사냥터 공략 중";
	const string bossConqueringText = "보스 공략 중";
	#endregion

	#region CurrentMonsterStatus
	public Image monsterImage;
	public Text monsterLevel;
	public Text monsterName;
	public Text monsterHealth;
	public Text monsterDefence;
	public Text monsterMoveSpeed;
	public Text monsterAttackRange;
	public Text monsterDPS;
	public Text monsterAttack;
	public Text monsterAttackSpeed;
	public Text monsterCritical;
	public Text monsterCriticalAttack;
	public Text monsterPenetrate;
	public Text monsterUniqueSkillName;
	public Text monsterUniqueSkillCategory;
	public Text monsterUniqueSkillContent;
	#endregion

	public List<GameObject> huntingAreaButtons;
	public List<Image> huntingAreaButtonImages;
	Color buttonHighlightColor = new Color(0.738f, 0.933f, 0.957f);
	Color buttonNormalColor = Color.white;

	WaitForSeconds updateTick = new WaitForSeconds(3.0f);
	Coroutine updateProgressCoroutine = null;
	CombatAreaManager cam;
	JSONNode monsterColorsJson;
	List<Color> monsterColors;
	List<Color> bossColors;
	List<Sprite> monsterSprites;
	List<Sprite> bossSprites;
	int sceneNum = 0;

	public void Awake()
	{
		sceneNum = int.Parse(SceneManager.GetActiveScene().name);
		monsterColors = new List<Color>();
		bossColors = new List<Color>();
		monsterSprites = new List<Sprite>();
		bossSprites = new List<Sprite>();
		monsterColorsJson = JSON.Parse(Resources.Load<TextAsset>("Monsters/MonsterColors").ToString());
		for(int i = 0; i<monsterColorsJson["colorsCount"].AsInt; i++)
		{
			monsterColors.Add(new Color(monsterColorsJson["colors"][i]["r"].AsFloat, monsterColorsJson["colors"][i]["g"].AsFloat, monsterColorsJson["colors"][i]["b"].AsFloat));
		}
		for(int i = 0; i<monsterColorsJson["bossColorsCount"].AsInt; i++)
		{
			bossColors.Add(new Color(monsterColorsJson["bossColors"][i]["r"].AsFloat, monsterColorsJson["bossColors"][i]["g"].AsFloat, monsterColorsJson["bossColors"][i]["b"].AsFloat));
		}
		for(int i = 0; i < monsterColors.Count; i++)
		{
			monsterSprites.Add(Resources.Load<Sprite>("Monsters/Image/" + i.ToString()));
			Debug.Log("Color + " + i);
		}
		if (monsterSprites[0] == null)
			Debug.Log("sprite 0 is Null.......");
		Debug.Log("MonsterSpritesCount.. = " + monsterSprites.Count);
		for(int i = 0; i < bossColors.Count; i++)
		{
			bossSprites.Add(Resources.Load<Sprite>("Monster/Image/Boss" + i.ToString()));
		}
	}
	public void OnEnable()
	{
		cam = CombatAreaManager.Instance;
		SetButtonsByHuntingAreas();
		ShowMonsterInfo(0);
		updateProgressCoroutine = StartCoroutine(UpdateProgress());
	}
	public void OnDisable()
	{
		if(updateProgressCoroutine != null)
			StopCoroutine(updateProgressCoroutine);
	}
	public void SetMonsterSprites()
	{
		//현재 사냥터에
		//샘플1,샘플2, 보스 이미지 로드
		
	}
	public void SetButtonsByHuntingAreas()
	{		
		if(cam.huntingAreas[cam.ConqueringHuntingAreaIndex].IsBossArea == true)
		{
			//보스까지 표시
			huntingAreaButtons[2].SetActive(true);
		}
		else
		{
			huntingAreaButtons[2].SetActive(false);
		}
	}
	public void ShowMonsterInfo(int monsterNum)
	{
		//몬스터 이미지, 이름, 레벨, 능력치, 고유능력.
		Monster displayingMonster;
		BattleStat displayingBattleStat;
		string monsterSet;
		switch (monsterNum)
		{
			case 0: //0
				//버튼 색상 변경
				for(int i = 0; i<huntingAreaButtons.Count; i++)
				{
					if (i == monsterNum)
						huntingAreaButtonImages[i].color = buttonHighlightColor;
					else
						huntingAreaButtonImages[i].color = buttonNormalColor;
				}
				///
				displayingMonster = cam.huntingAreas[cam.ConqueringHuntingAreaIndex].GetSampleMonster1();
				displayingBattleStat = displayingMonster.GetBattleStat();
				monsterSet = cam.huntingAreaJson[sceneNum][cam.ConqueringHuntingAreaIndex]["monsterSet"];
				monsterName.text = cam.monsterJson[monsterSet][displayingMonster.monsterNum]["name"];
				monsterLevel.text = displayingBattleStat.Level.ToString();
				monsterHealth.text = string.Format("{0:0}", displayingBattleStat.HealthMax);
				monsterDefence.text = string.Format("{0:0.0}", displayingBattleStat.Defence);
				monsterMoveSpeed.text = string.Format("{0:0}%", (displayingBattleStat.MoveSpeed*100));
				monsterAttackRange.text = displayingBattleStat.BaseAttackRange.ToString();
				monsterDPS.text = string.Format("{0:0.00}", (displayingBattleStat.Attack * displayingBattleStat.AttackSpeed * ((1 + displayingBattleStat.CriticalChance) * (displayingBattleStat.CriticalDamage - 1))));
				monsterAttack.text = string.Format("{0:0}", displayingBattleStat.Attack);
				monsterAttackSpeed.text = string.Format("{0:0.0000}", displayingBattleStat.AttackSpeed);
				monsterCritical.text = string.Format("{0:0.00}%", displayingBattleStat.CriticalChance * 100);
				monsterCriticalAttack.text = string.Format("{0:0.0}%", displayingBattleStat.CriticalDamage * 100);
				monsterPenetrate.text = string.Format("{0:0}", displayingBattleStat.PenetrationFixed);
				monsterImage.sprite = monsterSprites[displayingMonster.monsterNum];
				monsterImage.color = monsterColors[displayingMonster.monsterNum];
				break;
			case 1: //1
					//버튼 색상 변경
				for (int i = 0; i < huntingAreaButtons.Count; i++)
				{
					if (i == monsterNum)
						huntingAreaButtonImages[i].color = buttonHighlightColor;
					else
						huntingAreaButtonImages[i].color = buttonNormalColor;
				}
				///
				displayingMonster = cam.huntingAreas[cam.ConqueringHuntingAreaIndex].GetSampleMonster2();
				displayingBattleStat = displayingMonster.GetBattleStat();
				monsterSet = cam.huntingAreaJson[sceneNum][cam.ConqueringHuntingAreaIndex]["monsterSet"];
				monsterName.text = cam.monsterJson[monsterSet][displayingMonster.monsterNum]["name"];
				monsterLevel.text = displayingBattleStat.Level.ToString();
				monsterHealth.text = string.Format("{0:0}", displayingBattleStat.HealthMax);
				monsterDefence.text = string.Format("{0:0.0}", displayingBattleStat.Defence);
				monsterMoveSpeed.text = string.Format("{0:0}%", (displayingBattleStat.MoveSpeed * 100));
				monsterAttackRange.text = displayingBattleStat.BaseAttackRange.ToString();
				monsterDPS.text = string.Format("{0:0.00}", (displayingBattleStat.Attack * displayingBattleStat.AttackSpeed * ((1 + displayingBattleStat.CriticalChance) * (displayingBattleStat.CriticalDamage - 1))));
				monsterAttack.text = string.Format("{0:0}", displayingBattleStat.Attack);
				monsterAttackSpeed.text = string.Format("{0:0.0000}", displayingBattleStat.AttackSpeed);
				monsterCritical.text = string.Format("{0:0.00}%", displayingBattleStat.CriticalChance * 100);
				monsterCriticalAttack.text = string.Format("{0:0.0}%", displayingBattleStat.CriticalDamage * 100);
				monsterPenetrate.text = string.Format("{0:0}", displayingBattleStat.PenetrationFixed);
				monsterImage.sprite = monsterSprites[displayingMonster.monsterNum];
				monsterImage.color = monsterColors[displayingMonster.monsterNum];
				break;
			case 2: //boss
					//버튼 색상 변경
				for (int i = 0; i < huntingAreaButtons.Count; i++)
				{
					if (i == monsterNum)
						huntingAreaButtonImages[i].color = buttonHighlightColor;
					else
						huntingAreaButtonImages[i].color = buttonNormalColor;
				}
				///
				displayingMonster = cam.bossAreas[cam.BossAreaIndex].GetBossMonsterAsComponent();
				displayingBattleStat = displayingMonster.GetBattleStat();
				monsterName.text = cam.monsterJson["Boss"][displayingMonster.monsterNum]["name"];
				monsterLevel.text = displayingBattleStat.Level.ToString();
				monsterHealth.text = string.Format("{0:0}", displayingBattleStat.HealthMax);
				monsterDefence.text = string.Format("{0:0.0}", displayingBattleStat.Defence);
				monsterMoveSpeed.text = string.Format("{0:0}%", (displayingBattleStat.MoveSpeed * 100));
				monsterAttackRange.text = displayingBattleStat.BaseAttackRange.ToString();
				monsterDPS.text = string.Format("{0:0.00}", (displayingBattleStat.Attack * displayingBattleStat.AttackSpeed * ((1 + displayingBattleStat.CriticalChance) * (displayingBattleStat.CriticalDamage - 1))));
				monsterAttack.text = string.Format("{0:0}", displayingBattleStat.Attack);
				monsterAttackSpeed.text = string.Format("{0:0.0000}", displayingBattleStat.AttackSpeed);
				monsterCritical.text = string.Format("{0:0.00}%", displayingBattleStat.CriticalChance * 100);
				monsterCriticalAttack.text = string.Format("{0:0.0}%", displayingBattleStat.CriticalDamage * 100);
				monsterPenetrate.text = string.Format("{0:0}", displayingBattleStat.PenetrationFixed);
				monsterImage.sprite = bossSprites[displayingMonster.monsterNum];
				monsterImage.color = monsterColors[displayingMonster.monsterNum];
				
				break;
			default:
				Debug.Log("Monster Number missing !");
				break;
		}
		
	}
	public IEnumerator UpdateProgress()
	{
		int originProgress = (cam.ConqueringHuntingAreaIndex);
		while(true)
		{
			if(originProgress <cam.ConqueringHuntingAreaIndex)
			{
				SetButtonsByHuntingAreas();
				ShowMonsterInfo(0);
				originProgress = cam.ConqueringHuntingAreaIndex;
			}
			//update Progress Info
			progressCurrent.text = (cam.ConqueringHuntingAreaIndex).ToString();
			progressTotal.text = (cam.huntingAreas.Count).ToString();
			currentHuntingAreaKind.text = GameManager.Instance.isBossPhase == true ? bossConqueringText : commonConqueringText;
			//if boss phase...
			remainMonsterCount.text = GameManager.Instance.isBossPhase == true ? "-1-" :(cam.huntingAreas[cam.ConqueringHuntingAreaIndex].GetConquerCondition() - cam.huntingAreas[cam.ConqueringHuntingAreaIndex].GetKillCount()).ToString();
			//end
			yield return updateTick;
		}
	}
}
