using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ChardMove.gameManager;

namespace ChardMove
{
    public class ResetButtonCache : MonoBehaviour
    {
        private Button _button;

        private void Awake() {
            _button =  GetComponent<Button>();
        }

        private void Start() {
            _button.onClick.AddListener(GameManager.Instance.Reset);
        }
    }
}
