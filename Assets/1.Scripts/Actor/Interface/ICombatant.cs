using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void HealthBelowZeroEventHandler(ICombatant victim, ICombatant killer);
//public delegate void MoveStartedEventHandler(TileForMove newDest);

public interface ICombatant
{
    Tile GetCurTile();
    TileForMove GetCurTileForMove();
    State GetState();
    BattleStat GetBattleStat();
    SuperState GetSuperState();
    bool TakeDamage(ICombatant attacker, float damage, float penFixed, float penMult, bool isCrit, out float actualDamage); // 받은 데미지를 방어력 계산해서 자기자신의 체력에 반영. 비전투시라면 전투상태로 변경.
    IEnumerator DisplayHitEffect(float actualDamage, bool isCrit, bool isEvaded); // 피격 효과 디스플레이
    int RewardGold();
    int RewardExp();
    float CurHealth();
    event HealthBelowZeroEventHandler healthBelowZeroEvent; // 내가 죽을 때 나를 공격하던 적들에게 알림
    //event MoveStartedEventHandler moveStartedEvent; // 움직이기 시작했을 때 나를 타겟으로 지정한 적들에게 알림
    //void AddMoveStartedEventHandler(MoveStartedEventHandler newEvent);
    void HealthBelowZeroNotify(ICombatant victim, ICombatant attacker);
    //void MoveStartedNotify();
    void OnEnemyHealthBelowZero(ICombatant victim, ICombatant attacker); // 적이 죽었을 때 이벤트 처리(보상 획득 및 전투상태 탈출)
    Vector3 GetPosition();
    Transform GetTransform();
    //GameObject GetGameObject();
    ICombatant GetEnemy();
    bool ValidatingEnemy(ICombatant enemy);
    void DisplayHeal(float healAmount);
    void DisplayBuff();
    void DisplayDebuff();
    void RemoveHealthBelowZeroEventHandler(HealthBelowZeroEventHandler healthBelowZeroEventHandler);
    IEnumerator RefreshTemporaryEffects(); // 버프 디버프 체크해서 지속시간 지났으면 해제.
    void ClearTemporaryEffects();
    void RemoveTemporaryEffect(TemporaryEffect toBeRemoved);
    void AddTemporaryEffect(TemporaryEffect toBeAdded);
    void AddSkill(string key);
    void RemoveSkill(string key);
    bool IsInBattle();
    void HealFullHealth(bool displayEffect);
}