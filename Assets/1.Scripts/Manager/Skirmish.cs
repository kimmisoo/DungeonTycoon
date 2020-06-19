﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skirmish
{
    public List<SpecialAdventurer> skirmishParticipants;
    //public List<SpecialAdventurer> skirmishSurvivors;
    public List<SpecialAdventurer> skirmishLosers;
    public List<SpecialAdventurer> skirmishBracket;
    // 보스 페이즈인가?
    //public bool isBossPhase;
    // 응답한 일선 모험가 수
    //public int responsedSpAdvCnt;
    // 보스 레이드 준비 타이머 끝났나?
    //public bool bossRaidPrepTimeOver;
    // 토너먼트 끝?
    public bool isSkirmishEnded;
    public int roundCnt;
    public int curRound;
    public int matchCntCurRound;
    public int curMatch;

    public Skirmish()
    {
        skirmishParticipants = new List<SpecialAdventurer>();
        //skirmishSurvivors = new List<SpecialAdventurer>();
        skirmishLosers = new List<SpecialAdventurer>();
        skirmishBracket = new List<SpecialAdventurer>();

        roundCnt = 0;
        curRound = 0;
    }

    public void AddSkirmishParticipant(SpecialAdventurer participant)
    {
        skirmishParticipants.Add(participant);
        //skirmishSurvivors.Add(participant);
    }

    public void StartSkirmish()
    {
        MakeBracket();

        int temp = skirmishParticipants.Count;
        while (temp > 1)
        {
            temp = temp / 2 + temp % 2;
            roundCnt++;
        }

        NextRound();
    }

    private void MakeBracket()
    {
        List<SpecialAdventurer> tempList = new List<SpecialAdventurer>(skirmishParticipants);

        while (skirmishBracket.Count != skirmishParticipants.Count)
        {
            int index = Random.Range(0, tempList.Count);

            skirmishBracket.Add(tempList[index]);
            tempList.RemoveAt(index);
        }

        string Temp = "Bracket :";

        foreach (SpecialAdventurer spAdv in skirmishBracket)
        {
            Temp += " ";
            Temp += spAdv.index;
        }

        Debug.Log(Temp);
    }

    //public SpecialAdventurer GetNextSkirmishOpponent(SpecialAdventurer requester)
    //{
    //    int index = skirmishSurvivors.IndexOf(requester);
    //    if (index >= skirmishBracket.Count)
    //        return null;
    //    else
    //        return skirmishBracket[index];
    //}

    // 라운드 진행
    public void NextRound()
    {
        // 현재 라운드에서 진행해야 할 매치 수
        matchCntCurRound = skirmishBracket.Count / 2 + skirmishBracket.Count % 2;
        curMatch = 0;
        curRound++;

        Debug.Log("roundCnt : " + roundCnt + ", curRound : " + curRound + ", matchesCurRound : " + matchCntCurRound);

        // 라운드 다 진행했으면 스커미시 종료
        if (curRound > roundCnt)
        {
            EndSkirmish();
            return;
        }

        for (int i = 0; i < matchCntCurRound; i++)
        {
            if (i * 2 + 1 < skirmishBracket.Count)
            {
                Debug.Log("Match " + i + ", " + skirmishBracket[i * 2].name + " vs " + skirmishBracket[i * 2 + 1].name);
                skirmishBracket[i * 2].enemy = skirmishBracket[i * 2 + 1];
                skirmishBracket[i * 2 + 1].enemy = skirmishBracket[i * 2];

                skirmishBracket[i * 2].HealFullHealth(false);
                skirmishBracket[i * 2 + 1].HealFullHealth(false);

                skirmishBracket[i * 2].curState = State.StartingSkirmish;
                skirmishBracket[i * 2 + 1].curState = State.StartingSkirmish;
            }
            else
            {
                Debug.Log("Match " + i + ", " + skirmishBracket[i * 2].name + " walk over");
                skirmishBracket[i * 2].enemy = null;

                skirmishBracket[i * 2].HealFullHealth(false);

                skirmishBracket[i * 2].curState = State.StartingSkirmish;
            }
        }
    }

    private void EndSkirmish()
    {
        foreach (SpecialAdventurer spAdv in skirmishLosers)
        {
            spAdv.HealFullHealth(true);
            spAdv.curState = State.Idle;
        }
        //Debug.Log("EndSkirmish : " + skirmishSurvivors[0].curState + " , " + skirmishSurvivors[0].GetSuperState());
        skirmishBracket[0].HealFullHealth(true);
        skirmishBracket[0].curState = State.EnteringBossArea;

        // 그 외 인터페이스 보여주려면 여기서.
    }

    /// <summary>
    /// 패배 시 리스트에서 빼줌.
    /// </summary>
    /// <param name="loser"></param>
    public void ReportMatchDefeated(SpecialAdventurer loser)
    {
        //skirmishSurvivors.Remove(loser);
        skirmishLosers.Add(loser);
        skirmishBracket.Remove(loser);
    }

    public void ReportMatchWon(SpecialAdventurer winner)
    {
        curMatch++;
        if (curMatch == matchCntCurRound)
            NextRound();
    }
}
