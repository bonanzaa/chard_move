using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChardMove
{
    public class LatchSwitch : MonoBehaviour
    {
        public bool GateSwitch;
        public bool MovingPlatformSwitch;
        public List<Roadblock> Gates;
        public List<MovingTile> MovingPlatforms;
        private void OnTriggerEnter2D(Collider2D other) {
            if(other.CompareTag("Bot")){
                if(GateSwitch){
                    foreach (var gate in Gates)
                    {
                        gate.Activate();
                    }
                }
                if(MovingPlatformSwitch){
                    foreach (var platform in MovingPlatforms)
                    {
                        if(!platform.Active) platform.Activate();
                    }
                }
            }
        }
    }
}
