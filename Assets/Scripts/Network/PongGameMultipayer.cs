using System;
using Unity.Netcode;
using UnityEngine;

public class PongGameMultipayer : NetworkBehaviour
{
    private const string PLAYER_PREFS_PLAYER_NAME_MULTIPLAYER = "PlayerNameMultiplayer";
    
    public event EventHandler OnShotDown;
    public event EventHandler OnStartHost;
    public event EventHandler OnStartClient;
    public event EventHandler OnTryingToJoinGame;
    public event EventHandler OnFailedToJoinGame;
    public event EventHandler OnPlayerDataNetworkListChanged;

    public static PongGameMultipayer Instance { get; private set; }

    private NetworkList<PlayerData> _playerDataNetworkList;
    private bool _isConnecting;
    private string _playerName;
    private PongGameManager _pongGameManager;

    public void Initialize(PongGameManager pongGameManager)
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        _pongGameManager = pongGameManager;

        _playerName = PlayerPrefs.GetString(PLAYER_PREFS_PLAYER_NAME_MULTIPLAYER, "PlayerName" + UnityEngine.Random.Range(100, 1000));
        _playerDataNetworkList = new NetworkList<PlayerData>();

        NetworkManager.Singleton.OnServerStopped += OnServerStopped;
    }

    public void SetPlayerName(string playerName) 
    {
        _playerName = playerName;
        PlayerPrefs.SetString(PLAYER_PREFS_PLAYER_NAME_MULTIPLAYER, playerName);
    }

    public string GetPlayerName() => _playerName;
    public bool IsConnecting() => _isConnecting;

    public void ShutDown()
    {
        if (NetworkManager.Singleton.IsServer)
        {
            PongGameLobby.Instance.DeleteLobby();
            _playerDataNetworkList.Clear();
        }
        else
        {
            PongGameLobby.Instance.LeaveLobby();
        }

        NetworkManager.Singleton.Shutdown();
        OnShotDown?.Invoke(this, EventArgs.Empty);
    }

    public void StartHostGame()
    {
        OnStartHost?.Invoke(this, EventArgs.Empty);

        NetworkManager.Singleton.ConnectionApprovalCallback += ConnectionApprovalCallback;
        NetworkManager.Singleton.StartHost();
        NetworkManager.Singleton.OnClientDisconnectCallback += ServerOnClientDisconnectCallback;
    }

    public void StartClientGame()
    {
        OnStartClient?.Invoke(this, EventArgs.Empty);
        _isConnecting = true;
        OnTryingToJoinGame?.Invoke(this, EventArgs.Empty);

        NetworkManager.Singleton.OnClientDisconnectCallback += ClientOnClientDisconnectCallback;
        NetworkManager.Singleton.StartClient();
    }

     [ServerRpc(RequireOwnership = false)]
    public void SetPlayerNameServerRpc(string playerName, ServerRpcParams serverRpcParams = default) 
    {
        int playerDataIndex = GetPlayerDataIndexFromClientId(serverRpcParams.Receive.SenderClientId);

        PlayerData playerData = _playerDataNetworkList[playerDataIndex];

        playerData.PlayerName = playerName;

        _playerDataNetworkList[playerDataIndex] = playerData;
    }

    [ServerRpc(RequireOwnership = false)]
    public void SetPlayerIdServerRpc(string playerId, ServerRpcParams serverRpcParams = default) 
    {
        int playerDataIndex = GetPlayerDataIndexFromClientId(serverRpcParams.Receive.SenderClientId);

        PlayerData playerData = _playerDataNetworkList[playerDataIndex];

        playerData.PlayerId = playerId;

        _playerDataNetworkList[playerDataIndex] = playerData;
    }

    private void ClientOnClientDisconnectCallback(ulong clientId)
    {
        OnFailedToJoinGame?.Invoke(this, EventArgs.Empty);

        for (int i = 0; i < _playerDataNetworkList.Count; i++) 
        {
            PlayerData playerData = _playerDataNetworkList[i];
            if (playerData.ClientId == clientId) 
            {
                PongGameLobby.Instance.KickPlayer(playerData.PlayerId.ToString());
            }
        }
    }

    private void ConnectionApprovalCallback(NetworkManager.ConnectionApprovalRequest connectionApprovalRequest, NetworkManager.ConnectionApprovalResponse connectionApprovalResponse)
    {
        if (NetworkManager.Singleton.ConnectedClientsIds.Count >= PongGameManager.MAX_PLAYER_AMOUNT)
        {
            connectionApprovalResponse.Approved = false;
            connectionApprovalResponse.Reason = "Game is full";
            return;
        }

        if (_pongGameManager.IsGamePlaying())
        {
            connectionApprovalResponse.Approved = false;
            connectionApprovalResponse.Reason = "Game is started";
            return;
        }

        connectionApprovalResponse.CreatePlayerObject = true;
        connectionApprovalResponse.Approved = true;
    }

    public void OnPlayerJoinGame(ulong clientId)
    {
        if (NetworkManager.Singleton.IsServer)
        {
            if (_playerDataNetworkList.Count !=0)
                _playerDataNetworkList.Clear();
        }
        _isConnecting = false;
        _playerDataNetworkList.OnListChanged += PlayerDataNetworkListOnListChanged;
        AddPlayerDataNetworkListServerRpc(clientId);
    }

    public int GetPlayerDataIndexFromClientId(ulong clientId)
    {
        for (int i=0; i < _playerDataNetworkList.Count; i++) {
            if (_playerDataNetworkList[i].ClientId == clientId) {
                return i;
            }
        }
        return -1;
    } 

    public string GetPlayerDataNameByPlayerIndex(int playerIndex)
    {
        if (_playerDataNetworkList.Count > playerIndex)
        {
            return _playerDataNetworkList[playerIndex].PlayerName.ToString();
        }

        return "";
    }

    [ServerRpc(RequireOwnership = false)]
    private void AddPlayerDataNetworkListServerRpc(ulong clientId)
    {
        _playerDataNetworkList.Add(new PlayerData { ClientId = clientId });
    }

    private void PlayerDataNetworkListOnListChanged(NetworkListEvent<PlayerData> changeEvent)
    {
        if (_playerDataNetworkList.Count != 0)
            OnPlayerDataNetworkListChanged?.Invoke(this, EventArgs.Empty);
    }

    private void ServerOnClientDisconnectCallback(ulong clientId)
    {
        _pongGameManager.ResetAutoTestGamepausedState();

        for (int i = 0; i < _playerDataNetworkList.Count; i++) 
        {
            PlayerData playerData = _playerDataNetworkList[i];
            if (playerData.ClientId == clientId) 
            {
                PongGameLobby.Instance.KickPlayer(playerData.PlayerId.ToString());
                try
                {
                    _playerDataNetworkList.RemoveAt(i);
                }
                catch (Exception e)
                {
                    Debug.LogError(e);
                }
            }
        }
    }

    private void OnServerStopped(bool obj)
    {
        NetworkManager.Singleton.ConnectionApprovalCallback -= ConnectionApprovalCallback;
        NetworkManager.Singleton.OnClientDisconnectCallback -= ServerOnClientDisconnectCallback;
    }
}
