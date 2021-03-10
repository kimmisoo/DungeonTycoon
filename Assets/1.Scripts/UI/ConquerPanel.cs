using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConquerPanel : UIObject {

	#region StageProgress
	public Text progressCurrent;
	public Text progressTotal;
	public Text currentHuntingAreaKind;
	public Text remainMonsterCount;
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
}
