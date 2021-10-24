using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ChardMove.BotMovement;

namespace ChardMove.gameManager
{
    public class GameManager : MonoBehaviour
    {
        public GameObject Level;

        public delegate void ResetButtonPressed();
        public static event ResetButtonPressed resetButtonPressed;

        public delegate void UndoButtonPressed();
        public static event UndoButtonPressed undoButtonPressed;
        public delegate void UndoDirectionalChoice();
        public static event UndoDirectionalChoice undoDirectionalChoice;
        public Dictionary<Vector2,Tile> TileDB =  new Dictionary<Vector2, Tile>();
        public Dictionary<Vector2,(IPushable,GameObject)> PushableDB = new Dictionary<Vector2, (IPushable,GameObject)>();
        public Dictionary<Vector2,BotGridMovement> BotDB = new Dictionary<Vector2, BotGridMovement>();
        public Draggable LastCardPlayed;
        public static GameManager Instance;
        public List<Draggable> PlayerCards = new List<Draggable>();
        public List<Draggable> _tempPlayerCards = new List<Draggable>();

        private LevelLoader _levelLoader;
        private List<Draggable> _originalPlayerCards = new List<Draggable>();
        private GameObject _currentLevel;
        public bool LevelLoaded = false;
        public bool _botMoving = false;

        private void Awake() {
            _levelLoader = LevelLoader.Instance;
            LevelCompleteReference.nextLevel += OnNextLevelLoad;
            _botMoving = false;
            Instance = this;
            if(Level == null && LevelLoader.Instance != null)
            {
                Level = _levelLoader.Levels[LevelLoader.LevelIndex];
            }
            if(Level != null){
                LevelLoaded = true;
                ClearDictionaries();
            }
        }

        private void OnNextLevelLoad()
        {
            LoadLevel(_levelLoader.Levels[LevelLoader.LevelIndex]);
        }

        private void Start() {
            _originalPlayerCards = PlayerCards;
            if(Level != null){
                LoadLevel(Level.gameObject);
            }
        }

        public void LoadLevel(GameObject level) {
            if(Level != null){
                Destroy(_currentLevel);
                ClearDictionaries();
                ResetPlayerCards();
            //    Vector3 lvlOffset = level.transform.position;
            //    if(lvlOffset == Vector3.zero){
            //        _currentLevel = Instantiate(level,new Vector3(1.32f,3.62f,0),Quaternion.identity);
            //    }else{
            //         _currentLevel = Instantiate(level,lvlOffset,Quaternion.identity);
            //    }
               //_currentLevel = Instantiate(level, new Vector3(0,0,0),Quaternion.identity);
               StartCoroutine(LevelInstantiatingTimer(level));
               
            }else{
                ClearDictionaries();
                ResetPlayerCards();
                LevelLoaded = true;
            }

            if(level.TryGetComponent(out CameraOffsetRegister _cameraOffset)){
                Camera.main.gameObject.transform.position = _cameraOffset.CameraPosition;
                GameObject.FindGameObjectWithTag("Canvas").gameObject.transform.position = new Vector3(_cameraOffset.CameraPosition.x,_cameraOffset.CameraPosition.y,0);
            }

            CardStacker.Instance.LoadCards();
        }

        private IEnumerator LevelInstantiatingTimer(GameObject level){
            yield return new WaitForEndOfFrame();
            _currentLevel = Instantiate(level, new Vector3(0,0,0),Quaternion.identity);
            CardStacker.Instance.LoadCards();
        }

        public void Reset(){
            if(_botMoving) return;
            BotDB.Clear();
            PushableDB.Clear();
            resetButtonPressed();
            DeletePlayerCards(); 
        }

        public void OnBotStartedMoving(){
            _botMoving = true;
        }

        public void OnBotFinishedMoving(){
            _botMoving = false;
        }

        public void DeletePlayerCards(){
            PlayerCards.Clear();
        }
        private void ResetPlayerCards(){
            foreach (var item in PlayerCards)
            {
                item.gameObject.SetActive(true);
            }
        }

        public void Undo(){
            if(_botMoving) return;
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
