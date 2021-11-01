using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ChardMove
{
    public class LevelSelectNameDisplay : MonoBehaviour
    {
        [SerializeField] private List<GameObject> _levelButtons;

        private Image _button;
        private SpriteRenderer _spriteRenderer;
        private TextMeshProUGUI _textMeshProUGUI;
        private LevelLoader _levelLoader;
        private void Awake()
        {
            _levelLoader = LevelLoader.Instance;
            AssignNames();
            AssignImage();
            //GetAllChildren();
        }
        private void AssignImage()
        {
            int count = 0;
            
            for (int i = 0; i < _levelButtons.Count; i++)
            {
                //_button = _levelLoader.Levels[count].GetComponent<Image>();
                _spriteRenderer = _levelButtons[i].GetComponent<SpriteRenderer>();
                if (count < _levelLoader.Levels.Count)
                {
                    //_button.sprite = _levelLoader.Levels[count].GetComponent<CardContainer>().LevelPic;
                    _spriteRenderer.sprite = _levelLoader.Levels[count].GetComponent<CardContainer>().LevelPic;
                    //_textMeshProUGUI.text = _levelLoader.Levels[count].GetComponent<CardContainer>().LevelName;
                    count++;
                }
            }
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
