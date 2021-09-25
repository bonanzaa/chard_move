using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChardMove
{
    public class LatchSwitch : MonoBehaviour, SwitchBase
    {
        public bool GateSwitch;
        public bool MovingPlatformSwitch;
        public List<Roadblock> Gates;
        public List<MovingTile> MovingPlatforms;
        public bool isTarget = false;
        private bool _isActive;

        public void SetTarget(){
            isTarget = true;
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
