using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ChardMove.gameManager;
using ChardMove.BotMovement;

namespace ChardMove
{

    public class MovingTile : Tile{
        public MovementDirection Direction;
        [SerializeField] private int Speed;
        [SerializeField] private int Distance;
        [SerializeField] private int CurrentStep;
        public bool Active = true;

        [Header("Sprites for movement sprite switching")]
        [SerializeField] private Sprite _LeftRight;
        [SerializeField] private Sprite _ForwardBackward;
        [SerializeField] private Sprite _GoingRight;
        [SerializeField] private Sprite _GoingLeft;
        [SerializeField] private Sprite _GoingForward;
        [SerializeField] private Sprite _GoingBackward;

        public GameObject GhostPrefab;
        public GameObject _currentBot;
        private PushableBlock _pushableBlock;
        private GameObject _ghost;
        private Vector3 _originalPosition;
        private Vector3 _targetPosition;
        private MovementDirection _originalDirection;
        private Vector3 _lastPosition;
        private int _lastStep;
        private MovementDirection _lastDirection;
        private bool _lastIsActive;
        private bool _originalIsActive;
        private int _originalStep;
        public bool _botOnPlatform = false;
        private bool _moving;
        private SpriteRenderer _spriteRenderer;

        public void CachePushableBlock(GameObject pushableGO){
            _currentBot = pushableGO;
            _pushableBlock = pushableGO.GetComponent<PushableBlock>();
        }

        private void ChangeSprite(MovementDirection direction){
            if(Active){
                switch(direction){
                    case(MovementDirection.Forward):
                    _spriteRenderer.sprite = _GoingForward;
                    break;

                    case(MovementDirection.Left):
                    _spriteRenderer.sprite = _GoingLeft;
                    break;

                    case(MovementDirection.Right):
                    _spriteRenderer.sprite = _GoingRight;
                    break;

                    case(MovementDirection.Backward):
                    _spriteRenderer.sprite = _GoingBackward;
                    break;
                }
            }else{
                switch(direction){
                    case(MovementDirection.Forward):
                    _spriteRenderer.sprite = _ForwardBackward;
                    break;

                    case(MovementDirection.Left):
                    _spriteRenderer.sprite = _LeftRight;
                    break;

                    case(MovementDirection.Right):
                    _spriteRenderer.sprite = _LeftRight;
                    break;

                    case(MovementDirection.Backward):
                    _spriteRenderer.sprite = _ForwardBackward;
                    break;
                }
            }
        }

        public override void Start() {
            base.Start();
        }

        private void ManageGhost(){
            if(_ghost == null) return;
            CheckPath();
            SpriteRenderer ghostSpriteRenderer = _ghost.GetComponent<SpriteRenderer>();
            if(Direction == MovementDirection.Forward || Direction == MovementDirection.Backward){
                ghostSpriteRenderer.sprite = _ForwardBackward;
            }else if(Direction == MovementDirection.Right || Direction == MovementDirection.Left){
                ghostSpriteRenderer.sprite = _LeftRight;
            }


            StartCoroutine(GhostLerp());
        }

        public void ShowGhost(){
            if(_ghost != null){
                _ghost.SetActive(true);
            }
        }

        public void HideGhost(){
            if(_ghost != null){
                _ghost.SetActive(false);
            }
        }

        private IEnumerator GhostLerp(){
            if(_ghost == null){
                yield break;
            }
            while(_ghost.transform.position != _targetPosition){
                _ghost.transform.position = Vector3.Lerp(_ghost.transform.position,_targetPosition,0.2f);
                yield return null;
            }
            _ghost.transform.position = _targetPosition;
        }

        public void RemovePushableBlock(){
            if(_botOnPlatform) return;
            _currentBot = null;
        }

        private void Awake() {
            // cache in our original stats for the Reset
            _originalPosition = transform.position;
            _originalDirection = Direction;
            _originalStep = CurrentStep;
            _originalIsActive = Active;
            _botOnPlatform = false;
            _currentBot = null;

            BotGridMovement.botMoved += OnBotMoved;
            GameManager.resetButtonPressed += OnResetButtonPressed;
            GameManager.undoButtonPressed += OnUndoButtonPressed;
            GameManager.undoDirectionalChoice += OnUndoDirectionalChoice;
            BotGridMovement.botAboutToDie += OnBotAboutToDie;
            GameManager.onLevelUnload += OnLevelUnload;
            GameManager.onLevelFullyLoaded += OnLevelFullyLoaded;
            

            CacheLastInfo();
            _spriteRenderer =  GetComponent<SpriteRenderer>();
            ChangeSprite(Direction);
            GameManager.Instance.TileDB.Add(new Vector2(transform.position.x,transform.position.y),this);

            CheckPath();
            _ghost = Instantiate(GhostPrefab,_targetPosition,Quaternion.identity);
            _ghost.SetActive(false);
            _ghost.transform.SetParent(this.gameObject.transform);
            ManageGhost();
            GameManager.Instance._ghosts.Add(_ghost);
        }

        private void OnLevelFullyLoaded(){
            CheckPath();
            ManageGhost();
        }

        private void OnLevelUnload(){
            Destroy(_ghost);
        }

        private void OnBotAboutToDie(GameObject bot){
            if(bot == _currentBot){
                _currentBot = null;
                _botOnPlatform = false;
            }
        }

        private void CacheLastInfo(){
            // used for Undo
            _lastDirection = Direction;
            _lastPosition = transform.position;
            _lastStep = CurrentStep;
            _lastIsActive = Active;
        }

        private void OnResetButtonPressed(){
            // remove reference for our old position from TileTB;
            GameManager.Instance.RemoveFromTileDB(transform.position);
            
            transform.position = _originalPosition;
            GameManager.Instance.TileDB.Add(transform.position,this);
            CurrentStep = _originalStep;
            Direction = _originalDirection;
            Active = _originalIsActive;
            TileType = TileType.Walkable;
            _botOnPlatform = false;
            // choose a new target
            //StartCoroutine(DelayBeforeCheckPath());
        }

        private IEnumerator DelayBeforeCheckPath(){
            yield return new WaitForEndOfFrame();
            CheckPath();
            _targetPosition = TargetTilePosition();
            ChangeSprite(Direction);
            ManageGhost();
        }

        private void OnUndoDirectionalChoice(){
            _highlight.SetActive(false);
        }

        private void OnUndoButtonPressed(){
            StartCoroutine(UndoTimer());
        }

        private IEnumerator UndoTimer(){
            yield return new WaitForEndOfFrame();
            GameManager.Instance.RemoveFromTileDB(transform.position);
            transform.position = _lastPosition;
            GameManager.Instance.TileDB.Add(transform.position,this);
            CurrentStep = _lastStep;
            Direction = _lastDirection;
            Active = _lastIsActive;
            CheckPath();
        }

        public void OnUndoBotLanded(GameObject bot){
            SetTarget(bot);
        }

        public void Activate(){
            Active = true;
            ChangeSprite(Direction);
            StartCoroutine(Move());
        }

        private void OnDisable()
        {
            BotGridMovement.botMoved -= OnBotMoved;
            GameManager.undoDirectionalChoice -= OnUndoDirectionalChoice;
            GameManager.resetButtonPressed -= OnResetButtonPressed;
            BotGridMovement.botAboutToDie -= OnBotAboutToDie;
            GameManager.undoButtonPressed -= OnUndoButtonPressed;
            GameManager.onLevelUnload -= OnLevelUnload;
            GameManager.onLevelFullyLoaded -= OnLevelFullyLoaded;
        }

        private void OnEnable() {
            BotGridMovement.botMoved += OnBotMoved;
            GameManager.undoDirectionalChoice += OnUndoDirectionalChoice;
            GameManager.resetButtonPressed += OnResetButtonPressed;
            BotGridMovement.botAboutToDie += OnBotAboutToDie;
            GameManager.undoButtonPressed += OnUndoButtonPressed;
            GameManager.onLevelUnload += OnLevelUnload;
            GameManager.onLevelFullyLoaded += OnLevelFullyLoaded;
        }

        private void OnDestroy() {
            BotGridMovement.botMoved -= OnBotMoved;
            GameManager.resetButtonPressed -= OnResetButtonPressed;
            BotGridMovement.botAboutToDie -= OnBotAboutToDie;
            GameManager.undoDirectionalChoice -= OnUndoDirectionalChoice;
            GameManager.undoButtonPressed -= OnUndoButtonPressed;
            GameManager.onLevelUnload -= OnLevelUnload;
            GameManager.onLevelFullyLoaded -= OnLevelFullyLoaded;
        }

        public void Deactivate(){
            Active = false;
        }

        public void SetTarget(GameObject bot){
            _currentBot = bot;
            _botOnPlatform = true;
            if(!_currentBot.GetComponent<BotGridMovement>().IsPushable)
                TileType = TileType.Unwalkable;
        }

        public void RemoveTarget(){
            _currentBot = null;
            _botOnPlatform = false;
            TileType = TileType.Walkable;
        }

        private void OnBotMoved(){
            if(!Active) return;
            if(_moving) return;
            StartCoroutine(Move());
        }

        private float EaseOutQuart(float x){
            return 1-Mathf.Pow(1-x,4);
        }

        private IEnumerator Move(){
            if(GameManager.Instance._botMoving) yield break;
            if(GameManager.Instance.PlayerWon) yield break;

            _lastPosition = transform.position;
            ChangeSprite(Direction);
            // need to check for pushable on top of us
            if(_pushableBlock != null){
                _pushableBlock.CacheLastPos();
            }
            if(_currentBot != null){
                GameManager.Instance.RemoveBotFromDB(_currentBot.transform.position);
            }
            _moving = true;
            Vector2 target = TargetTilePosition();
            _targetPosition = target;
            CheckPath();
            CacheLastInfo();
            // in case CheckPath() detects there is an obstacle in our way,
            // it overwrites _targetPosition, adjusting for the obstacle.
            Vector2 startPosition = transform.position;
            
            float totalDistance = Vector2.Distance(_targetPosition,transform.position);
            float t = 0;
            float duration = totalDistance / Speed;
            // GameManager.Instance.TileDB.Remove(transform.position);
            while(true){
                Vector2 newPos = new Vector2();

                t += Time.deltaTime;
                if(t >= duration){
                    if(_currentBot != null){
                        if(_pushableBlock != null){
                            _currentBot.transform.position =  new Vector3(_targetPosition.x,_targetPosition.y+0.125f,_targetPosition.z);
                        }else{
                            _currentBot.transform.position = new Vector3(_targetPosition.x,_targetPosition.y,_targetPosition.z);
                        }
                        _currentBot.GetComponent<BotGridMovement>().UpdateDB();
                    }
                    transform.position = new Vector3(_targetPosition.x,_targetPosition.y,_targetPosition.z);
                    _moving = false;
                    break;
                }

                newPos = Vector2.Lerp(startPosition,_targetPosition,EaseOutQuart(t/duration));

                transform.position = new Vector3(newPos.x,newPos.y,transform.position.z);
                if(_currentBot != null){
                    // disable player controls here
                    //_currentBot.transform.position = Vector2.MoveTowards(transform.position, target, Speed * Time.deltaTime);
                    if(_pushableBlock != null){
                        _currentBot.transform.position = new Vector3(transform.position.x,transform.position.y+0.125f,transform.position.z);
                    }else{
                        _currentBot.transform.position = new Vector3(transform.position.x,transform.position.y,_targetPosition.z);
                    }
                }

                if((Vector2)transform.position == (Vector2)_targetPosition){

                    transform.position = new Vector3(_targetPosition.x,_targetPosition.y,transform.position.z);

                    if(_currentBot != null){
                        if(_pushableBlock != null){
                            _currentBot.transform.position =  new Vector3(_targetPosition.x,_targetPosition.y+0.125f,_targetPosition.z);
                        }else{
                            _currentBot.transform.position = new Vector3(_targetPosition.x,_targetPosition.y,_targetPosition.z);
                        }
                         _currentBot.GetComponent<BotGridMovement>().UpdateDB();
                    }
                    _moving = false;
                    break;
                }
                yield return null;
            }
            transform.position = new Vector3(_targetPosition.x,_targetPosition.y,_targetPosition.z);
            if(_pushableBlock == null && !_botOnPlatform){
                _currentBot = null;
            }
            // update TileDB with our new position
            if(_pushableBlock != null){
                _pushableBlock.UpdateDB();
            }
            GameManager.Instance.UpdateTileDB(transform.position,this,_lastPosition);
            
            ChangeDirection();
            ChangeSprite(Direction);
            CheckPath();
            _moving = false;
            yield return new WaitForSeconds(0.1f);
            ManageGhost();
        }

        private void ChangeDirection(){
            // chooses the opposite direction
            switch(Direction){
                case(MovementDirection.Forward):
                Direction = MovementDirection.Backward; 
                break;

                case(MovementDirection.Backward):
                Direction = MovementDirection.Forward;
                break;

                case(MovementDirection.Left):
                Direction = MovementDirection.Right;
                break;

                case(MovementDirection.Right):
                Direction = MovementDirection.Left;
                break;
            }
        }

        private Vector2 TargetTilePosition(){
            Vector2 target = new Vector2();
            switch(Direction){
                case(MovementDirection.Forward):
                target =  new Vector2(transform.position.x + 0.5f*Distance, transform.position.y + 0.25f*Distance);
                return target;

                case(MovementDirection.Backward):
                target =  new Vector2(transform.position.x - 0.5f*Distance, transform.position.y - 0.25f*Distance);
                return target;

                case(MovementDirection.Left):
                target =  new Vector2(transform.position.x - 0.25f*Distance, transform.position.y + 0.5f*Distance);
                return target;

                case(MovementDirection.Right):
                target =  new Vector2(transform.position.x + 0.25f*Distance, transform.position.y - 0.5f*Distance);
                return target;

                default:
                return Vector2.zero;
            }
        }

        private void CheckPath(){
            Vector2 target = new Vector2();
            TileType tileType = new TileType();
            Vector2 lastTarget = new Vector2();
            lastTarget = _targetPosition;
            switch(Direction){
                case(MovementDirection.Forward):
                for (int i = 1; i < Distance+1; i++)
                {
                    target =  new Vector2(transform.position.x + 0.5f*i, transform.position.y + 0.25f*i);
                    tileType = GameManager.Instance.GetTileType(target);
                    if(tileType == TileType.Death){
                        _targetPosition = target;
                        lastTarget = target;
                    }else{
                        if(i == 1){
                            _targetPosition = transform.position;
                        }else{
                            _targetPosition = lastTarget;
                        }
                        break;
                    }
                    _targetPosition = target;
                }
                break;

                case(MovementDirection.Backward):
                for (int i = 1; i < Distance+1; i++)
                {
                    target =  new Vector2(transform.position.x - 0.5f*i, transform.position.y - 0.25f*i);
                    tileType = GameManager.Instance.GetTileType(target);
                    if(tileType == TileType.Death){
                        _targetPosition = target;
                        lastTarget = target;
                    }else{
                        if(i == 1){
                            _targetPosition = transform.position;
                        }else{
                            _targetPosition = lastTarget;
                        }
                        break;
                    }
                    _targetPosition = target;
                }
                break;

                case(MovementDirection.Left):
                for (int i = 1; i < Distance+1; i++)
                {
                    target =  new Vector2(transform.position.x - 0.5f*i, transform.position.y + 0.25f*i);
                    tileType = GameManager.Instance.GetTileType(target);
                    if(tileType == TileType.Death){
                        _targetPosition = target;
                        lastTarget = target;
                    }else{
                        if(i == 1){
                            _targetPosition = transform.position;
                        }else{
                            _targetPosition = lastTarget;
                        }
                        break;
                    }
                    _targetPosition = target;
                }
                break;

                case(MovementDirection.Right):
                for (int i = 1; i < Distance+1; i++)
                {
                    target =  new Vector2(transform.position.x + 0.5f*i, transform.position.y - 0.25f*i);
                    tileType = GameManager.Instance.GetTileType(target);
                    if(tileType == TileType.Death){
                        _targetPosition = target;
                        lastTarget = target;
                    }else{
                        if(i == 1){
                            _targetPosition = transform.position;
                        }else{
                            _targetPosition = lastTarget;
                        }
                        break;
                    }
                    _targetPosition = target;
                }
                break;
            }
        }

    }

    // EVERYTHING BELOW IS ME BEING RETARDED AND CREATING A "Player moves 1 tile - platform moves 1 tile" SYSTEM
    // WHICH WE DON"T FUCKING USE, BUT DANIEL CONVINCED ME TO KEEP THIS ABOMINATION AS A REMINDER OF SHAME TO ME.


    // public class MovingTile : Tile
    // {
    //     public MovementDirection Direction;
    //     public int Distance;
    //     public int CurrentStep = 0;
    //     public float Speed = 5f;
    //     public bool Active = false;

    //     private Vector2 _originalPos;
    //     private Vector2 _targetTilePos;
    //     private GameObject _currentBot;
    //     private Vector2 _myLastPosition;
    //     public Tile _targetTile;
    //     private bool _moving;

    //     private void Awake() {
    //         _originalPos = new Vector2(transform.position.x, transform.position.y);
    //         BotGridMovement.botMoved += OnBotMoved;
    //         _myLastPosition = _originalPos;
    //         _currentBot = null;
    //         CurrentStep = 0;
    //         Active = true;
    //     }

    //     public void Activate(){
    //         Active = true;
    //     }

    //     private void OnTriggerEnter2D(Collider2D other) {
    //         if(other.CompareTag("Bot")){
    //             _currentBot = other.gameObject;
    //             StartCoroutine(MoveWithPlayer());
    //         }
    //     }

    //     private void OnTriggerExit2D(Collider2D other) {
    //         if(other.CompareTag("Bot")){
    //             _currentBot = null;
    //         }
    //     }

    //     private void MoveTowards(Vector2 target){
    //         Vector2 newPos = new Vector2();
    //         newPos = Vector2.MoveTowards(transform.position, target, Speed * Time.deltaTime);
    //         if(_currentBot != null){
    //             // disable player controls here
    //             _currentBot.transform.position = Vector2.MoveTowards(transform.position, target, Speed * Time.deltaTime);
    //         }
    //         transform.position = new Vector3(newPos.x,newPos.y,transform.position.z);
    //     }

    //     public void OnBotMoved(){
    //         if(!Active) return;
    //         if(_moving) return;
    //         StartCoroutine(MoveOneTile());
    //     }

    //     private IEnumerator MoveOneTile(){
    //         _moving = true;
    //         Vector2 target = NextTilePosition();
    //         _targetTile = GameManager.Instance.GetTile(target);
    //         if(_targetTile != null) yield break;
    //          while(true){
    //                 MoveTowards(target);
    //                 if((Vector2)transform.position == target){
    //                     _targetTile = null;
    //                     yield return new WaitForSeconds(0.5f);
    //                     break;
    //                 }
    //                 yield return null;
    //             }
    //         CurrentStep++;
    //         // if we reached the max distance, change direction and move in the opposite direction
    //         if(CurrentStep == Distance){
    //             CurrentStep = 0;
    //             ChangeDirection();
    //         }
    //         yield return null;
    //         _myLastPosition = transform.position;
    //         GameManager.Instance.AddToTileDB(transform.position,this,_myLastPosition);
    //         _moving = false;
    //         yield break;
    //     }

    //     private IEnumerator MoveWithPlayer(){
    //         _moving = true;
    //         Vector2 target = TargetTilePosition();
    //         _targetTile = GameManager.Instance.GetTile(target);
    //         if(_targetTile != null) yield break;
    //           for (int i = 0; i < Distance; i++)
    //         {
    //             while(true){
    //                 MoveTowards(target);
    //                 if((Vector2)transform.position == target){
    //                     yield return new WaitForSeconds(0.2f);
    //                     break;
    //                 }
    //                 yield return null;
    //             }
    //             yield return null;
    //         }
    //         CurrentStep = Distance;
    //         // if we reached the max distance, change direction and move in the opposite direction
    //         if(CurrentStep == Distance){
    //             CurrentStep = 0;
    //             ChangeDirection();
    //         }
    //         yield return null;
    //         _targetTile = null;
    //         _targetTilePos = Vector2.zero;
    //         _moving = false;
    //         yield break;
    //     }

    //     private Vector2 NextTilePosition(){
    //         Vector2 target = new Vector2();
    //         switch(Direction){
    //             case(MovementDirection.Forward):
    //             target =  new Vector2(transform.position.x + 0.5f, transform.position.y + 0.25f);
    //             return target;

    //             case(MovementDirection.Backward):
    //             target =  new Vector2(transform.position.x - 0.5f, transform.position.y - 0.25f);
    //             return target;

    //             case(MovementDirection.Left):
    //             target =  new Vector2(transform.position.x - 0.5f, transform.position.y + 0.25f);
    //             return target;

    //             case(MovementDirection.Right):
    //             target =  new Vector2(transform.position.x + 0.5f, transform.position.y - 0.25f);
    //             return target;

    //             default:
    //             return Vector2.zero;
    //         }
    //     }

    //     private Vector2 TargetTilePosition(){
    //         Vector2 target = new Vector2();
    //         int Distance = Distance - CurrentStep;
    //         switch(Direction){
    //             case(MovementDirection.Forward):
    //             target =  new Vector2(transform.position.x + 0.5f*Distance, transform.position.y + 0.25f*Distance);
    //             return target;

    //             case(MovementDirection.Backward):
    //             target =  new Vector2(transform.position.x - 0.5f*Distance, transform.position.y - 0.25f*Distance);
    //             return target;

    //             case(MovementDirection.Left):
    //             target =  new Vector2(transform.position.x - 0.5f*Distance, transform.position.y + 0.25f*Distance);
    //             return target;

    //             case(MovementDirection.Right):
    //             target =  new Vector2(transform.position.x + 0.5f*Distance, transform.position.y - 0.25f*Distance);
    //             return target;

    //             default:
    //             return Vector2.zero;
    //         }
    //     }

    //     private void ChangeDirection(){
    //         switch(Direction){
    //             case(MovementDirection.Forward):
    //             Direction = MovementDirection.Backward;
    //             break;

    //             case(MovementDirection.Backward):
    //             Direction = MovementDirection.Forward;
    //             break;

    //             case(MovementDirection.Left):
    //             Direction = MovementDirection.Right;
    //             break;

    //             case(MovementDirection.Right):
    //             Direction = MovementDirection.Left;
    //             break;
    //         }
    //     }
    // }
}
