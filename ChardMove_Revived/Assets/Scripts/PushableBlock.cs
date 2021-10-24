using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ChardMove.BotMovement;
using ChardMove.gameManager;
using System.Linq;

namespace ChardMove
{
    public class PushableBlock : Tile, IPushable
    {
        
        public delegate void CannotBePushed();
        public static event CannotBePushed cannotBePushed;
        private Vector2 _lastPosition;
        private Vector2 _lastPositionWorldSpace;
        private float _moveSpeed;
        private bool _transformIntoTile = false;
        private bool _lastTransformIntoTile = false;
        private MovingTile _movingTileReference = null;
        private MovingTile _lastMovingTileReference = null;
        private void Awake() {
            _lastPosition = new Vector2(transform.position.x,transform.position.y - 0.125f);
            _lastPositionWorldSpace = transform.position;
            _transformIntoTile = false;
            _lastTransformIntoTile = _transformIntoTile;
            _lastMovingTileReference = null;
            _movingTileReference = null;
            GameManager.resetButtonPressed += OnResetButtonPressed;
            GameManager.undoButtonPressed += OnUndoButtonPressed;
            BotGridMovement.botStartedMoving += OnBotStartedMoving;
            if(transform.childCount != 0){
                _highlight = transform.GetChild(0).gameObject;
                _highlight.SetActive(false);
            }
        }

        public override void Start() {
            Vector2 myPos = new Vector2(transform.position.x,transform.position.y-0.125f);
            GameManager.Instance.AddToPushableDB(myPos,this,this.gameObject,_lastPosition);
            if(GameManager.Instance.PushableDB.TryGetValue(myPos,out var _value)){
               var myKey = GameManager.Instance.PushableDB.FirstOrDefault(x => x.Value == (this,this.gameObject)).Key;
            }
        }

        public void UpdateDB(){
            Vector2 myPos = new Vector2(transform.position.x,transform.position.y-0.125f);
            GameManager.Instance.AddToPushableDB(myPos,this,this.gameObject,_lastPosition);
        }

        public void CacheLastPos(){
            _lastPosition = new Vector2(transform.position.x,transform.position.y - 0.125f); 
        }

        private void OnDisable() {
            GameManager.resetButtonPressed -= OnResetButtonPressed;
            BotGridMovement.botStartedMoving -= OnBotStartedMoving;
            GameManager.undoButtonPressed -= OnUndoButtonPressed;
        }

        private void OnResetButtonPressed()
        {
            Vector2 pos = new Vector2(transform.position.x,transform.position.y + 0.375f);
            if(_transformIntoTile){
                GameManager.Instance.RemoveFromTileDB(pos);
            }

            GameManager.Instance.PushableDB.Clear();
            Destroy(this.gameObject);
        }

        private void OnUndoButtonPressed(){
            if(!_lastTransformIntoTile){
                //check for moving tile
                if(_lastMovingTileReference != null){
                    if(GameManager.Instance.GetTile(_lastPosition).TryGetComponent(out MovingTile _tile)){
                        _movingTileReference = _tile;
                        _tile.CachePushableBlock(this.gameObject);
                    }
                }else if(_movingTileReference != null){
                    _movingTileReference.RemovePushableBlock();
                    _movingTileReference = null;
                }

                // check if we are now tile
                if(_transformIntoTile){
                    _transformIntoTile = false;
                    Vector2 newpos = new Vector2(transform.position.x,transform.position.y+0.375f);
                    GameManager.Instance.RemoveFromTileDB(newpos);
                }

                Vector2 myPos = new Vector2(transform.position.x,transform.position.y-0.125f);
                GameManager.Instance.RemovePushableFromDB(myPos);
                transform.position = _lastPositionWorldSpace;
                transform.localScale = new Vector3(1f,1f,1f);
                _lastPosition = new Vector2(transform.position.x,transform.position.y - 0.125f);
                GameManager.Instance.PushableDB.Add(_lastPosition,(this,this.gameObject));
            }
        }

        private void OnBotStartedMoving(MovementDirection direction, int steps){
            // caching all the shit for the UNDO
            _lastPositionWorldSpace = transform.position;
            _lastTransformIntoTile = _transformIntoTile;
            _lastMovingTileReference = _movingTileReference;
        }

        public void Push(MovementDirection direction,float moveSpeed){
            if(_transformIntoTile) return;
            _moveSpeed = moveSpeed;
            Vector2 targetTile = TargetTilePosition(direction);
            bool botInTheWay =  GameManager.Instance.BotInTheWay(targetTile);
            if(CheckTargetTileType(direction)){
                FindPushable(direction,moveSpeed);
                StartCoroutine(MoveToNextTile(direction, targetTile));
            }else{
                cannotBePushed();
            }
        }

        public IEnumerator MoveToNextTile(MovementDirection direction, Vector2 target){
            FindPushable(direction,_moveSpeed);
            _lastPosition = new Vector2(transform.position.x,transform.position.y - 0.125f);
            yield return null;
            while(true){
                    MoveTowards(target);
                    if((Vector2)transform.position == target){
                        break;
                    }
                    yield return null;
                }
            if(!_transformIntoTile){
                Vector2 newPos = new Vector2(transform.position.x,transform.position.y-0.125f);
                 GameManager.Instance.AddToPushableDB(newPos,this,this.gameObject,_lastPosition);
                 yield break;
            }else{
                Vector2 pushablePos =  new Vector2(transform.position.x,transform.position.y-0.125f);
                GameManager.Instance.RemovePushableFromDB(pushablePos);
                Vector2 newpos = new Vector2(transform.position.x,transform.position.y-0.125f); // -0.125f
                GameManager.Instance.TileDB.Add(newpos,this);
                if(_movingTileReference != null){
                    _movingTileReference.RemovePushableBlock();
                }
                _movingTileReference = null;
                TileType = TileType.Walkable;
                GetComponent<SpriteRenderer>().sortingOrder = -2;
                
                StartCoroutine(ActivationAnimation());
                yield break;
            }
        }

        private IEnumerator ActivationAnimation(){
            float currentY = transform.position.y;
            float currentZ = transform.position.z;

            float currentScaleX = transform.localScale.x;
            float currentScaleY = transform.localScale.y;

            float targetScaleX = 1.5f;
            float targetScaleY = 1.5f;

            float targetY = currentY - 0.5f;
            float targetZ = currentZ + 0.81f;

            float newY = 0;
            float newZ = 0;

            float newScaleX = 0;
            float newScaleY = 0;

            float t  = 0;
            while(transform.position.y != targetY && transform.position.z != targetZ){
                t += Time.deltaTime;
                newY = Mathf.Lerp(currentY,targetY,t);
                newZ = Mathf.Lerp(currentZ,targetZ,t);

                newScaleX = Mathf.Lerp(currentScaleX,targetScaleX,t*3);
                newScaleY = Mathf.Lerp(currentScaleY,targetScaleY,t*3);

                transform.position = new Vector3(transform.position.x,newY,newZ);
                transform.localScale = new Vector3(newScaleX,newScaleY,0);
                yield return null;
            }
            transform.localScale = new Vector3(1.5f,1.5f,1.5f);
        }

        public Vector2 TargetTilePosition(MovementDirection direction){
            Vector2 target = new Vector2();
            switch(direction){
                case(MovementDirection.Forward):
                target =  new Vector2(transform.position.x + 0.5f, transform.position.y + 0.25f); // +0.25f
                return target;

                case(MovementDirection.Backward):
                target =  new Vector2(transform.position.x - 0.5f, transform.position.y - 0.25f); // -0.25f
                return target;

                case(MovementDirection.Left):
                target =  new Vector2(transform.position.x - 0.5f, transform.position.y + 0.25f); // +0.25f
                return target;

                case(MovementDirection.Right):
                target =  new Vector2(transform.position.x + 0.5f, transform.position.y - 0.25f); // -0.25f
                return target;

                default:
                return Vector2.zero;
            }
        }

        public bool CheckTargetTileType(MovementDirection direction){
            Vector2 target = new Vector2();
            switch(direction){
                case(MovementDirection.Forward):
                target =  new Vector2(transform.position.x + 0.5f, transform.position.y + 0.125f);
                break;

                case(MovementDirection.Backward):
                target =  new Vector2(transform.position.x - 0.5f, transform.position.y - 0.375f);
                break;

                case(MovementDirection.Left):
                target =  new Vector2(transform.position.x - 0.5f, transform.position.y + 0.125f);
                break;

                case(MovementDirection.Right):
                target =  new Vector2(transform.position.x + 0.5f, transform.position.y - 0.375f);
                break;
            }
            // check for any bots first
            bool botInTheWay = GameManager.Instance.BotInTheWay(target);
            if(botInTheWay) return false;

            // then check for the tileType
            TileType tileType = GameManager.Instance.GetTileType(target);
            if(tileType == TileType.Walkable){
    
                if(GameManager.Instance.GetTile(target).TryGetComponent(out MovingTile _tile)){
                    _movingTileReference = _tile;
                    _tile.CachePushableBlock(this.gameObject);
                }
                return true;
            }else if(tileType == TileType.Unwalkable){
                return false;
            }else{
                _transformIntoTile = true;
                return true;
            }

        }

        private void FindPushable(MovementDirection direction,float moveSpeed){
            Vector2 target = new Vector2();
            
            switch(direction){
                case(MovementDirection.Forward):
                target =  new Vector2(transform.position.x + 0.5f, transform.position.y + 0.125f); // y+0.375f
                break;

                case(MovementDirection.Backward):
                target =  new Vector2(transform.position.x - 0.5f, transform.position.y- 0.375f); // y-0.250f
                break;
                
                case(MovementDirection.Left):
                target =  new Vector2(transform.position.x - 0.5f, transform.position.y + 0.125f); //y+0.375f
                break;

                case(MovementDirection.Right):
                target =  new Vector2(transform.position.x + 0.5f,transform.position.y - 0.375f); // y-0.125f
                break;

                default:
                break;
            }
            GameObject pushableGO = GameManager.Instance.GetPushableGO(target);
            if(pushableGO == null){
                return;
            } 
            if(pushableGO.TryGetComponent(out IPushable component)){
                component.Push(direction,moveSpeed);
            }
        }

        public void MoveTowards(Vector2 target){
            this.transform.position = Vector2.MoveTowards(transform.position, target, _moveSpeed * Time.deltaTime);
        }
    
    }

}
