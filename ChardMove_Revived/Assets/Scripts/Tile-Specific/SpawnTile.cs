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
        private bool _botJustDied = false;

        private void Awake() {
            Vector3 myPos = new Vector3(transform.position.x,transform.position.y,-0.5f);
            _bot = Instantiate(BotPrefab,myPos,Quaternion.identity);
            _botJustDied = false;
            _lastBotPosition = transform.position;
            if(!GameManager.Instance.TileDB.TryGetValue(transform.position,out Tile value)){
                GameManager.Instance.TileDB.Add(new Vector2(transform.position.x,transform.position.y),this);
            }
        }

        public override void Start() {
            base.Start();
            GameManager.resetButtonPressed += OnResetButtonPressed;
            GameManager.undoButtonPressed += OnUndoButtonPressed;
            BotGridMovement.botAboutToDie += OnBotIsAboutToDie;
            BotGridMovement.botStartedMoving += OnBotStartedMoving;
        }

        private void OnBotStartedMoving(MovementDirection direction, int steps){
            _botJustDied = false;
        }

        private void OnDestroy() {
            GameManager.resetButtonPressed -= OnResetButtonPressed;
            GameManager.undoButtonPressed -= OnUndoButtonPressed;
            BotGridMovement.botAboutToDie -= OnBotIsAboutToDie;
            BotGridMovement.botStartedMoving -= OnBotStartedMoving;
            Destroy(_bot);
        }

        private void OnResetButtonPressed(){
            // if(_bot != null){
            //     Destroy(_bot);
            // }
            // StartCoroutine(AwakeTimer());
        }

        private IEnumerator AwakeTimer(){
            yield return new WaitForEndOfFrame();
            Awake();
        }

        private void OnBotIsAboutToDie(GameObject theBot){
            if(theBot == _bot){
                _botJustDied = true;
                _lastBotPosition = _bot.transform.position;
            }
        }

        private void OnUndoButtonPressed(){
            if(!_botJustDied) return;
            if(_bot == null){
                _bot = Instantiate(BotPrefab,_lastBotPosition,Quaternion.identity);
            }
        }

    }
}
