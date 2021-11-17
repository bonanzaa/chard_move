using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChardMove
{
    public class LoadScreenBehaviour : MonoBehaviour
    {
        [SerializeField] private Animator _animator;
        private Animation _fadeToBlack;
        private Animation _fadeFromBlack;
        private SceneLoader _sceneLoader;
    }
}
