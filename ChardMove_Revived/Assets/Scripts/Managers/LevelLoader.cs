using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChardMove
{
    public class LevelLoader : MonoBehaviour
    {
        [SerializeField] private List<GameObject> _levels;
        
        private GameObject _currentLevelGrid;
        private int _levelIndex = 0;
        
        public delegate void LevelFinished();
        public static event LevelFinished onLevelfinished;
        
        private void Awake()
        {
            //Have to pass the index of the current level fomr the lvl selection screen
            WinTile.playerWin += OnPlayerWin;
            if(_currentLevelGrid == null)
            {
                _currentLevelGrid = _levels[_levelIndex];
            }
        }

        private void OnPlayerWin()
        {
            _levelIndex++;
            LoadLevel(_levels[_levelIndex]);
        }
        private void Start()
        {
            
        }
        private void Update()
        {

        }
        private void LoadLevel(GameObject grid)
        {
            Destroy(_currentLevelGrid);
            //_currentLevelGrid.SetActive(false);
            //_levels.Remove(_currentLevelGrid);
            StartCoroutine(LoadBuffer(2));
            _currentLevelGrid = _levels[_levelIndex];

            Instantiate(_currentLevelGrid,transform.position,Quaternion.identity);
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
