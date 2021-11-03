using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ChardMove
{
    public class MouseoverHighlight : MonoBehaviour,IPointerExitHandler, IPointerEnterHandler
    {
        public GameObject Switch;
        public float AlphaThreshold = 1;
        private LatchSwitch _latch;
        private MomentarySwitch _momentary;
        private bool _useLatch = false;
        

        private void Awake() {
            if(Switch.TryGetComponent(out LatchSwitch latch)){
                _latch = latch;
                _useLatch = true;
            }else if(Switch.TryGetComponent(out MomentarySwitch momentary)){
                _momentary = momentary;
                _useLatch = false;
            }
        }

        private void Start() {
             this.GetComponent<Image>().alphaHitTestMinimumThreshold = AlphaThreshold;
        }
        public void OnPointerEnter(PointerEventData data){
            if(_useLatch){
                _latch.GetComponent<Tile>()._highlight.SetActive(true);
                if(_latch.Gates.Count != 0){
                    foreach (var item in _latch.Gates)
                    {
                        item._highlight.SetActive(true);
                    }
                }

                if(_latch.MovingPlatforms.Count != 0){
                    foreach (var item in _latch.MovingPlatforms)
                    {
                        item._highlight.SetActive(true);
                    }
                }
            }else{
                _momentary.GetComponent<Tile>()._highlight.SetActive(true);
                if(_momentary.Gates.Count !=0){
                    foreach (var item in _momentary.Gates)
                    {
                        item._highlight.SetActive(true);
                    }
                }

                if(_momentary.MovingPlatforms.Count != 0){
                    foreach (var item in _momentary.MovingPlatforms)
                    {
                        item._highlight.SetActive(true);
                    }
                }
            }
        }

         public void OnPointerExit(PointerEventData data){
            if(_useLatch){
                _latch.GetComponent<Tile>()._highlight.SetActive(false);
                if(_latch.Gates.Count != 0){
                    foreach (var item in _latch.Gates)
                    {
                        item._highlight.SetActive(false);
                    }
                }

                if(_latch.MovingPlatforms.Count != 0){
                    foreach (var item in _latch.MovingPlatforms)
                    {
                        item._highlight.SetActive(false);
                    }
                }
            }else{
                _momentary.GetComponent<Tile>()._highlight.SetActive(false);
                if(_momentary.Gates.Count !=0){
                    foreach (var item in _momentary.Gates)
                    {
                        item._highlight.SetActive(false);
                    }
                }

                if(_momentary.MovingPlatforms.Count != 0){
                    foreach (var item in _momentary.MovingPlatforms)
                    {
                        item._highlight.SetActive(false);
                    }
                }
            }
        }
    }
}
