using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using ChardMove.gameManager;
using ChardMove.BotMovement;

namespace ChardMove
{
    public class DropZone : MonoBehaviour, IDropHandler, IPointerExitHandler, IPointerEnterHandler
    {
        public GameObject Bot;
        public Canvas _canvas;
        public GameObject Highlight;
        public GameObject DirectionChoiceMenu;
        private MovementDirection _direction = MovementDirection.None;
        private int _distance;
        private void Awake() {
            _canvas.worldCamera = Camera.main;
        }
        public void OnPointerEnter(PointerEventData data){
            Highlight.SetActive(true);
        }

        public void OnDrop(PointerEventData data){
            var draggable = data.pointerDrag.GetComponent<Draggable>();
            _distance = draggable.Distance;
            GameManager.Instance.LastCardPlayed = draggable;
            //GameManager.Instance.PlayerCards.Remove(draggable);
            //Destroy(data.pointerDrag.gameObject);
            draggable.ChangeParent();
            data.pointerDrag.gameObject.SetActive(false);
            draggable.transform.SetSiblingIndex(draggable.gameObject.transform.GetSiblingIndex());
            DirectionChoiceMenu.SetActive(true);
            StartCoroutine(WaitForDirection());
            
        }

        public void OnPointerExit(PointerEventData data){
            Highlight.SetActive(false);
        }

        private IEnumerator WaitForDirection(){
            while(_direction == MovementDirection.None){
                yield return null;
            }
            DirectionChoiceMenu.SetActive(false);
            Bot.GetComponent<BotGridMovement>().Move(_direction,_distance);
            _distance = 0;
            _direction = MovementDirection.None;
            yield break;
        }

        public void ChooseToMoveForward(){
            _direction = MovementDirection.Forward;
        }

        public void ChooseToMoveBackward(){
            _direction = MovementDirection.Backward;
        }

        public void ChooseToMoveLeft(){
            _direction = MovementDirection.Left;
        }

        public void ChooseToMoveRight(){
            _direction = MovementDirection.Right;
        }
    }
}
