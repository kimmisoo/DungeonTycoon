using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActingResult {
	public bool isFoundEnemy = false;
	public bool isReachEnemy = false;
	public bool isDeadEnemy = false;
	public bool isDead = false;

	public void Clear()
	{
		isFoundEnemy = false;
		isReachEnemy = false;
		isDeadEnemy = false;
		
	}
}
