using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChardMove
{
    public class MomentarySwitch : MonoBehaviour, SwitchBase
    {
        public List<Roadblock> Gates;
        public bool isTarget = false;

        public void SetTarget(){
            isTarget = true;
        }
        private void OnTriggerEnter2D(Collider2D other) {
            if(other.CompareTag("Bot") && isTarget){
                foreach (var gate in Gates)
                {
                    gate.Activate();
                }
            }
        }

        private void OnTriggerExit2D(Collider2D other) {
            if(other.CompareTag("Bot")){
                foreach (var gate in Gates)
                {
                    gate.Deactivate();
                }
            }
        }
    }
}
