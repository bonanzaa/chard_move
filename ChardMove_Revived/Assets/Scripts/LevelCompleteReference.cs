using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChardMove
{
    public class LevelCompleteReference : MonoBehaviour
    {
        [SerializeField] private GameObject _winScreenUI;
        [SerializeField] private float _winScreenTimer;
        [SerializeField] private GameObject _winParticleSystem;
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
            GameObject particles = Instantiate(_winParticleSystem, transform.position, Quaternion.identity);
            StartCoroutine(nameof(Countdown));
            _winScreenUI.SetActive(true);
            Time.timeScale = 0 ;
            Destroy(particles);
        }
        public void MainMenuButtonPressed()
        {
            SceneLoader.Instance.GoToMainMenu();
            Time.timeScale = 1;
        }
        public void NextLevelButtonPressed()
        {
            _winScreenUI.SetActive(false);
            Time.timeScale = 1;
            nextLevel();
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
