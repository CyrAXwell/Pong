using UnityEngine;

public class Bootstrap : MonoBehaviour
{
    [SerializeField] private GameInput _gameInput;
    [SerializeField] private PongGameManager _pongGameManager;
    [SerializeField] private PongGameMultipayer _pongGameMultipayer;
    [SerializeField] private PongGameLobby _pongGameLobby;
    [SerializeField] private UIManager _uIManager;

    private void Awake()
    {  
        InitializeMultiplayer();
        InirializePongGameManager();
        InitializeUI();
    }

    private void InirializePongGameManager()
    {
        _pongGameManager.Initialize(_gameInput, _uIManager.GetPlayerRedyMenuUI());
    }

    private void InitializeUI()
    {
        _uIManager.Initialize(_pongGameManager, _gameInput);
    }

    private void InitializeMultiplayer()
    {
        _pongGameMultipayer.Initialize(_pongGameManager);
        _pongGameLobby.Initialize(_pongGameManager);
    }
}
