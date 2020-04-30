//#define DEBUG_SAVELOAD
//#define DEBUG_GETWAY

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 코드 재사용성 및 가독성을 위해 SuperState로 묶어주었음.
public enum SuperState
{
    None, Idle, Wandering, Battle, AfterBattle, Dead, // 몬스터
    ExitingDungeon, SolvingDesire, SolvingDesire_Wandering, // 트래블러
    EnteringHuntingArea, SearchingMonster, SearchingMonster_Wandering, PassedOut, ExitingHuntingArea // 어드벤처러

}

public enum State
{
    // SuperState의 입구가 되는 State
    None, Idle, AfterBattle, Dead,
    Wandering, SolvingDesire_Wandering, InitiatingBattle, SearchingStructure, SearchingExit, SearchingHuntingArea, SearchingMonster, ExitingHuntingArea, PassedOut, SearchingMonster_Wandering,
    // 그 외. 공용 State 포함.
    PathFinding, MovingToDestination, WaitingStructure, UsingStructure, Battle, Exit,
    ApproachingToEnemy, SearchRescueTeam, Rescued, SpontaneousRecovery
}
/*
 * Animator Tirggers
 * MoveFlg
 * AttackFlg
 * JumpFlg
 * DamageFlg
 * WinFlg
 * DeathFlg
 * SkillFlg
 * DownToUpFlg
 * UpToDownFlg
 * ResurrectingFlg
 */

public abstract class Actor : MonoBehaviour
{

    [SerializeField]
    protected SuperState superState;
    public State state;
    /*public string actorName { get; set; }
	public string explanation { get; set; }
	public int gold { get; set; }*/
    public PathFinder pathFinder;
    protected List<TileForMove> wayForMove;
    protected Direction direction;
    protected Animator animator;
    protected SpriteRenderer[] spriteRenderers;

    protected Tile curTile;
    public TileForMove curTileForMove;
    protected TileLayer tileLayer;
    protected Tile destinationTile;
    protected TileForMove destinationTileForMove;


    protected void Awake()
    {
        animator = GetComponent<Animator>();
        spriteRenderers = GetComponentsInChildren<SpriteRenderer>(true);
        pathFinder = GetComponent<PathFinder>();
        wayForMove = new List<TileForMove>();
        state = new State();
        direction = new Direction();
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
                if (tileForMoveTemp.Equals(tileForMoveTemp.GetRecentActor().GetCurTileForMove())) // tileForMoveTemp에 기록된 recentActor의 현재위치가 tileForMoveTemp와 일치하는지
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

    public void SetCurTile(Tile _tile)
    {
        curTile = _tile;
#if DEBUG_GETWAY
        if (curTile == null)
            Debug.Log("NULL!");
#endif
        pathFinder.SetCurTile(_tile);
    }
    public Tile GetCurTile()
    {
        return curTile;//pathFinder.GetCurTile();
    }
    public void SetCurTileForMove(TileForMove _tileForMove)
    {
        curTileForMove = _tileForMove;

    }
    public TileForMove GetCurTileForMove()
    {
        return curTileForMove;//pathFinder.GetCurTileForMove();
    }
    public int GetDistanceFromOtherActorForMove(Actor actor)
    {
        if (actor != null)
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

        if (absX == 0 && absY == 0)
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
        else if (distanceX >= 0 && distanceY < 0) //Right
        {
            if (absX > absY)
                direction = Direction.DownRight;
            else
                direction = Direction.UpRight;
        }
        else if (distanceX < 0 && distanceY >= 0) //Left
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
    public SuperState GetSuperState()
    {
        return superState;
    }
    public void SetSuperState(SuperState input)
    {
        superState = input;
    }
    public void SetDirection(Direction dir)
    {
        direction = dir;
    }
    public Direction GetDirection()
    {
        return direction;
    }

    protected virtual List<TileForMove> GetWay(List<PathVertex> path) // Pathvertex -> TileForMove
    {
        //Debug.Log("path.Count : " + path.Count);
        // 같은 칸 내에서의 이동
        if (path.Count == 1)
        {
            List<TileForMove> result = new List<TileForMove>();
            result.Add(curTileForMove);
            return result;
        }

        List<TileForMove> tileForMoveWay = new List<TileForMove>();

        // 현재 TileForMove, 다음 TileForMove 선언
        TileForMove next, cur;
        int count = 1;
        cur = curTileForMove;
        //Debug.Log("Start : " + cur.GetParent().GetX() + " , " + cur.GetParent().GetY() + ", dest = " + destinationTile.GetX() + " , " + destinationTile.GetY());

        // 다음 타일로의 방향 벡터
        Direction dir = curTile.GetDirectionFromOtherTile(path[count].myTilePos);
        Vector2 dirVector = DirectionVector.GetDirectionVector(dir);

        tileForMoveWay.Add(curTileForMove);
#if DEBUG_GETWAY
        // 디버깅용?
        Debug.Log(dir.ToString());
#endif
        string pathString = "";

        for (int i = 0; i < path.Count; i++)
        {
            pathString += path[i].myTilePos.GetX() + " , " + path[i].myTilePos.GetY() + "\n";
            //Debug.Log("path = " + path[i].myTilePos.GetX() + " , " + path[i].myTilePos.GetY());
        }
#if DEBUG_GETWAY
        Debug.Log(pathString);
        Debug.Log("progress : " + cur.GetX() + "(" + cur.GetParent().GetX() + ")" + " , " + cur.GetY() + "(" + cur.GetParent().GetY() + ")"); //19 49
#endif


        while (!(path[count].myTilePos.Equals(destinationTile)))
        {
            next = tileLayer.GetTileForMove(cur.GetX() + (int)dirVector.x, cur.GetY() + (int)dirVector.y);
            //Debug.Log("progress : " + next.GetX() + "(" + next.GetParent().GetX() + ")" + " , " + next.GetY() + "(" + next.GetParent().GetY() + ")");
            tileForMoveWay.Add(next);
            if (cur.GetParent().Equals(next.GetParent())) //한칸 진행했는데도 같은 타일일때
            {
                //Debug.Log("SameTile..");
                next = tileLayer.GetTileForMove(next.GetX() + (int)dirVector.x, next.GetY() + (int)dirVector.y);
                //Debug.Log("progress : " + next.GetX() + "(" + next.GetParent().GetX() + ")" + " , " + next.GetY() + "(" + next.GetParent().GetY() + ")");
                tileForMoveWay.Add(next);
            }
            cur = next;

            if (Random.Range(0, 2) >= 1 && destinationTileForMove != cur)
            {

                next = tileLayer.GetTileForMove(cur.GetX() + (int)dirVector.x, cur.GetY() + (int)dirVector.y);
                if (next == null)
                    continue;
                //Debug.Log("progress : " + next.GetX() + "(" + next.GetParent().GetX() + ")" + " , " + next.GetY() + "(" + next.GetParent().GetY() + ")");
                tileForMoveWay.Add(next);
                cur = next;
            }
            count++;
            dir = cur.GetParent().GetDirectionFromOtherTile(path[count].myTilePos);
            dirVector = DirectionVector.GetDirectionVector(dir);
            //Debug.Log(dir.ToString());
        }
        return tileForMoveWay;
    }
    // dX = 1 : UR
    // dX = -1: DL
    // dY = 1 : DR
    // dY = -1: UL
    protected List<TileForMove> GetWayTileForMove(List<PathVertex> path, TileForMove destTileForMove)
    {
        List<TileForMove> moveWayTFM = GetWay(path);
        TileLayer tileLayer = GameManager.Instance.GetTileLayer().GetComponent<TileLayer>();

        TileForMove lastTFM = moveWayTFM[moveWayTFM.Count - 1];

        int xDiff = destTileForMove.GetX() - lastTFM.GetX();
        int yDiff = destTileForMove.GetY() - lastTFM.GetY();
        int xSeq, ySeq;

        if (xDiff == 0)
            xSeq = 0;
        else
            xSeq = xDiff / Mathf.Abs(xDiff);

        if (yDiff == 0)
            ySeq = 0;
        else
            ySeq = yDiff / Mathf.Abs(yDiff);

#if DEBUG_GETWAY
        Debug.Log("원래 끝 TFM : [" + lastTFM.GetX() + ", " + lastTFM.GetY());
        Debug.Log("xDiff : " + xDiff + ", xSeq : " + xSeq);
        Debug.Log("xDiff : " + yDiff + ", xSeq : " + ySeq);
#endif

        for (int i = 0; i != xDiff; i += xSeq)
        {
            moveWayTFM.Add(tileLayer.GetTileForMove(lastTFM.GetX() + xSeq + i, lastTFM.GetY()));
        }

        lastTFM = moveWayTFM[moveWayTFM.Count - 1];

        for (int i = 0; i != yDiff; i += ySeq)
        {
            moveWayTFM.Add(tileLayer.GetTileForMove(lastTFM.GetX(), lastTFM.GetY() + ySeq + i));
        }

#if DEBUG_GETWAY
        Debug.Log("끝 타일1 : " + moveWayTFM[moveWayTFM.Count - 1].GetParent().GetX() + ", " + moveWayTFM[moveWayTFM.Count - 1].GetParent().GetY());
        Debug.Log("끝 TFM1 : " + moveWayTFM[moveWayTFM.Count - 1].GetX() + ", " + moveWayTFM[moveWayTFM.Count - 1].GetY());
        Debug.Log("시작지 TFM : " + curTileForMove.GetX() + ", " + curTileForMove.GetY());

        for (int i = 0; i < moveWayTFM.Count; i++)
        {
            if (i == 0)
                Debug.Log("출발");
            Debug.Log("[" + moveWayTFM[i].GetX() + ", " + moveWayTFM[i].GetY());
            if (i == moveWayTFM.Count - 1)
                Debug.Log("끝");
        }

        Debug.Log("끝 타일2 : " + destTileForMove.GetParent().GetX() + ", " + destTileForMove.GetParent().GetY());
        Debug.Log("끝 TFM2 : " + destTileForMove.GetX() + ", " + destTileForMove.GetY());
#endif
        return moveWayTFM;
    }

    protected IEnumerator MoveAnimation(List<TileForMove> tileForMoveWay)
    {
        yield return null;

        Direction dir = Direction.DownLeft;
        //FlipX true == Left, false == Right
        Vector3 dirVector;
        float distance, sum = 0.0f;

        animator.SetBool("MoveFlg", true);
        // GewWay에서 받은 경로대로 이동
        for (int i = 0; i < tileForMoveWay.Count - 1; i++)
        {
            tileForMoveWay[i].SetRecentActor(this);
            SetCurTile(tileForMoveWay[i].GetParent());
            SetCurTileForMove(tileForMoveWay[i]);
#if DEBUG_GETWAY
            Debug.Log("curTileForMove : [" + curTileForMove.GetX() + ", " + curTileForMove.GetY() + "]");
#endif

            // 방향에 따른 애니메이션 설정.
            SetAnimDirection(tileForMoveWay[i].GetDirectionFromOtherTileForMove(tileForMoveWay[i + 1]));
            
            //transform.position = tileForMoveWay[i].GetPosition();
            // 이동
            dirVector = tileForMoveWay[i + 1].GetPosition() - tileForMoveWay[i].GetPosition();
            distance = Vector3.Distance(tileForMoveWay[i].GetPosition(), tileForMoveWay[i + 1].GetPosition());
            while (Vector3.Distance(transform.position, tileForMoveWay[i].GetPosition()) < distance)
            {
                yield return null;
                transform.Translate(dirVector * Time.deltaTime);

            }
            sum = 0.0f;
            transform.position = tileForMoveWay[i + 1].GetPosition();
        }

        SetCurTile(tileForMoveWay[tileForMoveWay.Count - 1].GetParent());
        SetCurTileForMove(tileForMoveWay[tileForMoveWay.Count - 1]);
#if DEBUG_GETWAY
        Debug.Log("last curTileForMove : [" + curTileForMove.GetX() + ", " + curTileForMove.GetY() + "]");
#endif
        // 한칸 덜감
        animator.SetBool("MoveFlg", false);
    } // Adventurer에서 이동 중 피격 구현해야함. // Notify?

    /// <summary>
    /// 방향에 따른 애니메이션 설정.
    /// </summary>
    /// <param name="dir">방향. 현재타일.GetDirectionFromOtherTile(바라볼 방향의 타일)</param>
    protected void SetAnimDirection(Direction dir)
    {
        switch (dir)
        {
            case Direction.DownRight:
                animator.SetTrigger("UpToDownFlg");
                foreach (SpriteRenderer sr in spriteRenderers)
                {
                    sr.flipX = true;
                }
                break;
            case Direction.UpRight:

                animator.SetTrigger("DownToUpFlg");
                foreach (SpriteRenderer sr in spriteRenderers)
                {
                    sr.flipX = true;
                }
                break;
            case Direction.DownLeft:

                animator.SetTrigger("UpToDownFlg");
                foreach (SpriteRenderer sr in spriteRenderers)
                {
                    sr.flipX = false;
                }
                break;
            case Direction.UpLeft:

                animator.SetTrigger("DownToUpFlg");
                foreach (SpriteRenderer sr in spriteRenderers)
                {
                    sr.flipX = false;
                }
                break;
            default:
                break;
        }
    }
    /// <summary>
    /// CurTileForMove에 맞춰 transform.position 조정.
    /// </summary>
    public void AlignPositionToCurTileForMove()
    {
        transform.position = curTileForMove.GetPosition();
    }

    public abstract bool ValidateNextTile(Tile tile);
    public abstract void SetPathFindEvent();

#region Save Load
    public int GetDestinationTileSave()
    {
        if (destinationTile != null)
            return int.Parse(destinationTile.name);
        else
            return -1;
    }

    public bool SetDestinationTileLoad(int tileNum)
    {
        if (tileNum == -1)
            return false;

        GameObject tileLayer = GameManager.Instance.GetTileLayer();
        destinationTile = tileLayer.transform.GetChild(tileNum).gameObject.GetComponent<Tile>();
#if DEBUG_SAVELOAD
        Debug.Log("[Actor.SetDestinationTileLoad] : [" + destinationTile.GetX() + ", " + destinationTile.GetY() + "]");
#endif

        if (destinationTile == null)
            return false;
        else
            return true;
    }

    public int GetCurTileSave()
    {
        if (curTile != null)
            return int.Parse(curTile.name);
        else
            return -1;
    }

    public bool SetCurTileLoad(int tileNum)
    {
        if (tileNum == -1)
            return false;

        GameObject tileLayer = GameManager.Instance.GetTileLayer();
        if (tileLayer == null)
            Debug.Log("타일 레이어 없음");
        else
            Debug.Log("타일레이어 존재!");
        Debug.Log("Child 수 : " + tileLayer.transform.childCount);
        curTile = tileLayer.transform.GetChild(tileNum).gameObject.GetComponent<Tile>();

        if (curTile == null)
        {
            Debug.Log("curTile = NULL");
            return false;
        }
        else
        {
            Debug.Log("curTile 제대로 들어감");
            return true;
        }
    }

    public bool SetCurTileForMoveLoad(int childNum)
    {
        if (childNum == -1)
            return false;

        SetCurTileForMove(curTile.GetChild(childNum));
        return true;
    }
#endregion

    protected int DistanceBetween(TileForMove pos1, TileForMove pos2)
    {
        return Mathf.Abs(pos1.GetX() - pos2.GetX()) + Mathf.Abs(pos1.GetY() - pos2.GetY());
    }
}