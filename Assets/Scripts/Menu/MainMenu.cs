using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

namespace Menu
{
    public class MainMenu : MonoBehaviour
    {
        [Tooltip("The name of the scene to load when the play button is pressed.")]
        [SerializeField] private string sceneName;
        
        private VisualElement _mainMenu;
        private VisualElement _optionsMenu;

        private Button _playButton;
        private Button _optionsButton;
        private Button _quitButton;

        private Label _loadingLabel;

        private void Awake()
        {
            var root = GetComponent<UIDocument>().rootVisualElement;

            _mainMenu = root.Q<VisualElement>("main-menu");
            _optionsMenu = root.Q<VisualElement>("options-menu");

            _playButton = root.Q<Button>("play-button");
            _optionsButton = root.Q<Button>("options-button");
            _quitButton = root.Q<Button>("quit-button");

            _loadingLabel = root.Q<Label>("loading-label");

            _playButton.clicked += PlayGame;
            _optionsButton.clicked += Options;
            _quitButton.clicked += Application.Quit;
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

#if UNITY_EDITOR
        [Tooltip("The scene to load when the play button is pressed.")]
        [SerializeField] private SceneAsset sceneAsset;
        private void OnValidate()
        {
            if (sceneAsset != null) sceneName = sceneAsset.name;
        }
#endif
    }
}