using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChardMove
{
    public class StartButton : MonoBehaviour
    {
        private SaveSystem _saveSystem = new SaveSystem();
        public void OnStartButtonPressed()
        {
            _saveSystem.Deserialize();
            if(_saveSystem.RefreshLvlIndex() == 0)
            {
                SceneLoader.Instance.LoadScene(1);
            }
            else
            {
                SceneLoader.Instance.LoadScene(_saveSystem.RefreshLvlIndex());
            }
        }
        public void OnSelectButtonPressed(int index)
        {
            _saveSystem.LastSceneIndex = index;
            _saveSystem.Serialize();
            LevelSwitchAnimator.Instance.LoadLevel(index);
        }
    }
}
