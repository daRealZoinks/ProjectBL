using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private string levelToLoad = "PregameLobby";

    private Button _playButton;
    private Button _optionsButton;
    private Button _quitButton;

    private void Start()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;

        _playButton = root.Q<Button>("play-button");
        _optionsButton = root.Q<Button>("options-button");
        _quitButton = root.Q<Button>("quit-button");

        _playButton.clicked += PlayGame;
        _optionsButton.clicked += () => Debug.Log("Options");
        _quitButton.clicked += ExitGame;
    }


    public void PlayGame()
    {
        SceneManager.LoadScene(levelToLoad);
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
