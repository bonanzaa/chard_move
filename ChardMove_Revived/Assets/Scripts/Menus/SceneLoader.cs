using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ChardMove
{
    public class SceneLoader : MonoBehaviour
    {
        private int ogScene;

        public static SceneLoader Instance;
        public delegate void SceneLoaded();
        public static event SceneLoaded sceneLoaded;

        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
            ogScene = GetCurrentSceneIndex();
            sceneLoaded += OnSceneChanged;
        }
        public void SceneChange()
        {
            sceneLoaded();
        }
        public void OnSceneChanged()
        {
            LoadScene(1);
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
        private IEnumerator LoadBuffer(float timer)
        {
            while (timer > 0)
            {
                timer -= Time.deltaTime;
                yield return null;
            }
            yield break;
        }
    }
}
