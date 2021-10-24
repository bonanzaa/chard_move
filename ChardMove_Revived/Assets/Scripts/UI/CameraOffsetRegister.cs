using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChardMove
{
    [ExecuteInEditMode]
    public class CameraOffsetRegister : MonoBehaviour
    {
        public Vector3 CameraPosition;
        private Camera _mainCamera;
        [ExecuteInEditMode]
        void Start()
        {
            _mainCamera = Camera.main;
        }

        [ExecuteInEditMode]
        private void Update() {
            CameraPosition = _mainCamera.gameObject.transform.position;
        }
    }
}
