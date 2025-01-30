using System;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class LobbyMessageUI : MonoBehaviour
{
    [SerializeField] private TMP_Text _messageText;
    [SerializeField] private Button _closeButton;

    private PongGameManager _pongGameManager;
    private LobbyUI _lobbyUI;

    public void Initialize(PongGameManager pongGameManager, LobbyUI lobbyUI)
    {
        _pongGameManager = pongGameManager;
        _lobbyUI = lobbyUI;
        PongGameMultipayer.Instance.OnFailedToJoinGame += OnFailedToJoinGame;
        PongGameLobby.Instance.OnCreateLobbyStarted += OnCreateLobbyStarted;
        PongGameLobby.Instance.OnCreateLobbyFailed += OnCreateLobbyFailed;
        PongGameLobby.Instance.OnJoinStarted += OnJoinStarted;
        PongGameLobby.Instance.OnJoinFailed += OnJoinFailed;
        PongGameLobby.Instance.OnQuickJoinFailed += OnQuickJoinFailed;
        _pongGameManager.OnJoinGame += OnJoinGame;
    }

    private void Start()
    {
        _closeButton.onClick.AddListener( () => { Hide(); _lobbyUI.Show(); });
    }

    private void OnJoinGame(object sender, EventArgs e)
    {
        Hide();
    }

    private void OnJoinStarted(object sender, EventArgs e)
    {
        ShowMesssage("Joining lobby..."); 
        ShowButton(false);
    }

    private void OnJoinFailed(object sender, EventArgs e)
    {
        ShowMesssage("Failed to join lobby!"); 
        ShowButton(true);
    }

    private void OnQuickJoinFailed(object sender, EventArgs e)
    {
        ShowMesssage("Could not find a lobby to quick join!"); 
        ShowButton(true);
    }

    private void OnCreateLobbyStarted(object sender, EventArgs e)
    {
        ShowMesssage("Create lobby..."); 
        ShowButton(false);
    }

    private void OnCreateLobbyFailed(object sender, EventArgs e)
    {
        ShowMesssage("Failed to create lobby!"); 
        ShowButton(true);
    }

    private void OnFailedToJoinGame(object sender, EventArgs e)
    {
        if (!PongGameMultipayer.Instance.IsConnecting())
            return;

        if (NetworkManager.Singleton.DisconnectReason == "")
            ShowMesssage("Failed to connect"); 
        else
            ShowMesssage(NetworkManager.Singleton.DisconnectReason); 

        ShowButton(true);
    }

    private void ShowMesssage(string message)
    {
        Show();
        _messageText.text = message.ToUpper();
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }

    private void ShowButton(bool showButton)
    {
        _closeButton.gameObject.SetActive(showButton);
        if (showButton)
        {
            _closeButton.GetComponent<ButtonWithPointerUI>().ManualSelect();
            _closeButton.Select();
        }
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
