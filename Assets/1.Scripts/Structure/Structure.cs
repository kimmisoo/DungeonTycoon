using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class Structure : MonoBehaviour {
	public Tile point //extent 기준 0,0의 타일
	{
		get; set;
	}
    public int entCount = 0;
	private bool isConstructable = true; //건설 가능한 위치에 배치 되어있는가 아닌가
    public int sitInCount = 0;
	public bool isEnterable
	{
		get; set;
	}
	public string names
	{
		get; set;
	}
	public string path
	{
		get; set;
	}
	public string type
	{
		get; set;
	}
	public DesireType resolveType
	{
		get; set;
	}
	public int capacity
	{
		get; set;
	}
	public int duration
	{
		get; set;
	}
	public int charge
	{
		get; set;
	}

    
	public float resolveAmount = 0.0f;
    public Preference preference = new Preference();
	
    

    public List<Tile> entrance = new List<Tile>();
	Queue<Traveler> curUsingQueue = new Queue<Traveler>();
	Queue<Traveler> curWaitingQueue = new Queue<Traveler>();
	
    public void addEntrance(Tile t)
    {
        entrance.Add(t);
    } // 활성화된 입구만 담겨있음
	public Tile GetEntrance()
	{
		int randNum = Random.Range(0, entrance.Count);
		return entrance[randNum];
	}
	public int extentWidth
	{
		get; set;
	}
	public int extentHeight
	{
		get; set;
	}
	public int[,] extent;
	public string genre
	{
		get; set;
	}
	public int bonus
	{
		get; set;
	}
	public int expenses
	{
		get; set;
	}
    
	protected bool isMovable = false;


	public void StartMove()
	{
		isMovable = true;
		StartMoveEffect();
	}
	public void EndMove()
	{
		isMovable = false;
		GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1);
	}

	void StartMoveEffect()
	{
		StartCoroutine(_StartMoveEffect());
	}

	IEnumerator _StartMoveEffect()
	{
		SpriteRenderer sp = GetComponent<SpriteRenderer>();
		float factor = 0.1f;
        while (isMovable == true)
		{
			yield return new WaitForSeconds(0.1f);
			
			
			if ( sp.color.a < 0.3f)
				factor = 0.1f;
			else if (sp.color.a >= 1.0f)
				factor = -0.1f;
			sp.color = new Color(sp.color.r, sp.color.g, sp.color.b, sp.color.a + factor);
		}
	}

	public bool GetMovable()
	{
		return isMovable;
	}
	public int[,] GetExtent()
	{
		return extent;
	}
	
	public bool GetisConstructable()
	{
		return isConstructable;
	}
	public void SetisConstructable(bool isc)
	{
		isConstructable = isc;
	}
	public void EnterTraveler(Traveler t)
	{
		curUsingQueue.Enqueue(t);
		Invoke("ExitTraveler", duration);
	}
	public void AddWaitTraveler(Traveler t) // 첫번째로 호출.
	{
		curWaitingQueue.Enqueue(t);
		t.curState = State.WaitingStructure;
		if (curUsingQueue.Count < capacity)
		{
			EnterTraveler(curWaitingQueue.Dequeue());
			t.curState = State.UsingStructure;
		}
		else
		{
			//대기 코루틴?
		}
	}
	public void ExitTraveler()
	{
		Traveler exitTraveler = curUsingQueue.Dequeue();
		exitTraveler.curState = State.Idle;
		if(curWaitingQueue.Count > 0) // 대기열에 사람이 있다면
		{
			EnterTraveler(curWaitingQueue.Dequeue());
		}

	}
	public float GetWaitSeconds()
	{
		if (curUsingQueue.Count < capacity)
			return 0.0f;
		else
		{
			return (curWaitingQueue.Count) * duration;
		}
	}
    
}


