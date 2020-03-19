using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void HealthBelowZeroEventHandler(int victimIndex, int killerIndex);

public interface ICombatant
{
    Tile GetCurTile();
    TileForMove GetCurTileForMove();
    State GetState();
    SuperState GetSuperState();
    void TakeDamage(int attackerIndex, float damage, float penFixed, float penMult, bool isCrit);
    IEnumerator DisplayHitEffect(float actualDamage, bool isCrit, bool isEvaded);
    int RewardGold();
    int RewardExp();
    float CurHealth();
    event HealthBelowZeroEventHandler healthBelowZeroEvent;
    void HealthBelowZeroNotify(int victimIndex, int attackerIndex);
}