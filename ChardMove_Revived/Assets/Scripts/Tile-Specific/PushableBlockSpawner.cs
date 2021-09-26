using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChardMove
{
    public class PushableBlockSpawner : Tile
    {
        public GameObject PushableBlockPrefab;

        private void Awake() {
            Vector3 spawnPos = new Vector3(transform.position.x,transform.position.y + 0.125f,transform.position.z);
            Instantiate(PushableBlockPrefab,spawnPos,Quaternion.identity);
        }
    }
}