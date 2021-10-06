using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ChardMove.gameManager;

namespace ChardMove
{
    public class CardSpaceMarker : MonoBehaviour
    {
        public static CardSpaceMarker Instance;

        private void Start()
        {
            Instance = this;
        }
    }
}
