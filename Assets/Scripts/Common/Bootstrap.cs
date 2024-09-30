using UnityEngine;

public class Bootstrap : MonoBehaviour
{
    [SerializeField] private GameInput _gameInput;
    [SerializeField] private PongGameManager _pongGameManager;
    [SerializeField] private MainMenuUI _mainMenuUI;
    [SerializeField] private SingleplayerMenuUI _singleplayerMenuUI;
    [SerializeField] private MultiplayerMenuUI _multiplayerMenuUI;
    [SerializeField] private OptionsUI _optionsUI;
    [SerializeField] private ScoreUI _scoreUI;
    [SerializeField] private GameOverUI _gameOverUI;
    [SerializeField] private GamePauseUI _gamePauseUI;
    [SerializeField] private OneComputerMultiplayerMenuUI _oneComputerMultiplayerMenuUI;

    private void Awake()
    {
        InirializeMainMenuUI();
        InirializePongGameManager();
    }

    private void InirializeMainMenuUI()
    {
        _mainMenuUI.Initialize(_singleplayerMenuUI, _multiplayerMenuUI, _optionsUI);
        _singleplayerMenuUI.Initialize(_pongGameManager, _mainMenuUI);
        _multiplayerMenuUI.Initialize(_mainMenuUI, _oneComputerMultiplayerMenuUI);
        _optionsUI.Initialize(_gameInput, _mainMenuUI);
        _oneComputerMultiplayerMenuUI.Initialize(_pongGameManager, _multiplayerMenuUI, _gameInput);
    }

    private void InirializePongGameManager()
    {
        _pongGameManager.Initialize(_gameInput);
        _scoreUI.Initialize(_pongGameManager);
        _gameOverUI.Initialize(_pongGameManager, _mainMenuUI);
        _gamePauseUI.Initialize(_pongGameManager, _mainMenuUI);
    }
}
