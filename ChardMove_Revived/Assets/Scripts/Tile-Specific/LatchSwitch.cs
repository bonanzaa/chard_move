using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ChardMove.gameManager;

namespace ChardMove
{
    public class LatchSwitch : MonoBehaviour, SwitchBase
    {
        public bool GateSwitch;
        public bool MovingPlatformSwitch;
        public List<Roadblock> Gates;
        public List<MovingTile> MovingPlatforms;
        public bool isTarget = false;

        private void Awake() {
            GameManager.resetButtonPressed += OnResetButtonPressed;
        }

        public void SetTarget(){
            isTarget = true;
        }

        public void OnResetButtonPressed(){
            isTarget = false;
            Reset();
        }

        private void Reset(){
            if(GateSwitch){
                    foreach (var gate in Gates)
                    {
                       gate.Reset();
                    }
                }
                if(MovingPlatformSwitch){
                    foreach (var platform in MovingPlatforms)
                    {
                        if(!platform.Active) platform.Activate();
                    }
                }
        }

        private void OnTriggerEnter2D(Collider2D other) {
            if(other.CompareTag("Bot") && isTarget){
                if(GateSwitch){
                    foreach (var gate in Gates)
                    {
                        if(gate.IsActive){
                            gate.Deactivate();
                        }else{
                            gate.Activate();
                        }
                    }
                }
                if(MovingPlatformSwitch){
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
