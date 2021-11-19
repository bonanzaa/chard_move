using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ChardMove.gameManager;
using ChardMove.BotMovement;
using UnityEngine.SceneManagement;
using System.Linq;
using DG.Tweening;

namespace ChardMove
{
    public class LevelSwitchAnimator : MonoBehaviour
    {
        public static LevelSwitchAnimator Instance;
        public GameObject CurrentLevel;
        public int CurrentLevelIndex;

        public List<GameObject> _allEntitiesInLevel = new List<GameObject>();
        public int _k = 0;
        public int _j = 0;
        private int _nextLvlIndex;

        private void Awake() {
            if(Instance == null){
                Instance = this;
                DontDestroyOnLoad(this.gameObject);
            }else{
                Destroy(gameObject);
            }
        }

        private void Update() {
            if(Input.GetKeyDown(KeyCode.Alpha1)){
                LoadLevel(25);
            }
            // if(Input.GetKeyDown(KeyCode.Alpha2)){
            //     LoadLevel(2);
            // }
        }

        public void LoadLevel(int levelSceneIndex){
            if(CurrentLevel == null){
                SceneManager.LoadScene(levelSceneIndex);
            }else{
                _nextLvlIndex = levelSceneIndex;
                // animate OutTween for current level
                StartCoroutine(UnloadingAnimation());
            }
            CurrentLevelIndex = levelSceneIndex;
        }

        public void SetCurrentLevel(GameObject newCurrentLevel){
            CurrentLevel = newCurrentLevel;

            GameManager.Instance.AnimationInProgress = true;
            GameManager.Instance.PlayerWon = false;
            Camera.main.transparencySortAxis = new Vector3(0,0,-1);
            // animate InTween here
            StartCoroutine(LoadingInAnimation());
            

        }

        private IEnumerator LoadingInAnimation(){
            CardStacker.Instance.LoadCards();
            foreach (var item in CardStacker.Instance.CardCounters)
            {
                item.SetActive(true);
            }

            foreach (var item in GameManager.Instance.TileDB.Values)
            {
                if(item != null){
                    
                    _allEntitiesInLevel.Add(item.gameObject);
                }
            }

            foreach (var item in GameManager.Instance.PushableDB.Values)
            {
                if(item.Item2 != null && !item.Item2.TryGetComponent(out BotGridMovement botMovement)){
                    _allEntitiesInLevel.Add(item.Item2);
                }
            }
            foreach (var item in GameManager.Instance.BotDB.Values)
            {
                if(item != null){
                    _allEntitiesInLevel.Add(item.gameObject);
                }
            }


            OrderListByFinalY();

            foreach (var item in _allEntitiesInLevel)
            {
                if(item.TryGetComponent(out Tile tile)){
                    if(item.TryGetComponent(out IPushable pushable0)){
                        // DO NOTHING
                    }else{
                        if(item.gameObject.TryGetComponent(out WinTile wintile)){
                            StartCoroutine(InRedButtonTween(wintile));
                        }else{
                            StartCoroutine(InTileTween(tile));
                        }
                    }
                }

                if(item.TryGetComponent(out IPushable pushable)){
                    if(item.TryGetComponent(out BotGridMovement bot)){
                        StartCoroutine(InBotTween(bot));
                    }else{
                        StartCoroutine(InBlockTween(item));
                    }
                }
            }
            yield return null;
        }

        private void OrderListByFinalY(){
            for(var i = _allEntitiesInLevel.Count - 1; i > -1; i--)
            {
                if (_allEntitiesInLevel[i] == null)
                _allEntitiesInLevel.RemoveAt(i);
            }
            _allEntitiesInLevel = _allEntitiesInLevel.OrderBy(entity => entity.transform.position.y).ToList();
            float currentZ = 300;
            foreach (var item in _allEntitiesInLevel)
            {
                item.transform.position = new Vector3(item.transform.position.x,item.transform.position.y,currentZ);
                currentZ --;
            }
        }

        private IEnumerator InTileTween(Tile tile){
            Vector3 endPosition = tile.transform.position;
            tile.transform.position = new Vector3(tile.transform.position.x,8,tile.transform.position.z);
            float randSpeed = Random.Range(0.5f,4f);
            yield return new WaitForSeconds(0.47f);       
            tile.transform.DOMoveY(endPosition.y,randSpeed,false).SetEase(Ease.OutBack,0.75f).OnComplete(OnLevelFullyLoaded);
        }

        private IEnumerator InGhostTween(GameObject ghost){
            print("starting tweening on ghost");
            if(ghost == null) yield break;
            Vector3 endPosition = ghost.transform.position;
            ghost.transform.position = new Vector3(ghost.transform.position.x,8,ghost.transform.position.z);
            float randSpeed = Random.Range(0.5f,4f);
            print($"Target position: {endPosition}. Current: {ghost.transform.position}");
            yield return new WaitForSeconds(0.47f);       
            ghost.transform.DOMoveY(endPosition.y,randSpeed,false).SetEase(Ease.OutBack,0.75f).OnComplete(OnLevelFullyLoaded);
        }

        private IEnumerator InBotTween(BotGridMovement bot){
            Vector3 endPosition = bot.transform.position;
            bot.transform.position = new Vector3(bot.transform.position.x,8,bot.transform.position.z);
            float randSpeed = Random.Range(1.29f,2.48f);
            yield return new WaitForSeconds(2f);       
            bot.transform.DOMoveY(endPosition.y,randSpeed,false).SetEase(Ease.InQuart).OnComplete(OnLevelFullyLoaded);
            bot.transform.DOMoveX(bot.transform.position.x,randSpeed,false).OnComplete(OnBotLanded);
        }

        private IEnumerator InBlockTween(GameObject pushable){
            Vector3 endPosition = pushable.transform.position;
            pushable.transform.position = new Vector3(pushable.transform.position.x,8,pushable.transform.position.z);
            float randSpeed = Random.Range(1.5f,2.47f);
            yield return new WaitForSeconds(2.39f);       
            pushable.transform.DOMoveY(endPosition.y,randSpeed,false).SetEase(Ease.OutQuart).OnComplete(OnLevelFullyLoaded);
        }

        private IEnumerator InRedButtonTween(WinTile wintile){
            Vector3 endPosition = wintile.transform.position;
            wintile.transform.position = new Vector3(wintile.transform.position.x,8,wintile.transform.position.z);
            yield return new WaitForSeconds(1f);       
            wintile.transform.DOMoveY(endPosition.y,3.5f,false).SetEase(Ease.OutBack,0.4f).OnComplete(OnLevelFullyLoaded);
        }

        private void OnLevelFullyLoaded(){
            _k++;
            if(_k == _allEntitiesInLevel.Count){
                GameManager.Instance.AnimationInProgress = false;
                foreach (var item in _allEntitiesInLevel)
                {
                    item.transform.position = new Vector3(item.transform.position.x,item.transform.position.y,0.5f);
                }
                _k = 0;
                Camera.main.transparencySortAxis = new Vector3(0,1,0);
            }
        }

        private void OnBotLanded(){
            GameManager.Instance.OnBotLanded();
        }

        private IEnumerator UnloadingAnimation(){
            foreach (var item in CardStacker.Instance.CardCounters)
            {
                item.SetActive(false);
            }
            foreach (var item in GameManager.Instance.PlayerCards)
            {
                if(item == null) continue;
                item.transform.DOMoveX(-15,1,false).OnComplete(OnCardsTweened);
                yield return null;
            }
            foreach (var item in _allEntitiesInLevel)
            {
                if(item == null) continue;
                StartCoroutine(OutTween(item));
                yield return new WaitForSeconds(0.01f);
            }
        }
        private IEnumerator OutTween(GameObject entity){
            Vector3 endPosition = new Vector3(entity.transform.position.x,-15,entity.transform.position.z);
            entity.transform.DOMove(endPosition,0.6f,false).SetEase(Ease.InBack,0.1f).OnComplete(TweenCompletionCounter);
            yield return null;
        }

        private void OnCardsTweened(){
            _j++;
            if(_j == GameManager.Instance.PlayerCards.Count){
                for (int i = 0; i < GameManager.Instance.PlayerCards.Count; i++)
                {
                    if(GameManager.Instance.PlayerCards[i] == null) continue;
                    Destroy(GameManager.Instance.PlayerCards[i].gameObject);
                }
                _j = 0;
            }
        }

        private void TweenCompletionCounter(){
            _k++;
            if(_k == _allEntitiesInLevel.Count){
                //Destroy(_lastLevel);
                //_lastLevel = null;
                CurrentLevel = null;
                _k = 0;
                _allEntitiesInLevel.Clear();
                SceneManager.LoadScene(_nextLvlIndex);
            }
        }

    }
}
