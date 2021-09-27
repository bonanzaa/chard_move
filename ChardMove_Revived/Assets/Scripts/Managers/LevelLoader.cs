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
        
        private void Awake()
        {
            WinTile.playerWin += OnPlayerWin;
            _currentLevel = Instantiate(_levels[_levelIndex], transform.position, Quaternion.identity);
        }

        private void OnPlayerWin()
        {
            _levelIndex++;
            LoadLevel(_levelIndex);
        }
        private void LoadLevel(int index)
        {
            Destroy(_currentLevel);
            GameManager.Instance.ClearDictionaries();
            GameManager.Instance.DeletePlayerCards();
            //StartCoroutine(LoadBuffer(2));
            GameObject newLevel = _levels[index];
            _currentLevel = newLevel;
            //_currentLevelGrid = instance;
            _currentLevel = Instantiate(newLevel,transform.position,Quaternion.identity);
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
