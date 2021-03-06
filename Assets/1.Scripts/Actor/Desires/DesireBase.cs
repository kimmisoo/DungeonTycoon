using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class DesireBase {
	public const float desireMax = 100.0f;
	public const float desireMin = 0.0f;
	public const float tickMax = 100.0f;
	public const float tickMin = 0.0f;
	
	protected WaitForSeconds tickBetweenWait;


    public float desireValue
	{
		get
		{
			return _desireValue;
		}
		set
		{
			_desireValue = Mathf.Clamp(value, desireMin, desireMax);
		}
	}
    public void SetHighestPriority()
    {
        _desireValue = desireMax;
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
            return _desireName;
        }
	}

	
	protected float _desireValue;
	protected float _tickAmount;
	protected float _tickAmountMult = 1.0f;
	protected float _tickBetween = 1.0f;
	protected DesireType _desireName;
	protected Coroutine tickCoroutine = null;
	protected Traveler owner;
	
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

    public DesireBase(DesireBase input)
    {
        _desireName = input.desireName;
        desireValue = input.desireValue;
        tickAmount = input.tickAmount;
        tickAmountMult = input.tickAmountMult;
        tickBetween = input.tickBetween;
        tickBetweenWait = new WaitForSeconds(tickBetween);
        owner = input.owner;
    }

    public DesireBase(DesireBaseData input)
    {
        _desireName = input.desireName;
        desireValue = input.desireValue;
        tickAmount = input.tickAmount;
        tickAmountMult = input.tickAmountMult;
        tickBetween = input.tickBetween;
        //tickBetweenWait = new WaitForSeconds(tickBetween);
    }
	
	public virtual IEnumerator Tick()
	{
		if (owner is SpecialAdventurer)
			tickAmountMult = 0.05f;
		while(true)
		{
			yield return tickBetweenWait;
			
			if (owner.GetState() != State.UsingStructure)
				desireValue += tickAmount * tickAmountMult;
		}
	}
	public virtual string ToString()
	{
		return desireName.ToString();
	}
	
    public void SetOwner(Traveler owner)
    {
        this.owner = owner;
    }
	public void SetTickCoroutine(Coroutine c)
	{
		tickCoroutine = c;
	}
	public Coroutine GetTickCoroutine()
	{
		return tickCoroutine;
	}
}
