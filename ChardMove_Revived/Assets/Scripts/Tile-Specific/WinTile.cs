using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ChardMove.gameManager;

namespace ChardMove
{
    public class WinTile : Tile
    {
        [SerializeField] private GameObject _winScreenUI;
        public bool Target = false;

        private LevelLoader _levelLoader;

        public delegate void PlayerWin();
        public static event PlayerWin playerWin;
        private void Awake()
        {
            GameManager.Instance.RedButton = this;
            _levelLoader = LevelLoader.Instance;
            GameManager.Instance.TileDB.Add(new Vector2(transform.position.x,transform.position.y),this);
        }

        public void SetTarget(){
            playerWin();
        }
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.gameObject.CompareTag("Bot"))
            {
                if(!Target) return;
                //winscreen active, wait for button input, when pressed calls the load level blah
                //_levelLoader.CanLoadLevel = true;
                playerWin();

            }
        }
    }
}
