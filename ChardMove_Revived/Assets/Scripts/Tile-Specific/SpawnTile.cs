using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ChardMove.gameManager;

namespace ChardMove
{
    public class SpawnTile : Tile
    {
        public GameObject BotPrefab;

        private void Awake() {
            Instantiate(BotPrefab,transform.position,Quaternion.identity);
        }

        public override void Start() {
            base.Start();
            GameManager.resetButtonPressed += OnResetButtonPressed;
        }

        private void OnResetButtonPressed(){
            Awake();
        }

    }
}
