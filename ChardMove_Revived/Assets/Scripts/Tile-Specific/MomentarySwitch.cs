using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ChardMove.gameManager;

namespace ChardMove
{
    public class MomentarySwitch : MonoBehaviour, SwitchBase
    {
        public List<Roadblock> Gates; 
        public List<MovingTile> MovingPlatforms;
        public bool isTarget = false;
        private bool _lastIsTarget;
        public GameObject _currentBot;

        private void Awake() {
            GameManager.resetButtonPressed += OnResetButtonPressed;
            GameManager.undoButtonPressed += OnUndoButtonPressed;
            _lastIsTarget = isTarget;
        }

        private void OnDestroy() {
            GameManager.resetButtonPressed -= OnResetButtonPressed;
            GameManager.undoButtonPressed -= OnUndoButtonPressed;
        }

        private void OnUndoButtonPressed(){
            isTarget = _lastIsTarget;
            _currentBot = null;
        }

        private void OnResetButtonPressed(){
            if(Gates.Count != 0){
                foreach (var gate in Gates)
                    {
                        gate.Reset();
                    }
            }
        }

        public void SetTarget(){
            _lastIsTarget = isTarget;
            isTarget = true;
        }
        private void OnTriggerEnter2D(Collider2D other) {
            if(other.CompareTag("Bot") && isTarget){
                foreach (var gate in Gates)
                {
                    gate.Activate();
                }
                foreach (var item in MovingPlatforms)
                {
                    item.Activate();
                }
                isTarget = false;
            }
        }

        private void OnTriggerStay2D(Collider2D other) {
            if(other.CompareTag("Bot")){
                _currentBot = other.gameObject;
            }
        }

        private void OnTriggerExit2D(Collider2D other) {
            if(other.CompareTag("Bot")){
                foreach (var gate in Gates)
                {
                    if(gate.IsActive){
                        gate.Deactivate();
                    }
                }
                foreach (var item in MovingPlatforms)
                {
                    item.Deactivate();
                }
            }
        }
    }
}
