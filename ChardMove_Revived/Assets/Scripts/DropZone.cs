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
        public GameObject Highlight;
        public GameObject DirectionChoiceMenu;

        public delegate void DirectionChoiceActive();
        public static event DirectionChoiceActive directionChoiceActive;
        public delegate void DirectionChoiceInactive();
        public static event DirectionChoiceActive directionChoiceInactive;
        private MovementDirection _direction = MovementDirection.None;
        private Canvas _canvas;
        private int _distance;
        private int _goSiblingIndex;
        private bool _choosing = false;
        private void Awake() 
        {
            _canvas = GameObject.FindGameObjectWithTag("Canvas").GetComponent<Canvas>();
            _canvas.worldCamera = Camera.main;
        }

        private void Start() {
            DropZone.directionChoiceActive += OnDirectionChoiceActive;
            DropZone.directionChoiceInactive += OnDirectionChoiceInactive;
        }
        public void OnPointerEnter(PointerEventData data){
            Highlight.SetActive(true);
        }

        public void OnDrop(PointerEventData data){
            if(_choosing) return;
            _choosing = true;
            var draggable = data.pointerDrag.GetComponent<Draggable>();
            _distance = draggable.Distance;
            GameManager.Instance.LastCardPlayed = draggable;
            draggable.ChangeParent();
            int goSiblingIndex = data.pointerDrag.gameObject.transform.GetSiblingIndex();
            data.pointerDrag.gameObject.SetActive(false);
            draggable.transform.SetSiblingIndex(goSiblingIndex);
            DirectionChoiceMenu.SetActive(true);
            StartCoroutine(WaitForDirection());
            
        }

        private void OnDisable() {
            DropZone.directionChoiceActive -= OnDirectionChoiceActive;
            DropZone.directionChoiceInactive -= OnDirectionChoiceInactive;
        }

        private void OnDirectionChoiceActive(){
            CanvasGroup canvasGroup = GetComponent<CanvasGroup>();
            canvasGroup.blocksRaycasts = false;
        }

        private void OnDirectionChoiceInactive(){
            CanvasGroup canvasGroup = GetComponent<CanvasGroup>();
            canvasGroup.blocksRaycasts = true;
        }

        public void OnPointerExit(PointerEventData data){
            Highlight.SetActive(false);
        }

        private IEnumerator WaitForDirection(){
            DropZone.directionChoiceActive -= OnDirectionChoiceActive;
            directionChoiceActive();
            while(_direction == MovementDirection.None){
                yield return null;
            }
            DropZone.directionChoiceInactive -= OnDirectionChoiceInactive;
            directionChoiceInactive();
            _choosing = false;
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
