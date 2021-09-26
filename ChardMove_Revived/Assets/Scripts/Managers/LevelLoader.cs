using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ChardMove.gameManager;

namespace ChardMove
{
    public class LevelLoader : MonoBehaviour
    {
        [SerializeField] private List<GameObject> _levels;
        private GameObject _currentLevel;

        private GameObject _previousLevel;
        private int _levelIndex = 0;
        
        public delegate void LevelFinished();
        public static event LevelFinished onLevelfinished;
        
        private void Awake()
        {
            GameObject previousLevel = _levels[_levelIndex];
            WinTile.playerWin += OnPlayerWin;

            _currentLevel = Instantiate(previousLevel, transform.position, Quaternion.identity);
            
        }

        private void OnPlayerWin()
        {
            Destroy(_currentLevel);
            _levelIndex++;
            GameObject nextLevel = _levels[_levelIndex];
            LoadLevel(nextLevel);
            Debug.Log(_levelIndex);
            Debug.Log(_levels.Count);
        }
        private void LoadLevel(GameObject grid)
        {
            GameManager.Instance.ClearDictionaries();

            //StartCoroutine(LoadBuffer(2));
            _currentLevel = grid;
            //_currentLevelGrid = instance;
            Instantiate(_currentLevel,transform.position,Quaternion.identity);
            print("Player won");

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
