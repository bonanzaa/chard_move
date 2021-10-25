using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ChardMove.gameManager;
using System;

namespace ChardMove
{
    public class LevelLoader : MonoBehaviour
    {
        public static GameObject CurrentLevel;
        public static LevelLoader Instance;
        public static int LevelIndex;
        public int GetLevelCount { get => Levels.Count;}
        public bool CanLoadLevel = true;
        public List<GameObject> Levels;

        private SaveSystem _saveSystem = new SaveSystem();
        private LevelCompleteReference _levelCompleteReference;
        
        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
            if(Instance != null){
                Destroy(Instance.gameObject);
                Instance = this;
            }else{
                Instance = this;
            }
            _saveSystem.Deserialize();
            LevelIndex = _saveSystem.RefreshLvlIndex();
            WinTile.playerWin += OnPlayerWin;
            //SceneMarker.sceneMarked += OnSceneLoaded;

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

        private void OnDestroy() {
            WinTile.playerWin -= OnPlayerWin;
        }
        public void CacheLevelCompleteReference(LevelCompleteReference levelCompleteReference)
        {
            _levelCompleteReference = levelCompleteReference;
        }
        //private void Start()
        //{
        //    CatchReferences();
        //}

        //private void CatchReferences()
        //{
        //    _sceneLoader = SceneLoader.Instance;
        //    _cardContainer = CardSpaceMarker.Instance;
        //    if (_winScreenUI != null)
        //    {
        //        _winScreenUI = WinScreen.Instance.gameObject;
        //        _winScreenUI.SetActive(false);
        //    }
        //}

        //private void OnSceneLoaded()
        //{
        //    LoadLevel(LevelIndex);
        //    ////Click button event getting called before changing Scene to soooooooooooooooooooooooooooooooooooon
        //    //CatchReferences();
        //    //Awake();
        //}
        //private void OnDisable()
        //{
        //    SceneLoader.sceneLoaded -= OnSceneLoaded;
        //    SceneMarker.sceneMarked -= OnSceneLoaded;
        //}
        public void OnSelectedLevelLoad(int index)
        {
            LevelIndex = index;
            _saveSystem.Serialize();

        }
        private void OnPlayerWin()
        {
            LevelIndex++;
            _saveSystem.Serialize();
            _levelCompleteReference.OpenWinScreen();
        }
        //public void LoadLastSavedLevel()
        //{
        //    LoadLevel(LevelIndex);
        //}
        //private void LoadLevel(int index)
        //{
        //    //if(_currentLevel != null)
        //    //{
        //    //    Destroy(_currentLevel);
        //    //}
        //    GameManager.Instance.ClearDictionaries();
        //    GameManager.Instance.DeletePlayerCards();
        //    //StartCoroutine(LoadBuffer(2));
        //    GameObject newLevel = Levels[index];
        //    CurrentLevel = newLevel;
        //    print("loading level");
        //    //print(index);
        //    //_currentLevelGrid = instance;
        //    CurrentLevel = Instantiate(newLevel,transform.position,Quaternion.identity);

        //}
        
        //private IEnumerator LoadBuffer(float timer)
        //{
        //    while (timer > 0)
        //    {
        //        timer -= Time.deltaTime;
        //        yield return null;
        //    }
        //    yield break;
        //}
    }
}
