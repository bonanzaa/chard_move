using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChardMove
{
    public class CardContainer : MonoBehaviour
    {
        [SerializeField] private List<int> _amountOfCards = new List<int>(4);

        private GameObject _cardSpace;

        public List<GameObject> BaseCards;
        private void Start()
        {
            _cardSpace = CardSpaceMarker.Instance.gameObject;
            //for (int i = 0; i < BaseCards.Count; i++)
            //{
            //    Instantiate(BaseCards[i], _cardSpace.transform.position, Quaternion.identity, _cardSpace.transform);    
            //}
            CreateCards();
        }
        private void CreateCards()
        {
            for (int i = 0; i <_amountOfCards.Count; i++)
            {
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
            }
        }
    }
}
