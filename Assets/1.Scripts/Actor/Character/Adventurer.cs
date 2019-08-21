using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Adventurer : Actor, IDamagable {

	BattleStatus battleStatus;
	//StarBattle Method 필요함!
	public void SetBattleStatus(BattleStatus bStatus)
	{
		battleStatus = bStatus;
	}
	public BattleStatus GetBattleStatus()
	{
		return battleStatus;
	}
}
