using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICombatant
{
    Tile GetCurTile();
    TileForMove GetCurTileForMove();
    State GetState();
    void TakeDamage(float damage, float penFixed, float penMult);
    float CurHealth();
}
