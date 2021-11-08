using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ChardMove
{
    public class MouseoverHighlightMovingTiles : MonoBehaviour,IPointerExitHandler, IPointerEnterHandler
    {
        public MovingTile MovingTile;
        public float AlphaThreshold = 1;

        private void Start() {
             this.GetComponent<Image>().alphaHitTestMinimumThreshold = AlphaThreshold;
        }

        public void OnPointerEnter(PointerEventData data){
            MovingTile.ShowGhost();
        }

        public void OnPointerExit(PointerEventData data){
            MovingTile.HideGhost();
        }

    }
}
