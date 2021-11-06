using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChardMove
{
    public class ClearProgressButton : MonoBehaviour
    {
        private SaveSystem _saveSystem = new SaveSystem();
        public void Clear()
        {
            _saveSystem.ClearSaveData();
        }
    }
}
