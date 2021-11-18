using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ChardMove.gameManager;

namespace ChardMove
{
    public class CardContainer : MonoBehaviour
    {
        [Header("Level Name")]
        public string LevelName;
        [Header("Level Pic")]
        public Sprite LevelPic;

        [Header("Card Info")]
        public int OneTileCardCount;
        public int TwoTileCardCount;
        public int ThreeTileCardCount;
        public int FourTileCardCount;
        private List<GameObject> _cardList = new List<GameObject>();
        private UnityEngine.Object[] _prefabListObject;
        private List<GameObject> _prefabList =  new List<GameObject>();

        private void Awake() {
            _prefabListObject =  Resources.LoadAll("CardPrefabs");
            foreach (var item in _prefabListObject)
            {
                _prefabList.Add((GameObject)item);
            }
            PassCardList();
        }

        private void Start() {
            if(LevelSwitchAnimator.Instance != null){
                LevelSwitchAnimator.Instance.SetCurrentLevel(this.gameObject);
            }
            
        }

        private void PassCardList(){
            for (int i = 0; i < OneTileCardCount; i++)
            {
                _cardList.Add(_prefabList[0]);
            }
            for (int i = 0; i < TwoTileCardCount; i++)
            {
                _cardList.Add(_prefabList[1]);
            }
            for (int i = 0; i < ThreeTileCardCount; i++)
            {
                _cardList.Add(_prefabList[2]);
            }
            for (int i = 0; i < FourTileCardCount; i++)
            {
                _cardList.Add(_prefabList[3]);
            }
            foreach (var item in _cardList)
            {
                GameManager.Instance._tempPlayerCards.Add(item.GetComponent<Draggable>());
            }


            // if(!GameManager.Instance.LevelLoaded){
            //     GameManager.Instance.LoadLevel(this.gameObject);   
            // }
        }
    }
}
