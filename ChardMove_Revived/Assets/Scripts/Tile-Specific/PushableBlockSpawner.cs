using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ChardMove.gameManager;

namespace ChardMove
{
    public class PushableBlockSpawner : Tile
    {
        public GameObject PushableBlockPrefab;

        private void Awake() {
            GameManager.resetButtonPressed += OnResetButtonPressed;
            SpawnPushable();
            
        }

        private void SpawnPushable(){
            Vector3 spawnPos = new Vector3(transform.position.x,transform.position.y + 0.125f,transform.position.z);
            Instantiate(PushableBlockPrefab,spawnPos,Quaternion.identity);

        }

        private void OnResetButtonPressed()
        {
            SpawnPushable();
        }
    }
}