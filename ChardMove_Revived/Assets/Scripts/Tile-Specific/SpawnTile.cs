using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChardMove
{
    public class SpawnTile : Tile
    {
        public GameObject BotPrefab;

        private void Awake() {
            Instantiate(BotPrefab,transform.position,Quaternion.identity);
        }
    }
}
