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
            _originalSiblingIndex = transform.GetSiblingIndex();
            _originalParent = this.transform.parent;
            BigIcon.SetActive(false);
            SmallIcon.SetActive(true);
            this.transform.SetParent(this.transform.parent.parent);
            GetComponent<CanvasGroup>().blocksRaycasts = false;
            transform.localScale = new Vector3(0.8f,0.8f,0.8f);
        }
        public void OnDrag(PointerEventData data){
            Vector3 position = data.position;
            position.z = _canvas.planeDistance;
            transform.position = _canvas.worldCamera.ScreenToWorldPoint(position);
        }
        public void OnEndDrag(PointerEventData data){
            transform.SetParent(_originalParent);
            transform.SetSiblingIndex(_originalSiblingIndex);
            BigIcon.SetActive(true);
            SmallIcon.SetActive(false);
            GetComponent<CanvasGroup>().blocksRaycasts = true;
            transform.localScale = new Vector3(1,1,1);
        }
        public void ChangeParent() {
            transform.SetParent(_originalParent);
            BigIcon.SetActive(true);
            SmallIcon.SetActive(false);
            GetComponent<CanvasGroup>().blocksRaycasts = true;
            transform.localScale = new Vector3(1,1,1);
        }
    }
}
