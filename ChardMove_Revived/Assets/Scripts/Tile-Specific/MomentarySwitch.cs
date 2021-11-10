using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ChardMove.gameManager;
using ChardMove.BotMovement;

namespace ChardMove
{
    public class MomentarySwitch : MonoBehaviour, SwitchBase
    {
        public delegate void MomentaryActivated();
        public static event MomentaryActivated onMomentaryActivated;
        public List<Roadblock> Gates; 
        public List<MovingTile> MovingPlatforms;
        private bool _isActive = false;
        private bool _lastIsActive;
        public GameObject _currentBot;
        [Header("Sprite/Pole management")]
        public Sprite PoleActive;
        public Sprite PoleInactive;
        public GameObject Pole;
        private SpriteRenderer _spriteRenderer;

        private void Awake() {
            GameManager.resetButtonPressed += OnResetButtonPressed;
            GameManager.undoButtonPressed += OnUndoButtonPressed;
            _isActive = false;
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
            GameManager.undoButtonPressed -= OnUndoButtonPressed;
        }

        private void OnUndoButtonPressed(){
            if(_isActive != _lastIsActive){
                if(_lastIsActive){
                    SetTarget();
                }else{
                    RemoveTarget();
                }

            }
        }

        public void OnUndoBotLanded(){
            SetTarget();
        }

        public void CacheState(){
            _lastIsActive = _isActive;
        }

        private void OnResetButtonPressed(){
            _isActive = false;
            ChangePoleSprite();
            _lastIsActive = _isActive;
            if(Gates.Count != 0){
                foreach (var gate in Gates)
                    {
                        gate.Reset();
                    }
            }
        }

        public void SetTarget(){
            _lastIsActive = _isActive;
            _isActive = true;
            Activate();
        }

        public void RemoveTarget(){
            _lastIsActive = _isActive;
            _isActive = false;
            Deactivate();
        }

        private void Activate(){
            if(onMomentaryActivated != null)
                onMomentaryActivated();
            ChangePoleSprite();
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

    }
}
