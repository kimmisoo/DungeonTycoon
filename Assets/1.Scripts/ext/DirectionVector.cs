using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DirectionVector {

	/*DownRight,
	UpRight,
	DownLeft,
	UpLeft,
	None*/
	
	public static Vector2 GetDirectionVector(Direction dir)
	{
		switch(dir)
		{
			case Direction.DownRight:
				return Vector2.right;
				break;
			case Direction.UpRight:
				return Vector2.down;
				break;
			case Direction.DownLeft:
				return Vector2.up;
				break;
			case Direction.UpLeft:
				return Vector2.left;
				break;
			default:
				return Vector2.zero;
				break;
		}
	}
}
