using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChardMove
{
    public class DestroyParticles : MonoBehaviour
    {
        [SerializeField] private float _destroyTimer;
        private void Awake()
        {
            if(_destroyTimer == 0f)
            {
                _destroyTimer = 3f;
            }
            Destroy(gameObject, _destroyTimer);
        }
    }
}
