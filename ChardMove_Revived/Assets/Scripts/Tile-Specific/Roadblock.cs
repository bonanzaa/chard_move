using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ChardMove.gameManager;
using ChardMove.BotMovement;

namespace ChardMove
{
    public class Roadblock : Tile
    {
        public bool IsActive = false;
        private bool _originalIsActive;
        private TileType _lastTileType;
        public bool _lastIsActive;

        private void Awake() {
            _originalIsActive = IsActive;
            GameManager.undoButtonPressed += OnUndoButtonPressed;
            BotGridMovement.botStartedMoving += OnBotStartedMoving;
            _lastTileType = TileType;
        }
        public void Activate(){
            _lastTileType = TileType;
            _lastIsActive = IsActive;
            TileType = TileType.Walkable;
            IsActive = true;
            StartCoroutine(ActivationAnimation());
        }

        private void OnBotStartedMoving(MovementDirection direction, int steps){
            _lastIsActive = IsActive;
        }


        private void OnUndoButtonPressed(){
            if(_lastIsActive == IsActive) return;
            TileType = _lastTileType;
            IsActive = _lastIsActive;
            if(IsActive){
                StartCoroutine(ActivationAnimation());
            }
        }

        private IEnumerator ActivationAnimation(){
            float currentY = transform.position.y;
            float currentZ = transform.position.z;

            float targetY = currentY - 0.375f;
            float targetZ = currentZ + 0.5f;

            float newY = 0;
            float newZ = 0;

            float t  = 0;
            while(transform.position.y != targetY && transform.position.z != targetZ){
                t += Time.deltaTime;
                newY = Mathf.Lerp(currentY,targetY,t);
                newZ = Mathf.Lerp(currentZ,targetZ,t);
                transform.position = new Vector3(transform.position.x,newY,newZ);
                yield return null;
            }
        }

        public void Reset(){
            TileType = TileType.Unwalkable;
            if(IsActive == _originalIsActive){
                return;
            }
            IsActive = _originalIsActive;
            if(IsActive){
                StartCoroutine(ActivationAnimation());
            }else{
                StartCoroutine(DeactivationAnimation());
            }
        }

        public void Deactivate(){
            CheckForBot();
            _lastTileType = TileType;
            _lastIsActive = IsActive;
            TileType = TileType.Unwalkable;
            IsActive = false;
            StartCoroutine(DeactivationAnimation());
        }

        private void CheckForBot(){
            Vector2 potentialBotPos = new Vector2(transform.position.x,transform.position.y + 0.375f);
            var result = GameManager.Instance.BotInTheWayOutBot(potentialBotPos);
            var exists = result.Item1;
            var bot = result.Item2;
            if(exists){
                Destroy(bot.gameObject);
            }
        }

        private IEnumerator DeactivationAnimation(){
            float currentY = transform.position.y;
            float currentZ = transform.position.z;

            float targetY = currentY + 0.375f;
            float targetZ = currentZ - 0.5f;

            float newY = 0;
            float newZ = 0;

            float t  = 0;
            while(transform.position.y != targetY && transform.position.z != targetZ){
                t += Time.deltaTime;
                newY = Mathf.Lerp(currentY,targetY,t);
                newZ = Mathf.Lerp(currentZ,targetZ,t);
                transform.position = new Vector3(transform.position.x,newY,newZ);
                yield return null;
            }
        }
    }
}
