using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ChardMove
{
    public class SceneLoader : MonoBehaviour
    {
        public void LoadLevel (int levelIndex)
        {
            SceneManager.LoadScene(levelIndex);
        }
        public void GoToMainMenu()
        {
            SceneManager.LoadScene(0);
        }
        
    }
}
