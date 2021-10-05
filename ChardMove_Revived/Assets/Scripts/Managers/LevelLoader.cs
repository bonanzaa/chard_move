using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ChardMove.gameManager;

namespace ChardMove
{
    public class LevelLoader : MonoBehaviour
    {
        [SerializeField] private List<GameObject> _levels;
        [SerializeField] private GameObject _winScreenUI;
        //[SerializeField] private List<GameObject> _deactivatedUI;

        private SceneLoader _sceneLoader;
        private CardSpaceMarker _cardContainer;

        private GameObject _currentLevel;
        private GameObject _previousLevel;
        public static int LevelIndex;

        public bool CanLoadLevel = false;
        public static LevelLoader Instance;
        
        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
            Instance = this;
            _sceneLoader = SceneLoader.Instance;
            _cardContainer = CardSpaceMarker.Instance;
            if(_winScreenUI != null)
            {
                _winScreenUI.SetActive(false);
            }
            WinTile.playerWin += OnPlayerWin;

            if(_sceneLoader.GetCurrentSceneIndex() != 0)
            {

                _currentLevel = Instantiate(_levels[LevelIndex], transform.position, Quaternion.identity);
            }
            //if(_sceneLoader.GetCurrentSceneIndex() == 0)
            //{
            //    CanLoadLevel = false;
            //}
        }
        private void OnSceneLoaded()
        {

        }
        public void OnSelectedLevelLoad(int index)
        {
            LevelIndex = index;
            LoadLevel(index);
        }
        private void OnPlayerWin()
        {
            _winScreenUI.SetActive(true);
            print("Player won");
            
            ////check for next level button input, pass value, add a continue button on main menu
            //LevelIndex++;
            ////if(I add bool check for scene && button input))
            //LoadLevel(LevelIndex);
        }
        private void LoadLevel(int index)
        {
            if(_currentLevel != null)
            {
                Destroy(_currentLevel);
            }
            else if (_sceneLoader.GetCurrentSceneIndex() == 0)
            {
                _sceneLoader.LoadScene(1);
                return;
            }
            GameManager.Instance.ClearDictionaries();
            GameManager.Instance.DeletePlayerCards();
            //StartCoroutine(LoadBuffer(2));
            GameObject newLevel = _levels[index];
            _currentLevel = newLevel;
            //_currentLevelGrid = instance;
            _currentLevel = Instantiate(newLevel,transform.position,Quaternion.identity);

        }
        private IEnumerator LoadBuffer(float timer)
        {
            while(timer > 0)
            {
                timer -= Time.deltaTime;
                yield return null;
            }
            yield break;
        }
    }
}
