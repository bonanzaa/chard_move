using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChardMove
{
    public class WinScreen : MonoBehaviour
    {
        public static WinScreen Instance;

        private void Awake()
        {
            if(Instance != null)
            {
                Destroy(this);
            }
            else
            {
                Instance = this;
            }
        }
    }
}
