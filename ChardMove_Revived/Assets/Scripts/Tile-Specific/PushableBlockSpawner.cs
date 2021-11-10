using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ChardMove.gameManager;

namespace ChardMove
{
    public class PushableBlockSpawner : Tile
    {
        public GameObject PushableBlockPrefab;
        private GameObject _pushableGO;

        private void Awake() {
            GameManager.Instance.TileDB.Add(new Vector2(transform.position.x,transform.position.y),this);
            GameManager.resetButtonPressed += OnResetButtonPressed;
            SpawnPushable();
            
        }

        private void SpawnPushable(){
            Vector3 spawnPos = new Vector3(transform.position.x,transform.position.y + 0.125f,transform.position.z);
            _pushableGO = Instantiate(PushableBlockPrefab,spawnPos,Quaternion.identity);
        }

        private void OnDestroy() {
            GameManager.resetButtonPressed -= OnResetButtonPressed;
            Destroy(_pushableGO);
        }

        private void OnResetButtonPressed()
        {
            //SpawnPushable();
        }
    }
}