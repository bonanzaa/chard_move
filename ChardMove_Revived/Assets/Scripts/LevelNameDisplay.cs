using ChardMove.gameManager;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace ChardMove
{
    public class LevelNameDisplay : MonoBehaviour
    {
        [SerializeField] private GameObject _levelNameDisplay;
        
        private LevelLoader _levelLoader;
        private TextMeshProUGUI _textMeshProUGUI;
        private void Awake()
        {
            _textMeshProUGUI = _levelNameDisplay.GetComponent<TextMeshProUGUI>();
            _levelLoader = LevelLoader.Instance;
            GameManager.onNewLevelLoaded += OnLevelLoad;
        }

        private void OnLevelLoad()
        {
            CardContainer cardContainer = _levelLoader.Levels[LevelLoader.SceneIndex].GetComponent<CardContainer>();
            if (cardContainer != null)
            {
                DisplayLevelName(cardContainer.LevelName);
            }
            else
            {
                DisplayLevelName("Level name not found");
            }
        }
        private void DisplayLevelName(string name)
        {
            _textMeshProUGUI.text = name;
        }
    }
}
