using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChardMove
{
    public class LevelCompleteReference : MonoBehaviour
    {
        [SerializeField] private GameObject _winScreenUI;

        private LevelLoader _levelLoader;

        public delegate void NextLevel();
        public static event NextLevel nextLevel;
        public static LevelCompleteReference Instance;

        private SceneLoader _sceneLoader;
        private void Awake()
        {
            if(SceneLoader.Instance != null){
                _sceneLoader = SceneLoader.Instance;
            }
            if(LevelLoader.Instance != null){
                _levelLoader = LevelLoader.Instance;
                _levelLoader.CacheLevelCompleteReference(this);
            }
        }
        public void OpenWinScreen()
        {
            _winScreenUI.SetActive(true);
            Time.timeScale = 0 ;
        }
        public void MainMenuButtonPressed()
        {
            SceneLoader.Instance.GoToMainMenu();
        }
        public void NextLevelButtonPressed()
        {
            _winScreenUI.SetActive(false);
            Time.timeScale = 1;
            nextLevel();
        }
    }
}
