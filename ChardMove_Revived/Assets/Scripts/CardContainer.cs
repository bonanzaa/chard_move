using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ChardMove.gameManager;

namespace ChardMove
{
    public class CardContainer : MonoBehaviour
    {
<<<<<<< HEAD
<<<<<<< HEAD
        [SerializeField] private List<int> _amountOfCards = new List<int>(4);
=======
        public List<GameObject> Cards;
>>>>>>> parent of 728bb82 (Working on UI / Card loading)

        private void Start()
        {
            GameObject _cardSpace = CardSpaceMarker.Instance.gameObject;
            for (int i = 0; i < Cards.Count; i++)
            {
<<<<<<< HEAD
                int cardCount = _amountOfCards[i];
                if(_amountOfCards[i] == 0)
                {
                    continue;
                }
                while(cardCount > 0)
                {
                    Instantiate(BaseCards[i], _cardSpace.transform.position, Quaternion.identity, _cardSpace.transform);
                    cardCount--;
                }
=======
        public int OneTileCardCount;
        public int TwoTileCardCount;
        public int ThreeTileCardCount;
        public int FourTileCardCount;
        private List<GameObject> _cardList = new List<GameObject>();
        private UnityEngine.Object[] _prefabListObject;
        private List<GameObject> _prefabList =  new List<GameObject>();

        private void Awake() {
            _prefabListObject =  Resources.LoadAll("CardPrefabs");
            foreach (var item in _prefabListObject)
            {
                _prefabList.Add((GameObject)item);
            }
            PassCardList();
        }

        private void Start() {
        }

        private void PassCardList(){
            for (int i = 0; i < OneTileCardCount; i++)
            {
                _cardList.Add(_prefabList[0]);
            }
            for (int i = 0; i < TwoTileCardCount; i++)
            {
                _cardList.Add(_prefabList[1]);
            }
            for (int i = 0; i < ThreeTileCardCount; i++)
            {
                _cardList.Add(_prefabList[2]);
            }
            for (int i = 0; i < FourTileCardCount; i++)
            {
                _cardList.Add(_prefabList[3]);
            }

            foreach (var item in _cardList)
            {
                GameManager.Instance._tempPlayerCards.Add(item.GetComponent<Draggable>());
            }

            if(!GameManager.Instance.LevelLoaded){
                GameManager.Instance.LoadLevel(this.gameObject);
>>>>>>> 7680ab59b76b3e6107f14b68097733e390572c41
=======
                Instantiate(Cards[i], _cardSpace.transform.position, Quaternion.identity, _cardSpace.transform);    
>>>>>>> parent of 728bb82 (Working on UI / Card loading)
            }
        }
    }
}
