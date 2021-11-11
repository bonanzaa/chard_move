using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ChardMove.gameManager;
using TMPro;

namespace ChardMove
{
    public class SlotOrganizer : MonoBehaviour
    {
        public List<Transform> _ActiveChildren = new List<Transform>();
        public TMP_Text CardCountText;

        private void Awake() {
            GameManager.onNewLevelLoaded += DeleteInactiveCards;
            for (int i = 0; i < transform.childCount; i++)
            {
                if(!transform.GetChild(i).gameObject.activeSelf){
                    Destroy(transform.GetChild(i).gameObject);
                }
            }
            OrganizeSlots();
            ReorganizeSlots();
        }

        private void OnDestroy() {
            GameManager.onNewLevelLoaded -= DeleteInactiveCards;
        }

        public void DeleteInactiveCards(){
            for (int i = 0; i < transform.childCount; i++)
            {
                if(!transform.GetChild(i).gameObject.activeSelf){
                    Destroy(transform.GetChild(i).gameObject);
                }
            }
            OrganizeSlots();
            ReorganizeSlots();
        }

        private void RemoveInactive(){
            for (int i = 0; i < transform.childCount; i++)
            {
                if(!transform.GetChild(i).gameObject.activeSelf){
                    Destroy(transform.GetChild(i).gameObject);
                }
            }
        }

        private void Update() {
            CardCountText.text = string.Format($"x{_ActiveChildren.Count}");
        }
        public void OrganizeSlots(){
            _ActiveChildren.Clear();
            RemoveInactive();
            int activeChildCount = 0;
            foreach (Transform item in transform)
            {
                if(item.CompareTag("cardCount")) continue;
                if(item.gameObject.activeSelf && item != null){
                    activeChildCount ++;
                    _ActiveChildren.Add(item);
                }else if(item != null){
                    _ActiveChildren.Remove(item);
                }
            }

            if(activeChildCount == 0){
                this.gameObject.SetActive(false);
            }else{
                this.gameObject.SetActive(true);
            }

            if(activeChildCount == 1){
                _ActiveChildren[0].gameObject.GetComponent<RectTransform>().localPosition = new Vector3(0,0,0);
                return;
            }
            Vector3 offset = new Vector3();
            int childInd = 0;
            int count = 0;

            if(transform.childCount > 3){
                count = 3;
            }else{
                count = transform.childCount;
            }
            for (int i = 1; i < count; i++)
            {
                if(transform.GetChild(i).CompareTag("cardCount")) continue;
                childInd = i;
                RectTransform currentChild = transform.GetChild(childInd).GetComponent<RectTransform>();
                if(currentChild.TryGetComponent(out TMP_Text text)){
                    continue;
                }else{
                    Vector3 _plusOffset = new Vector3(-17,17,0);
                    offset += _plusOffset;
                    currentChild.localPosition += offset;
                    currentChild.SetSiblingIndex(0);
                }
            }
        }
        public void ReorganizeSlots(){
            foreach (RectTransform item in transform)
            {
                if(item.CompareTag("cardCount")) continue;
                item.localPosition = new Vector3(0,0,0);
            }
            OrganizeSlots();
        }

        public void ClearSlot(){
            foreach (Transform item in transform)
            {
                if(item.CompareTag("cardCount")) continue;
                Destroy(item.gameObject);
            }
            _ActiveChildren.Clear();
        }
    }
}
