using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChardMove
{
    public class BotWin : MonoBehaviour
    {
        private void Awake()
        {
            WinTile.playerWin += OnPlayerWin;
        }

        private void OnPlayerWin()
        {
            //Destroy(this.gameObject);
        }

        private void OnDisable()
        {
            WinTile.playerWin -= OnPlayerWin;
        }
    }
}
