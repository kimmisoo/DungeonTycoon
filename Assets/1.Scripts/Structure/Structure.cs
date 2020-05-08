using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class Structure : Place
{
	public class TravelerTimer : MonoBehaviour
	{
		public Traveler traveler;
		public delegate void NotifyStateChange();
		public NotifyStateChange OnUsingStructure, OnExitStructure;
		WaitForSeconds Tick;
		public int elapsedTime = 0;
		public TravelerTimer(Traveler t, NotifyStateChange onUsing, NotifyStateChange onExit)
		{
			traveler = t;
			OnUsingStructure = onUsing;
			OnExitStructure = onExit;
			Tick = new WaitForSeconds(1.0f);
		}
		public IEnumerator UsingStructure(int duration)
		{
			for(int i = 0; i<duration; i++)
			{
				yield return Tick;
				elapsedTime++;
			}
			//call ExitTraveler
			OnExitStructure();
		}
		public int GetRemainTime(int duration) // queue라 쓸모가 ..?
		{
			return Mathf.Abs(duration - elapsedTime);
		}
		
	}
    public int entCount = 0;
	private bool isConstructable = true; //건설 가능한 위치에 배치 되어있는가 아닌가
    public int sitInCount = 0;

    // 세이브, 로드용
    public string structureCategory;
    public int structureNumber;
    public int structureIndex;
    //

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

	//Queue<Traveler> curUsingQueue = new Queue<Traveler>();  
	//Queue<Traveler> curWaitingQueue = new Queue<Traveler>();
	Queue<TravelerTimer> curUsingQueue = new Queue<TravelerTimer>();
	Queue<TravelerTimer> curWaitingQueue = new Queue<TravelerTimer>();
	
    public Traveler[] GetCurUsingQueueAsArray()
    {
		//return curUsingQueue.ToArray();
		List<Traveler> travelers = new List<Traveler>();
		foreach(TravelerTimer t in curUsingQueue)
		{
			travelers.Add(t.traveler);
		}
		return travelers.ToArray();
    }
	/*public float[] GetEnteredTimeQueueAsArray()
    {
		//return EnteredTimeQueue.ToArray();
		List<float> enteredTimeList = new List<float>();

    }*///ElapsedTime을 반환하는 함수로 변경
	public float[] GetElapsedTimeQueueAsArray()
	{
		List<float> elapsedTimeList = new List<float>();
		foreach(TravelerTimer t in curUsingQueue)
		{
			elapsedTimeList.Add(t.elapsedTime);
		}
		return elapsedTimeList.ToArray();
	}

	public Traveler[] GetCurWaitingQueueAsArray()
    {
		List<Traveler> travelers = new List<Traveler>();
		foreach (TravelerTimer t in curWaitingQueue)
		{
			travelers.Add(t.traveler);
		}
		return travelers.ToArray();
	}

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
	
	public bool GetisConstructable()
	{
		return isConstructable;
	}
	public void SetisConstructable(bool isc)
	{
		isConstructable = isc;
	}
	public void EnterTraveler()
	{
		if (curWaitingQueue.Count > 0)
		{
			TravelerTimer t = curWaitingQueue.Dequeue();
			t.OnUsingStructure();
			curUsingQueue.Enqueue(t);
		}
	}
    // data에 있던 잔여시간을 보고 관광객 입장시킴
    public void LoadEnterdTraveler(Traveler t, float elapsedTime)
    {
		TravelerTimer timer = new TravelerTimer(t, t.OnUsingStructure, t.OnExitStructure);
		timer.elapsedTime = (int)elapsedTime;
        curUsingQueue.Enqueue(timer);
    }

    public override void Visit(Actor visitor)
    {
        AddWaitTraveler(visitor as Traveler);
    }

    public void AddWaitTraveler(Traveler t) // 첫번째로 호출.
	{
		TravelerTimer timer = new TravelerTimer(t, t.OnUsingStructure, t.OnExitStructure);
		curWaitingQueue.Enqueue(timer);
		
		if (curUsingQueue.Count < capacity) // 사용가능한 자리가 있을때
		{
			EnterTraveler();
		}
		else
		{
			//대기 처리
			
		}
	}
	public void ExitTraveler() // Timer에서 시간지나면 자동호출됨.
	{
		//UsingQueue 에서 한명 빠질때 . 
		TravelerTimer t = curUsingQueue.Dequeue();
		EnterTraveler();
	}
	public float GetWaitSeconds()
	{
		float waitTime = ((curWaitingQueue.Count / capacity)*duration) + duration;
		//					WaitingQueue에서 내 앞에있는 해당 대기자들 + curUsingQueue 최대 대기시간.

		return waitTime;
	}
    
}


