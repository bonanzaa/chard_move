using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ChardMove.gameManager;
using ChardMove.BotMovement;

namespace ChardMove
{
    public class LatchSwitch : MonoBehaviour, SwitchBase
    {
        public List<Roadblock> Gates;
        public List<MovingTile> MovingPlatforms;
        private bool _isActive = false;
        private bool _lastIsActive;

        private void Awake() {
            // subscribe to reset/undo events
            GameManager.resetButtonPressed += OnResetButtonPressed;
            GameManager.undoButtonPressed += OnUndoBotLanded;
            // caching isTarget bool for Undo
            _lastIsActive = _isActive;
        }

        private void OnDestroy() {
            GameManager.resetButtonPressed -= OnResetButtonPressed;
            GameManager.undoButtonPressed -= OnUndoBotLanded;
        }

        public void SetTarget(){
            _lastIsActive = _isActive;
            _isActive = true;
            //print($"Setting target. LastIsActive: ({_lastIsActive}). IsActive: ({_isActive})");
            Activate();
        }

        private void Activate(){
            if(Gates.Count != 0){
                foreach (var gate in Gates)
                {
                    gate.Activate();
                }
            }

            if(MovingPlatforms.Count != 0){
                foreach (var item in MovingPlatforms)
                {
                    item.Activate();
                }
            }
        }

        private void Deactivate(){
            if(Gates.Count != 0){
                foreach (var gate in Gates)
                {
                    gate.Deactivate();
                }
            }

            if(MovingPlatforms.Count != 0){
                foreach (var item in MovingPlatforms)
                {
                    item.Deactivate();
                }
            }
        }

        private void OnResetButtonPressed(){
            _isActive = false;
            _lastIsActive = _isActive;
            Reset();
        }

        public void CacheState(){
            _lastIsActive = _isActive;
        }

        public void OnUndoBotLanded(){
            if(_isActive == _lastIsActive) return;
            if(_isActive != _lastIsActive){
                if(_lastIsActive){
                    Activate();
                }else{
                    Deactivate();
                }
            }
            _isActive = _lastIsActive;
        }

        private void Reset(){
            foreach (var gate in Gates)
            {
                gate.Reset();
            }
        }

    }
}
