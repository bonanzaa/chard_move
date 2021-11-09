using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChardMove
{
    [ExecuteInEditMode]
    public class CameraOffsetRegister : MonoBehaviour
    {
        public bool UseCurrentCameraPosition = false;
        public Vector3 CameraPosition;
        private Camera _mainCamera;
        [ExecuteInEditMode]
        void Start()
        {
            _mainCamera = Camera.main;
        }

        [ExecuteInEditMode]
        private void Update() {
            if(UseCurrentCameraPosition){
                CameraPosition = _mainCamera.gameObject.transform.position;
            }
        }
    }
}
