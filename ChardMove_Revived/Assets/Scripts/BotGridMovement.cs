using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ChardMove.gameManager;

namespace ChardMove.BotMovement
{
    public class BotGridMovement : MonoBehaviour
    {
        public float moveSpeed = 5f;

        private bool _canMove = true;
        private IEnumerator walkingCoroutine;

        public delegate void BotMoved();
        public static event BotMoved botMoved;


        public void Move(MovementDirection direction, int steps){
            // the event, indicating start of the movement for a bot
            botMoved();
            var moveCheck = CanMove(direction);
            var canMove = moveCheck.Item1;
            var target = moveCheck.Item2;
            if(canMove){
                walkingCoroutine = MoveToNextTile(direction,steps,target);
                StartCoroutine(walkingCoroutine);
            }else{
                print($"I cannot move {direction.ToString()}...");
            }
        }

        private IEnumerator MoveToNextTile(MovementDirection direction, int steps, Vector2 target){
            _canMove = false;
            for (int i = 0; i < steps; i++)
            {
                while(true){
                    MoveTowards(target);
                    if((Vector2)transform.position == target){
                        yield return new WaitForSeconds(0.5f);
                        break;
                    }
                    yield return null;
                }
                var stepsLeft = steps - (i+1);

                if(stepsLeft!=0){
                    var canMove = CanMove(direction);
                    var canMoveBool = canMove.Item1;
                    var nextTarget = canMove.Item2;
                    if(!canMoveBool){
                        yield break;
                    }else{
                        target = nextTarget;
                    }
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
                    print("Player has died!");
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
