using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChardMove
{
    public class StartButton : MonoBehaviour
    {
        public void OnStartButtonPressed()
        {
            SceneLoader.Instance.LoadScene(1);
        }
    }
}
