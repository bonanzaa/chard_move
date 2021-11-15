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
       [HideInInspector] public List<Draggable> _tempPlayerCards = new List<Draggable>();

        private LevelLoader _levelLoader;
        private List<Draggable> _originalPlayerCards = new List<Draggable>();
        private GameObject _currentLevel;
        public GameObject _lastLevel;
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
        public GameObject NextLevelDebug;
        private int _i = 0;
        public int _k = 0;
        private int _j = 0;
        private GameObject _oldLevel;

        private void Awake() {
            //Time.timeScale = 1;
            _levelLoader = LevelLoader.Instance;
            LevelCompleteReference.nextLevel += OnNextLevelLoad;
            BotGridMovement.botMovedPos += OnBotMoved;
            BotGridMovement.botStartedMovingPos += OnBotStartedMoving;
            BotGridMovement.botUndoPressed += OnBotUndoMoved;
            _botMoving = false;
            LevelLoaded = false;
            Instance = this;
            if(Level == null && LevelLoader.Instance != null)
            {
                Level = _levelLoader.Levels[LevelLoader.LevelIndex];
            }
            if(Level != null){
                LevelLoaded = true;
                LoadLevel(Level);
            }
        }

        private void OnBotMoved(Vector2 pos){
            FindSwitch(pos);
            FindWinTile(pos);
            FindMovingPlatform(pos);

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


        private void OnNextLevelLoad()
        {
            LoadLevel(_levelLoader.Levels[LevelLoader.LevelIndex]);
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
            LevelCompleteReference.nextLevel -= OnNextLevelLoad;
        }

        public void LoadLevel(GameObject level) {
            // if(Level != null){
            //     Destroy(_currentLevel);
            //     ClearDictionaries();
            //     ResetPlayerCards();
            //     LevelLoaded = true;
            //     StartCoroutine(LevelInstantiatingTimer(level));
               
            // }else{
            //     //ResetPlayerCards();
            //     LevelLoaded = true;
            //     CardStacker.Instance.LoadCards();
            // }

            // if(level.TryGetComponent(out CameraOffsetRegister _cameraOffset)){
            //     Camera.main.gameObject.transform.position = _cameraOffset.CameraPosition;
            //     GameObject.FindGameObjectWithTag("Canvas").gameObject.transform.position = new Vector3(_cameraOffset.CameraPosition.x,_cameraOffset.CameraPosition.y,0);
            // }
            AnimationInProgress = true;
            Camera.main.transparencySortAxis = new Vector3(0,0,-1);

            // that's being called when you have the level in the scene already
            // (level design)

            if(Level == null && !LevelLoaded){
                LevelLoaded = true;
                LoadNewLevelNoInstantiate(level);
                return;
            }

            if(_lastLevel != null){
                ResetPlayerCards();
                StartCoroutine(TweenPlayerCards());
                StartCoroutine(UnloadLevelWithAnimation(level));
            }else{
                StartCoroutine(TweenPlayerCards());
                LoadNewLevelDebug(level);
            }

            //CardStacker.Instance.LoadCards();
            //StartCoroutine(LoadLevelWithAnimation(level));
        }

        private void LoadNewLevelDebug(GameObject level){
            StartCoroutine(LevelInstantiatingTimer(level));
            //StartCoroutine(LoadLevelWithAnimation(level));
        }

        private void LoadNewLevelNoInstantiate(GameObject level){
            StartCoroutine(LoadLevelWithAnimationNoInstantiate(level));
        }

        public IEnumerator LoadLevelWithAnimation(GameObject level){
            LevelLoaded = true;
            _allEntitiesToLoad.Clear();
            //instantiate level here

            _currentLevel = Instantiate(level, new Vector3(0,0,0),Quaternion.identity);
            Level = _currentLevel;
            _lastLevel = _currentLevel;
            //CardStacker.Instance.LoadCards();

            
            
            foreach (var item in TileDB.Values)
            {
                if(item != null){
                    if(!item.enabled){
                        item.enabled = true;
                    }
                    _allEntitiesToLoad.Add(item.gameObject);
                    if(item.gameObject.GetComponent<WinTile>()){
                        continue;
                    }else{
                        //StartCoroutine(InTileTween(item));
                    }

                }
            }

            //StartCoroutine(InRedButtonTween(RedButton));

            foreach (var item in PushableDB.Values)
            {
                if(item.Item2 != null){
                    if(item.Item2.GetComponent<BotGridMovement>()){
                        continue;
                    }else{
                        _allEntitiesToLoad.Add(item.Item2);
                        //StartCoroutine(InBlockTween(item.Item2));
                    }

                }
            }
            foreach (var item in BotDB.Values)
            {
                if(item != null){
                    _allEntitiesToLoad.Add(item.gameObject);
                    //StartCoroutine(InBotTween(item));

                }
            }

            OrderListByFinalY();

            foreach (var item in TileDB.Values)
            {
                if(item == null) continue;
                if(item.gameObject.GetComponent<WinTile>()){
                    continue;
                }else{
                    StartCoroutine(InTileTween(item));
                }
            }

            StartCoroutine(InRedButtonTween(RedButton));

            foreach (var item in PushableDB.Values)
            {
                if(item.Item2 == null) continue;
                if(item.Item2.GetComponent<BotGridMovement>()){
                    continue;
                }else{
                    StartCoroutine(InBlockTween(item.Item2));
                }
            }
            foreach (var item in BotDB.Values)
            {
                if(item == null) continue;
                StartCoroutine(InBotTween(item));
            }
            yield return null;

        }

        private IEnumerator LoadLevelWithAnimationNoInstantiate(GameObject level){
            LevelLoaded = true;
            //instantiate level here

            _currentLevel = level;
            Level = _currentLevel;
            CardStacker.Instance.LoadCards();

            
            
            foreach (var item in TileDB.Values)
            {
                _allEntitiesToLoad.Add(item.gameObject);
                if(item.gameObject.GetComponent<WinTile>()){
                    continue;
                }else{
                    //StartCoroutine(InTileTween(item));
                }
            }

            //StartCoroutine(InRedButtonTween(RedButton));

            foreach (var item in PushableDB.Values)
            {
                if(item.Item2.GetComponent<BotGridMovement>()){
                    continue;
                }else{
                    _allEntitiesToLoad.Add(item.Item2);
                    //StartCoroutine(InBlockTween(item.Item2));
                }
            }
            foreach (var item in BotDB.Values)
            {
                _allEntitiesToLoad.Add(item.gameObject);
                //StartCoroutine(InBotTween(item));
            }
            yield return null;
            OrderListByFinalY();

            // iterate through the shit again, but now we are actually tweening the shit
            // this is retarded way to do it, but cba

            foreach (var item in TileDB.Values)
            {
                if(item.gameObject.GetComponent<WinTile>()){
                    continue;
                }else{
                    StartCoroutine(InTileTween(item));
                }
            }
            StartCoroutine(InRedButtonTween(RedButton));

            foreach (var item in PushableDB.Values)
            {
                if(item.Item2.GetComponent<BotGridMovement>()){
                    continue;
                }else{
                    StartCoroutine(InBlockTween(item.Item2));
                }
            }
            foreach (var item in BotDB.Values)
            {
                StartCoroutine(InBotTween(item));
            }
            yield return null;
        }


        public IEnumerator UnloadLevelWithAnimation(GameObject level){
            _allEntitiesToUnload.Clear();
            if(_lastLevel == null){
                LoadNewLevelDebug(level);
                yield break;
            }
            LevelLoaded = true;
            _lastLevel.transform.SetParent(Camera.main.transform);
            yield return null;
            foreach (var item in TileDB.Values)
            {
                if(item != null){
                    _allEntitiesToUnload.Add(item.gameObject);

                }
            }

            foreach (var item in PushableDB.Values)
            {
                if(item.Item2 != null){
                    _allEntitiesToUnload.Add(item.Item2.gameObject);

                }
            }

            foreach (var item in BotDB.Values)
            {
                if(item != null){
                    _allEntitiesToUnload.Add(item.gameObject);

                }
            }
            yield return null;


            _allEntitiesToUnload = _allEntitiesToUnload.OrderBy(entity => entity.transform.position.x).ToList();
            ClearDictionaries();
            if(onLevelUnload != null)
                onLevelUnload();
            foreach (var item in _allEntitiesToUnload)
            {
                if(item == null) continue;
                StartCoroutine(OutTween(item));
                yield return new WaitForSeconds(0.01f);
            }
            LoadNewLevelDebug(level);
        }

        private IEnumerator InTileTween(Tile tile){
            Vector3 endPosition = tile.transform.position;
            tile.transform.position = new Vector3(tile.transform.position.x,5,tile.transform.position.z);
            float randSpeed = Random.Range(TileSpeedMin,TileSpeedMax);
            yield return new WaitForSeconds(DelayBeforeTiles);       
            tile.transform.DOMoveY(endPosition.y,randSpeed,false).SetEase(Ease.OutBack,0.75f).OnComplete(OnLevelFullyLoaded);
        }

        private IEnumerator InGhostTween(GameObject ghost){
            if(ghost == null) yield break;
            Vector3 endPosition = ghost.transform.position;
            ghost.transform.position = new Vector3(ghost.transform.position.x,5,ghost.transform.position.z);
            float randSpeed = Random.Range(TileSpeedMin,TileSpeedMax);
            yield return new WaitForSeconds(DelayBeforeTiles);       
            ghost.transform.DOMoveY(endPosition.y,randSpeed,false).SetEase(Ease.OutBack,0.75f).OnComplete(OnLevelFullyLoaded);
        }

        private IEnumerator InBotTween(BotGridMovement bot){
            Vector3 endPosition = bot.transform.position;
            bot.transform.position = new Vector3(bot.transform.position.x,5,bot.transform.position.z);
            float randSpeed = Random.Range(BotSpeedMin,BotSpeedMax);
            yield return new WaitForSeconds(DelayBeforeBots);       
            bot.transform.DOMoveY(endPosition.y,randSpeed,false).SetEase(Ease.InQuart).OnComplete(OnLevelFullyLoaded);
            bot.transform.DOMoveX(bot.transform.position.x,randSpeed,false).OnComplete(OnBotLanded);
        }

        private IEnumerator InBlockTween(GameObject pushable){
            Vector3 endPosition = pushable.transform.position;
            pushable.transform.position = new Vector3(pushable.transform.position.x,5,pushable.transform.position.z);
            float randSpeed = Random.Range(BlockSpeedMin,BlockSpeedMax);
            yield return new WaitForSeconds(DelayBeforeBlocks);       
            pushable.transform.DOMoveY(endPosition.y,randSpeed,false).SetEase(Ease.OutQuart).OnComplete(OnLevelFullyLoaded);
        }

        private IEnumerator InRedButtonTween(WinTile wintile){
            Vector3 endPosition = wintile.transform.position;
            wintile.transform.position = new Vector3(wintile.transform.position.x,5,wintile.transform.position.z);
            yield return new WaitForSeconds(DelayBeforeRedButton);       
            wintile.transform.DOMoveY(endPosition.y,RedButtonSpeed,false).SetEase(Ease.OutBack,0.4f).OnComplete(OnLevelFullyLoaded);
        }

        private IEnumerator OutTween(GameObject entity){
            Vector3 endPosition = new Vector3(entity.transform.position.x,-15,entity.transform.position.z);
            entity.transform.DOMove(endPosition,0.6f,false).SetEase(Ease.InBack,0.1f).OnComplete(TweenCompletionCounter);
            yield return null;
        }

        private IEnumerator LevelInstantiatingTimer(GameObject level){
             yield return new WaitForSeconds(0.7f); // 0.5f


            if(level.TryGetComponent(out CameraOffsetRegister _cameraOffset)){
                Camera.main.gameObject.transform.position = _cameraOffset.CameraPosition;
                GameObject.FindGameObjectWithTag("Canvas").gameObject.transform.position = new Vector3(_cameraOffset.CameraPosition.x,_cameraOffset.CameraPosition.y,0);
            }
            yield return new WaitForSeconds(0.1f);
            StartCoroutine(LoadLevelWithAnimation(level));
            yield return new WaitForEndOfFrame();
            if(onNewLevelLoaded != null)
                onNewLevelLoaded();
        }

        private IEnumerator ResetUnloadLevel(GameObject level){
            level.name = "OLD LEVEL";
            foreach (var item in _allEntitiesToUnload)
            {
               StartCoroutine(OutTweenReset(item));
               yield return new WaitForSeconds(0.01f); 
            }
            _oldLevel = level;
        }

        private IEnumerator OutTweenReset(GameObject entity){
            Vector3 endPosition = new Vector3(entity.transform.position.x,-15,entity.transform.position.z);
            entity.transform.DOMove(endPosition,0.6f,false).SetEase(Ease.InBack,0.1f).OnComplete(OnUnloaded);
            yield return null;
        }

        private void OnUnloaded(){
            _j++;
            if(_j == _allEntitiesToUnload.Count){
                _j = 0;
                foreach (var item in _allEntitiesToUnload)
                {
                    Destroy(item);
                }
                //Destroy(_oldLevel);
                //_allEntitiesToUnload.Clear();
            }
        }

        private IEnumerator ResetLoadLevel(GameObject level){
            level.SetActive(true);
            LevelLoaded = true;
            //instantiate level here
            _currentLevel = level;
            Level = _currentLevel;
            //CardStacker.Instance.LoadCards();

            
            foreach (var item in TileDB.Values)
            {
                _allEntitiesToLoad.Add(item.gameObject);
                if(item.gameObject.GetComponent<WinTile>()){
                    continue;
                }else{
                    //StartCoroutine(InTileTween(item));
                }
            }

            foreach (var item in PushableDB.Values)
            {
                if(item.Item2.GetComponent<BotGridMovement>()){
                    continue;
                }else{
                    _allEntitiesToLoad.Add(item.Item2);
                    //StartCoroutine(InBlockTween(item.Item2));
                }
            }
            foreach (var item in BotDB.Values)
            {
                _allEntitiesToLoad.Add(item.gameObject);
                //StartCoroutine(InBotTween(item));
            }

            OrderListByFinalY();

            // iterate through the shit again, but now we are actually tweening the shit
            // this is retarded way to do it, but cba

            foreach (var item in TileDB.Values)
            {
                if(item.gameObject.GetComponent<WinTile>()){
                    continue;
                }else{
                    StartCoroutine(InTileTween(item));
                }
            }

            StartCoroutine(InRedButtonTween(RedButton));

            foreach (var item in PushableDB.Values)
            {
                if(item.Item2.GetComponent<BotGridMovement>()){
                    continue;
                }else{
                    StartCoroutine(InBlockTween(item.Item2));
                }
            }
            foreach (var item in BotDB.Values)
            {
                StartCoroutine(InBotTween(item));
            }
            yield return null;
            _currentLevel = level;
        }

        private void TweenCompletionCounter(){
            _i++;
            if(_i == _allEntitiesToUnload.Count){
                //Destroy(_lastLevel);
                //_lastLevel = null;
                _i = 0;
                _allEntitiesToUnload.Clear();
                Destroy(_oldLevel);
                Destroy(_lastLevel);
            }
        }

        private void Update() {
            if(Input.GetKeyDown(KeyCode.A)){
                print($"Bot moving: {_botMoving}. Animation in progress: {AnimationInProgress}");
            }
        }

        private void OnLevelFullyLoaded(){
            _k++;
            if(_k == _allEntitiesToLoad.Count){
                // event
                AnimationInProgress = false;
                foreach (var item in _allEntitiesToLoad)
                {
                    item.transform.position = new Vector3(item.transform.position.x,item.transform.position.y,0.5f);
                }
                _allEntitiesToLoad.Clear();
                _k = 0;
                Camera.main.transparencySortAxis = new Vector3(0,1,0);
                if(_lastLevel != null){
                    //Destroy(_lastLevel);
                }
                _allEntitiesToUnload.Clear();
                onLevelFullyLoaded();


                GameObject oldLevel = Camera.main.transform.GetChild(0).gameObject;
                if(oldLevel.TryGetComponent(out Canvas canvas)){

                }else{
                    Destroy(oldLevel);
                }
                AnimationInProgress = false;

            }
        }

        private void OrderListByFinalY(){
            for(var i = _allEntitiesToLoad.Count - 1; i > -1; i--)
            {
                if (_allEntitiesToLoad[i] == null)
                _allEntitiesToLoad.RemoveAt(i);
            }
            _allEntitiesToLoad = _allEntitiesToLoad.OrderBy(entity => entity.transform.position.y).ToList();
            float currentZ = 300;
            foreach (var item in _allEntitiesToLoad)
            {
                item.transform.position = new Vector3(item.transform.position.x,item.transform.position.y,currentZ);
                currentZ --;
            }
        }


        private void CacheTilesInTheScene(){
            _allEntitiesToUnload.Clear();
            _lastLevel = _currentLevel;
            _lastLevel.transform.SetParent(Camera.main.transform);
            foreach (var item in TileDB.Values)
            {
                if(item != null){
                    _allEntitiesToUnload.Add(item.gameObject);
                }
            }

            foreach (var item in PushableDB.Values)
            {
                if(item.Item2 != null){
                    _allEntitiesToUnload.Add(item.Item2.gameObject);
                }
            }

            foreach (var item in BotDB.Values)
            {
                if(item != null){
                    _allEntitiesToUnload.Add(item.gameObject);
                }
            }


            _allEntitiesToUnload = _allEntitiesToUnload.OrderBy(entity => entity.transform.position.x).ToList();
        }


        public void Reset(){
            if(_botMoving || AnimationInProgress) return;
            AnimationInProgress = true;
            _allEntitiesToUnload.Clear();
            _allEntitiesToLoad.Clear();
            //DeletePlayerCards();
            resetButtonPressed();
            StartCoroutine(TweenPlayerCards());
            CacheTilesInTheScene();
            StartCoroutine(ResetDictClearTimer());
            //ResetPlayerCards();
        }

        private void OnBotLanded(){
            if(onBotLanded != null)
                onBotLanded();
        }

        private IEnumerator TweenPlayerCards(){
            _tempPlayerCards.Clear();
            foreach (var item in PlayerCards)
            {
                if(item == null) continue;
                item.transform.DOMoveX(-15,1,false);
                yield return null;
            }
            yield return new WaitForSeconds(1.1f);
            foreach (var item in PlayerCards)
            {
                if(item == null) continue;
                Destroy(item.gameObject);
                yield return null;
            }
            PlayerCards.Clear();
            
            CardStacker.Instance.LoadCards();
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


        private IEnumerator ResetDictClearTimer(){
            yield return new WaitForSeconds(0.05f);
            BotDB.Clear();
            PushableDB.Clear();
            TileDB.Clear();
            // spawn in a new one
            GameObject newLevel = Instantiate(_currentLevel,new Vector3(0,0,0),Quaternion.identity);
            newLevel.name = "NEW LEVEL";
            newLevel.SetActive(false);
            //load in a copy of the level
            StartCoroutine(ResetLoadLevel(newLevel));

            // unload current level
            StartCoroutine(ResetUnloadLevel(_currentLevel));
            _currentLevel = null;
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
            for (int i = 0; i < PlayerCards.Count; i++)
            {
                Destroy(PlayerCards[i].gameObject);
            }
            _tempPlayerCards.Clear();
            PlayerCards.Clear();
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
