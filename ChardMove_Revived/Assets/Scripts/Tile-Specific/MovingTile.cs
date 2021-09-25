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
        private GameObject _currentBot;
        private bool _moving;

            private void OnTriggerEnter2D(Collider2D other) {
            if(other.CompareTag("Bot")){
                _currentBot = other.gameObject;
            }
        }

        private void OnTriggerExit2D(Collider2D other) {
            if(other.CompareTag("Bot")){
                _currentBot = null;
            }
        }


        private void Awake() {
            BotGridMovement.botMoved += OnBotMoved;
        }

        public void Activate(){
            Active = true;
        }

        private void OnBotMoved(){
            StartCoroutine(Move());
        }

        private IEnumerator Move(){
            Vector2 target = TargetTilePosition();
              for (int i = 0; i < Distance; i++)
            {
                while(true){
                    MoveTowards(target);
                    if((Vector2)transform.position == target){
                        yield return new WaitForSeconds(0.2f);
                        break;
                    }
                    yield return null;
                }
                yield return null;
            }
            _currentBot = null;
            ChangeDirection();
        }

        private void ChangeDirection(){
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
                target =  new Vector2(transform.position.x - 0.5f*Distance, transform.position.y + 0.25f*Distance);
                return target;

                case(MovementDirection.Right):
                target =  new Vector2(transform.position.x + 0.5f*Distance, transform.position.y - 0.25f*Distance);
                return target;

                default:
                return Vector2.zero;
            }
        }

        private void MoveTowards(Vector2 target){
            Vector2 newPos = new Vector2();
            newPos = Vector2.MoveTowards(transform.position, target, Speed * Time.deltaTime);
            if(_currentBot != null){
                // disable player controls here
                _currentBot.transform.position = Vector2.MoveTowards(transform.position, target, Speed * Time.deltaTime);
            }
            transform.position = new Vector3(newPos.x,newPos.y,transform.position.z);
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
