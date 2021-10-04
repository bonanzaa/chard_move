using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ChardMove.gameManager;
using ChardMove.BotMovement;

namespace ChardMove
{
    public class SpawnTile : Tile
    {
        public GameObject BotPrefab;
        private GameObject _bot;
        private Vector2 _lastBotPosition;

        private void Awake() {
            _bot = Instantiate(BotPrefab,transform.position,Quaternion.identity);
        }

        public override void Start() {
            base.Start();
            GameManager.resetButtonPressed += OnResetButtonPressed;
            GameManager.undoButtonPressed += OnUndoButtonPressed;
            BotGridMovement.botAboutToDie += OnBotIsAboutToDie;
        }

        private void OnResetButtonPressed(){
            Awake();
        }

        private void OnBotIsAboutToDie(GameObject theBot){
            if(theBot == _bot)
                _lastBotPosition = _bot.transform.position;
        }

        private void OnUndoButtonPressed(){
            if(_bot == null){
                _bot = Instantiate(BotPrefab,_lastBotPosition,Quaternion.identity);
            }
        }

    }
}
