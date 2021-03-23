using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
	public Text monsterRange;
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

	public List<Button> huntingAreaButtons;

	WaitForSeconds updateTick = new WaitForSeconds(3.0f);
	Coroutine updateProgressCoroutine = null;

	public void OnEnable()
	{
		ShowMonsterInfo(0);
		updateProgressCoroutine = StartCoroutine(UpdateProgress());
	}
	public void OnDisable()
	{
		if(updateProgressCoroutine != null)
			StopCoroutine(updateProgressCoroutine);
	}
	
	public void ShowMonsterInfo(int monsterNum)
	{
		//몬스터 이미지, 이름, 레벨, 능력치, 고유능력.
		
		
	}
	public IEnumerator UpdateProgress()
	{
		CombatAreaManager cam = CombatAreaManager.Instance;
		while(true)
		{
			//update Progress Info
			progressCurrent.text = (cam.ConqueringHuntingAreaIndex + 1).ToString();
			progressTotal.text = (cam.huntingAreas.Count + cam.bossAreas.Count).ToString();
			currentHuntingAreaKind.text = GameManager.Instance.isBossPhase == true ? commonConqueringText : bossConqueringText;
			//if boss phase...
			remainMonsterCount.text = (cam.huntingAreas[cam.ConqueringHuntingAreaIndex].GetConquerCondition() - cam.huntingAreas[cam.ConqueringHuntingAreaIndex].GetKillCount()).ToString();
			//end
			yield return updateTick;
		}
	}
}
