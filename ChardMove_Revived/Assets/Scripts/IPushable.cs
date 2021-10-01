using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ChardMove.gameManager;

namespace ChardMove
{
    public interface IPushable
    {
        void Push(MovementDirection direction, float moveSpeed);
        IEnumerator MoveToNextTile(MovementDirection direction, Vector2 target);
        Vector2 TargetTilePosition(MovementDirection direction);
        bool CheckTargetTileType(MovementDirection direction);
        void MoveTowards(Vector2 target);
    }
}
