using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChardMove
{
    public class WinTile : Tile
    {
        public delegate void PlayerWin();
        public static event PlayerWin playerWin;
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.gameObject.CompareTag("Bot"))
            {
                playerWin();
            }
        }
    }
}
