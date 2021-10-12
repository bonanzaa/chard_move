using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChardMove
{
    public class CardContainer : MonoBehaviour
    {
        public List<GameObject> Cards;

        private void Start()
        {
            GameObject _cardSpace = CardSpaceMarker.Instance.gameObject;
            for (int i = 0; i < Cards.Count; i++)
            {
                Instantiate(Cards[i], _cardSpace.transform.position, Quaternion.identity, _cardSpace.transform);    
            }
        }
    }
}
