using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ChardMove.gameManager;
using ChardMove.BotMovement;

namespace ChardMove
{
    public class Roadblock : Tile
    {
        [HideInInspector] public bool IsActive = false;
        public Animator AnimatorBase;
        public Animation SpikesDownAnimation;
        private bool _originalIsActive;
        private TileType _lastTileType;
        [HideInInspector] public bool _lastIsActive;

        private void Awake() {
            GameManager.Instance.TileDB.Add(new Vector2(transform.position.x,transform.position.y),this);
            _originalIsActive = IsActive;
            BotGridMovement.botStartedMoving += OnBotStartedMoving;
            GameManager.undoDirectionalChoice += OnUndoDirectionalChoice;
            _lastTileType = TileType;
            if(transform.childCount != 0){
                _highlight = transform.GetChild(0).gameObject;
                _highlight.SetActive(false);
            }
        }

        private void OnUndoDirectionalChoice(){
            _highlight.SetActive(false);
        }

        private void OnDisable() {
            BotGridMovement.botStartedMoving -= OnBotStartedMoving;
            GameManager.undoDirectionalChoice -= OnUndoDirectionalChoice;
        }

        private void OnDestroy() {
            BotGridMovement.botStartedMoving -= OnBotStartedMoving;
            GameManager.undoDirectionalChoice -= OnUndoDirectionalChoice;
        }
        public void Activate(){
            print("Activating roablock");
            _lastTileType = TileType;
            _lastIsActive = IsActive;
            TileType = TileType.Walkable;
            IsActive = true;
            
            // AnimatorBase.SetBool("RoadblockDown",true);
            // SpikesDownAnimation.wrapMode = WrapMode.Once;
            // SpikesDownAnimation.Play();
            StartCoroutine(ActivationAnimationCoroutine());
            
        }
        public void Deactivate(){
            CheckForBot();
            _lastTileType = TileType;
            _lastIsActive = IsActive;
            TileType = TileType.Unwalkable;
            IsActive = false;
            // ACTIVATE HERE
            StartCoroutine(DeactivationAnimationCoroutine());
        }

        private void OnBotStartedMoving(MovementDirection direction, int steps){
            _lastIsActive = IsActive;
        }

        private IEnumerator ActivationAnimationCoroutine(){
            GameManager.Instance.AnimationInProgress = true;
            float currentY = transform.position.y;
            float currentZ = transform.position.z;

            float targetY = currentY - 0.375f;
            float targetZ = currentZ + 1.5f;

            float newY = 0;
            float newZ = 0;

            float t  = 0;
                                                // && transform.position.z != targetZ
            while(transform.position.y != targetY){
                t += Time.deltaTime;
                newY = Mathf.Lerp(currentY,targetY,t);
                newZ = Mathf.Lerp(currentZ,targetZ,t);
                transform.position = new Vector3(transform.position.x,newY,newZ);
                yield return null;
            }
            GameManager.Instance.AnimationInProgress = false;
        }

        public void Reset(){
            TileType = TileType.Unwalkable;
            if(IsActive == _originalIsActive){
                return;
            }
            IsActive = _originalIsActive;
            if(IsActive){
                //StartCoroutine(ActivationAnimation());
            }else{
                //StartCoroutine(DeactivationAnimation());
            }
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

        private IEnumerator DeactivationAnimationCoroutine(){
            GameManager.Instance.AnimationInProgress = true;
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
            GameManager.Instance.AnimationInProgress = false;
        }
    }
}
