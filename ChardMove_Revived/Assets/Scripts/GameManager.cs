using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ChardMove.BotMovement;

namespace ChardMove.gameManager
{
    public class GameManager : MonoBehaviour
    {
        public List<Draggable> lastPlayerCards;
        public List<Draggable> _originalPlayerCards = new List<Draggable>();
        private Dictionary<Vector2,Tile> lastTileDB;
        public GameObject CardParent;
        public static GameManager Instance;
        public Draggable LastCardPlayed;
        public Dictionary<Vector2,Tile> TileDB =  new Dictionary<Vector2, Tile>();
        public List<Draggable> PlayerCards = new List<Draggable>();
        public delegate void ResetButtonPressed();
        public static event ResetButtonPressed resetButtonPressed;

        public delegate void UndoButtonPressed();
        public static event UndoButtonPressed undoButtonPressed;

        private void Awake() {
            Instance = this;
            BotGridMovement.botMoved += OnBotMoved;
        }

        private void Start() {
            _originalPlayerCards = PlayerCards;
        }


        private void OnBotMoved(){
            SaveTheProgress();
        }

        private void SaveTheProgress(){
            
        }

        public void Reset(){
            resetButtonPressed();
            ResetPlayerCards();
        }

        private void ResetPlayerCards(){
            foreach (var item in PlayerCards)
            {
                item.gameObject.SetActive(true);
            }
        }

        public void Undo(){
            undoButtonPressed();
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

        public void AddToTileDB(Vector2 pos, Tile tile, Vector2 previousPos){
            if(TileDB.TryGetValue(pos, out Tile _tile)){
                RemoveFromDB(pos);
                AddToDB(pos, tile);
            }else{
                AddToDB(pos, tile);
            }
        }

        private void AddToDB(Vector2 pos, Tile tile){
            TileDB.Add(pos,tile);
        }

        private void RemoveFromDB(Vector2 pos){
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
