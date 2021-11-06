using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ChardMove.gameManager;

namespace ChardMove
{
    public class Tile : MonoBehaviour
    {
        public TileType TileType = TileType.Walkable;
        [HideInInspector] public GameObject _highlight;
        private SpriteRenderer _renderer;
        private void Awake() {
            GameManager.resetButtonPressed += OnResetButtonPressed;
            GameManager.Instance.TileDB.Add(new Vector2(transform.position.x,transform.position.y),this);
            _renderer = GetComponent<SpriteRenderer>();
        }
        public void Highlight(){
            if(_highlight != null)
                _highlight.SetActive(true);
        }
        public void DisableHighlight(){
            if(_highlight != null)
                _highlight.SetActive(false);
        }
        public virtual void Start() {
            if(transform.childCount != 0){
                _highlight = transform.GetChild(0).gameObject;
                _highlight.SetActive(false);
            }
            //GameManager.Instance.TileDB.Add(new Vector2(transform.position.x,transform.position.y),this);
        }

        private void OnDestroy() {
            GameManager.Instance.RemoveFromTileDB(transform.position);
            GameManager.resetButtonPressed -= OnResetButtonPressed;
        }

        private void OnResetButtonPressed(){
            GameManager.Instance.TileDB.Remove(new Vector2(transform.position.x,transform.position.y));
            GameManager.Instance.TileDB.Add(new Vector2(transform.position.x,transform.position.y),this);
        }
    }
}
