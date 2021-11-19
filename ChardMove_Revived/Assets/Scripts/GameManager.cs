using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ChardMove.BotMovement;
using DG.Tweening;
using System.Linq;

namespace ChardMove.gameManager
{
    public class GameManager : MonoBehaviour
    {
        public GameObject Level;

        public delegate void ResetButtonPressed();
        public static event ResetButtonPressed resetButtonPressed;

        public delegate void NewLevelLoaded();
        public static event NewLevelLoaded onNewLevelLoaded;
        public delegate void BotLanded();
        public static event BotLanded onBotLanded;
        public delegate void UndoButtonPressed();
        public static event UndoButtonPressed undoButtonPressed;
        public delegate void LevelUnload();
        public static event LevelUnload onLevelUnload;
        public delegate void LevelFullyLoaded();
        public static event LevelFullyLoaded onLevelFullyLoaded;
        public delegate void UndoDirectionalChoice();
        public static event UndoDirectionalChoice undoDirectionalChoice;
        public Dictionary<Vector2,Tile> TileDB =  new Dictionary<Vector2, Tile>();
        public Dictionary<Vector2,(IPushable,GameObject)> PushableDB = new Dictionary<Vector2, (IPushable,GameObject)>();
        public Dictionary<Vector2,BotGridMovement> BotDB = new Dictionary<Vector2, BotGridMovement>();
        public List<GameObject> _allEntitiesToUnload;
        public List<GameObject> _allEntitiesToLoad;
        public List<GameObject> _ghosts;
        [HideInInspector]public WinTile RedButton;
        public Draggable LastCardPlayed;
        public static GameManager Instance;
        public List<Draggable> PlayerCards = new List<Draggable>();
        public List<Draggable> _tempPlayerCards = new List<Draggable>();

        private LevelLoader _levelLoader;
        private List<Draggable> _originalPlayerCards = new List<Draggable>();
        private GameObject _currentLevel;
        public GameObject _lastLevel;
        public bool PlayerWon = false;
       [HideInInspector] public bool LevelLoaded = false;
       [HideInInspector] public bool _botMoving = false;
       [HideInInspector] public bool AnimationInProgress = false;
       [Header("Tiles")]

       [Range(0.01f,2f)]
       public float DelayBeforeTiles;
       [Range(0.5f,6f)]
       public float TileSpeedMin;
       [Range(0.5f,6f)]
       public float TileSpeedMax;

       [Range(0.2f,1f)]
       public float DelayBeforeRedButton;
       [Range(0.5f,4f)]
       public float RedButtonSpeed;

       [Header("Pushable Blocks")]
       [Range(0.5f,4f)]
       public float DelayBeforeBlocks;
       [Range(0.5f,4f)]
       public float BlockSpeedMin;
       [Range(0.5f,4f)]
       public float BlockSpeedMax;
       
       [Header("Bots")]

       [Range(0.5f,4f)]
       public float DelayBeforeBots;
       [Range(0.5f,4f)]
       public float BotSpeedMin;
       [Range(0.5f,4f)]
       public float BotSpeedMax;

        // for loading animation
        private int _i = 0;
        private int _k = 0;
        private int _j = 0;
        private GameObject _oldLevel;

        private void Awake() {
            //Time.timeScale = 1;
            _levelLoader = LevelLoader.Instance;
            //LevelCompleteReference.nextLevel += OnNextLevelLoad;
            BotGridMovement.botMovedPos += OnBotMoved;
            BotGridMovement.botStartedMovingPos += OnBotStartedMoving;
            BotGridMovement.botUndoPressed += OnBotUndoMoved;
            BotGridMovement.onPushableBotMoved += OnPushableBotMoved;
            PlayerWon = false;
            _botMoving = false;
            LevelLoaded = false;
            Instance = this;
            if(Level == null && LevelLoader.Instance != null)
            {
                Level = _levelLoader.Levels[LevelLoader.SceneIndex];
            }
            //if(Level != null){
            //    LevelLoaded = true;
            //    LoadLevel(Level);
            //}
        }

        private void OnBotMoved(Vector2 pos){
            FindSwitch(pos);
            //FindWinTile(pos);
            FindMovingPlatform(pos);

        }

        private void OnPushableBotMoved(Vector2 pos){
            FindSwitch(pos);
            FindWinTile(pos);
            FindMovingPlatform(pos);
        }
        public void OnBotStartedMoving(){
            _botMoving = true;
        }

        public void OnBotFinishedMoving(Vector3 pos){
            _botMoving = false;
            FindWinTile(pos);
        }

        private void OnBotUndoMoved(Vector2 pos, Vector2 lastpos){
            if(pos == lastpos) return;
            if(TileDB.TryGetValue(lastpos,out Tile value)){
   
                if(value.TryGetComponent(out LatchSwitch component)){
                    component.OnUndoBotLanded();
                }else if(value.TryGetComponent(out MomentarySwitch component1)){
                    component1.OnUndoBotLanded();
                }else if(value.TryGetComponent(out MovingTile component2)){
                    var currentbot = BotInTheWayOutBot(pos);
                    var botGO = currentbot.Item2.gameObject;
                    component2.OnUndoBotLanded(botGO);
                }
            }

            if(TileDB.TryGetValue(pos, out Tile value1)){
                if(value1.TryGetComponent(out MomentarySwitch component1)){
                    component1.RemoveTarget();
                }else if(value1.TryGetComponent(out MovingTile component2)){
                    component2.RemoveTarget();
                    BotDB.Remove(pos);
                }
            }
        }

        private void OnBotStartedMoving(Vector2 pos){
            if(TileDB.TryGetValue(pos,out Tile value)){

                if(value.TryGetComponent(out MomentarySwitch component)){
                    component.RemoveTarget();
                }

                if(value.TryGetComponent(out LatchSwitch component1)){
                    component1.CacheState();
                }

                if(value.TryGetComponent(out MovingTile component2)){
                    component2.RemoveTarget();
                }
            }

        }

        private void FindSwitch(Vector2 pos){
            if(TileDB.TryGetValue(pos,out Tile value)){
                // if we find a tile at bot's position look for a SwitchBase interface, indicating
                // the tile is a switch
                if(value.TryGetComponent(out SwitchBase switchbase)){
                    switchbase.SetTarget();
                }
            }
        }

        private void FindWinTile(Vector2 pos){
            if(TileDB.TryGetValue(pos,out Tile value)){

                if(value.TryGetComponent(out WinTile wintile)){
                    wintile.SetTarget();
                }
            }else{
                if((Vector2)RedButton.transform.position == pos){
                    RedButton.GetComponent<WinTile>().SetTarget();
                }
            }
        }

        private void FindMovingPlatform(Vector2 pos){
            if(TileDB.TryGetValue(pos, out Tile value)){

                if(value.TryGetComponent(out MovingTile platform)){
                    // reference player for the moving platform here
                    var currentbot = BotInTheWayOutBot(pos);
                    var botGO = currentbot.Item2.gameObject;
                    platform.SetTarget(botGO);

                    RemoveFromBotDB(pos);
                }
            }
        }


        private void Start() {
            _originalPlayerCards = PlayerCards;
            // if(Level != null){
            //     print("Loading level from Start on GM");
            //     LoadLevel(Level.gameObject);
            // }
        }

        private void OnDestroy() {
            BotGridMovement.botMovedPos -= OnBotMoved;
            BotGridMovement.botStartedMovingPos -= OnBotStartedMoving;
            BotGridMovement.onPushableBotMoved -= OnPushableBotMoved;
            //LevelCompleteReference.nextLevel -= OnNextLevelLoad;
        }

        public void Reset(){
            if(_botMoving || AnimationInProgress) return;
            LevelSwitchAnimator.Instance.LoadLevel(LevelSwitchAnimator.Instance.CurrentLevelIndex);
        }

        public void OnBotLanded(){
            if(onBotLanded != null)
                onBotLanded();
        }

        public void LoadCardsWithTween(GameObject card){
            GameObject bigIcon = card.GetComponent<Draggable>().BigIcon;
            bigIcon.SetActive(false);
            StartCoroutine(LoadCardsTween(card,bigIcon));
        }

        public IEnumerator LoadCardsTween(GameObject card, GameObject icon){
            yield return new WaitForEndOfFrame();
            Vector3 endPos = card.transform.position;
            card.transform.position = new Vector3(15,card.transform.position.y,card.transform.position.z);
            icon.SetActive(true);
            card.transform.DOMove(endPos,1,false);
            yield return null;
        }

        public void Undo(){
            if(_botMoving || AnimationInProgress) return;
            if(LastCardPlayed!= null)
                PlayerCards.Add(LastCardPlayed);
            undoButtonPressed();
        }

        public void UndoDirectionalChoiceEvent(){
            undoDirectionalChoice();
        }

        // first bool is: is the tile walkable?
        // second bool: will the player die if they step on it?
        // although in the else statement both are true, even though
        // death tile isn't technically walkable - used by the MoveToDeath()
        // function, that moves player to that tile (first bool true) and "kills" them (second bool)
        public (bool,bool) TileWalkable(Vector2 pos){
            if(TileDB.TryGetValue(pos,out Tile value)){
                if(value.TileType == TileType.Walkable){
                    //print("Walkable");
                    return (true,false);
                }else{
                    //print("Not walkable");
                    return (false,false);
                }

            }else{
                //print("Death");
                return (true,true);
            }
        }

        public void AddMeToCardList(Draggable card){
            // used when instantiating new cards during Level Loading
            PlayerCards.Add(card);
        }
        public TileType GetTileType(Vector2 pos){
            if(TileDB.TryGetValue(pos,out Tile value)){
                return value.TileType;
            }else{
                return TileType.Death;
            }
        }
        public Tile GetTile(Vector2 pos){
            if(TileDB.TryGetValue(pos, out Tile tile)){
                return tile;
            }else{
                return null;
            }
        }

        public void AddBotToDB(Vector2 pos, BotGridMovement botScript, Vector2 previousPos){
            // used for bot collision detection
            if(BotDB.TryGetValue(previousPos,out BotGridMovement _bot)){
                BotDB.Remove(previousPos);

                if(BotDB.TryGetValue(pos,out BotGridMovement newValue)){
                    BotDB.Remove(pos);
                    BotDB.Add(pos,botScript);
                }else{
                    BotDB.Add(pos,botScript);
                }

            }else if(!BotDB.TryGetValue(pos,out BotGridMovement _value)){
                BotDB.Add(pos,botScript);
            }
        }

        public void RemoveBotFromDB(Vector2 pos){
            BotDB.Remove(pos);
        }

        public bool BotInTheWay(Vector2 pos){
            // collision detection for bots
            if(BotDB.TryGetValue(pos,out BotGridMovement value)){
                // if we find a bot at given position,
                // see if it is pushable
                if(value.IsPushable){
                    return false;
                }else{
                    return true;
                }
            }else{
                return false;
            }
        }

        public (bool,BotGridMovement) BotInTheWayOutBot(Vector2 pos){
            // collision detection for bots
            if(BotDB.TryGetValue(pos,out BotGridMovement value)){
                // if we find a bot at given position,
                // see if it is pushable
                return (true,value);
            }else{
                return (false,value);
            }
        } 

        private void RemoveFromBotDB(Vector2 pos){
            BotDB.Remove(pos);
        }

        public void RemoveCard(Draggable card){
            PlayerCards.Remove(card);
        }

        public void ClearDictionaries(){
            // used when loading new level
            TileDB.Clear();
            PushableDB.Clear();
            BotDB.Clear();
        }

        public void UpdateTileDB(Vector2 pos, Tile tile, Vector2 previousPos){
            // used for tiles that need to be stored in the dictionary, but change their position
            if(TileDB.TryGetValue(previousPos, out Tile _tile)){
                RemoveFromTileDB(previousPos);
                AddtoTileDB(pos, tile);
            }else if(!TileDB.TryGetValue(pos,out Tile _tile1)){
                AddtoTileDB(pos, tile);
            }
        }

        public void AddToPushableDB(Vector2 pos, IPushable value, GameObject gameObject, Vector2 previousPos){
            // everything that can be pushed gets registered here
            if(PushableDB.TryGetValue(previousPos,out var _value)){
                var pushableValue = _value.Item1;
                var pushableGO = _value.Item2;
                PushableDB.Remove(previousPos);

                if(PushableDB.TryGetValue(pos,out var newValue)){
                    PushableDB.Remove(pos);
                    PushableDB.Add(pos,(value,gameObject));
                }else{
                    PushableDB.Add(pos,(value,gameObject));
                }
            }else if(!PushableDB.TryGetValue(pos,out var __value)){
                PushableDB.Add(pos,(value,gameObject));
            }
        }

        public void RemovePushableFromDB (Vector2 pos)
        {
            PushableDB.Remove(pos);
        }

        public GameObject GetPushableGO(Vector2 pos){
            if(PushableDB.TryGetValue(pos, out var _value)){
                var pushable = _value.Item1;
                var pushableGO = _value.Item2;
                return pushableGO;
            }else{
                return null;
            }
        }

        public bool PushableInTheWay(Vector2 pos){
            if(PushableDB.TryGetValue(pos,out var _value)){
                return true;
            }else{
                return false;
            }
        }

        private void AddtoTileDB(Vector2 pos, Tile tile){
            TileDB.Add(pos,tile);
        }

        public void RemoveFromTileDB(Vector2 pos){
            TileDB.Remove(pos);
        }
    }

    public enum TileType{
        Walkable,
        Unwalkable,
        Death
    }
    public enum MovementDirection{
        Forward,
        Right,
        Left,
        Backward,
        None
    }
}