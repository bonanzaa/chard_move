using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ChardMove.gameManager;

namespace ChardMove.BotMovement
{
    public class BotGridMovement : MonoBehaviour, IPushable
    {
        [SerializeField] private float moveSpeed = 5f;
        public bool IsPushable = false;

        public delegate void BotStartedMoving(MovementDirection direction1, int steps);
        public static event BotStartedMoving botStartedMoving;
        public delegate void BotCannotBePushed();
        public static event BotCannotBePushed botCannotBePushed;
        public delegate void BotMoved();
        public static event BotMoved botMoved;
        private bool _canMove = true;
        private IEnumerator walkingCoroutine;
        private Vector2 _originalPosition;
        private Vector2 _lastPosition;
        private bool _amGoingToDie = false;
        [SerializeField] private GameObject Highlight;


        private void Awake() {
            _originalPosition = transform.position;
            _lastPosition = transform.position;
            GameManager.resetButtonPressed += OnResetButtonPressed;
            GameManager.undoButtonPressed += OnUndoButtonPressed;
            PushableBlock.cannotBePushed += OnCannotBePushed;
        }

        private void Start() {
            if(IsPushable){
                Vector2 realPos = new Vector2(transform.position.x,transform.position.y + 0.125f);
                GameManager.Instance.AddToPushableDB(realPos,this,this.gameObject,_lastPosition);
            }else{
                BotGridMovement.botCannotBePushed += OnCannotBePushed;
            }
        }

        public void Push(MovementDirection direction, float Speed){
            if(!IsPushable) return;
            Vector2 targetTile = TargetTilePosition(direction);
            if(CheckTargetTileType(direction)){
                StartCoroutine(MoveToNextTile(direction, targetTile));
            }else{
                if(_amGoingToDie){
                    print("Moving to my death");
                    StartCoroutine(MoveToDeath(direction,targetTile));
                }else{
                    print("Bot cannot be pushed");
                    botCannotBePushed();
                }
            }
        }

         private IEnumerator MoveToNextTile(MovementDirection direction, Vector2 target){
            _lastPosition = transform.position;
            yield return null;
            while(true){
                    MoveTowards(target);
                    if((Vector2)transform.position == target){
                        break;
                    }
                    yield return null;
                }
            }

        private Vector2 TargetTilePosition(MovementDirection direction){
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

        private bool CheckTargetTileType(MovementDirection direction){
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
                _amGoingToDie = false;
                return false;
            }else{
                _amGoingToDie = true;
                return false;
            }

        }

        private void OnDisable() {
            GameManager.resetButtonPressed -= OnResetButtonPressed;
            GameManager.undoButtonPressed -= OnUndoButtonPressed;
            PushableBlock.cannotBePushed -= OnCannotBePushed;
            if(!IsPushable){
                BotGridMovement.botCannotBePushed -= OnCannotBePushed;
            }
        }

        private void OnResetButtonPressed(){
            Destroy(this.gameObject);
        }

        private void OnCannotBePushed(){
            StopAllCoroutines();
            _canMove = true;
            if(botMoved != null)
                botMoved();
        }

        private void OnUndoButtonPressed(){
            this.transform.position = _lastPosition;
            Highlight.transform.localPosition = Vector3.zero;
        }


        public void Move(MovementDirection direction, int steps){
            botStartedMoving(direction,steps);
            _lastPosition = transform.position;
            Highlight.transform.localPosition = Vector3.zero;
            var moveCheck = CanMove(direction);
            var canMove = moveCheck.Item1;
            var target = moveCheck.Item2;
            if(canMove){
                FindPushableBlock(direction);
                CalculateTargetPosAndFindASwitch(direction,steps);
                walkingCoroutine = MoveToNextTile(direction,steps,target);
                StartCoroutine(walkingCoroutine);
            }else{
                if(botMoved != null)
                    botMoved();
                print($"I cannot move {direction.ToString()}...");
            }
        }

        private void TryToFindSwitch(Vector2 pos){
            Tile targetTile = GameManager.Instance.GetTile(pos);
            if(targetTile == null) return;
            if(targetTile.gameObject.TryGetComponent(out SwitchBase component)){
                component.SetTarget();
            }
        }

        private void FindPushableBlock(MovementDirection direction){
            Vector2 target = new Vector2();
            
            switch(direction){
                case(MovementDirection.Forward):
                target =  new Vector2(transform.position.x + 0.5f, transform.position.y + 0.375f);
                break;

                case(MovementDirection.Backward):
                target =  new Vector2(transform.position.x - 0.5f, transform.position.y- 0.125f);
                break;
                
                case(MovementDirection.Left):
                target =  new Vector2(transform.position.x - 0.5f, transform.position.y + 0.375f);
                break;

                case(MovementDirection.Right):
                target =  new Vector2(transform.position.x + 0.5f,transform.position.y - 0.125f);
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

        private void CalculateTargetPosAndFindASwitch(MovementDirection direction, int distance){
            Vector2 target = new Vector2();
            switch(direction){
                case(MovementDirection.Forward):
                target =  new Vector2(transform.position.x + 0.5f*distance, transform.position.y + 0.25f*distance);
                break;

                case(MovementDirection.Backward):
                target =  new Vector2(transform.position.x - 0.5f*distance, transform.position.y - 0.25f*distance);
                break;
                
                case(MovementDirection.Left):
                target =  new Vector2(transform.position.x - 0.5f*distance, transform.position.y + 0.25f*distance);
                break;

                case(MovementDirection.Right):
                target =  new Vector2(transform.position.x + 0.5f*distance, transform.position.y - 0.25f*distance);
                break;

                default:
                break;
            }

            TryToFindSwitch(target);
        }

        private IEnumerator MoveToNextTile(MovementDirection direction, int steps, Vector2 target){
            _canMove = false;
            for (int i = 0; i < steps; i++)
            {
                while(true){
                    MoveTowards(target);
                    if((Vector2)transform.position == target){
                        if(i+1 == steps){
                            yield return new WaitForSeconds(0.1f);
                            break;
                        }
                        yield return new WaitForSeconds(0.5f);
                        break;
                    }
                    yield return null;
                }
                var stepsLeft = steps - (i+1);

                if(stepsLeft!=0){
                    FindPushableBlock(direction);
                    var canMove = CanMove(direction);
                    var canMoveBool = canMove.Item1;
                    var nextTarget = canMove.Item2;
                    if(!canMoveBool){
                        // the event, indicating end of the movement for a bot.
                        // Time to update gamestate!
                        if(botMoved != null)
                            botMoved();
                        yield break;
                    }else{
                        target = nextTarget;
                    }
                }else{
                    // gets called in case we only move 1 
                    if(botMoved != null)
                            botMoved();
                }

                yield return null;
            }
            _canMove = true;
        }

        private IEnumerator MoveToDeath(MovementDirection direction, Vector2 target){
            while(true){
                MoveTowards(target);
                if((Vector2)transform.position == target){
                    // play death animation here
                    _canMove = false;
                    yield return new WaitForSeconds(0.5f);
                    print("Bot has died!");
                    Destroy(this.gameObject);
                    break;
                }
                yield return null;
            }
        }

        private void MoveTowards(Vector2 target){
            this.transform.position = Vector2.MoveTowards(transform.position, target, moveSpeed * Time.deltaTime);
        }

        private (bool,Vector2) CanMove(MovementDirection direction){
            switch(direction){
                case(MovementDirection.Forward):
                return(CheckForward());

                case(MovementDirection.Right):
                return CheckRight();

                case(MovementDirection.Left):
                return CheckLeft();

                case(MovementDirection.Backward):
                return CheckBackward();

                // never gets called. Only exists to satisfy c#'s "not all paths return a value"
                default:
                return (false,new Vector2(0,0));
            }
        }

        private (bool,Vector2) CheckForward(){
            Vector2 nextTilePos = new Vector2(transform.position.x + 0.5f, transform.position.y + 0.25f);
            var tileWalkable = GameManager.Instance.TileWalkable(nextTilePos);
            var walkable = tileWalkable.Item1;
            var playerDead = tileWalkable.Item2;

            if(walkable && !playerDead){
                return (true, nextTilePos);
            }else if(!walkable && !playerDead){
                return (false, nextTilePos);
            }else if(walkable && playerDead){

                StopCoroutine(walkingCoroutine);
                StartCoroutine(MoveToDeath(MovementDirection.Forward,nextTilePos));
                return (false, nextTilePos);
            }
            // never gets called. Only exists to satisfy c#'s "not all paths return a value"
            return (false, nextTilePos);
        }

        private (bool,Vector2) CheckLeft(){
            Vector2 nextTilePos = new Vector2(transform.position.x - 0.5f, transform.position.y + 0.25f);
            var tileWalkable = GameManager.Instance.TileWalkable(nextTilePos);
            var walkable = tileWalkable.Item1;
            var playerDead = tileWalkable.Item2;

            if(walkable && !playerDead){
                return (true, nextTilePos);
            }else if(!walkable && !playerDead){
                return (false, nextTilePos);
            }else if(walkable && playerDead){
                StopCoroutine(walkingCoroutine);
                StartCoroutine(MoveToDeath(MovementDirection.Left,nextTilePos));
                return (false, nextTilePos);
            }
            return (false, nextTilePos);
        }

        private (bool,Vector2) CheckRight(){
            Vector2 nextTilePos = new Vector2(transform.position.x + 0.5f, transform.position.y - 0.25f);
            var tileWalkable = GameManager.Instance.TileWalkable(nextTilePos);
            var walkable = tileWalkable.Item1;
            var playerDead = tileWalkable.Item2;

            if(walkable && !playerDead){
                return (true, nextTilePos);
            }else if(!walkable && !playerDead){
                return (false, nextTilePos);
            }else if(walkable && playerDead){
                StopCoroutine(walkingCoroutine);
                StartCoroutine(MoveToDeath(MovementDirection.Right,nextTilePos));
                return (false, nextTilePos);
            }
            return (false, nextTilePos);
        }

        private (bool,Vector2) CheckBackward(){
            Vector2 nextTilePos = new Vector2(transform.position.x - 0.5f, transform.position.y - 0.25f);
            var tileWalkable = GameManager.Instance.TileWalkable(nextTilePos);
            var walkable = tileWalkable.Item1;
            var playerDead = tileWalkable.Item2;

            if(walkable && !playerDead){
                return (true, nextTilePos);
            }else if(!walkable && !playerDead){
                return (false, nextTilePos);
            }else if(walkable && playerDead){
                StopCoroutine(walkingCoroutine);
                StartCoroutine(MoveToDeath(MovementDirection.Backward,nextTilePos));
                return (false, nextTilePos);
            }
            return (false, nextTilePos);
        }
    }
}
