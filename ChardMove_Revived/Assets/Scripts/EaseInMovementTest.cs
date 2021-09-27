using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChardMove
{
    public class EaseInMovementTest : MonoBehaviour
    {
        public Vector3 TargetPos;
        private Vector2 _originalPos;
        public float Speed = 0.05f;

        private void Awake() {
            _originalPos = transform.position;
            TargetPos = new Vector3(transform.position.x + 10, transform.position.y + 5f);
        }

        private void Update() {
            if(Input.GetKeyDown(KeyCode.A)){
                StartCoroutine(Move());
            }
            if(Input.GetKeyDown(KeyCode.S)){
                StopCoroutine(Move());
                StopAllCoroutines();
            }
        }

        private IEnumerator Move(){
            yield return null;
        }


        private float EaseOutQuart(float x){
            return 1-Mathf.Pow(1-x,4);
        }


    }
}
