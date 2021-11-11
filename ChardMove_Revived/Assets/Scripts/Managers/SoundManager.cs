using ChardMove.BotMovement;
using ChardMove.gameManager;
using FMODUnity;
using System;
using UnityEngine;

namespace ChardMove
{
    public class SoundManager : MonoBehaviour
    {
        [SerializeField] [EventRef] private string _clickEvent = null;
        [SerializeField] [EventRef] private string _arrowClickedEvent = null;
        [SerializeField] [EventRef] private string _hoverEvent = null;
        [SerializeField] [EventRef] private string _pushableBoxEvent = null;
        [SerializeField] [EventRef] private string _cardPickedEvent = null;
        [SerializeField] [EventRef] private string _cardDroppedEvent = null;
        [SerializeField] [EventRef] private string _sliderChangedEvent = null;
        [SerializeField] [EventRef] private string _latchActivatedEvent = null;
        [SerializeField] [EventRef] private string _momentaryActivatedEvent = null;
        [SerializeField] [EventRef] private string _roadBlockEvent = null;
        [SerializeField] [EventRef] private string _playerWinEvent = null;
        [SerializeField] [EventRef] private string _botMovedEvent = null;
        [SerializeField] [EventRef] private string _botDeathEvent = null;
        [SerializeField] [EventRef] private string _botLandedEvent = null;
        [SerializeField] [EventRef] private string _loadLevel = null;

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
            AssignBusses();
            ButtonEvent.onButtonPressed += OnButtonClick;
            ButtonEvent.onToggleChecked += OnMuteToggled;
            ButtonEvent.onButtonHovered += OnButtonHover;
            BotGridMovement.onBotMovedATile += OnBotMoved;
            //BOT DIED NOT BOT ABOUT TO DIE FOR SOUND
            BotGridMovement.botAboutToDie += OnBotDeath;
            DirectionalButtonClick.onButtonPressed += OnDirectionalButtonClick;
            Draggable.onBeginDrag += onCardDrag;
            DropZone.directionChoiceActive += OnCardDropped;
            //ARROW DIRECTION CLICKED
            //CARD DRAG AND DROP CARD EVENT
            //LEVEL LOAD
            GameManager.onNewLevelLoaded += OnNewLevelLoaded;
            //PUSHABLE BLOCK BECOME A WALKABLE TILE (FALLS)
            //RESET BUTTON (LEVEL LOAD)
            GameManager.resetButtonPressed += OnResetButtonPressed;
            //BOT LANDS ON TILE AFTER RAIN ANIMATION
            GameManager.onBotLanded += OnbotLanded;
            //BOT MOVE 
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
            Master = RuntimeManager.GetBus("bus:/");
            Music = RuntimeManager.GetBus("bus:/BGM");
            SFX = RuntimeManager.GetBus("bus:/SFX");

        }
        public void OnDestroy()
        {
            ButtonEvent.onButtonPressed -= OnButtonClick;
            ButtonEvent.onToggleChecked -= OnMuteToggled;
            ButtonEvent.onButtonHovered -= OnButtonHover;
            BotGridMovement.onBotMovedATile -= OnBotMoved;
            BotGridMovement.botAboutToDie -= OnBotDeath;
            DirectionalButtonClick.onButtonPressed -= OnDirectionalButtonClick;
            Draggable.onBeginDrag -= onCardDrag;
            DropZone.directionChoiceActive -= OnCardDropped;
            //LEVEL LOAD
            GameManager.onNewLevelLoaded -= OnNewLevelLoaded;
            //PUSHABLE BLOCK BECOME A WALKABLE TILE (FALLS)
            //RESET BUTTON (LEVEL LOAD)
            GameManager.resetButtonPressed -= OnResetButtonPressed;
            //BOT LANDS ON TILE AFTER RAIN ANIMATION
            GameManager.onBotLanded -= OnbotLanded;
            //BOT MOVE 
            LatchSwitch.onLatchActivated -= OnLatchSwitchActivated;
            MomentarySwitch.onMomentaryActivated -= OnMomentarySwitchActivated;
            Roadblock.onRoadblockActivated -= OnRoadblockActivated;
            PushableBlock.onPushableBlockMoved -= OnPushedBlock;
            WinTile.playerWin -= OnPlayerWin;
        }

        private void OnbotLanded()
        {
            if(_botLandedEvent != null)
            {
                RuntimeManager.PlayOneShot(_botLandedEvent);
            }
        }

        private void onCardDrag(int distance)
        {
            if(_cardPickedEvent!= null)
            {
                RuntimeManager.PlayOneShot(_cardPickedEvent);
            }
        }

        private void OnDirectionalButtonClick(bool pressed)
        {
            if (_arrowClickedEvent != null)
            {
                RuntimeManager.PlayOneShot(_arrowClickedEvent);
            }
        }
        #region VolumeHandler
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
        #endregion

        #region Events
        private void OnNewLevelLoaded()
        {
            if (_loadLevel != null)
            {
                RuntimeManager.PlayOneShot(_loadLevel);
            }
        }
        private void OnResetButtonPressed()
        {
            if (_loadLevel != null)
            {
                RuntimeManager.PlayOneShot(_loadLevel);
            }
        }

        private void OnBotDeath(GameObject theBot)
        {
            if (_botDeathEvent != null)
            {
                RuntimeManager.PlayOneShot(_botDeathEvent);
            }
        }
        private void OnBotMoved()
        {
            if (_botMovedEvent != null)
            {
                RuntimeManager.PlayOneShot(_botMovedEvent);
            }
        }
        private void OnRoadblockActivated()
        {
            if (_roadBlockEvent != null)
            {
                RuntimeManager.PlayOneShot(_roadBlockEvent);
            }
        }
        private void OnMomentarySwitchActivated()
        {
            if (_momentaryActivatedEvent != null)
            {
                RuntimeManager.PlayOneShot(_momentaryActivatedEvent);
            }
        }

        private void OnLatchSwitchActivated()
        {
            if (_latchActivatedEvent != null)
            {
                RuntimeManager.PlayOneShot(_latchActivatedEvent);
            }
        }
        private void OnPlayerWin()
        {
            if (_playerWinEvent != null)
            {
                RuntimeManager.PlayOneShot(_playerWinEvent);
            }
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
            if (_hoverEvent != null)
            {
                RuntimeManager.MuteAllEvents(true);
                RuntimeManager.PlayOneShot(_hoverEvent);
                RuntimeManager.MuteAllEvents(false);
            }
        }
        public void OnPushedBlock()
        {
            if (_pushableBoxEvent != null)
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
