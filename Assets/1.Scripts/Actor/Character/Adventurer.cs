using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Adventurer : Traveler//, IDamagable {
{
    // 전투 스탯
    protected BattleStat battleStat_;
    public BattleStat BattleStat
    {
        get; set;
    }

    protected void EnterState(State nextState)
    {
        switch (nextState)
        {
            case State.Idle:
                Debug.Log("Idle");
                if (structureListByPref == null)
                {
                    //Do something at first move...
                }
                curState = State.SearchingStructure;
                //Traveler이므로 무조건 SearchingStructure 부터
                //이외에 체크할거 있으면 여기서
                break;
            case State.Wandering:
                Debug.Log("Wandering");
                curCoroutine = StartCoroutine(Wandering());
                break;
            case State.SearchingStructure:
                Debug.Log("SS");
                curCoroutine = StartCoroutine(StructureFinding());
                break;
            case State.PathFinding:
                Debug.Log("PF");
                curCoroutine = StartCoroutine(PathFinding());
                break;
            case State.MovingToDestination:
                Debug.Log("MTS");
                curCoroutine = StartCoroutine(MoveToDestination());
                break;
            case State.WaitingStructure:
                Debug.Log("WS");
                destinationStructure.AddWaitTraveler(this);
                break;
            case State.UsingStructure:
                Debug.Log("US");
                //욕구 감소
                //소지 골드 감소
                stat.gold -= destinationStructure.charge;
                stat.GetSpecificDesire(destinationStructure.resolveType).desireValue -= destinationStructure.resolveAmount; // ??
                break;
            case State.Exit:
                Debug.Log("EXIT");
                //Going to outside 
                break;
            case State.None:
                curState = State.Idle;
                break;
        }
    }
    protected void ExitState()
    {
        switch (curState)
        {
            case State.Idle:
                break;
            case State.Wandering:
                break;
            case State.SearchingStructure:
                break;
            case State.PathFinding:
                break;
            case State.MovingToDestination:
                break;
            case State.WaitingStructure:
                break;
            case State.UsingStructure:
                break;
            case State.Exit:
                break;
            case State.None:
                break;
        }
    }
}