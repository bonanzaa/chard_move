using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChardMove
{
    public class LvlSelectButton : MonoBehaviour
    {
        public void OnLoadThisLevelPressed(int index)
        {
            LevelLoader.Instance.OnSelectedLevelLoad(index);
        }
    }
}
