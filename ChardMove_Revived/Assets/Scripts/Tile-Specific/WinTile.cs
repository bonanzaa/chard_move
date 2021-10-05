using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChardMove
{
    public class WinTile : Tile
    {
        [SerializeField] private GameObject _winScreenUI;

        private LevelLoader _levelLoader;

        public delegate void PlayerWin();
        public static event PlayerWin playerWin;
        private void Awake()
        {
            _levelLoader = LevelLoader.Instance;
        }
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.gameObject.CompareTag("Bot"))
            {
                //winscreen active, wait for button input, when pressed calls the load level blah
                //_levelLoader.CanLoadLevel = true;
                playerWin();
            }
        }
    }
}
