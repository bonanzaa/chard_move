using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace ChardMove
{
    public class DirectionalButtonClick : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        public delegate void ButtonPressed(bool pressed);
        public static event ButtonPressed onButtonPressed;
 
        public void OnPointerDown(PointerEventData eventData){
            onButtonPressed(true);
        }
 
        public void OnPointerUp(PointerEventData eventData){
            onButtonPressed(false);
        }
    }
}
