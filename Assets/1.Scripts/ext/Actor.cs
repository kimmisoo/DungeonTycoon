using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum State
{
	Idle, Chasing, Moving, Battle, Indoor, Dead
}
public abstract class Actor : MonoBehaviour {

	//이름
	//설명
	//가진금액
	//이동메소드
	//목적지
	//현재상태
	
	public State state;
	public string actorName { get; set; }
	public string explanation { get; set; }
	public int gold { get; set; }
	public PathFinder pathFinder;
	public List<TileForMove> wayForMove = new List<TileForMove>();
	protected Direction direction;

	public Animator animator;
	public SpriteRenderer spriteRenderer;


	public List<Enchantment> enchantmentList = new List<Enchantment>();
	public List<EquipmentEffect> equipmentEffectList = new List<EquipmentEffect>();
	public List<Item> itemList = new List<Item>();
	public TileLayer tileLayer;
	
	
	public virtual void Awake()
	{
		animator = GetComponent<Animator>();
		spriteRenderer = GetComponent<SpriteRenderer>();
		
	}


	/*public abstract void Die(Actor Opponent);
	public abstract void StartBattle(Actor opponent);
	public abstract void EndBattle(Actor opponent);

	public abstract void AddEnchantment(Enchantment enchantment);
	public abstract void RemoveEnchantment(Enchantment enchantment);
	public void AddEquipmentEffect(EquipmentEffect equipmentEffect)
	{
		equipmentEffectList.Add(equipmentEffect);
	}
	public void RemoveEquipmentEffect(EquipmentEffect equipmentEffect)
	{
		equipmentEffectList.Remove(equipmentEffect);
	}
	public void RemoveAllEquipmentEffectByParent(IHasEquipmentEffect parent)
	{
		int i = 0;
		while(i<equipmentEffectList.Count)
		{
			if (equipmentEffectList[i].GetParent().Equals(parent))
			{
				equipmentEffectList.RemoveAt(i--);
			}
			i++;
		}
	}
	public abstract void TakeStunned(Actor from, Enchantment enchantment, float during);
	*/
	public int GetGold()
	{
		return gold;
	}
	public void SetGold(int _gold)
	{
		gold = _gold;
	}
	
	
	public Actor[] GetAdjacentActor(int range)
	{
		List<Actor> adjacentActors = new List<Actor>();
		TileLayer layer = pathFinder.tileLayer;
		int x = GetCurTileForMove().GetX();
		int y = GetCurTileForMove().GetY();
		TileForMove tileForMoveTemp;
		for (int i = -range; i <= range; i++)
		{
			for (int j = -range; j <= range; j++)
			{
				if (Mathf.Abs(i) + Mathf.Abs(j) > range) // + 실제 actor가 타일 위에 있는지
					continue;
				tileForMoveTemp = layer.GetTileForMove(x + i, y + j);
				if(tileForMoveTemp.Equals(tileForMoveTemp.GetRecentActor().pathFinder.GetCurTileForMove())) // tileForMoveTemp에 기록된 recentActor의 현재위치가 tileForMoveTemp와 일치하는지
				{
					adjacentActors.Add(layer.GetTileForMove(x + i, y + j).GetRecentActor());
				}
				else
				{
					tileForMoveTemp.SetRecentActor(null);
				}
			}
		}
		return adjacentActors.ToArray();
	}
	public EquipmentEffect[] GetSameCategoryEffects(int category)
	{
		List<EquipmentEffect> tempList = new List<EquipmentEffect>();
		foreach(EquipmentEffect e in equipmentEffectList)
		{
			if (e.category != -1)
				tempList.Add(e);
		}
		return tempList.ToArray();
	}
	
	public void SetCurTile(Tile _tile)
	{
		pathFinder.SetCurTile(_tile);
	}
	public Tile GetCurTile()
	{
		return pathFinder.GetCurTile();
	}
	public void SetCurTileForMove(TileForMove _tileForMove)
	{ 
		pathFinder.SetCurTileForMove(_tileForMove);
	}
	public TileForMove GetCurTileForMove()
	{
		return pathFinder.GetCurTileForMove();
	}
	public int GetDistanceFromOtherActorForMove(Actor actor)
	{
		if(actor != null)
		{
			return Mathf.Abs(actor.GetCurTileForMove().GetX() - GetCurTileForMove().GetX()) + Mathf.Abs(actor.GetCurTileForMove().GetY() - GetCurTileForMove().GetY()); 
		}
		else
		{
			return int.MaxValue;
		}
	}
	
	public Direction GetDirectionFromOtherTileForMove(TileForMove tileForMove)
	{
		Direction direction = Direction.DownLeft;
		int distanceX = GetCurTileForMove().GetX() - tileForMove.GetX();
		int distanceY = GetCurTileForMove().GetY() - tileForMove.GetY();
		int absX = Mathf.Abs(distanceX);
		int absY = Mathf.Abs(distanceY);

		if(absX == 0 && absY == 0)
		{
			direction = Direction.None; // 겹침
			return direction;
		}
		if (distanceX >= 0 && distanceY >= 0)
		{
			if (absX > absY)
				direction = Direction.DownRight;
			else
				direction = Direction.DownLeft;
			
		}
		else if (distanceX >=0 && distanceY < 0) //Right
		{
			if (absX > absY)
				direction = Direction.DownRight;
			else
				direction = Direction.UpRight;
		}
		else if(distanceX < 0 && distanceY >= 0) //Left
		{
			if (absX > absY)
				direction = Direction.UpLeft;
			else
				direction = Direction.DownLeft;
		}
		else
		{
			if (absX > absY)
				direction = Direction.UpLeft;
			else
				direction = Direction.UpRight;
		}
		return direction;
	}
	public TileForMove GetNextTileForMoveFromDirection(Direction direction)
	{
		//TileForMove tempTileForMove;
		switch (direction)
		{
			case Direction.DownRight:
				return tileLayer.GetTileForMove(GetCurTile().GetX() + 1, GetCurTile().GetY());
				break;
			case Direction.UpRight:
				return tileLayer.GetTileForMove(GetCurTile().GetX(), GetCurTile().GetY() - 1);
				break;
			case Direction.DownLeft:
				return tileLayer.GetTileForMove(GetCurTile().GetX() + 1, GetCurTile().GetY());
				break;
			case Direction.UpLeft:
				return tileLayer.GetTileForMove(GetCurTile().GetX() - 1, GetCurTile().GetY());
				break;
			case Direction.None:
				return GetCurTileForMove();
				break;
			default:
				return GetCurTileForMove();
				break;
		}
	}
	public State GetState()
	{
		return state;
	}
	public void SetDirection(Direction dir)
	{
		direction = dir;
	}
	public Direction GetDirection()
	{
		return direction;
	}


}


