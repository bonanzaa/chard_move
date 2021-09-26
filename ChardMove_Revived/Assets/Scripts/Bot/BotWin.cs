using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChardMove
{
    public class BotWin : MonoBehaviour
    {
        public delegate void LevelFinished();
        public static event LevelFinished onLevelfinished;

        private void Awake()
        {
            WinTile.playerWin += OnPlayerWin;
        }

        private void OnPlayerWin()
        {
            Destroy(this.gameObject);
        }
    }
}
