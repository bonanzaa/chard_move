using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ChardMove.gameManager;

namespace ChardMove
{
    public class Tile : MonoBehaviour
    {
        public TileType TileType = TileType.Walkable;
        private void Awake() {
            GameManager.resetButtonPressed += OnResetButtonPressed;
        }
        public virtual void Start() {
            GameManager.Instance.TileDB.Add(new Vector2(transform.position.x,transform.position.y),this);
        }

        private void OnResetButtonPressed(){
            GameManager.Instance.TileDB.Remove(new Vector2(transform.position.x,transform.position.y));
            GameManager.Instance.TileDB.Add(new Vector2(transform.position.x,transform.position.y),this);
        }

    }
}
