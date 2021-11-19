using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChardMove
{
    public class LevelCompleteReference : MonoBehaviour
    {
        [SerializeField] private GameObject _winScreenUI;
        [SerializeField] private float _winScreenTimer;
        [SerializeField] private GameObject _resetButton;
        [SerializeField] private GameObject _pauseButton;
        private LevelLoader _levelLoader;
        private LevelSwitchAnimator _levelSwitchAnimator;

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
            if(LevelSwitchAnimator.Instance != null)
            {
                _levelSwitchAnimator = LevelSwitchAnimator.Instance;
            }
        }

        public void OpenWinScreen()
        {
            StartCoroutine(nameof(Countdown));
            _winScreenUI.SetActive(true);
            _resetButton.SetActive(false);
            _pauseButton.SetActive(false);
           Time.timeScale = 0 ;
        }
        public void MainMenuButtonPressed()
        {
            Time.timeScale = 1;
            SceneLoader.Instance.GoToMainMenu();
        }
        public void NextLevelButtonPressed()
        {
             if(_sceneLoader.GetCurrentSceneIndex() == 30)
             {
                _sceneLoader.GoToMainMenu();
             }
            _winScreenUI.SetActive(false);
            _resetButton.SetActive(true);
            _pauseButton.SetActive(true);
            _levelSwitchAnimator.LoadLevel(_sceneLoader.GetCurrentSceneIndex() + 1);
            Time.timeScale = 1;
            nextLevel();
        }
        public void GameMenuPressed()
        {
            Time.timeScale = 0;
        }
        public void GameMenuClosed()
        {
            Time.timeScale = 1;
        }
        private IEnumerator Countdown()
        {
            while (true)
            {
                yield return new WaitForSeconds(_winScreenTimer);
            }
        }
    }
}
