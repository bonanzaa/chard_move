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
        [SerializeField] private TMP_Text _cardCountText;
        public void OrganizeSlots(){
            _ActiveChildren.Clear();
            int activeChildCount = 0;
            foreach (Transform item in transform)
            {
                if(item.gameObject.activeSelf && item != null && !item.TryGetComponent(out TMP_Text text)){
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

        private void FixedUpdate() {
            _cardCountText.text = _ActiveChildren.Count.ToString();
        }

        public void ReorganizeSlots(){
            foreach (RectTransform item in transform)
            {
                if(!item.gameObject.TryGetComponent(out TMP_Text text))
                    item.localPosition = new Vector3(0,0,0);
            }
            OrganizeSlots();
        }

        public void ClearSlot(){
            foreach (Transform item in transform)
            {
                if(!item.gameObject.TryGetComponent(out TMP_Text text))
                    Destroy(item.gameObject);
            }
            _ActiveChildren.Clear();
        }
    }
}
