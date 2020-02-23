using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class DesireBase {
	public const float desireMax = 100.0f;
	public const float desireMin = 0.0f;
	public const float tickMax = 100.0f;
	public const float tickMin = 0.001f;
    public const float DesireHighestPriority = 200.0f;
	
	protected WaitForSeconds tickBetweenWait;
	public float desireValue
	{
		get
		{
			return _desireValue;
		}
		set
		{
			if (value > desireMin && value < desireMax)
				_desireValue = value;
		}
	}
    public void SetHighestPriority()
    {
        _desireValue = DesireHighestPriority;
    }
	public float tickAmount
	{
		get
		{
			return _tickAmount;
		}
		set
		{
			if (value > tickMin && value < tickMax)
				_tickAmount = value;
		}
	}
	public float tickAmountMult
	{
		get
		{
			return _tickAmountMult;
		}
		set
		{
			if (value > tickMin && value < tickMax)
				_tickAmountMult = value;
		}
	}
	public float tickBetween
	{
		get
		{
			return _tickBetween;
		}
		set
		{
			if (value > 0.01f && value < 10.0f)
			{
				_tickBetween = value;
				tickBetweenWait = new WaitForSeconds(value);
			}
			
		}
	}
	public DesireType desireName
	{
		get
		{
			return desireName;
		}
	}
	protected float _desireValue;
	protected float _tickAmount;
	protected float _tickAmountMult = 1.0f;
	protected float _tickBetween = 1.0f;
	protected DesireType _desireName;
	protected Coroutine tickCoroutine;
	protected Traveler owner;
	/*
	public void Init(DesireType name, float initDesireValue, float initTickAmount, float initTickMult, float initTickBetween, Traveler _owner)
	{
		_desireName = name;
		desireValue = initDesireValue;
		tickAmount = initTickAmount;
		tickAmountMult = initTickMult;
		tickBetween = initTickBetween;
		tickBetweenWait = new WaitForSeconds(tickBetween);
		owner = _owner;
	}*/
	public DesireBase(DesireType name, float initDesireValue, float initTickAmount, float initTickMult, float initTickBetween, Traveler _owner)
	{
		_desireName = name;
		desireValue = initDesireValue;
		tickAmount = initTickAmount;
		tickAmountMult = initTickMult;
		tickBetween = initTickBetween;
		tickBetweenWait = new WaitForSeconds(tickBetween);
		owner = _owner;
	}
	
	public virtual IEnumerator Tick()
	{
		while(true)
		{
			yield return tickBetweenWait;
			if(owner.GetState() != State.UsingStructure)
				desireValue += tickAmount * tickAmountMult;
		}
	}
	public virtual string ToString()
	{
		return desireName.ToString();
	}
	
}
