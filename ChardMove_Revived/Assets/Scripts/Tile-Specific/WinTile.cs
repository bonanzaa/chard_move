using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
            _levelLoader = LevelLoader.Instance;
        }

        public void SetTarget(){
            Target = true;
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
