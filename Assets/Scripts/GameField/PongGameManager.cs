using System;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Services.Authentication;
using UnityEngine;

public class PongGameManager : NetworkBehaviour
{
    public const int MAX_PLAYER_AMOUNT = 2;

    public event EventHandler OnScoreChange;
    public event EventHandler OnStartGame;
    public event EventHandler OnGameOver;
    public event EventHandler OnLocalGamePaused;
    public event EventHandler OnLocalGameUnpaused;
    public event EventHandler OnMultiplayerGamePaused;
    public event EventHandler OnMultiplayerGameUnpaused;
    public event EventHandler OnPlayerDisconnected;
    public event EventHandler OnShotDown;
    public event EventHandler OnJoinGame;
    public event EventHandler OnResetGame;

    private enum State 
    {
        WaitingPlayers,
        GamePlaying,
        GameOver,
        PlayerDisconected,
    }

    [SerializeField] private Player _playerPrafab;
    [SerializeField] private Ball _ballPrefab;
    [SerializeField] private Transform[] _playerSpawnPointArray;
    [SerializeField] private Transform _ballSpawnPoint;
    [SerializeField] private PlayerGoal _leftGoal;
    [SerializeField] private PlayerGoal _rightGoal;
    [SerializeField] private LevelSettingSO _levelSettingSO;
    
    private GameInput _gameInput;
    private Ball _ball;
    private Player _secondPlayer;
    private bool _isSingleplayerGame;
    private bool _isCPUPlayer;
    private PongAIDifficultySO _difficulty;
    private State _gameState;
    private NetworkVariable<int> _leftPlayerScore = new NetworkVariable<int>();
    private NetworkVariable<int> _rightPlayerScore = new NetworkVariable<int>();
    private NetworkVariable<int> _targetGoal = new NetworkVariable<int>(5);
    private NetworkVariable<bool> _isGamePaused = new NetworkVariable<bool>(false);
    private bool _isLocalGamePaused;
    private bool _isLocalPlayerReady;
    private PlayerRedyMenuUI _playerRedyMenuUI;
    private Dictionary<ulong, bool> _playerReadyDictionary;
    private Dictionary<ulong, bool> _playerPausedDictionary;
    private bool _autoTestGamepausedState;

    public void Initialize(GameInput gameInput, PlayerRedyMenuUI playerRedyMenuUI)
    {
        _gameInput = gameInput;
        _playerRedyMenuUI = playerRedyMenuUI;

        _gameInput.OnPauseAction += OnGamePauseGameInputAction;
        _playerRedyMenuUI.OnPlayerReady += OnPlayerReady;
        _playerRedyMenuUI.OnPlayerNotReady += OnPlayerNotReady;
        _playerRedyMenuUI.OnTargetGoalChanged += OnTargetGoalChanged;
        _leftGoal.OnGoal += OnGoal;
        _rightGoal.OnGoal += OnGoal;
        _leftPlayerScore.OnValueChanged += OnPlayerScoreNetworkValueChanged;
        _rightPlayerScore.OnValueChanged += OnPlayerScoreNetworkValueChanged;
        _isGamePaused.OnValueChanged += OnPauseStateChanged;

        NetworkManager.Singleton.OnClientConnectedCallback += OnPlayerConnetcion;
        NetworkManager.Singleton.OnClientDisconnectCallback += OnPlayerDisconnection;

        PongGameMultipayer.Instance.OnShotDown += PongGameMultipayerOnShotDown;
        PongGameMultipayer.Instance.OnStartHost += OnStartHost;
        PongGameMultipayer.Instance.OnStartClient += OnStartClient;
    }

    public LevelSettingSO GetLevelSettingSO() => _levelSettingSO;
    public bool IsPlayerDisconected() => _gameState == State.PlayerDisconected;
    public bool IsWaitingPlayersState() => _gameState == State.WaitingPlayers;
    public bool IsGamePlaying() => _gameState == State.GamePlaying;
    public bool IsGameOver() => _gameState == State.GameOver;
    public int GetTargetGoal() => _targetGoal.Value;
    public int GetLeftPlayerScore() => _leftPlayerScore.Value;
    public int GetRightPlayerScore() => _rightPlayerScore.Value;
    public bool IsLocalPlayerReady() => _isLocalPlayerReady;
    public bool IsPlayerReadyMenuActiveInHierarchy() => _playerRedyMenuUI.gameObject.activeInHierarchy;
    public void SetIsMulpiplayerGame() => _isSingleplayerGame = false;
    public bool IsSingleplayer() => _isSingleplayerGame;

    private void PongGameMultipayerOnShotDown(object sender, EventArgs e)
    {
        Time.timeScale = 1f;
        OnMultiplayerGameUnpaused?.Invoke(this, EventArgs.Empty);
        OnShotDown?.Invoke(this, EventArgs.Empty);
    }

    private void OnStartHost(object sender, EventArgs e)
    {
        _gameState = State.WaitingPlayers;
        _playerReadyDictionary = new Dictionary<ulong, bool>();
        _playerPausedDictionary = new Dictionary<ulong, bool>(); 
    }

    private void OnStartClient(object sender, EventArgs e)
    {
        _gameState = State.WaitingPlayers;
    }

    private void OnPlayerConnetcion(ulong clientId)
    {
        if (clientId == NetworkManager.Singleton.LocalClientId)
        {
            PongGameMultipayer.Instance.OnPlayerJoinGame(clientId);
            OnJoinGame?.Invoke(this, EventArgs.Empty);

            int playerIndex = IsServer ? 0 : 1;
            NetworkObject playerObject = NetworkManager.Singleton.SpawnManager.GetLocalPlayerObject();
            Player player = playerObject.GetComponent<Player>();
            player.Initialize(0);
            player.GetComponent<PlayerMovement>().Initialize(_gameInput, this, _playerSpawnPointArray[playerIndex].position);

            PongGameMultipayer.Instance.SetPlayerNameServerRpc(PongGameMultipayer.Instance.GetPlayerName());
            PongGameMultipayer.Instance.SetPlayerIdServerRpc(AuthenticationService.Instance.PlayerId);
            if (!_isSingleplayerGame)
                _playerRedyMenuUI.Show();
        }

        foreach (var obj in NetworkManager.Singleton.SpawnManager.SpawnedObjects)
        {
            obj.Value.TryGetComponent<Player>(out Player player);
            if(player != null)
                player.InitializeNetwork(this);
        }
    }

    private void OnTargetGoalChanged(object sender, EventArgs e)
    {
        _targetGoal.Value = _playerRedyMenuUI.GetTargetGoal();
        UpdatePlayerRedyMenuVisualClientRpc(_targetGoal.Value);
    }

    [ClientRpc]
    private void UpdatePlayerRedyMenuVisualClientRpc(int targetGoal)
    {
        _playerRedyMenuUI.UpdateGoatText(targetGoal);
    }

    private void OnPlayerNotReady(object sender, EventArgs e)
    {
        SetPlayerNotReadyServerRpc();
    }
    
    [ServerRpc(RequireOwnership = false)]
    private void SetPlayerNotReadyServerRpc(ServerRpcParams serverRpcParams = default)
    {
        _playerReadyDictionary[serverRpcParams.Receive.SenderClientId] = false;
    }

    private void OnPlayerReady(object sender, EventArgs e)
    {
        SetPlayerReadyServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    private void SetPlayerReadyServerRpc(ServerRpcParams serverRpcParams = default)
    {
        _playerReadyDictionary[serverRpcParams.Receive.SenderClientId] = true;
        if (NetworkManager.Singleton.ConnectedClientsIds.Count < 2)
            return;

        bool allClientsReady = true;
        foreach (ulong clientId in NetworkManager.Singleton.ConnectedClientsIds)
        {
            if (!_playerReadyDictionary.ContainsKey(clientId) || !_playerReadyDictionary[clientId])
            {
                allClientsReady = false;
                break;
            }
        }

        if (allClientsReady)
        {
            //PongGameLobby.Instance.DeleteLobby();
            _leftPlayerScore.Value = 0;
            _rightPlayerScore.Value = 0;
            _ball = Instantiate(_ballPrefab, _ballSpawnPoint.position, Quaternion.identity);
            var ballNetworkObject = _ball.GetComponent<NetworkObject>();
            ballNetworkObject.Spawn();
            _ball.CanMove();

            _isGamePaused.Value = false;
            StartGameClientRpc();
        }
    }

    [ClientRpc]
    private void StartGameClientRpc()
    {
        _isLocalGamePaused = false;
        _playerRedyMenuUI.Hide();
        _gameState = State.GamePlaying;
        OnStartGame?.Invoke(this, EventArgs.Empty);
    }

    public void ResetAutoTestGamepausedState()
    {
        _autoTestGamepausedState = false;
    }
    
    private void LateUpdate()
    {
        if (_autoTestGamepausedState)
        {
            _autoTestGamepausedState = false;
            TestGamePauseStateServerRpc();
        }
    }

    private void OnGoal(object sender, PlayerGoal.OnGoalEventArgs e)
    {
        if (!NetworkManager.Singleton.IsServer)
            return;
        
        AddScore(e.goalSide);
        _ball.gameObject.SetActive(false);
        _ball.transform.position = _ballSpawnPoint.position;
        _ball.gameObject.SetActive(true);
        _ball.ResetMove(e.goalSide);
    }

    private void AddScore(GoalSide goalSide)
    {
        if (goalSide == GoalSide.rightGoal)
            _leftPlayerScore.Value++;
        else
            _rightPlayerScore.Value++;
    }

    private void OnPlayerScoreNetworkValueChanged(int previousValue, int newValue)
    {
        if (_gameState != State.GamePlaying)
            return;
        OnScoreChange?.Invoke(this, EventArgs.Empty);

        if (_leftPlayerScore.Value >= _targetGoal.Value || _rightPlayerScore.Value >= _targetGoal.Value)
        {   
            if (IsServer)
                Destroy(_ball.gameObject);

            _gameState = State.GameOver;
            SetPlayerNotReadyServerRpc();
            OnGameOver?.Invoke(this, EventArgs.Empty);
        }
    }

    private void OnPauseStateChanged(bool previousValue, bool newValue)
    {
        if (_isGamePaused.Value)
        {
            Time.timeScale = 0f;
            OnMultiplayerGamePaused?.Invoke(this, EventArgs.Empty);
        }
        else
        {
            Time.timeScale = 1f;
            OnMultiplayerGameUnpaused?.Invoke(this, EventArgs.Empty);
        }
    }

    private void OnGamePauseGameInputAction(object sender, EventArgs e)
    {
        if (_gameState == State.GamePlaying)
            TogglePauseGame();
    }

    public void TogglePauseGame()
    {
        _isLocalGamePaused = !_isLocalGamePaused;
        if (_isLocalGamePaused)
        {
            PauseGameServerRpc();
            OnLocalGamePaused?.Invoke(this, EventArgs.Empty);
        }
        else
        {
            UnpauseGameServerRpc();
            OnLocalGameUnpaused?.Invoke(this, EventArgs.Empty);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void PauseGameServerRpc(ServerRpcParams serverRpcParams = default)
    {
        _playerPausedDictionary[serverRpcParams.Receive.SenderClientId] = true;

        TestGamePauseStateServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    private void UnpauseGameServerRpc(ServerRpcParams serverRpcParams = default)
    {
        _playerPausedDictionary[serverRpcParams.Receive.SenderClientId] = false;

        TestGamePauseStateServerRpc();
    }
    
    [ServerRpc]
    private void TestGamePauseStateServerRpc()
    {
        foreach (ulong clientId in NetworkManager.Singleton.ConnectedClientsIds)
        {
            if (_playerPausedDictionary.ContainsKey(clientId) && _playerPausedDictionary[clientId])
            {
                _isGamePaused.Value = true;
                return;
            }
        }

        _isGamePaused.Value = false;
    }

    private void OnPlayerDisconnection(ulong clientId)
    {
        if (NetworkManager.Singleton.IsServer)
        {   
            bool isConnectionApprovalClient = false;
            foreach (ulong connectedclientId in NetworkManager.Singleton.ConnectedClientsIds)
            {
                if (connectedclientId == clientId)
                {
                    isConnectionApprovalClient = true;
                    break;
                }
            }
            
            if (!isConnectionApprovalClient)
                return;

            if (_gameState == State.WaitingPlayers)
                return;

            if (IsGamePlaying() && _ball != null)
                Destroy(_ball.gameObject); 
        }
        else
        
        Time.timeScale = 1f;
        OnMultiplayerGameUnpaused?.Invoke(this, EventArgs.Empty);
        OnPlayerDisconnected?.Invoke(this, EventArgs.Empty);
    }

    public void OnPlayAgainButton()
    {
        OnResetGame?.Invoke(this, EventArgs.Empty);
        if (!_isSingleplayerGame)
        {
            if (IsServer)
            {
                _leftPlayerScore.Value = 0;
                _rightPlayerScore.Value = 0;
            }

            _gameState = State.WaitingPlayers;
            _playerRedyMenuUI.Show();
        }
        else
        {
            RestartGame();
        }
        
    }

    public void StartGame(bool isCPUPlayer, int targetGoal, PongAIDifficultySO difficulty)
    {
        _isSingleplayerGame = true;
        PongGameMultipayer.Instance.StartHostGame();
        _gameState = State.GamePlaying;
        _isCPUPlayer = isCPUPlayer;
        _targetGoal.Value = targetGoal;
        _difficulty = difficulty;

        InitializeGame();
    }

    private void InitializeGame()
    {
        InitializeBall();
        InitializeOtherPlayer();
        _leftPlayerScore.Value = 0;
        _rightPlayerScore.Value = 0;
        _ball.CanMove();

        OnStartGame?.Invoke(this, EventArgs.Empty);
    }

    private void InitializeBall()
    {
        _ball = Instantiate(_ballPrefab, _ballSpawnPoint.position, Quaternion.identity);
        var ballNetworkObject = _ball.GetComponent<NetworkObject>();
        ballNetworkObject.Spawn();
    }

    private void InitializeOtherPlayer()
    {
        int playerIndex = 1;
        _secondPlayer =  Instantiate(_playerPrafab, _playerSpawnPointArray[playerIndex].position, Quaternion.identity);
        _secondPlayer.Initialize(playerIndex);

        if (!_isCPUPlayer)
        {
            _secondPlayer.GetComponent<PlayerMovement>().Initialize(_gameInput, this, _playerSpawnPointArray[playerIndex].position);
        }
        else
        {
            NetworkObject playerObject = NetworkManager.Singleton.SpawnManager.GetLocalPlayerObject();
            Player player = playerObject.GetComponent<Player>();
            PongAIMovementInput pongAIMovementInput = new PongAIMovementInput(_ball, _difficulty, _secondPlayer, player);
            _secondPlayer.GetComponent<PlayerMovement>().Initialize(pongAIMovementInput, this, _playerSpawnPointArray[playerIndex].position);
        }

        var playerNetworkObject = _secondPlayer.GetComponent<NetworkObject>();
        playerNetworkObject.Spawn();
    }

    public void RestartGame()
    {   
        DestroyPlayers();
        InitializeGame();
        _gameState = State.GamePlaying;
    }

    public void DestroyPlayers()
    {
        Destroy(_secondPlayer.gameObject);
    }
}
