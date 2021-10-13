using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ChardMove.gameManager;

namespace ChardMove
{
    public class Tile : MonoBehaviour
    {
        public TileType TileType = TileType.Walkable;
        private GameObject _highlight;
        private void Awake() {
            GameManager.resetButtonPressed += OnResetButtonPressed;
        }
        public void Highlight(){
            _highlight.SetActive(true);
        }
        public void DisableHighlight(){
            _highlight.SetActive(false);
        }
        public virtual void Start() {
            GameManager.Instance.TileDB.Add(new Vector2(transform.position.x,transform.position.y),this);
            if(transform.childCount != 0){
                _highlight = transform.GetChild(0).gameObject;
                _highlight.SetActive(false);
            }
        }

        private void OnResetButtonPressed(){
            GameManager.Instance.TileDB.Remove(new Vector2(transform.position.x,transform.position.y));
            GameManager.Instance.TileDB.Add(new Vector2(transform.position.x,transform.position.y),this);
        }

    }
}
