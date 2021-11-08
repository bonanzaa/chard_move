using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChardMove
{
    public class ButtonEvent : MonoBehaviour
    {
        private SoundManager _instance;

        public delegate void ButtonPressed();
        public static event ButtonPressed onButtonPressed;
        private void Awake()
        {
            _instance = SoundManager.Instance;
        }
        public void OnButtonPressed()
        {
            onButtonPressed();
        }
    }
}
