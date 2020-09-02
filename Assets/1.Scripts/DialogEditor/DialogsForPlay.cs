using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
/*Onselected ; 
Delete*/ //2018-03-10


public class DialogsForPlay
{
	int index;
	public DialogEditor de;
	Selectable se;
	public string fullText;
	public int representImageLNum = 0;
	public int representImageRNum = 0;

	public Dictionary<int, List<int>> effects = new Dictionary<int, List<int>>();
	public Dictionary<int, List<int>> illustrationsL = new Dictionary<int, List<int>>();
	public Dictionary<int, List<int>> illustrationsR = new Dictionary<int, List<int>>();
	public Dictionary<int, List<string>> names = new Dictionary<int, List<string>>();

	public Dictionary<int, int> smallIllustrations = new Dictionary<int, int>();
	public Dictionary<int, int> bigIllustrations = new Dictionary<int, int>();
	public Dictionary<int, int> backgrounds = new Dictionary<int, int>();
	public int type;
	public List<Markers> markersList = new List<Markers>();
	public List<Vector3> markerPositionList = new List<Vector3>();
	public Markers selected;
	public GameObject effectObjectOrigin;
	public GameObject effectListContent;
	public GameObject smallMarkerOrigin;
	public bool isRecorded = false;
	public bool isStart = false;

	
	public void SetIndex(int i)
	{
		index = i;

	}
	public void SetDialogEditor(DialogEditor d)
	{
		de = d;
	}
	public int GetIndex()
	{
		return index;
	}

	public string GetFullText()
	{
		return fullText;
	}
	public void SetFullText(string _t, bool isEditor)
	{
		if (isEditor == false)
			fullText = _t;
	}

	public void AddBackground(int key, int bgNum)
	{
		backgrounds[key] = bgNum;
	}
	public void AddSmallIllustration(int key, int illustNum)
	{
		smallIllustrations[key] = illustNum;
	}

	public void AddBigIllustration(int key, int illustNum)
	{
		bigIllustrations[key] = illustNum;
	}
	public void AddEffect(int key, int effectNum)
	{
		if (effects.ContainsKey(key))
		{
			effects[key].Add(effectNum);
		}
		else
		{
			effects[key] = new List<int>();
			effects[key].Add(effectNum);
		}
	}

	public void AddIllustrationL(int key, int illustrationNum)
	{
		if (illustrationsL.ContainsKey(key))
		{
			illustrationsL[key].Add(illustrationNum);
		}
		else
		{
			illustrationsL[key] = new List<int>();
			illustrationsL[key].Add(illustrationNum);
		}
	}
	
	public void AddIllustrationR(int key, int illustrationNum)
	{
		if (illustrationsR.ContainsKey(key))
		{
			illustrationsR[key].Add(illustrationNum);
		}
		else
		{
			illustrationsR[key] = new List<int>();
			illustrationsR[key].Add(illustrationNum);
		}
	}
	
	public void AddName(int key, string name)
	{
		if (names.ContainsKey(key))
		{
			names[key].Add(name);
		}
		else
		{
			names[key] = new List<string>();
			names[key].Add(name);
		}
	}
	
	public void SetDialogType(int tp)
	{
		type = tp;
	}

	public void SetIsRecorded()
	{
		isRecorded = true;
	}
}