using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ChardMove.gameManager;

namespace ChardMove
{
    public class Tile : MonoBehaviour
    {
        public TileType TileType = TileType.Walkable;
        public virtual void Start() {
            GameManager.Instance.TileDB.Add(new Vector2(transform.position.x,transform.position.y),this);
        }
    }
}
