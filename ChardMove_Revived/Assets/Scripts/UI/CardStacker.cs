using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ChardMove.gameManager;
using UnityEngine.UI;
namespace ChardMove
{
    public class CardStacker : MonoBehaviour
    {
        public static CardStacker Instance;
        public List<GameObject> Slots;
        public List<GameObject> CardCounters;


        private void Awake() {
            Instance = this;
            Draggable.onBeginDrag += Reorganize;
            Draggable.onEndDrag += Reorganize;
            GameManager.resetButtonPressed += OnReset;
            GameManager.undoButtonPressed += OnUndo;
            GameManager.undoDirectionalChoice  += OnUndo;
            foreach (var item in Slots)
            {
                item.SetActive(false);
            }
        }

        private void OnDestroy() {
            Draggable.onBeginDrag -= Reorganize;
            Draggable.onEndDrag -= Reorganize;
            GameManager.resetButtonPressed -= OnReset;
            GameManager.undoButtonPressed -= OnUndo;
            GameManager.undoDirectionalChoice -= OnUndo;
        }

        private void OnReset(){
            ClearCards();
            StartCoroutine(LoadTimer());
        }

        private void OnUndo(){
            if(GameManager.Instance.LastCardPlayed == null) return;
            SetupSlotsOnUndo();
            PopulateSlotOnUndo();
            Reorganize(GameManager.Instance.LastCardPlayed.Distance);
            GameManager.Instance.LastCardPlayed = null;
        }



        private IEnumerator LoadTimer(){
            yield return new WaitForEndOfFrame();
            // gotta wait a little, otherwise unity freaks out and
            // starts adding empty objects to the list
            LoadCards();
        }


        public void LoadCards() {
            SetupSlots();
            PopulateSlots();
            OrganizeSlots();
            ReorganizeAll();
        }

        public void ClearCards(){
            foreach (var item in Slots)
            {
                item.SetActive(false);
                item.GetComponent<SlotOrganizer>().ClearSlot();
            }
        }
        private void SetupSlots(){
            foreach (var item in GameManager.Instance._tempPlayerCards)
            {
                switch(item.Distance){
                    case 1:
                    Slots[0].SetActive(true);
                    break;

                    case 2:
                    Slots[1].SetActive(true);
                    break;

                    case 3:
                    Slots[2].SetActive(true);
                    break;

                    case 4:
                    Slots[3].SetActive(true);
                    break;
                }
            }
        }

        private void SetupSlotsOnUndo(){
            foreach (var item in GameManager.Instance.PlayerCards)
            {
                switch(item.Distance){
                    case 1:
                    Slots[0].SetActive(true);
                    break;

                    case 2:
                    Slots[1].SetActive(true);
                    break;

                    case 3:
                    Slots[2].SetActive(true);
                    break;

                    case 4:
                    Slots[3].SetActive(true);
                    break;
                }
            }
        }


        private void PopulateSlots(){
            foreach (var item in GameManager.Instance._tempPlayerCards)
            {
                switch(item.Distance){
                    case 1:
                    GameObject card = Instantiate(item,Slots[0].transform).gameObject;
                    GameManager.Instance.LoadCardsWithTween(card);
                    break;

                    case 2:
                    GameObject card1 = Instantiate(item,Slots[1].transform).gameObject;
                    GameManager.Instance.LoadCardsWithTween(card1);
                    break;

                    case 3:
                    GameObject card2 = Instantiate(item,Slots[2].transform).gameObject;
                    GameManager.Instance.LoadCardsWithTween(card2);
                    break;

                    case 4:
                    GameObject card3 = Instantiate(item,Slots[3].transform).gameObject;
                    GameManager.Instance.LoadCardsWithTween(card3);
                    break;
                }
            }
        }

        private void PopulateSlotOnUndo(){
            GameManager.Instance.LastCardPlayed.gameObject.SetActive(true);
        }

        private void OrganizeSlots(){
            foreach (var item in Slots)
            {
                item.GetComponent<SlotOrganizer>().OrganizeSlots();
            }
        }

        private void Reorganize(int distance){
            switch(distance){
                case 1:
                Slots[0].GetComponent<SlotOrganizer>().ReorganizeSlots();
                break;

                case 2:
                Slots[1].GetComponent<SlotOrganizer>().ReorganizeSlots();
                break;

                case 3:
                Slots[2].GetComponent<SlotOrganizer>().ReorganizeSlots();
                break;

                case 4:
                Slots[3].GetComponent<SlotOrganizer>().ReorganizeSlots();
                break;
            }
        }

        private void ReorganizeAll(){
            Slots[0].GetComponent<SlotOrganizer>().ReorganizeSlots();
            Slots[1].GetComponent<SlotOrganizer>().ReorganizeSlots();
            Slots[2].GetComponent<SlotOrganizer>().ReorganizeSlots();
            Slots[3].GetComponent<SlotOrganizer>().ReorganizeSlots();
        }
    }
}
