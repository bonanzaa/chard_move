using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using ChardMove.gameManager;

namespace ChardMove
{
    public class Draggable : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        public TMP_Text DistanceText;
        public GameObject BigIcon;
        public GameObject SmallIcon;
        public TMP_Text SmallIconText;
        public int Distance;
        public delegate void BeginDrag(int distance);
        public static event BeginDrag onBeginDrag;
        public delegate void EndDrag(int distance);
        public static event EndDrag onEndDrag;
        private Transform _originalParent;
        private int _originalSiblingIndex;
        private Canvas _canvas;

        private void Awake() {
            _canvas = GameObject.FindGameObjectWithTag("Canvas").GetComponent<Canvas>();
            DistanceText.text = Distance.ToString();
            SmallIconText.text = Distance.ToString();
        }
        private void Start() {
            // add ourselves to the TileTB;
            GameManager.Instance.AddMeToCardList(this);
            
        }
        public void OnBeginDrag(PointerEventData data){
            // cache our index in the hierarchy
            _originalSiblingIndex = transform.GetSiblingIndex();
            // cache our original parent
            _originalParent = this.transform.parent;
            
            BigIcon.SetActive(false);
            SmallIcon.SetActive(true);

            // reparent ourselves to canvas, so that 
            // CardSpace's horizontal layout group could be updated
            this.transform.SetParent(this.transform.parent.parent.parent);
            // we don't block raycasts from camera - used by OnPointerEnter and OnDrop for 
            // the bot. Blocking raycasts makes all other UI events not fire (they are not detected).
            GetComponent<CanvasGroup>().blocksRaycasts = false;
            // make ourselves a tad bit smaller
            transform.localScale = new Vector3(0.8f,0.8f,0.8f);
            onBeginDrag(Distance);
        }
        public void OnDrag(PointerEventData data){
            // move with the cursor
            Vector3 position = data.position;
            position.z = _canvas.planeDistance;
            transform.position = _canvas.worldCamera.ScreenToWorldPoint(position);
        }
        public void OnEndDrag(PointerEventData data){
            // Gets called after OnDrop. So if we haven't dropped this card
            // onto a bot, then just snap it right back to its original position in
            // the hand.
            // Otherise - this card is set to SetActive(false) and this method never gets called
            transform.SetParent(_originalParent);
            transform.SetSiblingIndex(_originalSiblingIndex);
            BigIcon.SetActive(true);
            SmallIcon.SetActive(false);
            GetComponent<CanvasGroup>().blocksRaycasts = true;
            transform.localScale = new Vector3(1.4f,1.4f,1.4f);
            onEndDrag(Distance);
            GameManager.Instance.UndoDirectionalChoiceEvent();
        }
        public void ChangeParent() {
            // reparenting utility
            transform.SetParent(_originalParent);
            transform.SetSiblingIndex(_originalSiblingIndex);
            BigIcon.SetActive(true);
            SmallIcon.SetActive(false);
            GetComponent<CanvasGroup>().blocksRaycasts = true;
            transform.localScale = new Vector3(1.4f,1.4f,1.4f);
        }
    }
}
