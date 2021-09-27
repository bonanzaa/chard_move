using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ChardMove.BotMovement;

namespace ChardMove.gameManager
{
    public class GameManager : MonoBehaviour
    {
        public delegate void ResetButtonPressed();
        public static event ResetButtonPressed resetButtonPressed;

        public delegate void UndoButtonPressed();
        public static event UndoButtonPressed undoButtonPressed;
        public Dictionary<Vector2,Tile> TileDB =  new Dictionary<Vector2, Tile>();
        public Dictionary<Vector2,(IPushable,GameObject)> PushableDB = new Dictionary<Vector2, (IPushable,GameObject)>();
        public Dictionary<Vector2,BotGridMovement> BotDB = new Dictionary<Vector2, BotGridMovement>();
        public Draggable LastCardPlayed;
        public static GameManager Instance;
        public List<Draggable> PlayerCards = new List<Draggable>();

        private List<Draggable> _originalPlayerCards = new List<Draggable>();

        private void Awake() {
            Instance = this;
        }

        private void Start() {
            _originalPlayerCards = PlayerCards;
        }
        public void Reset(){
            resetButtonPressed();
            ResetPlayerCards();
        }

        public void DeletePlayerCards(){
            foreach (var item in PlayerCards)
            {
                Destroy(item.gameObject);
            }
            PlayerCards.Clear();
        }
        private void ResetPlayerCards(){
            foreach (var item in PlayerCards)
            {
                item.gameObject.SetActive(true);
            }
        }

        private void UndoPlayerCards(){
            LastCardPlayed.gameObject.SetActive(true);
        }
        public void Undo(){
            undoButtonPressed();
            UndoPlayerCards();
        }

        // first bool is: is the tile walkable?
        // second bool: will the player die if they step on it?
        // although in the else statement both are true, even though
        // death tile isn't technically walkable - used by the MoveToDeath()
        // function, that moves player to that tile (first bool true) and "kills" them (second bool)
        public (bool,bool) TileWalkable(Vector2 pos){
            if(TileDB.TryGetValue(pos,out Tile value)){
                if(value.TileType == TileType.Walkable){
                    return (true,false);
                }else{
                    return (false,false);
                }

            }else{
                return (true,true);
            }
        }

        public void AddMeToCardList(Draggable card){
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
            if(BotDB.TryGetValue(previousPos,out BotGridMovement _bot)){
                RemoveFromBotDB(previousPos);
                BotDB.Add(pos,botScript);
            }else{
                BotDB.Add(pos,botScript);
            }
        }

        public void RemoveBotFromDB(Vector2 pos){
            BotDB.Remove(pos);
        }

        public bool BotInTheWay(Vector2 pos){
            if(BotDB.TryGetValue(pos,out BotGridMovement value)){
                if(value.IsPushable){
                    return false;
                }else{
                    return true;
                }
            }else{
                return false;
            }
        }

        private void RemoveFromBotDB(Vector2 pos){
            BotDB.Remove(pos);
        }

        public void ClearDictionaries(){
            TileDB.Clear();
            PushableDB.Clear();
            BotDB.Clear();
        }

        public void UpdateTileDB(Vector2 pos, Tile tile, Vector2 previousPos){
            if(TileDB.TryGetValue(previousPos, out Tile _tile)){
                RemoveFromTileDB(previousPos);
                AddtoTileDB(pos, tile);
            }else{
                AddtoTileDB(pos, tile);
            }
        }

        public void AddToPushableDB(Vector2 pos, IPushable value, GameObject gameObject, Vector2 previousPos){
            if(PushableDB.TryGetValue(previousPos,out var _value)){
                var pushableValue = _value.Item1;
                var pushableGO = _value.Item2;
                PushableDB.Remove(previousPos);
                PushableDB.Add(pos,(value,gameObject));
            }else{
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
