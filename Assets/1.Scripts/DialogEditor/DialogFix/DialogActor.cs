using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//나레이션
//
public enum ActorName {Catherine, Maximilian, Iris, Emil, Hana, John, Jangyeonhwa, Murat, Wal, Nyang, Oldman, Player, Narration, Other}
public class DialogActor{

	public string actorName;
	public Sprite portrait;
	public Color nameColor;
	public ActorName enumName;

	public DialogActor()
	{
		portrait = null;
		nameColor = Color.black;

	}
	public DialogActor(string _name, Sprite _portrait, Color _color)
	{
		customName = _name;
		portrait = _portrait;
		nameColor = _color;
	}

	public string GetName()
	{
		switch()
	}

}
