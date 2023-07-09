using UnityEngine;
using UnityEngine.UIElements;

namespace Menu
{
    public class OptionsMenu : MonoBehaviour
    {
        private Button _backButton;
        private VisualElement _graphicsMenu;

        private Button _graphicsMenuButton;

        private VisualElement _mainMenu;
        private VisualElement _optionsMenu;
        private VisualElement _root;
        private VisualElement _soundMenu;
        private Button _soundMenuButton;

        private void Awake()
        {
            _root = GetComponent<UIDocument>().rootVisualElement;

            _mainMenu = _root.Q<VisualElement>("main-menu");
            _optionsMenu = _root.Q<VisualElement>("options-menu");
            _graphicsMenu = _root.Q<VisualElement>("graphics-menu");
            _soundMenu = _root.Q<VisualElement>("sound-menu");

            _backButton = _root.Q<Button>("options-menu-back-button");

            _graphicsMenuButton = _root.Q<Button>("graphics-menu-button");
            _soundMenuButton = _root.Q<Button>("sound-menu-button");

            _backButton.clicked += () =>
            {
                _mainMenu.style.display = DisplayStyle.Flex;
                _optionsMenu.style.display = DisplayStyle.None;
            };
            _graphicsMenuButton.clicked += () =>
            {
                _optionsMenu.style.display = DisplayStyle.None;
                _graphicsMenu.style.display = DisplayStyle.Flex;
            };
            _soundMenuButton.clicked += () =>
            {
                _optionsMenu.style.display = DisplayStyle.None;
                _soundMenu.style.display = DisplayStyle.Flex;
            };
        }
    }
}