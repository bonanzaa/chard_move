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
        private float _moveSpeed;
        private bool _transformIntoTile = false;
        private void Awake() {
            _lastPosition = transform.position;
            GameManager.resetButtonPressed += OnResetButtonPressed;
            _transformIntoTile = false;
        }

        public override void Start() {
            Vector2 myPos = new Vector2(transform.position.x,transform.position.y-0.125f);
            GameManager.Instance.AddToPushableDB(myPos,this,this.gameObject,_lastPosition);
            print($"Spawning pushable at ({transform.position.x},{transform.position.y})");
            if(GameManager.Instance.PushableDB.TryGetValue(myPos,out var _value)){
               var myKey = GameManager.Instance.PushableDB.FirstOrDefault(x => x.Value == (this,this.gameObject)).Key;
            }
        }

        private void OnDisable() {
            GameManager.resetButtonPressed -= OnResetButtonPressed;
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

        public void Push(MovementDirection direction,float moveSpeed){
            if(_transformIntoTile) return;
            _moveSpeed = moveSpeed;
            Vector2 targetTile = TargetTilePosition(direction);
            if(CheckTargetTileType(direction)){
                FindPushable(direction,moveSpeed);
                StartCoroutine(MoveToNextTile(direction, targetTile));
            }else{
                cannotBePushed();
            }
        }

        public IEnumerator MoveToNextTile(MovementDirection direction, Vector2 target){
            _lastPosition = transform.position;
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
                TileType = TileType.Walkable;
                GetComponent<SpriteRenderer>().sortingOrder = -2;
                transform.localScale = new Vector3(1,1,1);
                Vector2 newpos = new Vector2(transform.position.x,transform.position.y - 0.125f);
                GameManager.Instance.UpdateTileDB(newpos,this,_lastPosition);
                StartCoroutine(ActivationAnimation());
                yield break;
            }
        }

        private IEnumerator ActivationAnimation(){
            float currentY = transform.position.y;
            float currentZ = transform.position.z;

            float targetY = currentY - 0.5f;
            float targetZ = currentZ + 1f;

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

        public Vector2 TargetTilePosition(MovementDirection direction){
            Vector2 target = new Vector2();
            switch(direction){
                case(MovementDirection.Forward):
                target =  new Vector2(transform.position.x + 0.5f, transform.position.y + 0.25f);
                return target;

                case(MovementDirection.Backward):
                target =  new Vector2(transform.position.x - 0.5f, transform.position.y - 0.25f);
                return target;

                case(MovementDirection.Left):
                target =  new Vector2(transform.position.x - 0.5f, transform.position.y + 0.25f);
                return target;

                case(MovementDirection.Right):
                target =  new Vector2(transform.position.x + 0.5f, transform.position.y - 0.25f);
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
                target =  new Vector2(transform.position.x - 0.125f, transform.position.y + 0.5f);
                break;

                case(MovementDirection.Right):
                target =  new Vector2(transform.position.x + 0.5f, transform.position.y - 0.375f);
                break;
            }
            TileType tileType = GameManager.Instance.GetTileType(target);
            if(tileType == TileType.Walkable){
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
                target =  new Vector2(transform.position.x + 0.5f, transform.position.y + 0.250f); // y+0.375f
                break;

                case(MovementDirection.Backward):
                target =  new Vector2(transform.position.x - 0.5f, transform.position.y- 0.250f); // y-0.125f
                break;
                
                case(MovementDirection.Left):
                target =  new Vector2(transform.position.x - 0.5f, transform.position.y + 0.250f); //y+0.375f
                break;

                case(MovementDirection.Right):
                target =  new Vector2(transform.position.x + 0.5f,transform.position.y - 0.250f); // y-0.125f
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
