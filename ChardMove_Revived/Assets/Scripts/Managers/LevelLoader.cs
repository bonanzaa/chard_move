using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChardMove
{
    public class LevelLoader : MonoBehaviour
    {
        [SerializeField] private GameObject _currentLevelGrid;
        [SerializeField] private List<GameObject> _levels;

        private bool _levelFinished;
        private void Awake()
        {
            _levelFinished = false;
        }
        private void Start()
        {
            
        }
        private void Update()
        {

        }
        private void LoadLevel(GameObject grid)
        {

        }
    }
}
