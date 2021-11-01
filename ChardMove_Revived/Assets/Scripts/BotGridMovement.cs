using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ChardMove.gameManager;

namespace ChardMove.BotMovement
{
    public class BotGridMovement : MonoBehaviour, IPushable
    {
        [SerializeField] private float moveSpeed = 5f;
        [SerializeField] private GameObject Highlight;
        public bool IsPushable = false;

        [Header("Sprites for movement sprite switching")]
        [SerializeField] private Sprite _facingForward;
        [SerializeField] private Sprite _facingLeft;
        [SerializeField] private Sprite _facingRight;
        [SerializeField] private Sprite _facingBackward;


        public delegate void BotStartedMoving(MovementDirection direction1, int steps);
        public static event BotStartedMoving botStartedMoving;
        public delegate void BotStartedMovingPos(Vector2 pos);
        public static event BotStartedMovingPos botStartedMovingPos;
        public delegate void BotCannotBePushed();
        public static event BotCannotBePushed botCannotBePushed;
        public delegate void BotAboutToDie(GameObject theBot);
        public static event BotAboutToDie botAboutToDie;
        public delegate void BotMoved();
        public static event BotMoved botMoved;
        public delegate void BotMovedPos(Vector2 pos);
        public static event BotMovedPos botMovedPos;
        public delegate void BotUndoPressed(Vector2 pos, Vector2 lastpos);
        public static event BotUndoPressed botUndoPressed;
        private bool _canMove = true;
        private IEnumerator walkingCoroutine;
        private Vector2 _originalPosition;
        private Vector2 _lastPosition;
        private Vector2 _lastPositionBeforeMovement;
        private SpriteRenderer _spriteRenderer;
        private bool _amGoingToDie = false;
        private Vector2 _target;
        private Vector2 _originalTarget;
        private int _steps;


        private void Awake() {
            // used by reset
            transform.position = new Vector3(transform.position.x,transform.position.y,-0.5f);
            _originalPosition = transform.position;
            // used by undo
            _lastPosition = transform.position;
            _lastPositionBeforeMovement = transform.position;
            

            GameManager.resetButtonPressed += OnResetButtonPressed;
            GameManager.undoButtonPressed += OnUndoButtonPressed;
            PushableBlock.cannotBePushed += OnCannotBePushed;
            BotGridMovement.botCannotBePushed += OnCannotBePushed;

            _spriteRenderer =  GetComponent<SpriteRenderer>();
        }

        private void Start() {
            if(IsPushable){
                // we offset our position, because initially bot is a bit higher
                // than the tile it is on
                Vector2 realPos = new Vector2(transform.position.x,transform.position.y); // y+0.125f
                GameManager.Instance.AddToPushableDB(realPos,this,this.gameObject,_lastPosition);
            }else{
                // only subscribe to an event if we are not pushable
                BotGridMovement.botCannotBePushed += OnCannotBePushed;
            }
            GameManager.Instance.AddBotToDB(transform.position,this,_lastPosition);
        }

        public void Move(MovementDirection direction, int steps){ 
            _steps = steps;
            botStartedMoving(direction,_steps);
            botStartedMovingPos(transform.position);
            GameManager.Instance.OnBotStartedMoving();
            ChangeSprite(direction);
            _lastPositionBeforeMovement = transform.position;
            _lastPosition = transform.position;
            // readjust highlight GO
            //Highlight.transform.localPosition = Vector3.zero;
            var moveCheck = CanMove(direction);
            var canMove = moveCheck.Item1; // bool checking if the next tile is walkable/death
            var target = moveCheck.Item2; // Target Vector2 of the next tile
            walkingCoroutine = MoveToNextTile(direction,_steps,target);
            _originalTarget = target;
            _target = target;
            bool botInTheWay = GameManager.Instance.BotInTheWay(target); // bool checking if another bot is in the next tile
            if(canMove && !botInTheWay){
                FindPushable(direction);
                //walkingCoroutine = MoveToNextTile(direction,steps,target);
                StartCoroutine(walkingCoroutine);
            }else{
                // next tile is unwalkable (roadblock) so nothing happens
                // this is considered a move, so update world state
                GameManager.Instance.OnBotFinishedMoving();
                print($"I cannot move {direction.ToString()}...");
                // need to delay this, otherwise world state updates instanly,
                // which causes some fucked up interactions
                if(botMoved != null)
                    StartCoroutine(BotMovedEventTimer());
            }
        }

        private IEnumerator BotMovedEventTimer(){
            yield return new WaitForSeconds(0.125f);
            
            botMoved();
            botMovedPos(transform.position);
        }

        private void ChangeSprite(MovementDirection direction){
            switch(direction){
                case(MovementDirection.Forward):
                _spriteRenderer.sprite = _facingForward;
                break;

                case(MovementDirection.Left):
                _spriteRenderer.sprite = _facingLeft;
                break;

                case(MovementDirection.Right):
                _spriteRenderer.sprite = _facingRight;
                break;

                case(MovementDirection.Backward):
                _spriteRenderer.sprite = _facingBackward;
                break;
            }
        }
        
        private void FindTargetTiles(MovementDirection direction, int distance){
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
            TryToFindWinTile(target);
        }
        

        private void TryToFindWinTile(Vector2 target){
            Tile targetTile = GameManager.Instance.GetTile(target);
            if(targetTile == null) return;
            if(targetTile.gameObject.TryGetComponent(out WinTile component)){
                component.SetTarget();
            }
        }

        private IEnumerator MoveToNextTile(MovementDirection direction, int steps, Vector2 target){
            steps = _steps;
            _canMove = false;
            FindPushable(direction);
            yield return new WaitForEndOfFrame();
            for (int i = 0; i < steps; i++)
            {
                _lastPosition = transform.position;
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
                    FindPushable(direction);
                    var canMove = CanMove(direction);
                    var canMoveBool = canMove.Item1;
                    var nextTarget = canMove.Item2;
                    if(!canMoveBool){
                        // the event, indicating end of the movement for a bot.
                        // Time to update gamestate!
                        if(botMoved != null)
                            yield return new WaitForSeconds(0.125f);
                        GameManager.Instance.OnBotFinishedMoving();
                        if(botMoved != null)
                            botMoved();
                        GameManager.Instance.AddBotToDB(transform.position,this,_lastPosition);
                        if(IsPushable){
                            GameManager.Instance.AddToPushableDB(transform.position,this,this.gameObject,_lastPosition);
                        }
                        botMovedPos(transform.position);
                        yield break;
                    }else{
                        target = nextTarget;
                    }
                }else{
                    yield return new WaitForSeconds(0.125f);
                    GameManager.Instance.OnBotFinishedMoving();
                    // gets called in case we only move 1 
                    if(botMoved != null)
                        botMoved();
                        GameManager.Instance.AddBotToDB(transform.position,this,_lastPosition);
                        botMovedPos(transform.position);
                    } 

                yield return null;
                GameManager.Instance.AddBotToDB(transform.position,this,_lastPosition);
                if(IsPushable){
                    GameManager.Instance.AddToPushableDB(transform.position,this,this.gameObject,_lastPosition);
                }
            }
            GameManager.Instance.OnBotFinishedMoving();
            _canMove = true;
        }

        private IEnumerator MoveToDeath(MovementDirection direction, Vector2 target){
            botAboutToDie(this.gameObject);
            GameManager.Instance.RemoveBotFromDB(transform.position);
            if(IsPushable){
                GameManager.Instance.RemovePushableFromDB(transform.position);
            }
            while(true){
                MoveTowards(target);
                if((Vector2)transform.position == target){
                    GameManager.Instance._botMoving = false; 
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
        // pushable bot functionality
        public void Push(MovementDirection direction, float Speed){
            // gets called, when another bot detects a pushable bot in their way
            if(!IsPushable) return;
            Vector2 targetTile = TargetTilePosition(direction);
            if(CheckTargetTileType(direction)){
                FindTargetTiles(direction,1);
                StartCoroutine(MoveToNextTile(direction, targetTile));
            }else{
                if(_amGoingToDie){
                    StartCoroutine(MoveToDeath(direction,targetTile));
                }else{
                    botCannotBePushed();
                }
            }
        }

        public IEnumerator MoveToNextTile(MovementDirection direction, Vector2 target){
            if(!IsPushable) yield break;
                // only used in pushable bots
                FindPushable(direction);
                _lastPosition = transform.position;
                yield return new WaitForEndOfFrame();
                yield return null;
                while(true){
                    MoveTowards(target);
                    if((Vector2)transform.position == target){
                        break;
                    }
                    yield return null;
                }
                GameManager.Instance.AddBotToDB(transform.position,this,_lastPosition);
                GameManager.Instance.AddToPushableDB(transform.position,this,this.gameObject,_lastPosition);
            }

        public Vector2 TargetTilePosition(MovementDirection direction){
            Vector2 target = new Vector2();
            // calculating next tile's position in the given direction
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
            // get the type of the next tile in the given direction
            // used for bot navigation
            Vector2 target = new Vector2();
            switch(direction){
                case(MovementDirection.Forward):
                target =  new Vector2(transform.position.x + 0.5f, transform.position.y + 0.250f);
                break;

                case(MovementDirection.Backward):
                target =  new Vector2(transform.position.x - 0.5f, transform.position.y - 0.250f); // y-0.375f
                break;

                case(MovementDirection.Left):
                target =  new Vector2(transform.position.x - 0.5f, transform.position.y + 0.250f);
                break;

                case(MovementDirection.Right):
                target =  new Vector2(transform.position.x + 0.5f, transform.position.y - 0.250f);
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
        public void MoveTowards(Vector2 target){
            Vector3 targetWithZ = new Vector3(target.x,target.y,-0.5f);
            // used by both types of bots to move
            this.transform.position = Vector3.MoveTowards(transform.position, targetWithZ, moveSpeed * Time.deltaTime);
        }

        // event callbacks
        private void OnDisable() {
            GameManager.Instance.RemoveBotFromDB(this.transform.position);
            GameManager.resetButtonPressed -= OnResetButtonPressed;
            GameManager.undoButtonPressed -= OnUndoButtonPressed;
            PushableBlock.cannotBePushed -= OnCannotBePushed;
            if(!IsPushable){
                BotGridMovement.botCannotBePushed -= OnCannotBePushed;
            }else{
                GameManager.Instance.RemovePushableFromDB(transform.position);
            }
        }

        private void OnDestroy() {
            GameManager.Instance.RemoveBotFromDB(this.transform.position);
            GameManager.resetButtonPressed -= OnResetButtonPressed;
            GameManager.undoButtonPressed -= OnUndoButtonPressed;
            PushableBlock.cannotBePushed -= OnCannotBePushed;
            if(!IsPushable){
                BotGridMovement.botCannotBePushed -= OnCannotBePushed;
            }else{
                GameManager.Instance.RemovePushableFromDB(transform.position);
            }
        }

        private void OnResetButtonPressed(){
            Destroy(this.gameObject);
        }

        private void OnCannotBePushed(){
            StopAllCoroutines();
            _canMove = true;
            if(botMoved != null){
                botMoved();
            }
        }

        private void OnUndoButtonPressed(){
            botUndoPressed(transform.position, _lastPosition);
            if(IsPushable){
                GameManager.Instance.RemovePushableFromDB(transform.position);
            }
            GameManager.Instance.RemoveBotFromDB(transform.position);
            GameManager.Instance.RemoveBotFromDB(_lastPosition);
            transform.position = _lastPositionBeforeMovement;
            GameManager.Instance.BotDB.Add(transform.position,this);
            if(IsPushable){
                GameManager.Instance.PushableDB.Add(transform.position,(this,this.gameObject));
            }
        }


        // movement checks

        private void FindPushable(MovementDirection direction){
            Vector2 target = new Vector2();
            
            switch(direction){
                case(MovementDirection.Forward):
                target =  new Vector2(transform.position.x + 0.5f, transform.position.y + 0.250f); // y+0.375f
                break;

                case(MovementDirection.Backward):             // -0.5f                         // -0.25f
                target =  new Vector2(transform.position.x - 0.5f, transform.position.y- 0.250f); // y-0.250f
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
            bool _botInTheWay = GameManager.Instance.BotInTheWay(nextTilePos);
            if(walkable && !playerDead && !_botInTheWay){
                return (true, nextTilePos);
            }else if(!walkable && !playerDead){
                return (false, nextTilePos);
            }else if(walkable && playerDead){

                if(walkingCoroutine != null) StopCoroutine(walkingCoroutine);
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
            bool _botInTheWay = GameManager.Instance.BotInTheWay(nextTilePos);
            if(walkable && !playerDead && !_botInTheWay){
                return (true, nextTilePos);
            }else if((!walkable && !playerDead) || _botInTheWay){
                return (false, nextTilePos);
            }else if(walkable && playerDead){
                if(walkingCoroutine != null) StopCoroutine(walkingCoroutine);
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
            bool _botInTheWay = GameManager.Instance.BotInTheWay(nextTilePos);
            if(walkable && !playerDead && !_botInTheWay){
                return (true, nextTilePos);
            }else if((!walkable && !playerDead) || _botInTheWay){
                return (false, nextTilePos);
            }else if(walkable && playerDead){
                if(walkingCoroutine != null) StopCoroutine(walkingCoroutine);
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
            bool _botInTheWay = GameManager.Instance.BotInTheWay(nextTilePos);
            if(walkable && !playerDead && !_botInTheWay){
                return (true, nextTilePos);
            }else if((!walkable && !playerDead) || _botInTheWay){
                return (false, nextTilePos);
            }else if(walkable && playerDead){
                if(walkingCoroutine != null) StopCoroutine(walkingCoroutine);
                StartCoroutine(MoveToDeath(MovementDirection.Backward,nextTilePos));
                return (false, nextTilePos);
            }
            return (false, nextTilePos);
        }
    }
}
