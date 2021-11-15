using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ChardMove.gameManager;
using ChardMove.BotMovement;

namespace ChardMove
{
    public class Roadblock : Tile
    {
        public delegate void RoadblockActivated();
        public static event RoadblockActivated onRoadblockActivated;
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
            GameManager.onLevelFullyLoaded += OnLevelLoaded;
            GameManager.resetButtonPressed += OnReset;
            _lastTileType = TileType;
            if(transform.childCount != 0){
                _highlight = transform.GetChild(0).gameObject;
                _highlight.SetActive(false);
            }
            AnimatorBase.SetBool("RoadblockDown",false);
        }

        private void OnReset(){
            SpriteRenderer _spriteRenderer = AnimatorBase.gameObject.GetComponent<SpriteRenderer>();
            _spriteRenderer.sortingOrder = -1;
        }

        private void OnUndoDirectionalChoice(){
            _highlight.SetActive(false);
        }

        private void OnDisable() {
            BotGridMovement.botStartedMoving -= OnBotStartedMoving;
            GameManager.undoDirectionalChoice -= OnUndoDirectionalChoice;
            GameManager.onLevelFullyLoaded -= OnLevelLoaded;
            GameManager.resetButtonPressed -= OnReset;
        }

        private void OnDestroy() {
            BotGridMovement.botStartedMoving -= OnBotStartedMoving;
            GameManager.undoDirectionalChoice -= OnUndoDirectionalChoice;
            GameManager.onLevelFullyLoaded -= OnLevelLoaded;
            GameManager.resetButtonPressed -= OnReset;
        }
        public void Activate(){
            SpriteRenderer _spriteRenderer = AnimatorBase.gameObject.GetComponent<SpriteRenderer>();
            _spriteRenderer.sortingOrder = -1;
            if(onRoadblockActivated != null)
                onRoadblockActivated();
            _lastTileType = TileType;
            _lastIsActive = IsActive;
            TileType = TileType.Walkable;
            IsActive = true;
            
            AnimatorBase.SetBool("RoadblockDown",true);
            
        }
        public void Deactivate(){
            SpriteRenderer _spriteRenderer = AnimatorBase.gameObject.GetComponent<SpriteRenderer>();
            _spriteRenderer.sortingOrder = 0;
            CheckForBot();
            _lastTileType = TileType;
            _lastIsActive = IsActive;
            TileType = TileType.Unwalkable;
            IsActive = false;
            // ACTIVATE HERE
            AnimatorBase.SetBool("RoadblockDown",false);
        }

        private void OnBotStartedMoving(MovementDirection direction, int steps){
            _lastIsActive = IsActive;
        }

        private void OnLevelLoaded(){
            AnimatorBase.SetBool("RoadblockDown",false);
        }


        public void Reset(){
            TileType = TileType.Unwalkable;
            if(IsActive == _originalIsActive){
                return;
            }
            IsActive = _originalIsActive;
            
            AnimatorBase.SetBool("RoadblockDown",false);
            AnimatorBase.Play("spikes_up");
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

    }
}
