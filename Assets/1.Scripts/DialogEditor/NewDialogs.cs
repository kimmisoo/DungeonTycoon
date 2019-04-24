using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class NewDialogs : MonoBehaviour, ISelectHandler {
	public DialogEditor de;
	public void Start()
	{
		transform.SetSiblingIndex(100000);
	}
	public void OnSelect(BaseEventData baseEventData)
	{
		de.SelectNewDialog();
	}

}
