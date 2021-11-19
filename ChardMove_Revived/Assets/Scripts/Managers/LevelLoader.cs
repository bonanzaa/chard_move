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
        public static int SceneIndex;
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
            SceneIndex = _saveSystem.LastSceneIndex;
            WinTile.playerWin += OnPlayerWin;
            SaveSystem.onProgressCleared += OnProgressCleared;

        }
        private void OnProgressCleared()
        {
            Debug.Log($"Level index is{SceneIndex}");
            SceneIndex = 1;
            Debug.Log($"Level index is{SceneIndex} after refreshing");
        }

        private void OnDestroy() {
            WinTile.playerWin -= OnPlayerWin;
            SaveSystem.onProgressCleared -= OnProgressCleared;
        }
        public void CacheLevelCompleteReference(LevelCompleteReference levelCompleteReference)
        {
            _levelCompleteReference = levelCompleteReference;
        }

        public void OnSelectedLevelLoad(int index)
        {
            SceneIndex = index;
            Debug.Log(SceneIndex);
            _saveSystem.Serialize();
        }
        private void OnPlayerWin()
        {
            if(SceneIndex < Levels.Count - 1)
            {
                SceneIndex++;
            }
            else if (SceneIndex >= Levels.Count)
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
