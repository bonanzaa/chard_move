using ChardMove.BotMovement;
using ChardMove.gameManager;
using FMODUnity;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace ChardMove
{
    public class SoundManager : MonoBehaviour
    {
        [SerializeField] [EventRef] private string _clickEvent = null;
        [SerializeField] [EventRef] private string _hoverEvent = null;
        [SerializeField] [EventRef] private string _pushableBoxEvent = null;
        [SerializeField] [EventRef] private string _cardPickedEvent = null;
        [SerializeField] [EventRef] private string _cardDroppedEvent = null;
        [SerializeField] [EventRef] private string _sliderChangedEvent = null;
        [SerializeField] [EventRef] private string _latchActivatedEvent  = null;
        [SerializeField] [EventRef] private string _momentaryActivatedEvent  = null;
        [SerializeField] [EventRef] private string _roadBlockEvent  = null;
        [SerializeField] [EventRef] private string _playerWinEvent  = null;

        private float _musicVolume = 0.5f;
        private float _sfxVolume = 0.5f;
        private float _masterVolume = 1f;

        public static SoundManager Instance;
        
        FMOD.Studio.Bus Music;
        FMOD.Studio.Bus SFX;
        FMOD.Studio.Bus Master;
        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
            //AssignBusses();
            ButtonEvent.onButtonPressed += OnButtonClick;
            ButtonEvent.onToggleChecked += OnMuteToggled;
            ButtonEvent.onButtonHovered += OnButtonHover;
            LatchSwitch.onLatchActivated += OnLatchSwitchActivated;
            MomentarySwitch.onMomentaryActivated += OnMomentarySwitchActivated;
            Roadblock.onRoadblockActivated += OnRoadblockActivated;
            PushableBlock.onPushableBlockMoved += OnPushedBlock;
            WinTile.playerWin += OnPlayerWin;
        }


        private void Update()
        {
            ChangeVolume();
        }
        private void ChangeVolume()
        {
            Music.setVolume(_musicVolume);
            SFX.setVolume(_sfxVolume);
            Master.setVolume(_masterVolume);
        }
        public void OnMuteToggled()
        {
            Master.setMute(true);
            Debug.Log("Muting Master Bus");
        }

        public void AssignBusses()
        {
            Music = RuntimeManager.GetBus("bus:/Master/MUSIC");
            SFX = RuntimeManager.GetBus("bus:/Master/SFX");
            Master = RuntimeManager.GetBus("bus:/");

        }
        public void OnDestroy()
        {
            ButtonEvent.onButtonPressed -= OnButtonClick;
            WinTile.playerWin -= OnPlayerWin;
        }

        #region Events
        private void OnRoadblockActivated()
        {
            if(_roadBlockEvent != null)
            {
                RuntimeManager.PlayOneShot(_roadBlockEvent);
            }
        }
        private void OnMomentarySwitchActivated()
        {
            if(_momentaryActivatedEvent != null)
            {
                RuntimeManager.PlayOneShot(_momentaryActivatedEvent);
            }
        }

        private void OnLatchSwitchActivated()
        {
            if(_latchActivatedEvent != null)
            {
                RuntimeManager.PlayOneShot(_latchActivatedEvent);
            }
        }
        private void OnPlayerWin()
        {
            // idk the triumpgh sound c:
        }
        public void MasterVolumeLevel(float newMasterVolume)
        {
            _masterVolume = newMasterVolume;
        }
        public void MusicVolumeLevel(float newMusicVolume)
        {
            _masterVolume = newMusicVolume;
        }
        public void SFXVolumeLevel(float newSFXVolume)
        {
            _masterVolume = newSFXVolume;
        }
        public void OnButtonClick()
        {
            if (_clickEvent != null)
            {
                
                RuntimeManager.PlayOneShot(_clickEvent);
                
            }
        }
        public void OnButtonHover() 
        {
            if(_hoverEvent != null)
            {
                RuntimeManager.MuteAllEvents(true);
                RuntimeManager.PlayOneShot(_hoverEvent);
                RuntimeManager.MuteAllEvents(false);
            }
        }
        public void OnPushedBlock()
        {
            if (_pushableBoxEvent!= null)
            {
                RuntimeManager.PlayOneShot(_pushableBoxEvent);
            }
        }
        public void OnCardPickedUp()
        {
            if (_cardPickedEvent != null)
            {
                RuntimeManager.PlayOneShot(_cardPickedEvent);
            }
        }
        public void OnCardDropped()
        {
            if (_cardDroppedEvent != null)
            {
                RuntimeManager.PlayOneShot(_cardDroppedEvent);
            }
        }
        #endregion
    }
}
