using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class SpecPanel : UIObject {


	public Text nameText;
	public Text goldText;
	public Text explanation;
	public Text job;
	public Text race;
	public Text desireText;
	public SpriteRenderer spriteRenderer;
	public Sprite portrait;
	public Image image;
	public Text state;
	
	protected IEnumerator UpdateActorState()
	{
		yield return null;
		
	}
	
	public void OnChangeSelected()
	{
		//Hide
	}
	public void OnChangeSelected(Traveler t)
	{

	}
	public void OnChangeSelected(Adventurer adv)
	{

	}
	public void OnChangeSelected(SpecialAdventurer sAdv)
	{

	}
	public void OnChangeSelected(Monster mon)
	{

	}
	
}
