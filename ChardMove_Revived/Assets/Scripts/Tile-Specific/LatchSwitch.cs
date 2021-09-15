using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChardMove
{
    public class LatchSwitch : MonoBehaviour
    {
        public List<Roadblock> Gates;
        private void OnTriggerEnter2D(Collider2D other) {
            if(other.CompareTag("Bot")){
                foreach (var gate in Gates)
                {
                    gate.Activate();
                }
            }
        }
        
    }
}
