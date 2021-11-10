using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace ChardMove.Assets.Scripts.NewFolder1
{
    class FancyFloatUI : MonoBehaviour
    {
        [SerializeField]
        private float _speed = 0.1f;
        [SerializeField]
        private float _maxMoveDistance = 10f;
        private RectTransform _currentObj;
        private float _verticalSpeedMultiplier = -1f; // +1 moves upward, -1 moves downward
        private Vector3 _initialPosition;

        void Start()
        {
            _currentObj = gameObject.GetComponent<RectTransform>();
            _initialPosition = _currentObj.position;
            _currentObj.position += new Vector3(0, UnityEngine.Random.Range(-_maxMoveDistance, _maxMoveDistance), 0);
        }

        void Update()
        {
            _currentObj.position += new Vector3(0, _verticalSpeedMultiplier *Time.deltaTime* _speed, 0f);

            UpdateMoveDirection();
        }
        private void UpdateMoveDirection()
        {
            if (_currentObj.position.y > _initialPosition.y + _maxMoveDistance)
            {
                _verticalSpeedMultiplier = -1f; // now start moving downward
            }

            if (_currentObj.position.y < _initialPosition.y - _maxMoveDistance)
            {
                _verticalSpeedMultiplier = 1f; // now start moving upward
            }
        }
    }
}
