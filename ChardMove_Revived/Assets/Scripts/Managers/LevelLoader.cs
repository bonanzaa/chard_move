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
        private SceneLoader _sceneLoader;
        
        private void Awake()
        {
            _sceneLoader = GetComponent<SceneLoader>();
            DontDestroyOnLoad(gameObject);
            if(Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
            _saveSystem.Deserialize();
            LevelIndex = _saveSystem.RefreshLvlIndex();
            //Debug.Log($"Current Level INdex is:{LevelIndex}");
            WinTile.playerWin += OnPlayerWin;
            SaveSystem.onProgressCleared += OnProgressCleared;
            //SceneMarker.sceneMarked += OnSceneLoaded;

        }

        private void OnProgressCleared()
        {
            LevelIndex = _saveSystem.RefreshLvlIndex();
        }

        private void OnDestroy() {
            WinTile.playerWin -= OnPlayerWin;
        }
        public void CacheLevelCompleteReference(LevelCompleteReference levelCompleteReference)
        {
            _levelCompleteReference = levelCompleteReference;
        }

        public void OnSelectedLevelLoad(int index)
        {
            LevelIndex = index;
            _saveSystem.Serialize();
        }
        private void OnPlayerWin()
        {
            if(LevelIndex < Levels.Count - 1)
            {
                LevelIndex++;
            }
            else if (LevelIndex >= Levels.Count)
            {
              
                _sceneLoader.GoToMainMenu();
            }
            _saveSystem.Serialize();
            _saveSystem.Deserialize();
            _levelCompleteReference.OpenWinScreen();
        }
        private void CheckForLatyLevelIndex() { }
    }
}
