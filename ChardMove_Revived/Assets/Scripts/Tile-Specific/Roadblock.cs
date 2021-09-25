using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ChardMove.gameManager;

namespace ChardMove
{
    public class Roadblock : Tile
    {
        public bool IsActive = false;
        public void Activate(){
            TileType = TileType.Walkable;
            IsActive = true;
            StartCoroutine(ActivationAnimation());
        }

        private IEnumerator ActivationAnimation(){
            float currentY = transform.position.y;
            float currentZ = transform.position.z;

            float targetY = currentY - 0.375f;
            float targetZ = currentZ + 0.5f;

            float newY = 0;
            float newZ = 0;

            float t  = 0;
            while(transform.position.y != targetY && transform.position.z != targetZ){
                t += Time.deltaTime;
                newY = Mathf.Lerp(currentY,targetY,t);
                newZ = Mathf.Lerp(currentZ,targetZ,t);
                transform.position = new Vector3(transform.position.x,newY,newZ);
                yield return null;
            }
        }

        public void Deactivate(){
            TileType = TileType.Unwalkable;
            IsActive = false;
            StartCoroutine(DeactivationAnimation());
        }

        private IEnumerator DeactivationAnimation(){
            float currentY = transform.position.y;
            float currentZ = transform.position.z;

            float targetY = currentY + 0.375f;
            float targetZ = currentZ - 0.5f;

            float newY = 0;
            float newZ = 0;

            float t  = 0;
            while(transform.position.y != targetY && transform.position.z != targetZ){
                t += Time.deltaTime;
                newY = Mathf.Lerp(currentY,targetY,t);
                newZ = Mathf.Lerp(currentZ,targetZ,t);
                transform.position = new Vector3(transform.position.x,newY,newZ);
                yield return null;
            }
        }
    }
}
