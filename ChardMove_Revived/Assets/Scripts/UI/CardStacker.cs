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

        private void Awake() {
            Instance = this;
            Draggable.onBeginDrag += Reorganize;
            Draggable.onEndDrag += Reorganize;
            foreach (var item in Slots)
            {
                item.SetActive(false);
            }
        }

        public void LoadCards() {
            SetupSlots();
            PopulateSlots();
            OrganizeSlots();
        }

        private void SetupSlots(){
            print("Setting up slots");
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

        private void PopulateSlots(){
            print($"Populating slots with {GameManager.Instance._tempPlayerCards.Count} cards");
            foreach (var item in GameManager.Instance._tempPlayerCards)
            {
                switch(item.Distance){
                    case 1:
                    Instantiate(item,Slots[0].transform);
                    break;

                    case 2:
                    Instantiate(item,Slots[1].transform);
                    break;

                    case 3:
                    Instantiate(item,Slots[2].transform);
                    break;

                    case 4:
                    Instantiate(item,Slots[3].transform);
                    break;
                }
            }
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
    }
}
