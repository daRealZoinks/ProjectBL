using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

namespace Menu
{
    public class MainMenu : MonoBehaviour
    {
        [SerializeField] private string sceneName;

#if UNITY_EDITOR
        [SerializeField] private SceneAsset sceneAsset;
        private void OnValidate()
        {
            if (sceneAsset != null)
            {
                sceneName = sceneAsset.name;
            }
        }
#endif

        private VisualElement _mainMenu;
        private Button _playButton;
        private Button _optionsButton;
        private Button _quitButton;
        private Label _loadingLabel;

        private VisualElement _optionsMenu;
        private Button _backButton;

        private void Awake()
        {
            var root = GetComponent<UIDocument>().rootVisualElement;

            _mainMenu = root.Q<VisualElement>("main-menu");

            _playButton = root.Q<Button>("play-button");
            _optionsButton = root.Q<Button>("options-button");
            _quitButton = root.Q<Button>("quit-button");

            _loadingLabel = root.Q<Label>("loading-label");

            _optionsMenu = root.Q<VisualElement>("options-menu");

            _backButton = root.Q<Button>("back-button");

            _playButton.clicked += PlayGame;
            _optionsButton.clicked += Options;
            _quitButton.clicked += Application.Quit;

            _backButton.clicked += Back;
        }

        private void PlayGame()
        {
            SceneManager.LoadSceneAsync(sceneName);
            _loadingLabel.style.display = DisplayStyle.Flex;
        }

        private void Options()
        {
            _mainMenu.style.display = DisplayStyle.None;
            _optionsMenu.style.display = DisplayStyle.Flex;
        }

        private void Back()
        {
            _mainMenu.style.display = DisplayStyle.Flex;
            _optionsMenu.style.display = DisplayStyle.None;
        }
    }
}
