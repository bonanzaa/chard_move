using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChardMove.gameManager
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance;
        public Dictionary<Vector2,Tile> TileDB =  new Dictionary<Vector2, Tile>();

        private void Awake() {
            Instance = this;
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

        public TileType GetTileType(Vector2 pos){
            if(TileDB.TryGetValue(pos,out Tile value)){
                return value.TileType;
            }else{
                return TileType.Death;
            }
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
