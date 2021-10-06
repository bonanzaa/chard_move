using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChardMove
{
    public class SceneMarker : MonoBehaviour
    {
        public delegate void SceneMarked();
        public static event SceneMarked sceneMarked;

        private void Awake()
        {
            sceneMarked();
        }
    }
}
