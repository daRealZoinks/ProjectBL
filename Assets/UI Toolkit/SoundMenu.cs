using UnityEngine;
using UnityEngine.UIElements;

public class SoundMenu : MonoBehaviour
{
    private Button _backButton;


    private Slider _masterVolumeSlider;
    private Slider _musicVolumeSlider;
    private VisualElement _optionsMenu;
    private VisualElement _root;
    private Slider _sfxVolumeSlider;
    private VisualElement _soundMenu;

    private void Awake()
    {
        _root = GetComponent<UIDocument>().rootVisualElement;

        _optionsMenu = _root.Q<VisualElement>("options-menu");
        _soundMenu = _root.Q<VisualElement>("sound-menu");

        _backButton = _root.Q<Button>("sound-menu-back-button");
        _backButton.clicked += () =>
        {
            _optionsMenu.style.display = DisplayStyle.Flex;
            _soundMenu.style.display = DisplayStyle.None;
        };

        // InitializeSoundSliders();
    }

    private void InitializeSoundSliders()
    {
        _masterVolumeSlider = _root.Q<Slider>("master-volume-slider");
        _musicVolumeSlider = _root.Q<Slider>("music-volume-slider");
        _sfxVolumeSlider = _root.Q<Slider>("sfx-volume-slider");

        _masterVolumeSlider.RegisterValueChangedCallback(evt => { Debug.Log(evt.newValue); });

        _musicVolumeSlider.RegisterValueChangedCallback(evt => { Debug.Log(evt.newValue); });

        _sfxVolumeSlider.RegisterValueChangedCallback(evt => { Debug.Log(evt.newValue); });
    }
}