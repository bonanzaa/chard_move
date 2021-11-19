using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using ChardMove.gameManager;
using ChardMove.BotMovement;
using System;

namespace ChardMove
{
    public class DropZone : MonoBehaviour, IDropHandler, IPointerExitHandler, IPointerEnterHandler
    {
        public GameObject Bot;
        public GameObject Highlight;
        public GameObject DirectionChoiceMenu;

        public delegate void DirectionChoiceActive();
        public static event DirectionChoiceActive directionChoiceActive;
        public delegate void DirectionChoiceInactive();
        public static event DirectionChoiceActive directionChoiceInactive;
        private MovementDirection _direction = MovementDirection.None;
        private Canvas _canvas;
        private int _distance;
        private int _goSiblingIndex;
        private bool _choosing = false;
        private List<Tile> _availableTiles = new List<Tile>();
        private CanvasGroup _canvasGroup;
        private bool _buttonPressed = false;
        private void Awake() 
        {
            _canvas = GameObject.FindGameObjectWithTag("Canvas").GetComponent<Canvas>();
            _canvas.worldCamera = Camera.main;
            DropZone.directionChoiceActive += OnDirectionChoiceActive;
            DropZone.directionChoiceInactive += OnDirectionChoiceInactive;

            DirectionalButtonClick.onButtonPressed += OnButtonPressed;
            
            GameManager.undoButtonPressed += OnUndo;
            _direction = MovementDirection.None;
        }

        private void Start() {
            DropZone.directionChoiceActive += OnDirectionChoiceActive;
            DropZone.directionChoiceInactive += OnDirectionChoiceInactive;
        }
        public void OnPointerEnter(PointerEventData data){
            Highlight.SetActive(true);
            if(data.pointerDrag != null){
                int distance = data.pointerDrag.gameObject.GetComponent<Draggable>().Distance;
                HighlightAvailableTiles(distance);
            }
        }

        private void OnButtonPressed(bool pressed){
            _buttonPressed = pressed;
        }

        private void Update() {
            if(Input.GetKeyDown(KeyCode.Mouse0) && _choosing && !_buttonPressed){
                OnUndo();
                GameManager.Instance.UndoDirectionalChoiceEvent();
            }
        }

        private void HighlightAvailableTiles(int distance){
            foreach (MovementDirection direction in Enum.GetValues(typeof(MovementDirection)))
            {
                CheckPath(direction,distance);
            }
            foreach (var item in _availableTiles)
            {
                item.Highlight();
            }
        }

        private void OnUndo(){
            _direction = MovementDirection.None;
            if(!_choosing) return;
            _choosing = false;
            DirectionChoiceMenu.SetActive(false);
            if(directionChoiceInactive != null)
                directionChoiceInactive();
            ClearAvailableTiles();
        }

        private void CheckPath(MovementDirection direction,int distance){
            Vector2 target = new Vector2();
            TileType tileType = new TileType();
            BotGridMovement bot = Bot.GetComponent<BotGridMovement>();
            switch(direction){
                case(MovementDirection.Forward):
                for (int i = 1; i < distance+1; i++)
                {
                    target =  new Vector2(Bot.transform.position.x + 0.5f*i, Bot.transform.position.y + 0.25f*i);
                    tileType = GameManager.Instance.GetTileType(target);
                    if(tileType == TileType.Walkable && !GameManager.Instance.BotInTheWay(target)){
                        Tile newTile = GameManager.Instance.GetTile(target);
                        _availableTiles.Add(newTile);
                    }else{
                        break;
                    }
                }
                break;

                case(MovementDirection.Backward):
                for (int i = 1; i < distance+1; i++)
                {
                    target =  new Vector2(Bot.transform.position.x - 0.5f*i, Bot.transform.position.y  - 0.25f*i);
                    tileType = GameManager.Instance.GetTileType(target);
                    if(tileType == TileType.Walkable && !GameManager.Instance.BotInTheWay(target)){
                        Tile newTile = GameManager.Instance.GetTile(target);
                        _availableTiles.Add(newTile);
                    }else{
                        break;
                    }
                }
                break;

                case(MovementDirection.Left):
                for (int i = 1; i < distance+1; i++)
                {
                    target =  new Vector2(Bot.transform.position.x - 0.5f*i, Bot.transform.position.y  + 0.25f*i);
                    tileType = GameManager.Instance.GetTileType(target);
                    if(tileType == TileType.Walkable && !GameManager.Instance.BotInTheWay(target)){
                        Tile newTile = GameManager.Instance.GetTile(target);
                        _availableTiles.Add(newTile);
                    }else{
                        break;
                    }
                }
                break;

                case(MovementDirection.Right):
                for (int i = 1; i < distance+1; i++)
                {
                    target =  new Vector2(Bot.transform.position.x + 0.5f*i, Bot.transform.position.y  - 0.25f*i);
                    tileType = GameManager.Instance.GetTileType(target);
                    if(tileType == TileType.Walkable && !GameManager.Instance.BotInTheWay(target)){
                        Tile newTile = GameManager.Instance.GetTile(target);
                        _availableTiles.Add(newTile);
                    }else{
                        break;
                    }
                }
                break;
            }
        }

        public void OnDrop(PointerEventData data){
            if(_choosing || GameManager.Instance.AnimationInProgress || GameManager.Instance._botMoving) return;
            if(GameManager.Instance._botMoving || GameManager.Instance.AnimationInProgress) return;
            _choosing = true;
            var draggable = data.pointerDrag.GetComponent<Draggable>();
            _distance = draggable.Distance;
            GameManager.Instance.LastCardPlayed = draggable;
            GameManager.Instance.RemoveCard(draggable);
            draggable.ChangeParent();
            int goSiblingIndex = data.pointerDrag.gameObject.transform.GetSiblingIndex();
            data.pointerDrag.gameObject.SetActive(false);
            draggable.transform.SetSiblingIndex(goSiblingIndex);
            DirectionChoiceMenu.SetActive(true);
            StartCoroutine(WaitForDirection());
            
        }

        private void OnDisable() {
            DropZone.directionChoiceActive -= OnDirectionChoiceActive;
            DropZone.directionChoiceInactive -= OnDirectionChoiceInactive;
            GameManager.undoButtonPressed -= OnUndo;
        }

        private void OnDestroy() {
            DropZone.directionChoiceActive -= OnDirectionChoiceActive;
            DropZone.directionChoiceInactive -= OnDirectionChoiceInactive;
            GameManager.undoButtonPressed -= OnUndo;
        }

        private void OnDirectionChoiceActive(){
            CanvasGroup canvasGroup = GetComponent<CanvasGroup>();
            canvasGroup.blocksRaycasts = false;
            Bot.GetComponent<CanvasGroup>().blocksRaycasts = false;
        }

        private void OnDirectionChoiceInactive(){
            CanvasGroup canvasGroup = GetComponent<CanvasGroup>();
            canvasGroup.blocksRaycasts = true;
            Bot.GetComponent<CanvasGroup>().blocksRaycasts = true;
        }

        public void OnPointerExit(PointerEventData data){
            Highlight.SetActive(false);
            if(data.pointerDrag != null){
                ClearAvailableTiles();
            }
        }

        private void ClearAvailableTiles(){
            if(this == null) return;
            foreach (var item in _availableTiles)
                {
                    item.DisableHighlight();
                }
                _availableTiles.Clear();
        }

        private IEnumerator WaitForDirection(){
            DropZone.directionChoiceActive -= OnDirectionChoiceActive;
            if(directionChoiceActive != null){
                directionChoiceActive();
            }

            CanvasGroup canvasGroup = GetComponent<CanvasGroup>();
            canvasGroup.blocksRaycasts = true;
            Bot.GetComponent<CanvasGroup>().blocksRaycasts = true;
            
            while(_direction == MovementDirection.None){
                yield return null;
            }
            DropZone.directionChoiceInactive -= OnDirectionChoiceInactive;
            if(directionChoiceInactive != null){
                directionChoiceInactive();
            }
            _choosing = false;
            ClearAvailableTiles();
            DirectionChoiceMenu.SetActive(false);
            Bot.GetComponent<BotGridMovement>().Move(_direction,_distance);
            _distance = 0;
            _direction = MovementDirection.None;

            DropZone.directionChoiceInactive += OnDirectionChoiceInactive;
            DropZone.directionChoiceActive += OnDirectionChoiceActive;
            yield break;
        }

        public void ChooseToMoveForward(){
            _direction = MovementDirection.Forward;
        }

        public void ChooseToMoveBackward(){
            _direction = MovementDirection.Backward;
        }

        public void ChooseToMoveLeft(){
            _direction = MovementDirection.Left;
        }

        public void ChooseToMoveRight(){
            _direction = MovementDirection.Right;
        }
    }
}
