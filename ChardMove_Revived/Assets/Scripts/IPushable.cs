using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ChardMove.gameManager;

namespace ChardMove
{
    public interface IPushable
    {
        void Push(MovementDirection direction, float moveSpeed);
    }
}
