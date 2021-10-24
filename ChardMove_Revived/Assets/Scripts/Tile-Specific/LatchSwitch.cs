using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ChardMove.gameManager;

namespace ChardMove
{
    public class LatchSwitch : MonoBehaviour, SwitchBase
    {
        public List<Roadblock> Gates;
        public List<MovingTile> MovingPlatforms;
        public bool isTarget = false;
        private bool _lastIsTarget;

        private void Awake() {
            // subscribe to reset/undo events
            GameManager.resetButtonPressed += OnResetButtonPressed;
            GameManager.undoButtonPressed += OnUndoButtonPressed;
            // caching isTarget bool for Undo
            _lastIsTarget = isTarget;
        }

        private void OnDestroy() {
            GameManager.resetButtonPressed -= OnResetButtonPressed;
            GameManager.undoButtonPressed -= OnUndoButtonPressed;
        }

        public void SetTarget(){
            // is called from BotGridMovement, indicating that player has landed on this tile
            _lastIsTarget = isTarget;
            isTarget = true;
        }

        private void OnResetButtonPressed(){
            isTarget = false;
            Reset();
        }

        private void OnUndoButtonPressed(){
            isTarget = _lastIsTarget;
        }

        private void Reset(){
                    foreach (var gate in Gates)
                    {
                       gate.Reset();
                    }
                    foreach (var platform in MovingPlatforms)
                    {
                        if(!platform.Active) platform.Activate();
                    }
        }

        private void OnTriggerEnter2D(Collider2D other) {
            if(other.CompareTag("Bot") && isTarget){
                if(Gates.Count != 0){
                    foreach (var gate in Gates)
                    {
                        if(gate.IsActive){
                            gate.Deactivate();
                        }else{
                            gate.Activate();
                        }
                    }
                }

                if(MovingPlatforms.Count != 0){
                    foreach (var platform in MovingPlatforms)
                    {
                        if(!platform.Active) platform.Activate();
                    }

                }

                isTarget = false;
            }
        }
    }
}
