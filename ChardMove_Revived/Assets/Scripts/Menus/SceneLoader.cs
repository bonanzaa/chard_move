using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ChardMove
{
    public class SceneLoader : MonoBehaviour
    {  
        public static SceneLoader Instance;
        public delegate void SceneLoaded();
        public static event SceneLoaded sceneLoaded;
        private void Awake()
        {
            Instance = this;
        }
        private void Update()
        {
            int oldScene = GetCurrentSceneIndex();
            if(oldScene != GetCurrentSceneIndex())
            {
                sceneLoaded();
            }
        }
        public int GetCurrentSceneIndex()
        {
            return SceneManager.GetActiveScene().buildIndex;
        }
        public void LoadScene (int levelIndex)
        {
            SceneManager.LoadScene(levelIndex);
        }
        public void GoToMainMenu()
        {
            SceneManager.LoadScene(0);
        }
        public void QuitButtonPressed()
        {
            Application.Quit();
        }

    }
}
