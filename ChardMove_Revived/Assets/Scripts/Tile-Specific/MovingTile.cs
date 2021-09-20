using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ChardMove.gameManager;

namespace ChardMove
{
    public class MovingTile : Tile
    {
        public MovementDirection Direction;
        public int Distance;
        public float Speed = 5f;
        public bool Return;
        public bool Infinite;
        public bool Active = false;

        private Vector2 _originalPos;
        private MovementDirection _oppositeDirection;
        private MovementDirection _originalDirection;
        private GameObject _currentBot;

        private void Awake() {
            _originalPos = new Vector2(transform.position.x, transform.position.y);
        }

        public void Activate(){
            Active = true;
            StartCoroutine(StartMovement());
        }

        private void OnTriggerEnter2D(Collider2D other) {
            if(other.CompareTag("Bot")){
                _currentBot = other.gameObject;
                StartCoroutine(StartMovement());
            }
        }

        private void OnTriggerExit2D(Collider2D other) {
            if(other.CompareTag("Bot")){
                _currentBot = null;
            }
        }

        private IEnumerator StartMovement(){
            Vector2 targetCellPos = DirectionSwitch();
            while((Vector2)transform.position != targetCellPos){
                MoveTowards(targetCellPos);
                yield return null;
            }
            yield return new WaitForSeconds(3f);
            if(Return){
                if(!Infinite) Return = false;
                Direction = _oppositeDirection;
                StartCoroutine(StartMovement());
                yield break;
            }else{
                Active = false;
            }
        }

        private void MoveTowards(Vector2 target){
            Vector2 newPos = new Vector2();
            newPos = Vector2.MoveTowards(transform.position, target, Speed * Time.deltaTime);
            if(_currentBot != null){
                _currentBot.transform.position = Vector2.MoveTowards(transform.position, target, Speed * Time.deltaTime);
            }
            transform.position = new Vector3(newPos.x,newPos.y,transform.position.z);
        }

        private Vector2 DirectionSwitch(){
            Vector2 target = new Vector2();
            switch(Direction){
                case(MovementDirection.Forward):
                _oppositeDirection = MovementDirection.Backward;
                target =  new Vector2(transform.position.x + 0.5f*Distance, transform.position.y + 0.25f*Distance);
                return target;

                case(MovementDirection.Backward):
                _oppositeDirection = MovementDirection.Forward;
                target =  new Vector2(transform.position.x - 0.5f*Distance, transform.position.y - 0.25f*Distance);
                return target;

                case(MovementDirection.Left):
                _oppositeDirection = MovementDirection.Right;
                target =  new Vector2(transform.position.x - 0.5f*Distance, transform.position.y + 0.25f*Distance);
                return target;

                case(MovementDirection.Right):
                _oppositeDirection = MovementDirection.Left;
                target =  new Vector2(transform.position.x + 0.5f*Distance, transform.position.y - 0.25f*Distance);
                return target;

                default:
                return Vector2.zero;
            }
        }
    }
}
