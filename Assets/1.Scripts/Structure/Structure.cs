using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Structure : Place
{
    public class TravelerTimer
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
        public TravelerTimer(TravelerTimerData timerData)
        {
            switch(timerData.travelerType)
            {
                case ActorType.Traveler:
                    traveler = GameManager.Instance.travelersEnabled[timerData.travelerIndex].GetComponent<Traveler>();
                    break;
                case ActorType.Adventurer:
                    traveler = GameManager.Instance.adventurersEnabled[timerData.travelerIndex].GetComponent<Adventurer>();
                    break;
                case ActorType.SpecialAdventurer:
                    traveler = GameManager.Instance.specialAdventurers[timerData.travelerIndex].GetComponent<SpecialAdventurer>();
                    break;
            }

            OnUsingStructure = traveler.OnUsingStructure;
            OnExitStructure = traveler.OnExitStructure;
            Tick = new WaitForSeconds(1.0f);
        }
        public IEnumerator UsingStructure(int duration)
        {
            Debug.Log("--------------------Traveler Entered!!!!!!");
            while(elapsedTime < duration)
            {
                yield return Tick;
                elapsedTime++;
            }
            //call ExitTraveler
            OnExitStructure();
            Debug.Log("---------------------Traveler Exit!!!!!!");
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

    //public Traveler[] GetCurUsingQueueAsArray()
    //{
    //    //return curUsingQueue.ToArray();
    //    List<Traveler> travelers = new List<Traveler>();
    //    foreach (TravelerTimer t in curUsingQueue)
    //    {
    //        travelers.Add(t.traveler);
    //    }
    //    return travelers.ToArray();
    //}
    public List<TravelerTimer> GetCurUsingQueueAsList()
    {
        return curUsingQueue.ToList();
    }
    public List<TravelerTimer> GetCurWaitingQueueAsList()
    {
        return curWaitingQueue.ToList();
    }
    /*public float[] GetEnteredTimeQueueAsArray()
    {
		//return EnteredTimeQueue.ToArray();
		List<float> enteredTimeList = new List<float>();

    }*///ElapsedTime을 반환하는 함수로 변경
 //   public float[] GetElapsedTimeQueueAsArray()
	//{
	//	List<float> elapsedTimeList = new List<float>();
	//	foreach(TravelerTimer t in curUsingQueue)
	//	{
	//		elapsedTimeList.Add(t.elapsedTime);
	//	}
	//	return elapsedTimeList.ToArray();
	//}

	//public Traveler[] GetCurWaitingQueueAsArray()
 //   {
	//	List<Traveler> travelers = new List<Traveler>();
	//	foreach (TravelerTimer t in curWaitingQueue)
	//	{
	//		travelers.Add(t.traveler);
	//	}
	//	return travelers.ToArray();
	//}

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
			StartCoroutine(t.UsingStructure(duration));
		}
		Debug.Log(names+"-----------------------Current Using Queue Size = " + curUsingQueue.Count + "/" + capacity);
	}
    // data에 있던 잔여시간을 보고 관광객 입장시킴
    //   public void LoadEnteredTraveler(Traveler t, float elapsedTime)
    //   {
    //	TravelerTimer timer = new TravelerTimer(t, t.OnUsingStructure, t.OnExitStructure);
    //	timer.elapsedTime = (int)elapsedTime;
    //       curUsingQueue.Enqueue(timer);
    //	StartCoroutine(timer.UsingStructure(duration-timer.elapsedTime));
    //}

    

    public override void Visit(Actor visitor)
    {
        AddWaitTraveler(visitor as Traveler); // 대기시간 체크는 Traveler에서 이미 끝났음.
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
		Debug.Log(names+"-----------------------Current Using Queue Size = " + curUsingQueue.Count+"/"+capacity);
		EnterTraveler();
	}
	public float GetWaitSeconds()
	{
		float waitTime = ((curWaitingQueue.Count / capacity)*duration) + duration;
		//					WaitingQueue에서 내 앞에있는 해당 대기자들 + curUsingQueue 최대 대기시간.

		return waitTime;
	}

    #region SaveLoad
    public override PlaceType GetPlaceType()
    {
        return PlaceType.Structure;
    }

    public void LoadEnteredTraveler(TravelerTimerData timerData)
    {
        TravelerTimer timer = new TravelerTimer(timerData);
        timer.OnUsingStructure();
        curUsingQueue.Enqueue(timer);
        StartCoroutine(timer.UsingStructure(duration));
    }

    public void LoadWaitingTraveler(TravelerTimerData timerData)
    {
        curWaitingQueue.Enqueue(new TravelerTimer(timerData));
    }
    #endregion
}


