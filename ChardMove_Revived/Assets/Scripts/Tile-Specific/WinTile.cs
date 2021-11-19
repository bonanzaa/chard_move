using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ChardMove.gameManager;

namespace ChardMove
{
    public class WinTile : Tile
    {
        [SerializeField] private GameObject _winScreenUI;
        [SerializeField] private GameObject _winParticleSystem;
        [SerializeField] private List<Transform> _spawners;
        public bool Target = false;

        private LevelLoader _levelLoader;

        public delegate void PlayerWin();
        public static event PlayerWin playerWin;
        private void Awake()
        {
            GameManager.Instance.RedButton = this;
            _levelLoader = LevelLoader.Instance;
            GameManager.Instance.TileDB.Add(new Vector2(transform.position.x,transform.position.y),this);
            //playerWin += SpawnConfetti;
        }

        public void SetTarget()
        {
            if(GameManager.Instance.PlayerWon) return;
            GameManager.Instance.PlayerWon = true;
            playerWin();
        }
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.gameObject.CompareTag("Bot"))
            {
                if(!Target) return;
                playerWin();
            }
        }
        private void OnDestroy()
        {
            //playerWin -= SpawnConfetti;
        }
        public  void SpawnConfetti()
        {
            //Debug.Log("Particle system should spawn in");
            if (_winParticleSystem != null)
            {
                for (int i = 0; i < _spawners.Count; i++)
                {
                    GameObject particles = Instantiate(_winParticleSystem, _spawners[i].position, Quaternion.identity);
                }
            }
            //Debug.Log("Particle system should  have already spawned in");
        }
    }
}
