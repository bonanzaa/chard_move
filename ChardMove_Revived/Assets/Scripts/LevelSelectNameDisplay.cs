using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace ChardMove
{
    public class LevelSelectNameDisplay : MonoBehaviour
    {
        [SerializeField] private List<GameObject> _levelButtons;

        private TextMeshProUGUI _textMeshProUGUI;
        private LevelLoader _levelLoader;
        private void Awake()
        {
            _levelLoader = LevelLoader.Instance;
            AssignNames();
            //GetAllChildren();
        }

        private void AssignNames()
        {
            int levelCount = 0;
            for (int i = 0; i < _levelButtons.Count; i++)
            {
                _textMeshProUGUI = _levelButtons[i].GetComponentInChildren<TextMeshProUGUI>();
                if(levelCount < _levelLoader.Levels.Count)
                {
                    _textMeshProUGUI.text = _levelLoader.Levels[levelCount].GetComponent<CardContainer>().LevelName;
                    levelCount++;
                }
            }
        }

        //private void GetAllChildren()
        //{
        //    List<GameObject> levelButtons = new List<GameObject>();
        //    for (int i = 0; i < _levelBatches.Count; i++)
        //    {
        //        foreach (GameObject child in _levelBatches)
        //        {
        //            levelButtons.Add(child);
        //            foreach (GameObject childText in levelButtons)
        //            {

        //                _textMeshProUGUI = childText.GetComponent<TextMeshProUGUI>();
        //                _textMeshProUGUI.text = ahhh
        //            }
        //        }
        //    }
        //}

        ////Can also use for images, each grid containing it's own pic
        //private void AssignNames(string name)
        //{

        //}
    }
}
