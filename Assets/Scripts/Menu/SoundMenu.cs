using UnityEngine;
using UnityEngine.UIElements;

namespace Menu
{
    public class SoundMenu : MonoBehaviour
    {
        private Button _backButton;

        private Slider _masterVolumeSlider;
        private Slider _musicVolumeSlider;
        private VisualElement _optionsMenu;
        private Slider _sfxVolumeSlider;
        private VisualElement _soundMenu;

        private void Awake()
        {
            var root = GetComponent<UIDocument>().rootVisualElement;

            _optionsMenu = root.Q<VisualElement>("options-menu");
            _soundMenu = root.Q<VisualElement>("sound-menu");

            _masterVolumeSlider = root.Q<Slider>("master-volume-slider");
            _musicVolumeSlider = root.Q<Slider>("music-volume-slider");
            _sfxVolumeSlider = root.Q<Slider>("sfx-volume-slider");

            _backButton = root.Q<Button>("sound-menu-back-button");
            _backButton.clicked += () =>
            {
                _optionsMenu.style.display = DisplayStyle.Flex;
                _soundMenu.style.display = DisplayStyle.None;
            };

            InitializeSoundSliders();
        }

        private void InitializeSoundSliders()
        {
            _masterVolumeSlider.RegisterValueChangedCallback(evt => { Debug.Log(evt.newValue); });
            _musicVolumeSlider.RegisterValueChangedCallback(evt => { Debug.Log(evt.newValue); });
            _sfxVolumeSlider.RegisterValueChangedCallback(evt => { Debug.Log(evt.newValue); });
        }
    }
}