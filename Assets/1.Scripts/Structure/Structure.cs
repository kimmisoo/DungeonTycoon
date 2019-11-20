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
	Queue<Traveler> curTravelerQueue = new Queue<Traveler>();
	Queue<Traveler> curWaitingQueue = new Queue<Traveler>();
	Queue<Coroutine> curWaitingCoroutine = new Queue<Coroutine>();
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
    
	void Start()
	{
		
	}
	void Update()
	{

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
	public bool EnterTraveler(Traveler t)
	{
		if(curTravelerQueue.Count >= capacity)
		{
			return false;
		}
		curTravelerQueue.Enqueue(t);
		Invoke("ExitTraveler", duration);
		return true;
	}
	public void AddWaitTraveler(Traveler t, Coroutine waitCoroutine)
	{
		curWaitingQueue.Enqueue(t);
		curWaitingCoroutine.Enqueue(waitCoroutine);
	}
	public void ExitTraveler()
	{

		Traveler exitTraveler = curTravelerQueue.Dequeue();
		if(curWaitingCoroutine.Count > 0 && curWaitingQueue.Count > 0)
		{
			StopCoroutine(curWaitingCoroutine.Dequeue());
			EnterTraveler(curWaitingQueue.Dequeue());
		}

	}
	public float GetWaitSeconds()
	{
		if (curTravelerQueue.Count < capacity)
			return 0.0f;
		else
		{
			return curWaitingQueue.Count * duration;
		}
	}
    
}


