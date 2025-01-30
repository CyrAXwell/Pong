using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] private MainMenuUI _mainMenuUI;
    [SerializeField] private SingleplayerMenuUI _singleplayerMenuUI;
    [SerializeField] private MultiplayerMenuUI _multiplayerMenuUI;
    [SerializeField] private OptionsUI _optionsUI;
    [SerializeField] private ScoreUI _scoreUI;
    [SerializeField] private GamePauseUI _gamePauseUI;
    [SerializeField] private GameOverUI _gameOverUI;
    [SerializeField] private OneComputerMultiplayerMenuUI _oneComputerMultiplayerMenuUI;
    [SerializeField] private PlayerRedyMenuUI _playerRedyMenuUI;
    [SerializeField] private PauseMultiplayerUI _pauseMultiplayerUI;
    [SerializeField] private PlayerDisconectedUI _playerDisconectedUI;
    [SerializeField] private LobbyUI _lobbyUI;
    [SerializeField] private LobbyCreateUI _lobbyCreateUI;
    [SerializeField] private LobbyMessageUI _lobbyMessageUI;

    private PongGameManager _pongGameManager;
    private GameInput _gameInput;

    public PlayerRedyMenuUI GetPlayerRedyMenuUI() => _playerRedyMenuUI;

    public void Initialize(PongGameManager pongGameManager, GameInput gameInput)
    {
        _pongGameManager = pongGameManager;
        _gameInput = gameInput;

        _mainMenuUI.Initialize(_singleplayerMenuUI, _multiplayerMenuUI, _optionsUI);
        _singleplayerMenuUI.Initialize(_pongGameManager, _mainMenuUI);
        _multiplayerMenuUI.Initialize(_mainMenuUI, _oneComputerMultiplayerMenuUI, _lobbyUI);
        _optionsUI.Initialize(_gameInput, _mainMenuUI);
        _scoreUI.Initialize(_pongGameManager);
        _gameOverUI.Initialize(_pongGameManager, _mainMenuUI);
        _gamePauseUI.Initialize(_pongGameManager, _mainMenuUI);
        _oneComputerMultiplayerMenuUI.Initialize(_pongGameManager, _multiplayerMenuUI, _gameInput);
        _playerRedyMenuUI.Initialize(_pongGameManager, _lobbyUI);
        _pauseMultiplayerUI.Initialize(_pongGameManager);
        _playerDisconectedUI.Initialize(_pongGameManager, _mainMenuUI);
        _lobbyUI.Initialize(_pongGameManager, _multiplayerMenuUI, _lobbyCreateUI);
        _lobbyCreateUI.Initialize(_pongGameManager, _lobbyUI);
        _lobbyMessageUI.Initialize(_pongGameManager, _lobbyUI);
    }
}
