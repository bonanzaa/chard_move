using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ChardMove.gameManager;
using System;

namespace ChardMove
{
    public class LevelLoader : MonoBehaviour
    {
        [SerializeField] private List<GameObject> _levels;
        [SerializeField] private GameObject _winScreenUI;
        public static GameObject CurrentLevel;
        //[SerializeField] private List<GameObject> _deactivatedUI;

        private SceneLoader _sceneLoader;
        private CardSpaceMarker _cardContainer;
        private GameObject _previousLevel;

        public static int LevelIndex;
        public bool CanLoadLevel = true;
        public static LevelLoader Instance;
        
        private void Awake()
        {
            //DontDestroyOnLoad(gameObject);
            Instance = this;
            SceneLoader.sceneLoaded += OnSceneLoaded;
            WinTile.playerWin += OnPlayerWin;
            SceneMarker.sceneMarked += OnSceneLoaded;

            if (CanLoadLevel)
            {
                //CurrentLevel = _levels[LevelIndex];
                //print(CurrentLevel);
                //print(LevelIndex);
                //_currentLevel = Instantiate(_levels[LevelIndex], transform.position, Quaternion.identity);
                //LoadLevel(LevelIndex);
            }
            CanLoadLevel = true;
        }
        private void Start()
        {
            CatchReferences();
        }

        private void CatchReferences()
        {
            _sceneLoader = SceneLoader.Instance;
            _cardContainer = CardSpaceMarker.Instance;
            if (_winScreenUI != null)
            {
                _winScreenUI = WinScreen.Instance.gameObject;
                _winScreenUI.SetActive(false);
            }
        }

        private void OnSceneLoaded()
        {
            LoadLevel(LevelIndex);
            ////Click button event getting called before changing Scene to soooooooooooooooooooooooooooooooooooon
            //CatchReferences();
            //Awake();
        }
        private void OnDisable()
        {
            SceneLoader.sceneLoaded -= OnSceneLoaded;
            SceneMarker.sceneMarked -= OnSceneLoaded;
        }
        public void OnSelectedLevelLoad(int index)
        {
            LevelIndex = index;
        }
        private void OnPlayerWin()
        {
            _winScreenUI.SetActive(true);
            print("Player won");
        }
        private void LoadLevel(int index)
        {
            //if(_currentLevel != null)
            //{
            //    Destroy(_currentLevel);
            //}
            GameManager.Instance.ClearDictionaries();
            GameManager.Instance.DeletePlayerCards();
            //StartCoroutine(LoadBuffer(2));
            GameObject newLevel = _levels[index];
            CurrentLevel = newLevel;
            print("loading level");
            //print(index);
            //_currentLevelGrid = instance;
            CurrentLevel = Instantiate(newLevel,transform.position,Quaternion.identity);

        }
        private IEnumerator LoadBuffer(float timer)
        {
            while (timer > 0)
            {
                timer -= Time.deltaTime;
                yield return null;
            }
            yield break;
        }
    }
}
