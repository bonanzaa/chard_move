using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChardMove
{
    public class VolumeSliders : MonoBehaviour
    {
        private SoundManager _soundManager;

        private void Awake()
        {
            _soundManager = SoundManager.Instance;
        }
        public void SFXSlider(float sfxVolume)
        {
            _soundManager.SFXVolumeLevel(sfxVolume);
        }
        public void MusicSlider(float musicVolume)
        {
            _soundManager.MusicVolumeLevel(musicVolume);
        }
    }
}
