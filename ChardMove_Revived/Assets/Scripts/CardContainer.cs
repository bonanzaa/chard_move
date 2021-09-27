using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChardMove
{
    public class CardContainer : MonoBehaviour
    {
        [SerializeField] private List<GameObject> _cards;

        private void Start()
        {
            GameObject _cardSpace = CardSpaceMarker.Instance.gameObject;
            for (int i = 0; i < _cards.Count; i++)
            {
                Instantiate(_cards[i], _cardSpace.transform.position, Quaternion.identity, _cardSpace.transform);    
            }
        }
    }
}
