using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgressInformation
{
    public int huntingAreaIndex;
    public int guestCapacity;
    public int minLevel;
    public int maxLevel;
    public int curAdvNum;

    public ProgressInformation()
    {
        huntingAreaIndex = 0;
        guestCapacity = 0;
        minLevel = 0;
        maxLevel = 0;
        curAdvNum = 0;
    }

    public ProgressInformation(int totalAdvNum) // 연산용 생성자 전체 사냥터에 들어갈 사냥터 수를 넣음.
    {
        this.curAdvNum = totalAdvNum;
    }
}
                //"progressInformation" : [
                //            { "huntingAreaIdx" : 0,
                //              "guestCapacity" : 150,
                //              "minLevel" : 1,
                //              "maxLevel" : 10