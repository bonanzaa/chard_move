using ChardMove.BotMovement;
using ChardMove.gameManager;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChardMove
{
    public class SoundManager : MonoBehaviour
    {

        [SerializeField] private List<AudioClip> _audioClips;
        [SerializeField] private AudioClip _backGroundMusic;
        private AudioSource _audioSource;
        

        public static SoundManager Instance;
        private void Awake()
        {
            //DontDestroyOnLoad(gameObject);
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
            _audioSource = GetComponent<AudioSource>();
            WinTile.playerWin += OnPlayerWin;
            LevelCompleteReference.nextLevel += OnNextLevelLoad;
            BotGridMovement.botMoved += OnBotMoved;
            GameManager.resetButtonPressed += OnResetButtonPressed;
            //_audioSource.Play(_backGroundMusic);
            
        }
        //public void PlaySoundEvent()
        //{
        //    if(_eventPath!= null)
        //    {
        //        RuntimeManager.PlayOneShot(_eventPath);
        //    }
        //}

        private void OnResetButtonPressed()
        {
            //Pretty straight forward huh
        }

        private void OnBotMoved()
        {
            //Idk like robotic floating sounds?
        }

        private void OnNextLevelLoad()
        {
            //paper flipping, robot sounds, whooosh idk sound
        }

        private void OnPlayerWin()
        {
           //play celebrate sound;
        }
        private void OnBotDeath()
        {

        }
        private void OnDisable()
        {
            WinTile.playerWin -= OnPlayerWin;
            LevelCompleteReference.nextLevel -= OnNextLevelLoad;
            BotGridMovement.botMoved -= OnBotMoved;
            GameManager.resetButtonPressed -= OnResetButtonPressed;
        }
    }
}
