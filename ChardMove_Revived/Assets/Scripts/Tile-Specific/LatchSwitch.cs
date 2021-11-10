using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ChardMove.gameManager;
using ChardMove.BotMovement;

namespace ChardMove
{
    public class LatchSwitch : MonoBehaviour, SwitchBase
    {
        public delegate void LatchActivated();
        public static event LatchActivated onLatchActivated;
        public List<Roadblock> Gates;
        public List<MovingTile> MovingPlatforms;
        [Header("Sprite/Pole management")]
        public Sprite PoleActive;
        public Sprite PoleInactive;
        public GameObject Pole;
        private SpriteRenderer _spriteRenderer;
        private bool _isActive = false;
        private bool _lastIsActive;
        

        private void Awake() {
            // subscribe to reset/undo events
            GameManager.resetButtonPressed += OnResetButtonPressed;
            GameManager.undoButtonPressed += OnUndoBotLanded;
            // caching isTarget bool for Undo
            _lastIsActive = _isActive;
            _spriteRenderer = Pole.GetComponent<SpriteRenderer>();
        }

        private void ChangePoleSprite(){
            if(_isActive){
                _spriteRenderer.sprite = PoleActive;
            }else{
                _spriteRenderer.sprite = PoleInactive;
            }
        }

        private void OnDestroy() {
            GameManager.resetButtonPressed -= OnResetButtonPressed;
            GameManager.undoButtonPressed -= OnUndoBotLanded;
        }

        public void SetTarget(){
            _lastIsActive = _isActive;
            _isActive = true;
            ChangePoleSprite();
            //print($"Setting target. LastIsActive: ({_lastIsActive}). IsActive: ({_isActive})");
            Activate();
        }

        private void Activate(){
            if(onLatchActivated != null)
                onLatchActivated();
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
            _isActive = false;
            ChangePoleSprite();
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
            ChangePoleSprite();
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
