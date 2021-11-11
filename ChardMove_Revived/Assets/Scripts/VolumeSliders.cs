using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ChardMove
{
    public class VolumeSliders : MonoBehaviour
    {
        [SerializeField] private GameObject _slider;

        private SoundManager _soundManager;

        public delegate void SliderChanged();
        public static event SliderChanged onSliderChanged;

        private void Awake()
        {
            _soundManager = SoundManager.Instance;
        }
        public void SFXSlider()
        {
            float sfxVolume = _slider.GetComponent<Slider>().value;
            _soundManager.SFXVolumeLevel(sfxVolume);
        }
        public void MusicSlider()
        {
            float musicVolume = _slider.GetComponent<Slider>().value;
            _soundManager.MusicVolumeLevel(musicVolume);
        }
        public void OnSliderChanged()
        {
            onSliderChanged?.Invoke();
        }
    }
}
