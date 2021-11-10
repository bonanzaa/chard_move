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

        private float _musicVolume=0.5f;
        private float _sfxVolume = 0.5f;

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
            AssignBusses();
            //ButtonEvent.onButtonPressed += OnButtonClick;
            ButtonEvent.onToggleChecked += OnMuteToggled;
            ButtonEvent.onButtonHovered += OnButtonHover;
            WinTile.playerWin += OnPlayerWin;
        }

        public void OnMuteToggled()
        {
            Master.setMute(true);
        }

        public void AssignBusses()
        {
            Music = RuntimeManager.GetBus("bus:/Master/-MUSIC");
            SFX = RuntimeManager.GetBus("bus:/Master/-SFX");
            Master = RuntimeManager.GetBus("bus:/");

        }
        public void OnDestroy()
        {
            ButtonEvent.onButtonPressed -= OnButtonClick;
            WinTile.playerWin -= OnPlayerWin;
        }

        #region Events
        private void OnPlayerWin()
        {
            // idk the triumpgh sound c:
        }
        public void OnButtonClick()
        {
            if(_clickEvent != null)
            {
                RuntimeManager.PlayOneShot(_clickEvent);
            }
        }
        public void OnButtonHover() 
        {
            if(_hoverEvent != null)
            {
                RuntimeManager.PlayOneShot(_hoverEvent);
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
